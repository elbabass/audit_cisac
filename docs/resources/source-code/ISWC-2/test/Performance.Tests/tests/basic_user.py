import json
from locust import HttpUser, events, between
import logging

from data.submission import new_iswc_submission
from task_sets.task_sets import SearchTaskSet, AddUpdateTaskSet, search_large_title
from tests.merge_submission_user import MergeSubmissionUser
from utils.utils import get_token, generate_results_xml


class BasicUser(HttpUser):
    tasks = {AddUpdateTaskSet: 1, SearchTaskSet: 1, search_large_title: 1, MergeSubmissionUser: 1}
    wait_time = between(3, 5)

    def on_start(self):
        token = get_token(self)
        self.client.headers = {
            "authorization": f"Bearer {token}", 'Content-type': 'application/json'}
        self.client.submission = new_iswc_submission()
        res = self.client.post(
            "/submission", data=json.dumps(self.client.submission), name="Add submission")
        self.client.submission["preferredIswc"] = json.loads(
            res.content)["verifiedSubmission"]["iswc"]


@events.quitting.add_listener
def _(environment, **kw):
    failures = []
    passed = []
    environment.process_exit_code = 0
    if environment.stats.get("Add submission", "POST").avg_response_time > 3000:
        environment.process_exit_code = 1
        failures.append(('Add submission',
                         f"Test failed due to Add submission average > 3s: {environment.stats.get('Add submission', 'POST').avg_response_time}"))
    else:
        passed.append(
            f"Add submission average < 3s: {environment.stats.get('Add submission', 'POST').avg_response_time}")

    if environment.stats.get("Search agency/workcode", "GET").avg_response_time > 1500:
        environment.process_exit_code = 1
        failures.append(('Search agency/workcode',
                         f"Test failed due to Search agency/workcode average > 1.5s: {environment.stats.get('Search agency/workcode', 'GET').avg_response_time}"))
    else:
        passed.append(
            f"Search agency/workcode average < 1.5s: {environment.stats.get('Search agency/workcode', 'GET').avg_response_time}")

    if environment.stats.get("Search iswc", "GET").avg_response_time > 1500:
        environment.process_exit_code = 1
        failures.append(('Search iswc',
                         f"Test failed due to Search iswc average > 1.5s: {environment.stats.get('Search iswc', 'GET').avg_response_time}"))
    else:
        passed.append(
            f"Search iswc average < 1.5s: {environment.stats.get('Search iswc', 'GET').avg_response_time}")

    if environment.stats.get("Search title/contributor", "POST").avg_response_time > 1500:
        environment.process_exit_code = 1
        failures.append(('Search title/contributor',
                         f"Test failed due to Search title/contributor average > 1.5s: {environment.stats.get('Search title/contributor', 'POST').avg_response_time}"))
    else:
        passed.append(
            f"Search title/contributor < 1.5s: {environment.stats.get('Search title/contributor', 'POST').avg_response_time}")

    if environment.stats.get("Search large title", "POST").avg_response_time > 15000:
        environment.process_exit_code = 1
        failures.append(('Search large title',
                         f"Test failed due to Search large title average > 15s: {environment.stats.get('Search large title', 'POST').avg_response_time}"))
    else:
        passed.append(
            f"Search large title < 15s: {environment.stats.get('Search large title', 'POST').avg_response_time}")

    if environment.stats.get("Update submission", "PUT").avg_response_time > 3000:
        environment.process_exit_code = 1
        failures.append(('Update submission',
                         f"Test failed due to Update submission average > 3s: {environment.stats.get('Update submission', 'PUT').avg_response_time}"))
    else:
        passed.append(
            f"Update submission < 3s: {environment.stats.get('Update submission', 'PUT').avg_response_time}")

    if environment.stats.get("Add batch submission", "POST").avg_response_time > 5000:
        environment.process_exit_code = 1
        failures.append(('Add batch submission',
                         f"Test failed due to Add batch submission average > 5s: {environment.stats.get('Add batch submission', 'POST').avg_response_time}"))
    else:
        passed.append(
            f"Add batch submission < 5s: {environment.stats.get('Add batch submission', 'POST').avg_response_time}")
    
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
