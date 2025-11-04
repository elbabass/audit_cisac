from locust import HttpUser, events, between
import logging

from task_sets.task_sets import MergeChainTaskSet
from utils.utils import get_token, generate_results_xml


class MergeSubmissionUser(HttpUser):
    tasks = {MergeChainTaskSet}
    wait_time = between(3, 5)

    def on_start(self):
        token = get_token(self)
        self.client.headers = {
            "authorization": f"Bearer {token}", 'Content-type': 'application/json'}

@events.quitting.add_listener
def _(environment, **kw):
    passed = []
    failures = []
    environment.process_exit_code = 0

    if environment.stats.get("Merge submissions", "POST").avg_response_time > 15000:
        environment.process_exit_code = 1
        failures.append(('Merge submissions',
                         f"Test failed due to Merge submissions average > 15s: {environment.stats.get('Merge submissions', 'POST').avg_response_time}"))
    else:
        passed.append(
            f"Merge submissions < 3s: {environment.stats.get('Merge submissions', 'POST').avg_response_time}")

    if environment.stats.get("Update chain base", "PUT").avg_response_time > 3000:
        environment.process_exit_code = 1
        failures.append(('Update chain base',
                         f"Test failed due to Update chain base average > 3s: {environment.stats.get('Update chain base', 'PUT').avg_response_time}"))
    else:
        passed.append(
            f"Update chain base average < 3s: {environment.stats.get('Update chain base', 'PUT').avg_response_time}")

    generate_results_xml(passed, failures)