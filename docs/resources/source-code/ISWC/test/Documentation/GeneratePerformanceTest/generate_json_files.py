import datetime
import json
import os
import pyodbc
import shutil
import string
import random


## parameters ##

agency_id = '128'
total_submission_count = 10000
file_batch_size = int(total_submission_count / 20)
car_or_cur = 'CAR'

################

shutil.rmtree('.\\files', ignore_errors=True)
os.makedirs('.\\files')

sql = """
    select distinct top {}
    wi.AgencyID as 'agency',
    wi.AgencyWorkCode as 'workcode',
    wi.SourceDatabase as 'sourcedb',
    t.Title as 'originalTitle',
    (
        select c.IPNameNumber as 'nameNumber', trim(r.Code) as 'role' from iswc.Creator c
        join Lookup.RoleType r on r.RoleTypeID = c.RoleTypeID
        where (c.WorkInfoID = wi.WorkInfoID and c.IswcID = wi.IswcID and c.IPNameNumber IS NOT NULL and NOT r.RoleTypeID IN (5,6))
        for json path
    ) as interestedParties,
    'DOM' as category,
    0 as disambiguation,
    Iswc as preferredIswc
    from iswc.WorkInfo wi
    join iswc.Iswc i on wi.IswcID = i.IswcID
    join iswc.Creator c on wi.IswcID = c.IswcID and c.WorkInfoID = wi.WorkInfoID
    join iswc.Title t on wi.IswcID = t.IswcID and t.WorkInfoID = wi.WorkInfoID
    where wi.AgencyID = '{}' and t.TitleTypeID = 2
"""

cursor = (pyodbc
          .connect('DRIVER={ODBC Driver 17 for SQL Server};Server=cisaciswcwedev.database.windows.net;DATABASE=ISWC;UID=performancetest_readonly;PWD=fXyLk8dvZKk4c6At;')
          .cursor()
          .execute(sql.format(total_submission_count, agency_id)))

columns = [column[0] for column in cursor.description]

submissions = []

for idx, row in enumerate(cursor.fetchall()):
    submission = dict(zip(columns, row))
    if(submission['interestedParties'] != None):
        submission['submissionId'] = idx
        submission['interestedParties'] = json.loads(
            submission['interestedParties'])
        submission['disambiguation'] = bool(submission['disambiguation'])

        if car_or_cur == 'CAR':
            submission['workcode'] = ''.join(
                [random.choice(string.ascii_lowercase + string.digits) for i in range(19)])
            submission['preferredIswc'] = ''

        submissions.append(submission)

for idx in range(0, len(submissions), file_batch_size):
    batch = submissions[idx:idx + file_batch_size]
    batch_file = {
        "$schema": "./iswc_schema.json",
        "fileHeader": {
            "submittingAgency": "128",
            "submittingSourcedb": 128,
            "fileCreationDateTime": datetime.datetime.utcnow().isoformat(),
            "receivingAgency": "315"
        }
    }
    if car_or_cur == 'CAR':
        batch_file['addSubmissions'] = batch
    else:
        batch_file['updateSubmissions'] = batch

    file_name = 'perf_test_' + str(idx) + '.json'

    with open('.\\files\\' + file_name, 'w') as outfile:
        json.dump(batch_file, outfile)
