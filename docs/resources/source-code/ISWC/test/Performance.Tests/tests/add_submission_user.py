from locust import HttpUser, events, between
import logging

from task_sets.task_sets import AddBatchTaskSet
from utils.utils import get_token, generate_results_xml


class AddSubmissionUser(HttpUser):
    tasks = {AddBatchTaskSet: 1}
    wait_time = between(3, 5)

    def on_start(self):
        token = get_token(self)
        self.client.headers = {
            "authorization": f"Bearer {token}", 'Content-type': 'application/json'}


@events.quitting.add_listener
def _(environment, **kw):
    environment.process_exit_code = 0

    if environment.stats.get("Add batch submission", "POST").avg_response_time > 22000:
        logging.error(
            f"Test failed due to Add submission batch of 10 average > 22s: {environment.stats.get('Add batch submission', 'POST').avg_response_time}")
        environment.process_exit_code = 1
        generate_results_xml([], [('Add batch submission',
                                   f"Test failed due to Add submission batch of 10 average > 22s: {environment.stats.get('Add batch submission', 'POST').avg_response_time}")])
    else:
        generate_results_xml([('Add batch submission',
                               f"Add submission batch of 10 < 22s: {environment.stats.get('Add batch submission', 'POST').avg_response_time}")], [])
