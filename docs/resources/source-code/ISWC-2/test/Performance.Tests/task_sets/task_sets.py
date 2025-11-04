import json
import random
from time import sleep

from locust import TaskSet, task, between, SequentialTaskSet
from data.existing_dev_data import large_title_search
from data.submission import new_iswc_submission
from utils.utils import get_new_title


class SearchTaskSet(SequentialTaskSet):
    wait_time = between(0, 1)

    @task
    def wait_for_sub(self):
        sleep(5)

    @task
    def search_iswc(self):
        self.client.get(f"/iswc/searchByIswc?iswc={self.client.submission['preferredIswc']}",
                        name="Search iswc")

    @task
    def search_agency_workcode(self):
        parameters = {
            "agency": self.client.submission["agency"], "workcode": self.client.submission["workcode"]}
        self.client.get(f"/iswc/searchByAgencyWorkCode",
                        params=parameters, name="Search agency/workcode")

    @task
    def search_title_contributor(self):
        search_model = {
            "titles": [{"title": self.client.submission["originalTitle"], "type": 1}],
            "interestedParties": [random.choice(self.client.submission["interestedParties"])]
        }
        self.client.post(f"/iswc/searchByTitleAndContributor",
                         name="Search title/contributor", data=json.dumps(search_model))


class AddUpdateTaskSet(TaskSet):

    @task(3)
    def add_new_submission(self):
        self.client.submission = new_iswc_submission()
        res = self.client.post(
            "/submission", data=json.dumps(self.client.submission), name="Add submission")
        self.client.submission["preferredIswc"] = json.loads(
            res.content)["verifiedSubmission"]["iswc"]

    @task(1)
    def batch_add_submission(self):
        batch = [
            {
                "submissionId": 1,
                "submission": new_iswc_submission()
            },
            {
                "submissionId": 2,
                "submission": new_iswc_submission()
            },
            {
                "submissionId": 3,
                "submission": new_iswc_submission()
            }
        ]
        self.client.post("/submission/batch",
                         data=json.dumps(batch), name="Add batch submission")

    @task(2)
    def update_submission(self):
        self.client.submission["otherTitles"] = [{
            "title": get_new_title(),
            "type": 2
        }]
        self.client.put(f"/submission?preferredIswc={self.client.submission['preferredIswc']}",
                        data=json.dumps(self.client.submission), name="Update submission")


@task
def search_large_title(l):
    search_model = {
        "titles": [
            {
                "title": random.choice(large_title_search),
                "type": 1
            }],
        "interestedParties": []
    }
    l.client.post(f"/iswc/searchByTitleAndContributor",
                  name="Search large title", data=json.dumps(search_model))


class AddBatchTaskSet(TaskSet):

    @task(1)
    def batch_add_submission(self):
        batch = []
        for i in range(10):
            batch.append({
                "submissionId": i,
                "submission": new_iswc_submission()
            })
        self.client.post("/submission/batch",
                         data=json.dumps(batch), name="Add batch submission")


class UpdateBatchTaskSet(TaskSet):
    def on_start(self):
        self.client.submissions = []
        for i in range(10):
            submission = new_iswc_submission()
            res = self.client.post(
                "/submission", data=json.dumps(submission), name="Add submission")
            submission["preferredIswc"] = json.loads(
                res.content)["verifiedSubmission"]["iswc"]
            self.client.submissions.append({
                "submissionId": i,
                "submission": submission
            })
            sleep(2)

    @task
    def batch_update_submissions(self):
        for sub in self.client.submissions:
            sub['submission']['originalTitle'] = get_new_title()
        self.client.put("/submission/batch", data=json.dumps(
            self.client.submissions), name="Update batch submission")

class MergeChainTaskSet(SequentialTaskSet):

    submissions_for_merge = []
    chainlength = 5

    def on_start(self):
        i = 0
        for i in range(self.chainlength):
            self.client.submission = new_iswc_submission()
            res = self.client.post(
                "/submission", data=json.dumps(self.client.submission), name="Add submission")
            self.client.submission["preferredIswc"] = json.loads(
                res.content)["verifiedSubmission"]["iswc"]
            self.submissions_for_merge.append({
                "submissionId": self.submissions_for_merge.count,
                "submission": self.client.submission
            })  
            i+=1          
            sleep(2)
        j=0
        for j in range(self.chainlength - 1):
            body = { "iswcs" : [self.submissions_for_merge[j]["submission"]["preferredIswc"]]}
            self.client.post(f"/iswc/merge?preferredIswc={self.submissions_for_merge[j+1]['submission']['preferredIswc']}&agency={self.submissions_for_merge[j+1]['submission']['agency']}",
                            data=json.dumps(body), name="Merge submissions")
            j+=1
            sleep(5)
            
    @task
    def update_chain_base(self):
        self.client.submission = self.submissions_for_merge[0]["submission"]
        self.client.submission["otherTitles"] = [{
            "title": get_new_title(),
            "type": 2
        }]
        self.client.put(f"/submission?preferredIswc={self.client.submission['preferredIswc']}",
                        data=json.dumps(self.client.submission), name="Update chain base")
        



            


