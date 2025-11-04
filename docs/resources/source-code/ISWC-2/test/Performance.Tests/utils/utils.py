import json
import logging

from xml.dom import minidom
from faker import Faker
import datetime
from xml.etree.ElementTree import Element, SubElement, tostring

fake = Faker()


def get_token(l):
    headers = {"Content-Type": "application/x-www-form-urlencoded; charset=UTF-8"}
    with open('settings.json', 'r') as settings:
        data = json.load(settings)
        secret = data['AzureKeyVaultSecret-ISWC-Secret-IswcApiManagement']
        req = {
            "grant_type": "client_credentials",
            "client_id": "iswcapimanagement",
            "client_secret": secret,
            "scope": "iswcapi",
            "AgentID": "000"
        }
        res = l.client.post(f'{data["AzureKeyVaultSecret-ISWC-BaseAddress-IswcApi"]}/connect/token', headers=headers,
                            auth=(req["client_id"], secret), data=req)
        return json.loads(res.content)["access_token"]


def get_new_title():
    return f"{fake.pystr(min_chars=8, max_chars=10)} {fake.pystr(min_chars=8, max_chars=10)} {fake.pystr(min_chars=8, max_chars=10)}"


def get_new_workcode():
    return fake.pystr(min_chars=12, max_chars=20)


def generate_results_xml(passed, failures):
    timestamp = datetime.datetime.fromtimestamp(
        datetime.datetime.now().timestamp()).isoformat()

    suites = Element('testsuites', {'name': 'Locust',
                                    'time': '0.000',
                                    'tests': str((len(failures) + len(passed))),
                                    'failures': str(len(failures))
                                    })
    suite = SubElement(suites, 'testsuite', {
        'name': 'Locust',
        'timestamp': timestamp,
        'tests': str((len(failures) + len(passed))),
        'time': '0.000',
        'failures': str(len(failures))
    })
    for fail in failures:
        test_case = SubElement(suite, 'testcase', {
            'name': f'Locust - {fail[0]}',
            'classname': 'Locust',
            'time': '0.000'
        })
        SubElement(test_case, 'failure', {
            'message': fail[1],
            'type': 'AverageTimeFailure'
        })
        logging.error(f'{fail[0]}: {fail[1]}')
    for test in passed:
        SubElement(suite, 'testcase', {
            'name': f'Locust - {test}',
            'classname': 'Locust',
            'time': '0.000'
        })
    with open("locust.xml", "w+") as output:
        output.write(minidom.parseString(
            tostring(suites, 'utf-8')).toprettyxml(indent="  "))
