<a id="_Hlk15297321"></a>  
![A close up of a sign

Description automatically generated](1.png)

CISAC

ISWC Database JSON Format  

# <a id="_Toc399421500"></a><a id="_Toc399422154"></a><a id="_Toc485799633"></a><a id="_Toc66716817"></a>Document Control

## <a id="_Toc158527933"></a><a id="_Toc399421501"></a><a id="_Toc399422155"></a><a id="_Toc485799634"></a><a id="_Toc66716818"></a>Change Record

Date

Person

Version/Reference

6th Dec 2019

John Corley, 

Curnan Reidy  

V1\.0 / Initial Draft for review by design team

16th Dec 2019

John Corley

V1\.1 / Added additional info to Acknowledgement and Notification transactions

08 Jan 2020

John Corley

V1\.3 / Incorporated feedback from second design review workshop

12 Jan 2020

John Corley

V1\.4 / Incorporated final design team suggested changes and issued for sign off by SG

<a id="_Toc158527934"></a><a id="_Toc399421502"></a><a id="_Toc399422156"></a>15th Mar 2021

Curnan Reidy

V1\.5 / Added section for ISWCs Allocated from Local Ranges

27th May 2021

John Corley

V1\.6 / Minor corrections based on feedback from APRA 

## <a id="_Toc485799635"></a><a id="_Toc66716819"></a>Reviewers

<a id="_Toc158527935"></a><a id="_Toc399421503"></a><a id="_Toc399422157"></a><a id="_Toc485799636"></a>

Katrien Tielemans

Bolmar Carrasquilla 

Ed Osanani 

Bolmar Carrasquilla

Tim Carmichael

Didier Roy 

Hanna Mazur 

José Macarro

Sylvain Masson 

Sylvain Piat 

John Corley

Niamh McGarry

Declan Rudden

Curnan Reidy

## <a id="_Toc66716820"></a>Distribution

Reviewers

## <a id="_Toc158527936"></a><a id="_Toc399421504"></a><a id="_Toc399422158"></a><a id="_Toc485799637"></a><a id="_Toc66716821"></a>Approval

This document was approved electronically via email by the following people on the following dates:

Date/Time

Person

Note

# <a id="_Toc66716822"></a>Table of Contents<a id="_Toc485799638"></a>

[Document Control	2](#_Toc66716817)

[Change Record	2](#_Toc66716818)

[Reviewers	2](#_Toc66716819)

[Distribution	3](#_Toc66716820)

[Approval	3](#_Toc66716821)

[Table of Contents	4](#_Toc66716822)

[1	Introduction	6](#_Toc66716823)

[What does this document contain?	6](#_Toc66716824)

[Who should read this document?	6](#_Toc66716825)

[References	6](#_Toc66716826)

[2	Introduction to the ISWC JSON Data Format	7](#_Toc66716827)

[2\.1\. File Structure	7](#_Toc66716828)

[2\.1\.1\. Developer Artefacts	7](#_Toc66716829)

[2\.1\.2\. Schema and Validation	8](#_Toc66716830)

[2\.1\.3\. File Header	9](#_Toc66716831)

[2\.1\.4\. Transaction Groups	9](#_Toc66716832)

[2\.2\. File Naming, Transfer and Encoding	10](#_Toc66716833)

[2\.2\.1\. File Encoding	10](#_Toc66716834)

[2\.2\.2\. File Transfer and Location	10](#_Toc66716835)

[2\.2\.3\. File Naming Convention	10](#_Toc66716836)

[2\.3\. ISWCs Allocated from Local Ranges	10](#_Toc66716837)

[3	Transactions	12](#_Toc66716838)

[3\.1\. AddSubmission	12](#_Toc66716839)

[3\.1\.1\. Transaction Level Validation	14](#_Toc66716840)

[3\.1\.2\. Field Level Validation	14](#_Toc66716841)

[3\.1\.3\. Sample	15](#_Toc66716842)

[3\.2\. UpdateSubmission	16](#_Toc66716843)

[3\.2\.1\. Transaction Level Validation	18](#_Toc66716844)

[3\.2\.2\. Field Level Validation	18](#_Toc66716845)

[3\.2\.3\. Sample	19](#_Toc66716846)

[3\.3\. DeleteSubmission	19](#_Toc66716847)

[3\.3\.1\. Transaction Level Validation	20](#_Toc66716848)

[3\.3\.2\. Field Level Validation	21](#_Toc66716849)

[3\.3\.3\. Sample	21](#_Toc66716850)

[3\.4\. SearchByIswcSubmission	22](#_Toc66716851)

[3\.4\.1\. Transaction Level Validation	22](#_Toc66716852)

[3\.4\.2\. Field Level Validation	23](#_Toc66716853)

[3\.4\.3\. Sample	23](#_Toc66716854)

[3\.5\. SearchByAgencyWorkCodeSubmission 	23](#_Toc66716855)

[3\.5\.1\. Transaction Level Validation	24](#_Toc66716856)

[3\.5\.2\. Field Level Validation	24](#_Toc66716857)

[3\.5\.3\. Sample	25](#_Toc66716858)

[3\.6\. MergeSubmission	25](#_Toc66716859)

[3\.6\.1\. Transaction Level Validation	26](#_Toc66716860)

[3\.6\.2\. Field Level Validation	26](#_Toc66716861)

[3\.6\.3\. Sample	26](#_Toc66716862)

[3\.7\. UpdateWorkflowTask	27](#_Toc66716863)

[3\.7\.1\. Transaction Level Validation	27](#_Toc66716864)

[3\.7\.2\. Field Level Validation	27](#_Toc66716865)

[3\.7\.3\. Sample	28](#_Toc66716866)

[3\.8\. Acknowledgement Record Format	28](#_Toc66716867)

[3\.8\.1\. Transaction Level Validation	29](#_Toc66716868)

[3\.8\.2\. Field Level Validation	29](#_Toc66716869)

[3\.8\.3\. Sample	30](#_Toc66716870)

[3\.9\. Notification Record Format	31](#_Toc66716871)

[3\.9\.1\. Transaction Level Validation	33](#_Toc66716872)

[3\.9\.2\. Field Level Validation	33](#_Toc66716873)

[3\.9\.3\. Sample	33](#_Toc66716874)

[Appendix A – Open and Closed Items	35](#_Toc66716875)

1. <a id="_Toc399422159"></a><a id="_Toc399421505"></a><a id="_Toc158527937"></a><a id="_Toc53481878"></a><a id="_Toc399422160"></a><a id="_Toc399421506"></a><a id="_Toc158527938"></a><a id="_Toc485799639"></a><a id="_Toc66716823"></a>Introduction

## <a id="_Toc158527939"></a><a id="_Toc399421507"></a><a id="_Toc399422161"></a><a id="_Toc485799640"></a><a id="_Toc66716824"></a>What does this document contain?

It provides the detailed design of the new JSON based file format that will be supported by the ISWC Database\.  This new JSON format is the recommended file exchange format for interacting with the ISWC Database, is consistent with the ISWC REST API and provides a more robust data exchange mechanism than the EDI file formats available \(backwards compatible and modern\)\.

## <a id="_Toc158527940"></a><a id="_Toc399421508"></a><a id="_Toc399422162"></a><a id="_Toc485799641"></a><a id="_Toc66716825"></a>Who should read this document?

CISAC development and project management personnel\. Society development and project management personnel\.  Spanish Point development team members\.   

## <a id="_Toc158527942"></a><a id="_Toc399421510"></a><a id="_Toc399422164"></a><a id="_Toc485799643"></a><a id="_Toc66716826"></a>References

Reference

Description

SPE\_20190806\_ISWC\_EDI\_FileFormat

EDI data exchange file format for new ISWC system 

SPE\_20190218\_ISWCDataModel\.docx

New ISWC Database Data Model

SPE\_20190520\_ISWC SOAP & REST Services\.docx

Specification for the REST based services that will be used by this portal\.

ISWC Database REST OpenAPI Swagger\.yaml

Definition for the new ISWC Database REST based API

SPE\_20190424\_MVPValidationRules\.docx

Detailed Validation Rules Specification for new ISWC Database

SPE\_20190424\_MVPMatchingRules\.docx

Detailed Matching Rules Specification for new ISWC Database

REQ\_20190212\_MVP To Be Business Rules\.xlsx

‘To Be’ Business Rules Requirements

1. <a id="_Toc66716827"></a>Introduction to the ISWC JSON Data Format

The ISWC JSON data format is functionally equivalent to the modern ISWC EDI format described in “SPE\_20190806\_ISWC\_EDI\_FileFormat\.docx” in that it includes support for the following: 

- Additional disambiguation data
- Multi\-lingual character sets in work titles
- Merge transactions
- Derived works 

<a id="_Toc468452208"></a><a id="_Toc496111659"></a><a id="_Toc496111662"></a><a id="_Toc496111669"></a><a id="_Toc496111673"></a><a id="_Toc496111677"></a><a id="_Toc496111684"></a><a id="_Toc496111690"></a>

In addition, it has the following benefits over the modern ISWC EDI format:

- It is consistent with the ISWC REST API\.  This will enable societies to reuse components of their integration solution across both integration methods\. 
- It is schema based, enabling validation of the core message format, required fields, datatypes and list values at source\.  
- It is easier to process and manipulate in modern development tools\.

## <a id="_Toc66716828"></a>File Structure

The section describes the structure of the ISWC JSON Format and the developer artefacts that are included with this specification\.  

### <a id="_Toc66716829"></a>Developer Artefacts

This specification includes the following companion artefacts:

__File Name__

__Description__

__iswc\_schema\.json__

Schema that defines the valid ISWC JSON format\.

__iswc\_2019\-11\-25T18\-25\-43\_315\_128\_SampleSubmissions__

Sample ISWC JSON file that contains a set of ISWC transactions from an agency \(128\) to be processed by the ISWC Database \(315\) that conforms to the above schema\. 

__iswc\_2019\-11\-26T09\-30\-00\_128\_315\_SampleAcknowledgements__

Sample ISWC JSON file that contains a set of ISWC acknowledgement transactions from the ISWC database to an agency in response to the transactions sent in the previous file\. 

__iswc\_2019\-11\-26T09\-30\-00\_128\_315\_SampleNotifications__

Sample ISWC JSON file that contains a set of ISWC notifications from the ISWC database to an agency in response to transactions carried out\.

__Validator\.py__

Sample python application that validates all \.json files in the current folder against the iswc\_schema\.json document also stored in that folder\. 

### <a id="_Toc66716830"></a>Schema and Validation

All files should be validated against the schema, “iswc\_schema\.json” by the generator of the file\.  The ISWC database will validate all inbound files against the schema and any schema validation errors will result in the entire file being rejected\.  

The file structure is as follows:  

<a id="_bookmark4"></a>

Each file must consist of exactly one file header and zero or more transaction groups\.  Transaction groups include “addSubmissions”, “updateSubmissions”, “deleteSubmissions”, “acknowledgements” etc\. 

Each transaction group must contain zero or more transactions\.  E\.G\. the “addSubmissions” transaction group must contain zero or more “AddSubmission” type transactions\. 

### <a id="_Toc66716831"></a>File Header

The file header contains the required header level metadata\.  The following is the simplest valid, though not very useful ISWC database json message:

\{

    "$schema": "\./iswc\_schema\.json",

    "fileHeader": \{

        "submittingAgency": "128",

        "sunmittingSourcedb": 128,

        "fileCreationDateTime": "2019\-10\-01T18:25:43\.511Z",

        "receivingAgency": "315"

    \}

\}

### <a id="_bookmark5"></a><a id="_Toc66716832"></a>Transaction Groups

The following transaction groups are supported by the ISWC JSON format:

__Transaction Group__

__Description__

__Orchestration__

__addSubmissions__

Create new submissions\.  Each submission may be linked to an existing preferredISWC or a new preferredISWC may be generated for it

Expected from agencies to the ISWC Database

__updateSubmissions__

Update previously sent submissions, identified by agency, sourcedb, workcode

Expected from agencies to the ISWC Database

__deleteSubmissions__

Delete the agency submissions identified by agency, sourcedb and workcode from the provided preferredISWCs

Expected from agencies to the ISWC Database

__searchByIswcSubmissions__

Requests for the ISWC metadata for the provided preferred ISWCs

Expected from agencies to the ISWC Database

__searchByAgencyWorkCodeSubmissions__

Requests for the Preferred ISWCs for the provided set of Agency, Sourcedb and Workcode values

Expected from agencies to the ISWC Database

__mergeSubmissions__

Merge transactions \(merging of existing Preferred ISWCs together\) 

Expected from agencies to the ISWC Database

__updateWorkflowTasks__

Updates the status of existing workflow tasks

Expected from agencies to the ISWC Database

__acknowledgements__

Acknowledgements in response to transactions above

Expected from the ISWC Database to agencies

__notifications__

Notifications to agencies that are potentially impacted by a transaction processed by the ISWC database

Expected from the ISWC Database to agencies

## <a id="_Toc66716833"></a>File Naming, Transfer and Encoding 

### <a id="_Toc66716834"></a>File Encoding

All files will be encoded in UTF\-8\.

### <a id="_Toc66716835"></a>File Transfer and Location

The standard for transmission will be the Secure File Transfer Protocol \(SFTP\)\.  Each ISWC agency will push files to an agency specific secure folder in a central ISWC Database public SFTP site and pull files from that secure folder\. 

### <a id="_Toc66716836"></a>File Naming Convention

ISWC – identifies an ISWC file 

YYYY\-MM\-DDTHH\-MM\-SS identifies the creation date and time of the file converted to UTC

RRR – Represents the Agency identifier of the receiver of the file

SSS – Represents the Agency identifier of the sender of the file

NNN – Represents an additional optional descriptor for the file

If IMRO \(society code 128\) sends a file that was created at 18:25 on Nov 25th, 2019 to the ISWC Database Centre \(society code 315\), the filename would be:

iswc\_2019\-11\-25T18\-25\-43\_315\_128\_SampleSubmissions\.json

## <a id="_Toc66716837"></a>ISWCs Allocated from Local Ranges

To register ISWCs that have been allocated from a local range with the ISWC database societies will need to do the following:

- Create a special file, containing only AddSubmission transactions in the ISWC Database JSON file format that have the “locallyAllocatedIswc” field populated
- Use an extended naming convention for the file where the NNN optional descriptor part of the name contains the text “FromLocalRange”\.    A sample submission file name would be iswc\_2019\-11\-25T18\-25\-43\_315\_128\_FromLocalRange\.json
- This file will be picked up automatically by the ISWC Database file processing engine from the “\\In” folder and a corresponding acknowledgement file in the JSON file format will be placed in the “\\Out” folder when the file has been processed\. 

1. <a id="_Toc66716838"></a>Transactions 

The following section shows the expected format and validation rules for each transaction:  

## <a id="_Toc66716839"></a>AddSubmission 

Field

Req

Field Description

Field Rules Reference

submissionId

M

Submitter provided unique id for the transaction within the file\.   This id can be used to tie an acknowledgement record back to its original transaction

agency

M

Represents the agency/society that submitted the file\.

Values for this field reside in the Agency Code Table\.

16

sourcedb

M

Indicates the source database for the submission information\.  I\.E\. A hub or agency itself\.   E\.G\. An IMRO originating work sent to the ISWC database via the WID would have an agency value of “128” and a sourcedb of 300 \(Where 300 represents the WID hub\)\. 

Values for this field reside in the Agency Code Table

16

workcode

M

The agencies identifier for the work\.  The combination of agency, workcode and sourcedb is the information that identifies a musical work in the database of the society that contributed it to the ISWC Database\.

16

category

M

Identifies the submission as relating to a domestic or international work from the perspective of the submitting society

disambiguation

O

Flag\. Indicates if this submission should be disambiguated from other known ISWCs with similar metadata\.

If set to true, then a disambiguationReason must also be provided\. 

disambiguationReason

C

Reason for disambiguation\.  Must be provided if the disambiguation flag is set to true\. 

18

disambiguateFrom

C

Array of ISWCs\.  Must be provided if the disambiguation flag is set to true\. 

18

bvltr

O

May be provided if the disambiguation flag is set to True

Background, Logo, Theme, Visual or Rolled Up Cue\.  

17

derivedWorkType

O

Derived Work Type\. One of ModifiedVersion, Excerpt or        Composite\.    If not provided, then this isn't a derived work

derivedFromIswcs

C

Must be provided if dervicedWorkType is set\. 

Array of iswc’s or titles of the original work that this submission is derived from\. 

19

performers

C

May be provided if the disambiguation flag is set to True\. 

Known list of performer last name and first name pairs\. Used to provide more detail for submissions with the disambiguation flag set to true\. 

17

instrumentation

C

May be provided if the disambiguation flag is set to True\. 

Known list of performer last name and first name pairs\. Used to provide more detail for submissions with the disambiguation flag set to true\.

17

iswc

O

The submitted ISWC\.  May be provided if known, though for most AddSubmission transactions the iswc will not be known or provided\.  The iswc, if provided must exist in the ISWC database as a known Preferred ISWC\. 

12,13,

14, 15, 24

originalTitle

M

Original Work Title\.  Must be transliterated prior to submission\. 

16

otherTitles

O

Other titles associated with musical composition \(aka alternative titles\)\.  Title must include a title type and optionally, many include a language code \(ISO 639\-1\)

interestedParties

M

Interested party associated with musical composition, such as: composers, lyricist, translator, and original publishers\.  Interested Party includes a mandatory nameNumber and role fields\.  In addition, it includes optional name and baseNumber fields\. 

11,16

locallyAllocatedIswc

O

Allows assignment of a locally allocated Iswc

### <a id="_Toc66716840"></a>Transaction Level Validation

Ref

Description

Business Rules Document Ref

11

If work submission does not contain IP with creator role, transaction is rejected\. \(TR\)

IV/07

16

If The matching criteria not satisfied, transaction is rejected\. \(TR\)

MAT/01, MAT/02, MAT/03, MAT/04, MAT/42, MAT/40, MAT/39, MAT/09, MAT/10, MAT/41, MAT/43, MAT/30, MAT/31, MAT/39

17

If the disambiguation flag is set to True, then additional informational metadata may be provided\.   

IV/40

18

If the disambiguation flag is set to True, then a disambiguationReason and disambiguateFrom values must be provided\.

IV/40, PV/30

19

If the derivedWorkType field is set, then the derivedFromIswcs field should be provided\.   The derivedFromISwcs field should contain the list of ISWCs that the work is derived from where available and where the ISWCs are not known it should contain the list of title\(s\)\. 

IV/34, IV/36, IV/38

### <a id="_Toc66716841"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

3

Agency Code must be entered and must match an entry in the Agency Code Table\. It is a mandatory field for CAR, CUR and CDR\. \(TR\)

IV/10

4

Agency Work Code must be entered\. It is a mandatory field for CAR, CUR and CDR\. \(TR\)

IV/11

5

Source DB Code must be entered and must match an entry in the Agency Code Table\.  It is a mandatory field for CAR, CUR and CDR\. \(TR\)

IV/12

12,13,14

If provided the ISWC must have length of 11 characters, must begin with a “T” and must match the following pattern: \[T\]\[0\-9\]\{10\}  \(TR\)

IV/14

15

If provided, the ISWC must match valid “check digit” as per Annex B of ISO 15707 standard document\. \(TR\)

IV/15

17

If provided, the ISWC must exist in the ISWC database as a Preferred ISWC\. \(TR\)

PV/24

### <a id="_Toc66716842"></a>Sample

"addSubmissions": \[

        \{

            "submissionId": 1,

            "agency": "128",

            "sourcedb": 128,

            "workcode": "R28995186",

            "category": "DOM",

            "disambiguation": false,

            "originalTitle": "string",

            "interestedParties": \[

                \{

                    "nameNumber": 375001586,

                    "role": "CA"

                \}

            \]

        \},

        \{

            "submissionId": 2,

            "agency": "128",

            "sourcedb": 128,

            "category": "DOM",

            "workcode": "R28995187",

            "originalTitle": "string",

            "interestedParties": \[

                \{

                    "nameNumber": 375001586,

                    "role": "CA"

                \}

            \]

        \}

    \],

## <a id="_Toc66716843"></a>UpdateSubmission

Field

Req

Field Description

Field Rules Reference

submissionId

M

Submitter provided unique id for the transaction within the file\.   This id can be used to tie an acknowledgement record back to its original transaction

preferredIswc

M

Must be provided for an update submission and must correspond to an existing Preferred ISWC in the ISWC database\.    

8,9,10,21,17

agency

M

Represents the society that submitted the file\.

Values for this field reside in the Agency Code Table\.

3,16

sourcedb

M

Indicates the source database for the submission information\.  I\.E\. A hub or agency itself\.   E\.G\. An IMRO originating work sent to the ISWC database via the WID would have an agency value of “128” and a sourcedb of 300 \(Where 300 represents the WID hub\)\. 

Values for this field reside in the Agency Code Table

5,16

workcode

M

The agencies identifier for the work\.  The combination of agency, workcode and sourcedb is the information that identifies the  musical work in the database of the society that contributed it to the ISWC Database and is used to identify the submission to be updated\.

16

category

M

Identifies the submission as relating to a domestic or international work from the perspective of the submitting society

disambiguation

O

Flag\. Indicates if this submission should be disambiguated from other known ISWCs with similar metadata\.

If set to true, then a disambiguationReason must also be provided\. 

disambiguationReason

C

Reason for disambiguation\.  Must be provided if the disambiguation flag is set to true\. 

18

disambiguateFrom

C

Array of ISWCs\.  Must be provided if the disambiguation flag is set to true\. 

18

bvltr

O

May be provided if the disambiguation flag is set to True

Background, Logo, Theme, Visual or Rolled Up Cue\.  

17

derivedWorkType

O

Derived Work Type\. One of ModifiedVersion, Excerpt or        Composite\.    If not provided, then this isn't a derived work

derivedFromIswcs

C

Must be provided if dervicedWorkType is set\. 

Array of iswc’s or titles of the original work that this submission is derived from\. 

19

performers

C

May be provided if the disambiguation flag is set to True\. 

Known list of performer last name and first name pairs\. Used to provide more detail for submissions with the disambiguation flag set to true\. 

17

instrumentation

C

May be provided if the disambiguation flag is set to True\. 

Known list of performer last name and first name pairs\. Used to provide more detail for submissions with the disambiguation flag set to true\.

17

originalTitle

M

Original Work Title\.  Must be transliterated prior to submission\. 

16

otherTitles

O

Other titles associated with musical composition \(aka alternative titles\)\. Title must include a title type and optionally, many include a language code \(ISO 639\-1\)

interestedParties

M

Interested party associated with musical composition, such as: composers, lyricist, translator and original publishers\.  Interested Party includes a mandatory nameNumber and role fields\.  In addition, it includes optional name and baseNumber fields\.

11,16

### <a id="_Toc66716844"></a>Transaction Level Validation

Ref

Description

Business Rules Document Ref

11

If work submission does not contain IP with creator role, transaction is rejected\. \(TR\)

IV/07

16

If The matching criteria not satisfied, transaction is rejected\. \(TR\)

MAT/01, MAT/02, MAT/03, MAT/04, MAT/42, MAT/40, MAT/39, MAT/09, MAT/10, MAT/41, MAT/43, MAT/30, MAT/31, MAT/39

### <a id="_Toc66716845"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

3

Agency must be entered and must match an entry in the Agency Code Table\. \(TR\)

IV/10

5

Sourcedb must be entered and must match an entry in the Agency Code Table\. \(TR\)

IV/12

8,9,10

The Preferred ISWC must have length of 11 characters, must begin with a “T” and must match the following pattern: \[T\]\[0\-9\]\{10\}  \(TR\)

IV/14

12,13,14

If provided the iswc must have length of 11 characters, must begin with a “T” and must match the following pattern: \[T\]\[0\-9\]\{10\}  \(TR\)

IV/14

15

If provided, the iswc must match valid “check digit” as per Annex B of ISO 15707 standard document\. \(TR\)

IV/15

17

The Preferred ISWC must exist in the ISWC database as a Preferred ISWC \(TR\)

PV/24

18

If provided the Derived Work Type must match one of the values in the Derived Work Types table below\. \(TR\) 

IV/34, IV/36, IV/38

19

If provided the Disambiguation Reason must match one of the values in the Disambiguation Reasons table below\.

IV/40, PV/30

20

If provided the BVLTR value must match one of the values in the BVLTR table below\.

IV/40

21

The Preferred ISWC must match valid “check digit” as per Annex B of ISO 15707 standard document\.

IV/15

	__Derived Work Types__

ID

Code

Description

1

MV

Modified Version

2

EX

Excerpt

3

CO

Composite

	__Disambiguation Reasons__

ID

Code

Description

1

DIT

Different work with same title

2

DIA

Arrangement of work

3

DIE

Excerpt of another work

4

DIC

Cues

5

DIP

Different performer

6

DIV

Different version of work \(excerpt, modified work e\.g\. instrumental\) with different shares

	__BLTVR Values__

ID

Value

1

Background

2

Logo

3

Theme

4

Visual

5

Rolled Up Cue

### <a id="_Toc66716846"></a>Sample

"updateSubmissions": \[

        \{

            "submissionId": 3,

            "preferredIswc": "T0302332075",

            "agency": "128",

            "sourcedb": 128,

            "workcode": "R28995186",

            "category": "DOM",

            "originalTitle": "string",

            "interestedParties": \[

                \{

                    "nameNumber": 375001586,

                    "role": "CA"

                \}

            \]

        \}

    \]

## <a id="_Toc66716847"></a>DeleteSubmission

Field

Req

Field Description

Field Rules Reference

submissionId

M

Submitter provided unique id for the transaction within the file\.   This id can be used to tie an acknowledgement record back to its original transaction

preferredIswc

M

Must be provided for a delete submission and must correspond to an existing Preferred ISWC in the ISWC database\.  Must correspond to the Preferred iswc associated with the submitted agency, workcode and sourcedb

8,9,10,21,17

agency

M

Represents the society that submitted the file\.

Values for this field reside in the Agency Code Table\.

3,16

sourcedb

M

Indicates the source database for the submission information\.  I\.E\. A hub or agency itself\.   E\.G\. An IMRO originating work sent to the ISWC database via the WID would have an agency value of “128” and a sourcedb of 300 \(Where 300 represents the WID hub\)\. 

Values for this field reside in the Agency Code Table

5,16

workcode

M

The agencies identifier for the work\.  The combination of agency, workcode and sourcedb is the information that identifies a musical work in the database of the society that contributed it to the ISWC Database and is used to identify the submission to be deleted\. 

16

reasonCode

M

The reason code for the deletion

### <a id="_Toc66716848"></a>Transaction Level Validation

Ref

Description

Business Rules Document Ref

16

If The matching criteria not satisfied, transaction is rejected\. \(TR\)

MAT/01, MAT/02, MAT/03, MAT/04, MAT/42, MAT/40, MAT/39, MAT/09, MAT/10, MAT/41, MAT/43, MAT/30, MAT/31, MAT/39

### <a id="_Toc66716849"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

3

Agency must be entered and must match an entry in the Agency Code Table\. \(TR\)

IV/10

5

Sourcedb must be entered and must match an entry in the Agency Code Table\. \(TR\)

IV/12

8,9,10

The Preferred ISWC must have length of 11 characters, must begin with a “T” and must match the following pattern: \[T\]\[0\-9\]\{10\}  \(TR\)

IV/14

21

The Preferred ISWC must match valid “check digit” as per Annex B of ISO 15707 standard document\. \(TR\)

IV/15

17

If provided, the Preferred ISWC must exist in the ISWC database as a Preferred ISWC \(TR\) and Must correspond to the Preferred iswc associated with the submitted agency, workcode and sourcedb

PV/24

### <a id="_Toc66716850"></a>Sample

"deleteSubmissions": \[

        \{

            "submissionId": 4,

            "preferredIswc": "T0302332075",

            "agency": "128",

            "sourcedb": 128,

            "workcode": "R28995186",

            "reasonCode": "R01"

        \}

    \]

## <a id="_Toc66716851"></a>SearchByIswcSubmission

Requests for the ISWC metadata for the provided ISWCs

Field

Req

Field Description

Field Rules Reference

submissionId

M

Submitter provided unique id for the transaction within the file\.   This id can be used to tie an acknowledgement record back to its original transaction

iswc

M

Must be provided for a MetadataQuery submission and must correspond to an existing Preferred or Archived ISWC in the ISWC database\.  

16,8,9,10,11,17

### <a id="_Toc66716852"></a>Transaction Level Validation

Ref

Description

Business Rules Document Ref

16

If the matching criteria not satisfied, transaction is rejected\. \(TR\)

MAT/01, MAT/02, MAT/03, MAT/04, MAT/42, MAT/40, MAT/39, MAT/09, MAT/10, MAT/41, MAT/43, MAT/30, MAT/31, MAT/39

### <a id="_Toc66716853"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

8,9,10

If provided the Preferred ISWC must have length of 11 characters, must begin with a “T” and must match the following pattern: \[T\]\[0\-9\]\{10\}  \(TR\)

IV/14

11

If provided, the Preferred ISWC must match valid “check digit” as per Annex B of ISO 15707 standard document\.

IV/15

17

If provided, the Preferred ISWC must exist in the ISWC database as a Preferred ISWC \(TR\)

PV/24

### <a id="_Toc66716854"></a>Sample

    " searchByIswcSubmissions": \[

        \{

            "submissionId": 5,

            "iswc": "T0302332086"

        \}

## <a id="_Toc66716855"></a>SearchByAgencyWorkCodeSubmission 

Requests for the Preferred ISWCs for the provided set of Agency, Sourcedb and Workcode values\.

Field

Req

Field Description

Field Rules Reference

submissionId

M

Submitter provided unique id for the transaction within the file\.   This id can be used to tie an acknowledgement record back to its original transaction

agency

M

Represents the society that submitted the file\.

Values for this field reside in the Agency Code Table\.

3,16

sourcedb

M

Indicates the source database for the submission information\.  I\.E\. A hub or agency itself\.   E\.G\. An IMRO originating work sent to the ISWC database via the WID would have an agency value of “128” and a sourcedb of 300 \(Where 300 represents the WID hub\)\. 

Values for this field reside in the Agency Code Table

5,16

workcode

M

The agencies identifier for the work\.  The combination of agency, workcode and sourcedb is the information that identifies the musical work in the database of the society that contributed it to the ISWC Database and is used to identify the submission to be queried\. 

16

### <a id="_Toc66716856"></a>Transaction Level Validation

Ref

Description

Business Rules Document Ref

16

If the matching criteria not satisfied, transaction is rejected\. \(TR\)

MAT/01, MAT/02, MAT/03, MAT/04, MAT/42, MAT/40, MAT/39, MAT/09, MAT/10, MAT/41, MAT/43, MAT/30, MAT/31, MAT/39

### <a id="_Toc66716857"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

3

Agency must be entered and must match an entry in the Agency Code Table\. \(TR\)

IV/10

5

Sourcedb must be entered and must match an entry in the Agency Code Table\. \(TR\)

IV/12

### <a id="_Toc66716858"></a>Sample

    " searchByAgencyWorkCodeSubmissions": \[

        \{

            "submissionId": 6,

            "agency": "128",

            "sourcedb": 128,

            "workcode": "R28995187"

        \}

    \]

## <a id="_Toc66716859"></a>MergeSubmission

Field

Req

Field Description

Field Rules Reference

submissionId

M

Submitter provided unique id for the transaction within the file\.   This id can be used to tie an acknowledgement record back to its original transaction

preferredIswc

M

Must be provided for a Merge submission and must correspond to an existing Preferred ISWC in the ISWC database\.  This is the Preferred ISWC that the mergeIswcs will be merged into\.  

16,8,9,10,11,17

agency

M

Represents the society that submitted the file\.

Values for this field reside in the Agency Code Table\.

3,16

sourcedb

M

Indicates the source database for the submission information\.  I\.E\. A hub or agency itself\.   E\.G\. An IMRO originating work sent to the ISWC database via the WID would have an agency value of “128” and a sourcedb of 300 \(Where 300 represents the WID hub\)\. 

Values for this field reside in the Agency Code Table

5,16

workcode

M

The agencies identifier for the work\.  The combination of agency, workcode and sourcedb is the information that identifies the musical work in the database of the society that contributed it to the ISWC Database and is used to identify the submission to be merged into\. 

16

mergeIswcs

M

Array of ISWCs to be merged into the preferredIswc listed above\.   At least one ISWC is required\. All ISWCs must exist as Preferred ISWCs in the ISWC database\. 

16,8,9,10,11,17

### <a id="_Toc66716860"></a>Transaction Level Validation

Ref

Description

Business Rules Document Ref

16

If The matching criteria not satisfied, transaction is rejected\. \(TR\)

MAT/01, MAT/02, MAT/03, MAT/04, MAT/42, MAT/40, MAT/39, MAT/09, MAT/10, MAT/41, MAT/43, MAT/30, MAT/31, MAT/39

### <a id="_Toc66716861"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

8,9,10

The ISWC must have length of 11 characters, must begin with a “T” and must match the following pattern: \[T\]\[0\-9\]\{10\}  \(TR\)

IV/14

11

The ISWC must match valid “check digit” as per Annex B of ISO 15707 standard document\.

IV/15

17

The ISWC must exist in the ISWC database as a Preferred ISWC \(TR\)

PV/24

### <a id="_Toc66716862"></a>Sample

    "mergeSubmissions": \[

        \{

            "submissionId": 7,

            "agency": "128",

            "sourcedb": 128,

            "workcode": "R28995187",

            "preferredIswc": "T0302332086",

            "mergeIswcs": \[

                "T0302331403",

                "T0302331414"

            \]

        \}

    \]

## <a id="_Toc66716863"></a>UpdateWorkflowTask

Field

Req

Field Description

Field Rules Reference

submissionId

M

Submitter provided unique id for the transaction within the file\.   This id can be used to tie an acknowledgement record back to its original transaction

taskId

M

ISWC Database ID for the task to be updated

workflowType

M

Option type of workflow\. 

1

Status

M

The status that the submitter wants to use to update the task 

2

### <a id="_Toc66716864"></a>Transaction Level Validation

Ref

Description

Business Rules Document Ref

1

The workflowType must be provided and must correspond to the workflow type of the workflow task identified by the taskId

### <a id="_Toc66716865"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

2

Status can only be changed from status “Outstanding” to one of the following status values:

“Approved” or “Rejected”

### <a id="_Toc66716866"></a>Sample

    "updateWorkflowTasks": \[

        \{

            "submissionId": 8,

            "taskId": 1,

            "status": "Approved"

        \}

    \]

## <a id="_Toc66716867"></a>Acknowledgement Record Format

The Acknowledgement record indicates whether the transaction was accepted and provides a status on the transaction that the acknowledgement has been generated for\.  The acknowledgment will include any error or warning messages associated with the original transaction\.

Field

Req

Field Description

Field Rules Reference

submissionId

M

Submitter provided unique id for the transaction within the file\.   This id can be used to tie an acknowledgement record back to its original transaction

11

originalFileCreationDateTime

M

The creation date/time of the original file that contained the transaction that this transaction applies to as per the fileHeader of that original file\. 

1

originalSubmissionId

M

The submissionId of the original transaction that this acknowledgement applied to\.

2,11

originalTransactionType

M

The Transaction Type of the original transaction that this acknowledgement applied to\.

3

preferredIswc

O

Preferred ISWC assigned to the original transaction\. 

7

agency

O

Represents the society associated with the outcome of the original transaction\.  

Values for this field reside in the Agency Code Table\.

sourcedb

O

Represents the Source Database associated with the outcome of the original transaction\.  I\.E\. A hub or agency itself\.   E\.G\. An IMRO originating work sent to the ISWC database via the WID would have an agency value of “128” and a sourcedb of 300 \(Where 300 represents the WID hub\)\. 

Values for this field reside in the Agency Code Table

workcode

O

Represents the agencies identifier for the work associated with the outcome of the original transaction\.   

originalTitle

O

Represents the original title for the work associated with the outcome of the original transaction\. 

processingDateTime

M

The date/time this transaction or file was formally processed by the recipient \(typically the ISWC database\)

9

transactionStatus

M

The status or outcome of the original transaction\.  One of: FullyAccepted,  PartiallyAccepted or Rejected

10

otherTitles

O

Other titles associated with the musical composition\.  

interestedParties

O

Interested parties associated with the musical composition

workInfo

C

Other society work information associated with the returned ISWC\. 

12

errorMessages

C

Details of any errors that occurred with the transaction\.   

11

### <a id="_Toc66716868"></a>Transaction Level Validation

Ref

Description

Business Rules Document Ref

11

Only one acknowledgment is allowed per transaction\. \(TR\)

### <a id="_Toc66716869"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

1\.

Must match the creation date/time of the original file that contained the transaction that this acknowledgement applies to as per the fileHeader of that original file\. 

2\.

The originalSubmissionId must exist within the file referred to by originalFileCreationDateTime\. \(TR\)

3\.

originalTransactionType must match the transaction identified by referred to by originalSubmissionId above \(TR\)

7\.

If provided, preferred ISWC must be a valid ISWC Number\. \(TR\)

IV/14

9\.

Must be a valid date/time \(TR\)

10\.

Must be one of: FullyAccepted, PartiallyAccepted or Rejected

11\.

Will be provided if the transaction has a transactionStatus of “PartiallyAccepted” or “Rejected”

12\.

Other Society Work Information will always be provided for acknowledgement transactions that relate to “AddSubmission” or “UpdateSubmission” transactions that have a transactionStatus of “FullyAccepted” or “PartiallyAccepted”

### <a id="_Toc66716870"></a>Sample

 "acknowledgements": \[

        \{

            "submissionId": 1000,

            "originalFileCreationDateTime": "2019\-10\-01T18:25:43\.511Z",

            "originalSubmissionId": 1,

            "originalTransactionType": "AddSubmission",

            "preferredIswc": "T0302332075",

            "agency": "128",

            "sourcedb": 128,

            "workcode": "R28995186",

            "originalTitle": "",

            "processingDate": "2019\-11\-25T18:25:43\.511Z",

            "transactionStatus": "FullyAccepted",

            "workInfo": \[

                \{

                    "agency": "072",

                    "sourcedb": 72,

                    "workcode": "637947"

                \},

                \{

                    "agency": "029",

                    "sourcedb": 29,

                    "workcode": "177157029"

                \}

            \]

        \},

        \{

            "submissionId": 1001,

            "originalFileCreationDateTime": "2019\-10\-01T18:25:43\.511Z",

            "originalSubmissionId": 2,

            "originalTransactionType": "AddSubmission",

            "agency": "128",

            "sourcedb": 128,

            "workcode": "R28995187",

            "originalTitle": "",

            "processingDate": "2019\-11\-25T18:25:43\.511Z",

            "transactionStatus": "Rejected",

            "errorMessages": \[

                \{

                    "errorType": "Transaction Rejected",

                    "errorNumber": 104,

                    "errorMessage": "CSI:The provided IP Name Nbr is a Status 3 on the IPI DB which are not accepted\.  Please resubmit with the IP Name Nbr of the corresponding Status 1 IP Base Nbr\."

                \}

            \]

        \}

\]

## <a id="_Toc66716871"></a>Notification Record Format

The Notification record is generated as a notification to agencies that are potentially impacted by the submission of a AddSubmission / UpdateSubmission / DeleteSubmission / MergeSubmission from a submitter agency\.

Field

Req

Field Description

Field Rules Reference

submissionId

M

Submitter provided unique id for the transaction within the file\.   This id can be used to tie an acknowledgement record back to its original transaction

originalFileCreationDateTime

M

The creation date/time of the original file that contained the transaction that triggered this Notification\.   This field can be ignored by the receiving agency as they won’t be able to reference it against any transaction that they sent to the ISWC database\. It is preserved for ISWC Database troubleshooting and has been inherited from earlier CSI formats\. 

originalSubmissionId

M

The submission identifier of the original transaction that triggered this Notification\.  This field can be ignored by the receiving agency as they won’t be able to reference it against any transaction that they sent to the ISWC database\. It is preserved for ISWC Database troubleshooting and has been inherited from earlier CSI formats\.

originalTransactionType

M

Transaction type of the original transaction that triggered this Notification\. 

preferredIswc

M

Preferred ISWC assigned to ISWC being notified\. 

7

agency

M

The society/agency associated with the outcome of the original transaction\.  

Values for this field reside in the Agency Code Table\.

4

sourcedb

M

Represents the source database/hub associated with the original transaction\.  I\.E\. A hub or agency itself\.   E\.G\. An IMRO originating work sent to the ISWC database via the WID would have an agency value of “128” and a sourcedb of 300 \(Where 300 represents the WID hub\)\. 

Values for this field reside in the Agency Code Table\.

6

workcode

M

Represents the receiving agencies identifier for the work associated with the original transaction\.   

originalTitle

M

Represents the original title of the work being notified\.  

processingDateTime

M

The date/time this transaction or file was formally processed by the generator of this file \(typically the ISWC database\)\.

9

workflowTaskId

O

Represents the globally unique ID of the workflow task that has been assigned to the receiving agency as part of this notification\.  

If this is a Notification generated with a new Workflow Task, this field will be populated with the identifier for the Workflow Task\. 

workflowStatus

C

Represents the status of the workflow task that has been assigned to the receiving agency\.  If this is a Notification generated with a new Workflow Task, this field will be populated with the Workflow Task Status: 0 \(Outstanding\), 1 \(Approved\), 2 \(Rejected\), 3 \(Cancelled\)\.  

10

otherTitles

O

Other titles associated with the musical composition\.  

interestedParties

C

Interested parties associated with the musical composition

11

workInfo

C

Other society work information associated with the returned ISWC\. 

12

### <a id="_Toc66716872"></a>Transaction Level Validation

Ref

Description

Business Rules Document Ref

### <a id="_Toc66716873"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

4\.

Agency Code must be entered and must match an entry in the Agency Code Table\. \(TR\) 

IV/10

6\.

Source DB Code must be entered and must match an entry in the Agency Code Table\. \(TR\) 

IV/12

7\.

Preferred ISWC must be a valid ISWC Number\. \(TR\)

IV/14

9\.

Processing Date must be a valid date / time\. \(TR\)

10\. 

Workflow status must be provided if the workflowTaskId is provided\. 

11

IP information will be provided for notifications trigged by “AddSubmission”, “UpdateSubmission”, “DeleteSubmission” and “MergeSubmission” transactions

12

Other society work information will be provided for notifications trigged by “AddSubmission”, “UpdateSubmission”, “DeleteSubmission” and “MergeSubmission” transactions

<a id="_Toc355093999"></a><a id="_Toc485799689"></a>

### <a id="_Toc66716874"></a>Sample

 "notifications": \[

        \{

            "submissionId": 1000,

            "originalFileCreationDateTime": "2019\-10\-01T18:25:43\.511Z",

            "originalSubmissionId": 1,

            "originalTransactionType": "AddSubmission",

            "preferredIswc": "T0302332075",

            "agency": "128",

            "sourcedb": 128,

            "workcode": "R28995186",

            "originalTitle": "",

            "processingDate": "2019\-11\-25T18:25:43\.511Z",

            "transactionStatus": "FullyAccepted",

            "interestedParties": \[

                \{

                    "nameNumber": 375001586,

                    "role": "CA"

                \}

            \],

            "workInfo": \[

                \{

                    "agency": "128",

                    "sourcedb": 128,

                    "workcode": "R28995186"

                \}

            \]

        \},

\]

 

# <a id="_Toc66716875"></a>Appendix A – Open and Closed Items

This appendix provides a tracking list of specific issues/queries raised by CISAC during the specification process and how they were incorporated or excluded from this specification:

__Open and Closed Items__

__ID__

__Description__

__Response__

__Status__

__Next Action By__

1

Need to investigate if the CSN message needs to include IP, Linked ISWCs, Disambiguation and Derived work data\.  It is unclear from the existing specifications\.

Based on an analysis of sample files produced currently, I’ve added the relevant info

Closed

John C

2

Open Item: If a society sends an update but the update has a different preferred ISWC then does this trigger a move assuming the metadata matches\.  \(MAT/18\)

Yes – See rule AS/08 in the business rules document for details\.  

Closed

John C

3

John C to update the ISWCQuerySubmission and MetadataQuerySubmission to use the REST service Operation names instead\. 

I’ve changed the names of the operations now

Closed

John C

