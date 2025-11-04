import uuid
from datetime import datetime, timedelta
from io import StringIO

import pandas as pd
from pyspark.sql import SparkSession
from pyspark.sql.context import SQLContext
from pyspark.sql.functions import *
from pyspark.sql.types import (BooleanType, IntegerType, LongType, StringType,
                               StructField, StructType, TimestampType)
from reporting.reports.report_output import ReportOutput
from reporting.services.data_factory_service import DataFactoryService


class PotentialDuplicates:
    spark = SparkSession
    data_factory_service = DataFactoryService
    pipeline_name = 'PotentialDuplicates_RefreshCache'
    container = '/mnt/reporting/PotentialDuplicates'
    report_output = ReportOutput

    def __init__(self,  dependencies: dict):
        self.spark = dependencies['spark']
        self.data_factory_service = DataFactoryService(
            dependencies['subscription_id'], dependencies['aad_client_id'], dependencies['aad_secret'],
            dependencies['aad_tenant'], dependencies['resource_group_name'], dependencies['data_factory_name'],
            self.pipeline_name)
        self.report_output = ReportOutput()
        self.dbutils = dependencies['dbutils']

    def create_extract(self, parameters: dict):
        agency_code = parameters['AgencyCode']
        from_date = parameters['FromDate']
        to_date = parameters['ToDate']
        consider_original_titles_only = parameters['ConsiderOriginalTitlesOnly'] == 'True'
        temp_file_id = str(uuid.uuid1())

        iswc_data = (self.spark.read
                     .parquet('/mnt/reporting/PotentialDuplicates/ISWC_ISWC.parquet'))

        title_data = (self.spark.read
                      .parquet('/mnt/reporting/PotentialDuplicates/ISWC_Title.parquet'))

        workinfo_data = (self.spark.read
                         .parquet('/mnt/reporting/PotentialDuplicates/ISWC_WorkInfo.parquet'))

        creator_data = (self.spark.read
                        .parquet('/mnt/reporting/PotentialDuplicates/ISWC_Creator.parquet'))

        disambiguated_iswc_data = (self.spark.read
                                   .parquet('/mnt/reporting/PotentialDuplicates/ISWC_DisambiguationISWC.parquet'))

        merged_iswc_data = (self.spark.read
                            .parquet('/mnt/reporting/PotentialDuplicates/ISWC_ISWCLinkedTo.parquet'))

        workinfo_data.createOrReplaceTempView("Workinfo")
        iswc_data.createOrReplaceTempView('Iswc')
        title_data.createOrReplaceTempView('Title')
        creator_data.createOrReplaceTempView('Creator')
        disambiguated_iswc_data.createOrReplaceTempView('DisambiguationISWC')
        merged_iswc_data.createOrReplaceTempView('ISWCLinkedTo')

        self.spark.sql("""
            SELECT i.IswcId, i.Iswc, w.WorkinfoId, i.CreatedDate, i.AgencyId AS IswcAgencyId,
                t.StandardizedTitle, t.Title, t.TitleTypeID,
                w.AgencyId, w.AgencyWorkCode, w.IswcEligible, w.LastModifiedDate, w.WorkinfoId, w.Disambiguation,
                concat_ws(',', collect_list(c.IPBaseNumber)) AS IPBaseNumbers
            FROM Iswc i
            JOIN Workinfo w ON w.IswcId = i.IswcId
            JOIN Title t ON t.WorkinfoId = w.WorkinfoId
            JOIN Creator c on c.WorkInfoId = w.WorkInfoId
            WHERE w.Status = 1 AND t.Status = 1 AND c.Status = 1
            GROUP BY i.IswcId, i.Iswc, w.WorkinfoId, i.CreatedDate, IswcAgencyId,
                t.StandardizedTitle, t.Title, t.TitleTypeID,
                w.AgencyId, w.AgencyWorkCode, w.IswcEligible, w.LastModifiedDate, w.WorkinfoId, w.Disambiguation
        """).createOrReplaceTempView('Submissions')

        duplicates_query = """
            SELECT l.IswcId AS LeftIswcId, r.IswcId AS RightIswcId, l.CreatedDate, l.IswcAgencyId FROM Submissions l
            JOIN Submissions r ON l.StandardizedTitle = r.StandardizedTitle
            WHERE l.IPBaseNumbers = r.IPBaseNumbers
            AND l.IswcId <> r.IswcId
            AND l.IswcEligible = 1 AND r.IswcEligible = 1
        """

        if consider_original_titles_only:
            duplicates_query = f'{duplicates_query} AND l.TitleTypeId = 2 AND r.TitleTypeId = 2 AND l.IswcID = (select first(IswcID, false) from (select * from Submissions s where s.StandardizedTitle = l.StandardizedTitle and s.IPBaseNumbers = l.IPBaseNumbers ORDER BY s.CreatedDate NULLS LAST))'
        else:
            duplicates_query = f'{duplicates_query} AND l.IswcID = (select first(IswcID, false) from Submissions s where s.StandardizedTitle = l.StandardizedTitle and s.IPBaseNumbers = l.IPBaseNumbers ORDER BY s.CreatedDate NULLS LAST)'

        duplicates = self.spark.sql(
            duplicates_query)

        if agency_code:
            duplicates = duplicates.filter(col('IswcAgencyId') == agency_code)

        if from_date:
            if not to_date:
                to_date = datetime.now()
            to_date = to_date + timedelta(days=1)
            duplicates = duplicates.filter(col('CreatedDate') > from_date).filter(
                col('CreatedDate') < to_date)

        duplicates.createOrReplaceTempView('Duplicates')

        duplicates_data = self.spark.sql("""
            SELECT DISTINCT
                l.IswcId as IswcId,
                l.Iswc as Iswc,
                l.Title as IswcTitle,
                l.TitleTypeID as IswcTitleTypeID,
                l.AgencyId as IswcAgencyId,
                l.AgencyWorkCode as IswcAgencyWorkCode,
                l.LastModifiedDate as IswcWorkInfoLastModifiedDate,
                l.IswcEligible as IswcEligible,
                l.WorkinfoID as IswcWorkinfoID,
                l.Disambiguation as IswcDisambiguation,
                c.IPBaseNumber as IPBaseNumber,
                r.IswcId as IswcToBeMergedId,
                r.Iswc as IswcToBeMerged,
                r.Title as IswcToBeMergedTitle,
                r.TitleTypeID as IswcToBeMergedTitleTypeID,
                r.AgencyId as IswcToBeMergedAgencyId,
                r.AgencyWorkCode as IswcToBeMergedAgencyWorkCode,
                r.LastModifiedDate as IswcToBeMergedWorkInfoLastModifiedDate,
                r.IswcEligible as IswcToBeMergedEligible,
                r.WorkinfoID as IswcToBeMergedWorkinfoID,
                r.Disambiguation as IswcToBeMergedDisambiguation
            from Duplicates d
            JOIN Submissions l ON d.LeftIswcId = l.IswcId
            JOIN Submissions r ON d.RightIswcId = r.IswcId
            JOIN Creator c on c.WorkInfoId = l.WorkInfoId
        """)

        schema = StructType(
            [StructField("IswcId", LongType(), True),
             StructField("Iswc", StringType(), True),
             StructField("IswcTitle", StringType(), True),
             StructField("IswcTitleTypeID", IntegerType(), True),
             StructField("IswcAgencyId", StringType(), True),
             StructField("IswcAgencyWorkCode", StringType(), True),
             StructField("IswcWorkInfoLastModifiedDate",
                         TimestampType(), True),
             StructField("IswcEligible", BooleanType(), True),
             StructField("IswcWorkinfoID", LongType(), True),
             StructField("IswcDisambiguation", BooleanType(), True),
             StructField("IPBaseNumber", StringType(), True),
             StructField("IswcToBeMergedId", LongType(), True),
             StructField("IswcToBeMerged", StringType(), True),
             StructField("IswcToBeMergedTitle", StringType(), True),
             StructField("IswcToBeMergedTitleTypeID", IntegerType(), True),
             StructField("IswcToBeMergedAgencyId", StringType(), True),
             StructField("IswcToBeMergedAgencyWorkCode", StringType(), True),
             StructField("IswcToBeMergedWorkInfoLastModifiedDate",
                         TimestampType(), True),
             StructField("IswcToBeMergedEligible", BooleanType(), True),
             StructField("IswcToBeMergedWorkinfoID", LongType(), True),
             StructField("IswcToBeMergedDisambiguation", BooleanType(), True)]
        )

        duplicates_data = SQLContext(self.spark.sparkContext).createDataFrame(
            duplicates_data.rdd.repartition(32), schema)

        duplicates_data.write.parquet(
            '/mnt/reporting/PotentialDuplicates/tmp/' + temp_file_id)

        duplicates_data = self.spark.read.parquet(
            '/mnt/reporting/PotentialDuplicates/tmp/' + temp_file_id)

        duplicates_data.createOrReplaceTempView('DuplicatesData')

        disambiguated_iswc_data = self.spark.sql("""
            SELECT i.Iswc, i.WorkinfoID FROM DisambiguationISWC i
            JOIN DuplicatesData d ON (i.Iswc = d.Iswc AND i.WorkinfoID = d.IswcToBeMergedWorkinfoID)
            OR (i.Iswc = d.IswcToBeMerged AND i.WorkinfoID = d.IswcWorkinfoID)
        """).collect()

        merged_iswc_data = self.spark.sql("""
            SELECT i.IswcID, i.LinkedToIswc FROM ISWCLinkedTo i
            JOIN DuplicatesData d ON (d.IswcId = i.IswcID AND i.LinkedToIswc = d.IswcToBeMerged)
            OR (i.LinkedToIswc = d.Iswc AND d.IswcToBeMergedId = i.IswcID)
        """).collect()

        merged_iswc_data = list(dict.fromkeys(merged_iswc_data))

        duplicates_grouped_by_iswc = {}

        for partition in duplicates_data.rdd.mapPartitions(lambda part: [list(part)]).toLocalIterator():
            for duplicate_record in partition:
                duplicate_row = duplicate_record.asDict()
                iswc_id = f"{duplicate_row['IswcId']}|{duplicate_row['IswcToBeMergedId']}"
                duplicate_value = duplicates_grouped_by_iswc.get(iswc_id, [])
                if(len(duplicate_value) < 101):
                    duplicate_value.append(duplicate_row)
                    duplicates_grouped_by_iswc[iswc_id] = duplicate_value

        new_duplicates_grouped_by_iswc = {}

        for key, value in duplicates_grouped_by_iswc.items():
            key_values = key.split('|')
            key_values.sort()
            reversed_key = '|'.join(key_values)

            if reversed_key in new_duplicates_grouped_by_iswc:
                continue

            new_duplicates_grouped_by_iswc[reversed_key] = value

        csv_rows = []
        for iswc, value in new_duplicates_grouped_by_iswc.items():
            iswc_society_work_codes = []
            iswc_society_work_codes_to_be_merged = []
            ipi_base_numbers = []
            iswc_original_title = {
                'Title': '',
                'LastModifiedDate': datetime.min
            }
            iswc_to_be_merged_original_title = {
                'Title': '',
                'LastModifiedDate': datetime.min
            }
            iswc_other_titles = []
            iswc_to_be_merged_other_titles = []
            eligible_agencies = []
            ineligible_agencies = []

            iswcs_are_disambiguated = False
            iswcs_are_merged = False

            if any(record['IswcDisambiguation'] or record['IswcToBeMergedDisambiguation'] for record in value):
                if(any((value[0]['Iswc'] == disambiguated_iswc['Iswc'] and value[0]['IswcToBeMergedWorkinfoID'] == disambiguated_iswc['WorkinfoID']) or
                       (value[0]['IswcToBeMerged'] == disambiguated_iswc['Iswc'] and value[0]['IswcWorkinfoID'] == disambiguated_iswc['WorkinfoID']) for disambiguated_iswc in disambiguated_iswc_data)):
                    iswcs_are_disambiguated = True

            if(any((value[0]['Iswc'] == merged_iswcs['LinkedToIswc'] and value[0]['IswcToBeMergedId'] == merged_iswcs['IswcID']) or
                   (value[0]['IswcToBeMerged'] == merged_iswcs['LinkedToIswc'] and value[0]['IswcId'] == merged_iswcs['IswcID']) for merged_iswcs in merged_iswc_data)):
                iswcs_are_merged = True

            if iswcs_are_disambiguated or iswcs_are_merged:
                continue

            for record in value:
                if record['IswcEligible'] == 1 and record['IswcAgencyId'] not in eligible_agencies:
                    eligible_agencies.append(record['IswcAgencyId'])

                if record['IswcToBeMergedEligible'] == 1 and record['IswcToBeMergedAgencyId'] not in eligible_agencies:
                    eligible_agencies.append(record['IswcToBeMergedAgencyId'])

                if record['IswcEligible'] != 1 and record['IswcAgencyId'] not in ineligible_agencies:
                    ineligible_agencies.append(record['IswcAgencyId'])

                if record['IswcToBeMergedEligible'] != 1 and record['IswcToBeMergedAgencyId'] not in eligible_agencies:
                    ineligible_agencies.append(
                        record['IswcToBeMergedAgencyId'])

                work_code = f"{record['IswcAgencyId']}|{record['IswcAgencyWorkCode']}"
                if work_code not in iswc_society_work_codes:
                    iswc_society_work_codes.append(work_code)

                work_code_to_be_merged = f"{record['IswcToBeMergedAgencyId']}|{record['IswcToBeMergedAgencyWorkCode']}"
                if work_code_to_be_merged not in iswc_society_work_codes_to_be_merged:
                    iswc_society_work_codes_to_be_merged.append(
                        work_code_to_be_merged)

                if record['IswcEligible'] != 1:
                    continue

                if record['IPBaseNumber'] not in ipi_base_numbers:
                    ipi_base_numbers.append(record['IPBaseNumber'])

                if record['IswcTitleTypeID'] == 2 and record['IswcWorkInfoLastModifiedDate'] > iswc_original_title['LastModifiedDate']:
                    iswc_original_title['Title'] = record['IswcTitle']
                    iswc_original_title['LastModifiedDate'] = record['IswcWorkInfoLastModifiedDate']

                if record['IswcToBeMergedTitleTypeID'] == 2 and record['IswcToBeMergedWorkInfoLastModifiedDate'] > iswc_to_be_merged_original_title['LastModifiedDate']:
                    iswc_to_be_merged_original_title['Title'] = record['IswcToBeMergedTitle']
                    iswc_to_be_merged_original_title['LastModifiedDate'] = record['IswcToBeMergedWorkInfoLastModifiedDate']

                if record['IswcTitleTypeID'] != 2 and record['IswcTitle'] not in iswc_other_titles:
                    iswc_other_titles.append(record['IswcTitle'])

                if record['IswcToBeMergedTitleTypeID'] != 2 and record['IswcToBeMergedTitle'] not in iswc_to_be_merged_other_titles:
                    iswc_to_be_merged_other_titles.append(
                        record['IswcToBeMergedTitle'])

            categorisation = ''

            if(len(eligible_agencies) == 1 and len(ineligible_agencies) == 0):
                categorisation = 'Single Society Involved'
            if(len(eligible_agencies) == 1 and len(ineligible_agencies) > 0):
                categorisation = 'Single ISWC Eligible Society Involved'
            if(len(eligible_agencies) > 1):
                categorisation = 'Multiple ISWC Eligible Societies Involved'

            csv_row = {
                'ISWC': value[0]['Iswc'],
                'ISWC To Be Merged': value[0]['IswcToBeMerged'],
                'Categorisation': categorisation,
                'ISWC Society Work Codes': ','.join(iswc_society_work_codes),
                'ISWC To Be Merged Society Work Codes': ','.join(iswc_society_work_codes_to_be_merged),
                'IPI Base Numbers': ','.join(ipi_base_numbers),
                'ISWC Original Title': iswc_original_title['Title'],
                'ISWC To Be Merged Original Title': iswc_to_be_merged_original_title['Title'],
                'ISWC Other Titles': ','.join(iswc_other_titles),
                'ISWC To Be Merged Other Titles': ','.join(iswc_to_be_merged_other_titles)
            }

            csv_rows.append(csv_row)

        self.dbutils.fs.rm(
            '/mnt/reporting/PotentialDuplicates/tmp/' + temp_file_id, True)

        file_name = self.report_output.get_output_file_name(parameters)
        return (parameters['SubmittingAgencyCode'], file_name, StringIO(self.to_csv(pd.DataFrame(csv_rows))))

    def to_csv(self, df):
        extract_data_csv = df.to_csv(index=False)
        return extract_data_csv

    def execute_report(self, parameters: dict):
        potential_duplicates_create_extract_mode = parameters[
            'PotentialDuplicatesCreateExtractMode'] == 'True'
        most_recent_version = parameters['MostRecentVersion'] == 'True'
        consider_original_titles_only = parameters['ConsiderOriginalTitlesOnly']

        if(potential_duplicates_create_extract_mode):
            return self.create_extract(parameters)
        else:
            date_format = '%m/%d/%Y %I:%M:%S %p'
            from_date_param = parameters['FromDate'].strftime(
                date_format) if parameters['FromDate'] != '' else ''
            to_date_param = parameters['ToDate'].strftime(
                date_format) if parameters['ToDate'] != '' else ''

            self.data_factory_service.new_pipeline_run(
                parameters['AgencyCode'], most_recent_version, parameters['Email'], consider_original_titles_only, from_date_param, to_date_param, parameters['SubmittingAgencyCode'])

            return (None, None, None)
