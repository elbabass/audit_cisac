from locust import HttpUser, between, events
import logging
from task_sets.task_sets import UpdateBatchTaskSet
from utils.utils import get_token, generate_results_xml


class UpdateSubmissionUser(HttpUser):
    tasks = {UpdateBatchTaskSet: 1}
    wait_time = between(3, 5)

    def on_start(self):
        token = get_token(self)
        self.client.headers = {
            "authorization": f"Bearer {token}", 'Content-type': 'application/json'}


@events.quitting.add_listener
def _(environment, **kw):
    environment.process_exit_code = 0

    if environment.stats.get("Update batch submission", "PUT").avg_response_time > 22000:
        logging.error(
            f"Test failed due to Update submission batch of 10 average > 22s: {environment.stats.get('Update batch submission', 'PUT').avg_response_time}")
        environment.process_exit_code = 1
        generate_results_xml([], [('Update batch submission',
                                   f"Test failed due to Update submission batch of 10 average > 22s: {environment.stats.get('Update batch submission', 'PUT').avg_response_time}")])
    else:
        generate_results_xml([('Update batch submission',
                               f"Update submission batch of 10 < 22s: {environment.stats.get('Update batch submission', 'PUT').avg_response_time}")], [])
