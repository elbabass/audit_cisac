<a id="_Hlk15297321"></a>  
![A close up of a sign

Description automatically generated](1.png)

![](2.png)

CISAC

ISWC Database Allocation and Resolution Solution  

# <a id="_Toc399421500"></a><a id="_Toc399422154"></a><a id="_Toc485799633"></a><a id="_Toc135998115"></a>Document Control

## <a id="_Toc158527933"></a><a id="_Toc399421501"></a><a id="_Toc399422155"></a><a id="_Toc485799634"></a><a id="_Toc135998116"></a>Change Record

Date

Person

Version/Reference

24th Jan 2020

John Corley, 

Curnan Reidy  

V1\.0 / Initial Draft for review by design team

31st Jan 2020

John Corley

V1\.1 / Updates based on feedback to highlight that there will still be separate allocation and resolution services but that they will be delivered by the same software solution\. 

5th Feb 2020

John Corley

V1\.2 / Updated based on feedback from second design workshop

14th Feb 2020

John Corley

V1\.3 / Updates based on feedback from design team on 10th Feb and follow on discussion at PTM on 14th Feb

16th Mar 2020

John Corley

V1\.4 / Updates following publisher feedback\.  Added flat file data exchange format\.

24th Mar 2020

John Corley

V1\.5 / Updates following review with design team

30th Mar 2020

John Corley

V1\.6 / Updates following publisher feedback

1st Apr 2023

Curnan Reidy

Updated AdditionalIdentifiers/AgencyWorkCodes for both JSON and Flat File to support search by work code for the IRS

<a id="_Toc158527934"></a><a id="_Toc399421502"></a><a id="_Toc399422156"></a>

## <a id="_Toc485799635"></a><a id="_Toc135998117"></a>Reviewers

<a id="_Toc158527935"></a><a id="_Toc399421503"></a><a id="_Toc399422157"></a><a id="_Toc485799636"></a>

Katrien Tielemans

Bolmar Carrasquilla 

Ed Oshanani 

Didier Roy 

Hanna Mazur 

José Macarro

Christopher McKenzie

Roseany Fagundes

Janice Hooper

Sylvain Piat 

John Corley

Niamh McGarry

Declan Rudden

Curnan Reidy

Cynthia Lipskier multipleAgencyWorkCodes

## <a id="_Toc135998118"></a>Distribution

Reviewers

## <a id="_Toc158527936"></a><a id="_Toc399421504"></a><a id="_Toc399422158"></a><a id="_Toc485799637"></a><a id="_Toc135998119"></a>Approval

This document was approved electronically via email by the following people on the following dates:

Date/Time

Person

Note

# <a id="_Toc135998120"></a>Table of Contents<a id="_Toc485799638"></a>

[Document Control	2](#_Toc135998115)

[Change Record	2](#_Toc135998116)

[Reviewers	2](#_Toc135998117)

[Distribution	3](#_Toc135998118)

[Approval	3](#_Toc135998119)

[Table of Contents	4](#_Toc135998120)

[1	Introduction	7](#_Toc135998121)

[What does this document contain?	7](#_Toc135998122)

[Who should read this document?	7](#_Toc135998123)

[References	7](#_Toc135998124)

[2	Solution Introduction	8](#_Toc135998125)

[2\.1\. Overview	8](#_Toc135998126)

[2\.2\. Data Exchange File Locations	9](#_Toc135998127)

[2\.3\. JSON Data Exchange File Format	9](#_Toc135998128)

[2\.4\. JSON File Structure	9](#_Toc135998129)

[2\.4\.1\. Developer Artefacts	9](#_Toc135998130)

[2\.4\.2\. Schema and Validation	10](#_Toc135998131)

[2\.4\.3\. File Header	11](#_Toc135998132)

[2\.4\.3\.1\. Transaction Level Validation	13](#_Toc135998133)

[2\.4\.3\.2\. Field Level Validation	13](#_Toc135998134)

[2\.4\.4\. Transaction Groups	13](#_Toc135998135)

[2\.4\.5\. JSON File Naming, Transfer and Encoding	14](#_Toc135998136)

[2\.4\.6\. File Encoding	14](#_Toc135998137)

[2\.4\.7\. File Transfer and Location	14](#_Toc135998138)

[2\.4\.8\. File Naming Convention	14](#_Toc135998139)

[2\.5\. Flat File Data Exchange Format	15](#_Toc135998140)

[2\.5\.1\. Developer Artefacts	15](#_Toc135998141)

[2\.6\. Embedded Process for Allocations	16](#_Toc135998142)

[3	JSON Format Transactions	17](#_Toc135998143)

[3\.1\. AddSubmission	17](#_Toc135998144)

[3\.1\.1\. Transaction Level Validation	19](#_Toc135998145)

[3\.1\.2\. Field Level Validation	20](#_Toc135998146)

[3\.1\.3\. Sample	20](#_Toc135998147)

[3\.2\. FindSubmission	21](#_Toc135998148)

[3\.2\.1\. Transaction Level Validation	23](#_Toc135998149)

[3\.2\.2\. Field Level Validation	23](#_Toc135998150)

[3\.2\.3\. Sample	24](#_Toc135998151)

[3\.3\. Acknowledgement Record Format	24](#_Toc135998152)

[3\.3\.1\. Transaction Level Validation	25](#_Toc135998153)

[3\.3\.2\. Field Level Validation	25](#_Toc135998154)

[3\.3\.3\. Sample	26](#_Toc135998155)

[4	Flat File Data Exchange Format	29](#_Toc135998156)

[4\.1\. File Structure	29](#_Toc135998157)

[4\.1\.1\.1\. File Headers and Delimiters	29](#_Toc135998158)

[4\.1\.2\. AddSubmission / Allocation Record	30](#_Toc135998159)

[4\.1\.2\.1\. Transaction Level Validation	33](#_Toc135998160)

[4\.1\.2\.2\. Field Level Validation	34](#_Toc135998161)

[4\.1\.3\. FindSubmission / Resolution Record	34](#_Toc135998162)

[4\.1\.3\.1\. Transaction Level Validation	38](#_Toc135998163)

[4\.1\.3\.2\. Field Level Validation	38](#_Toc135998164)

[4\.1\.4\. Acknowledgment Record	39](#_Toc135998165)

[4\.1\.4\.1\. Transaction Level Validation	40](#_Toc135998166)

[4\.1\.4\.2\. Field Level Validation	40](#_Toc135998167)

[4\.2\. File Naming, Transfer and Encoding	41](#_Toc135998168)

[4\.2\.1\. File Encoding	41](#_Toc135998169)

[4\.2\.2\. File Transfer and Location	41](#_Toc135998170)

[4\.2\.3\. File Naming Convention	41](#_Toc135998171)

[5	Changes to Agency Solution	42](#_Toc135998172)

[5\.1\. ISWC Database Data\-Structure Changes	42](#_Toc135998173)

[5\.2\. ISWC Database Data\-Structure Changes	42](#_Toc135998174)

[5\.2\.1\. \[ISWC\]\.\[AdditionalIdentifier\] Table	42](#_Toc135998175)

[5\.2\.2\. \[Lookup\]\.\[NumberType\] Table	43](#_Toc135998176)

[5\.2\.3\. \[Lookup\]\.\[PublisherSubmitterCode\] Table	43](#_Toc135998177)

[6	Reporting	45](#_Toc135998178)

[Appendix A – Open and Closed Items	46](#_Toc135998179)

[Appendix B – Lookup Codes and Descriptions	50](#_Toc135998180)

1. <a id="_Toc399422159"></a><a id="_Toc399421505"></a><a id="_Toc158527937"></a><a id="_Toc53481878"></a><a id="_Toc399422160"></a><a id="_Toc399421506"></a><a id="_Toc158527938"></a><a id="_Toc485799639"></a><a id="_Toc135998121"></a>Introduction

## <a id="_Toc158527939"></a><a id="_Toc399421507"></a><a id="_Toc399422161"></a><a id="_Toc485799640"></a><a id="_Toc135998122"></a>What does this document contain?

It provides the detailed design of the new allocation and resolution solution that will be provided to societies and publishers\.   This new solution will be based on a JSON format file\-based exchange with the ability to extend to other formats if needed\.   

## <a id="_Toc158527940"></a><a id="_Toc399421508"></a><a id="_Toc399422162"></a><a id="_Toc485799641"></a><a id="_Toc135998123"></a>Who should read this document?

CISAC development and project management personnel\. Society development and project management personnel\.  Spanish Point development team members\.   

## <a id="_Toc158527942"></a><a id="_Toc399421510"></a><a id="_Toc399422164"></a><a id="_Toc485799643"></a><a id="_Toc135998124"></a>References

Reference

Description

SPE\_20191118\_ISWC\_JSON\_fileFormat

JSON data exchange file format for Agencies in new ISWC system 

https://members\.cisac\.org/CisacPortal/cisacDownloadFile\.do?docId=8558

CISAC CWR Send ID and codes 

http://members\.cisac\.org/CisacPortal/cisacDownloadFileSearch\.do?docId=24111&lang=en&type=pdf

CISAC society codes 

Iswc publisher JSON Format\.zip

Companion developer artefacts for this specification\. Includes JSON schema, sample files and sample validator application\.  

1. <a id="_Toc135998125"></a>Solution Introduction 

## <a id="_Toc135998126"></a>Overview

The goal of having a combined solution is that there will be a single consistent software system for both allocating new and resolving existing ISWCs\.   This solution will be an extension of the already developed solution for ISWC agencies, will use the same rules and will share the same code base\.   This will bring significant benefits for ISWC agencies including:

- A single consistent set of business rules applied to all ISWCs
- Ability to provide a central reporting solution that reports across all ISWC allocation and resolution submissions regardless of if they originated from a publisher or a society
- A single consistent modern data exchange format that can include allocation submissions, resolution submissions or both\.  
- Additional support for a simple consistent “flat file” format for allocations and resolution  

Having this single combined solution and data exchange format in no way limits the way in which societies can provide this functionality as a service to publishers\.   

- Currently the allocation of ISWCs prior to work registration is provided as a service by societies and this service is fulfilled at the back\-end by the CISAC hosted solution\.  The only changes proposed to this are: 
	- A new different consolidated back\-end system
	- A new single modern data exchange format and an additional simple flat file format
	- The option for a society to have the data exchange location hosted at a CISAC location instead of hosted at their society \(if required\)

I\.E\. CISAC will continue to validate a society for enablement of the allocation service and societies in turn will validate each publisher for use of the allocation service and will be the providers of the service to publishers\. 

- Currently the resolution of ISWCs for publisher back\-catalogues is provided as a central service by CISAC to publishers\. The only changes proposed to this are:
	- A new different consolidated back\-end system 
	- A new single modern data exchange format and an additional simple flat file format

I\.E\. CISAC will continue to validate each publisher for use of the resolution service and will be the provider of the service to publishers\.  

    

## <a id="_Toc135998127"></a>Data Exchange File Locations

For the IAS, all society sourced files in the new data exchange file format will be deposited in a society specific secure ftp location, with sub folders per publisher, for processing by the combined allocation and resolution solution\.  Resulting acknowledgement files in the same format will be deposited in the same location for pick up by the society\. 

Similarly, all publisher sourced files for the IRS in the new data exchange file format will be deposited in a publisher specific secure ftp location for processing by the combined allocation and resolution solution\.  Resulting acknowledgement files in the same format will be deposited in the same location for pick up by the publisher\. 

## <a id="_Toc135998128"></a>JSON Data Exchange File Format

The ISWC Publisher JSON data format is based on the ISWC Agency JSON format and includes the information supported by the different existing allocations and resolution service formats\.  

<a id="_Toc468452208"></a><a id="_Toc496111659"></a><a id="_Toc496111662"></a><a id="_Toc496111669"></a><a id="_Toc496111673"></a><a id="_Toc496111677"></a><a id="_Toc496111684"></a><a id="_Toc496111690"></a>

In addition, it has the following benefits over the existing formats:

- It is a single consistent format for both allocation and resolution 
- It is schema based, enabling validation of the core message format, required fields, datatypes and list values at source\.  
- It is easier to process and manipulate in modern development tools\.

## <a id="_Toc135998129"></a>JSON File Structure

The section describes the structure of the ISWC Publisher JSON Format and the developer artefacts that are included with this specification\.  

### <a id="_Toc135998130"></a>Developer Artefacts

This specification includes the following companion artefacts:

__File Name__

__Description__

__iswc\_publisher\_schema\.json__

Schema that defines the valid ISWC Publisher JSON format\.  

__SampleSubmissionsAllocation\.json__

Sample Publisher ISWC JSON file that contains a set of ISWC Allocation Submission transactions from a publisher, through an agency \(128\), to be processed by the ISWC Database \(315\) and that conforms to the above schema\. 

__SampleAcknowledgementsAllocation\.json__

Sample ISWC Publisher JSON file that contains a set of ISWC acknowledgement transactions from the ISWC database, to a publisher, through an  agency in response to the transactions sent in the previous file\. 

__SampleSubmissionsResolution\.json__

Sample Publisher ISWC JSON file that contains a set of ISWC Resolution Submission transactions from a publisher, through CISAC \(312\) as a Submitting Agency, to be processed by the ISWC Database \(315\) and that conforms to the above schema\. 

__SampleAcknowledgementsResolution\.json__

Sample ISWC Publisher JSON file that contains a set of ISWC acknowledgement transactions from the ISWC database, to a publisher, through a CISAC hosted Publisher SFTP folder

__Validator\.py__

Sample python application that validates all \.json files in the current folder against the iswc\_publisher\_schema\.json document also stored in that folder\. 

### <a id="_Toc135998131"></a>Schema and Validation

All files should be validated against the schema, “iswc\_publisher\_schema\.json” by the generator of the file\.  The ISWC database will validate all inbound files against the schema and any schema validation errors will result in the entire file being rejected\.  

The file structure is as follows:  

<a id="_bookmark4"></a>

Each file must consist of exactly one file header and zero or more transaction groups\.  Transaction groups include “addSubmissions”, “findSubmissions” and “acknowledgements”\. 

Each transaction group must contain zero or more transactions\.  E\.G\. the “addSubmissions” transaction group must contain zero or more “AddSubmission” type transactions\. 

### <a id="_Toc135998132"></a>File Header

The file header contains the required header level metadata\.  The following is the simplest valid, though not very useful ISWC database json message for the __Allocation__ Service:

\{

    "$schema": "\./iswc\_publisher\_schema\.json",

    "fileHeader": \{

        "submittingAgency": "128",

        "submittingSourcedb": 128,

        "submittingPublisher": \{

            "name": "Sony/ATV Music Publishing LLC",

            "nameNumber": 269021863,

            "email": "info@spanishpoint\.ie",

            "role": "AM"

        \},

        "fileCreationDateTime": "2019\-10\-01T18:25:43\.511Z",

        "receivingAgency": "315"

    \}

\}

*Note: The submittingPublisher nameNumber and role should be considered as included as an IP in each AddSubmission transaction where no administering or original role publisher is included in the interestedParties list\.  I\.E\. It acts as a default publisher and role where no publisher and role are included\.*

The following is the simplest valid ISWC database json message for the __Resolution__ Service\. Note that CISAC agency code 312 is inserted as submittingAgency for these transactions:

\{

    "$schema": "\./iswc\_publisher\_schema\.json",

    "fileHeader": \{

        "submittingAgency": "312",

        "submittingSourcedb": 315,

        "submittingPublisher": \{

            "name": "Sony/ATV Music Publishing LLC",

            "nameNumber": 269021863,

            "email": "info@spanishpoint\.ie",

            "role": "AM"

        \},

        "fileCreationDateTime": "2019\-10\-01T18:25:43\.511Z",

        "receivingAgency": "315"

    \}

\}

Field

Req

Field Description

Field Rules Reference

submittingAgency

M

Zero filled 3 digit Agency / Society code that the allocation transactions are being sent through\.  

submittingSourcedb

M

Hub through which the Agency / Society sends data to the ISWC database\.   In most cases this will be same as the submittingAgency value\.

submittingPublisher / name

M

Name of submitting publisher

submittingPublisher / nameNumber

M

IP Name Number for the submitting publisher

submittingPublisher / role

O

One of two valid roles that the submitting publisher has for this submission\.  

21,22

submittingPublisher / email

M

Contact email address for publisher

fileCreationDateTime

M

Date/Time file was created in ISO 8601 format\.  

receivingAgency

M

Agency / Society code that the transactions are being sent to

  

#### <a id="_Toc135998133"></a>Transaction Level Validation

Ref

Description

Business Rules Document Ref

22

The submitting publisher, if provided, must be a member of the society/agency designated in the submittingAgency field\. 

If none of the creators in a submission are members of that society/agency for the applicable creator roles, then the submitting publisher role must be provided\.  

I\.E\. In order to allocate an ISWC, the submitting publisher must have an “AM” or “E” role on the work being submitted and be a member of the society that the submission is being made through or one of the creators listed on the submission must be a member of the society that the submission is being made through for a valid creator role\. 

#### <a id="_Toc135998134"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

21

If provided the submitting publisher role must be one of “AM” or “E”\.   

### <a id="_bookmark5"></a><a id="_Toc135998135"></a>Transaction Groups

The following transaction groups are supported by the ISWC JSON format:

__Transaction Group__

__Description__

__Orchestration__

__addSubmissions__

New ISWC submissions\.  Each submission may be linked to an existing preferredISWC or a new preferredISWC may be generated for it\.   The society must be ISWC eligible for the metadata being submitted\.  These transactions are similar to the existing allocation service requests\.

Expected from publishers to the ISWC Database

__findSubmissions__

Retrieve an existing preferredISWC for a set of metadata provided\. The society does not have to be ISWC eligible for the metadata being submitted\.   These transactions are similar to the existing resolution service requests\. 

Expected from publishers to the ISWC Database

__Acknowledgements__

Acknowledgements in response to transactions above

Expected from the ISWC Database to publishers

### <a id="_Toc135998136"></a>JSON File Naming, Transfer and Encoding 

### <a id="_Toc135998137"></a>File Encoding

All files will be encoded in UTF\-8\.

### <a id="_Toc135998138"></a>File Transfer and Location

The standard for transmission will be the Secure File Transfer Protocol \(SFTP\)\.  Each ISWC agency or publisher will push files to an agency and publisher specific secure folder in a central ISWC Database public SFTP site and pull files from that secure folder\. 

### <a id="_Toc135998139"></a>File Naming Convention

ISWCP – identifies an ISWC Publisher file 

YYYY\-MM\-DDTHH\-MM\-SS identifies the creation date and time of the file converted to UTC

SSS – Represents the Agency \(Society\) identifier of the sender of the file

PPP – Represents the Publisher Sender ID code as per CWR Sender IDs involved in this exchange

RRR – Represents the Agency \(Society\) identifier of the receiver of the file

YYNN – Represents an additional optional descriptor for the file\.  E\.G\. Publishers could use the existing convention as used by CWR for this\. 

If Sony/ATV Music Publishing LLC \(Sender ID SA\) send through IMRO \(society code 128\), a file that was created at 18:25 on Nov 25th, 2019 to the ISWC Database Centre \(society code 315\), the filename would be:

iswcp\_2019\-11\-25T18\-25\-43\_128\_SA\_315\_SampleSubmissions\.json

## <a id="_Toc135998140"></a>Flat File Data Exchange Format

Chapter four below, describes the new combined flat file format that will also be supported\.   

### <a id="_Toc135998141"></a>Developer Artefacts

This specification includes the following additional companion artefacts that are specific to the flat file format:

__File Name__

__Description__

__SampleSubmissionsAllocation\.txt__

Sample Publisher ISWC flat file that contains a set of ISWC Allocation Submission transactions from a publisher, through an agency \(128\), to be processed by the ISWC Database \(315\) and that conforms to the above schema\. 

__SampleAcknowledgementsAllocation\.txt__

Sample ISWC Publisher flat file that contains a set of ISWC acknowledgement transactions from the ISWC database, to a publisher, through an  agency in response to the transactions sent in the previous file\. 

__SampleSubmissionsResolution\.txt__

Sample Publisher ISWC flat file that contains a set of ISWC Resolution Submission transactions from a publisher, through CISAC \(312\) as a Submitting Agency, to be processed by the ISWC Database \(315\) and that conforms to the above schema\. 

__SampleAcknowledgementsResolution\.txt__

Sample ISWC Publisher JSON file that contains a set of ISWC acknowledgement transactions from the ISWC database, to a publisher, through a CISAC hosted Publisher SFTP folder

## <a id="_Toc135998142"></a>Embedded Process for Allocations

Agencies can provide an embedded process for allocations in two different ways using this new combined solution:

1. Agencies can integrate directly with the REST API for Agencies to allocate ISWCs for publishers or creators
2. Agencies can extract data from publisher/creator data exchanges in existing formats into the new JSON format and then submit these for allocation, processing the result\. 

 

1. <a id="_Toc135998143"></a>JSON Format Transactions 

The following section shows the expected format and validation rules for each transaction:  

## <a id="_Toc135998144"></a>AddSubmission 

Field

Req

Field Description

Field Rules Reference

submissionId

M

Submitter provided unique id for the transaction within the file\.   This id can be used to tie an acknowledgement record back to its original transaction

workcode

M

The publisher’s identifier for the work\.  This information will be saved against the ISWC returned in the ISWC database and can be used by the society to cross validate ISWCs allocated by publishers with their subsequent CWR submissions\. 

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

performers

C

May be provided if the disambiguation flag is set to True\. 

Known list of performer last name and first name pairs\. Used to provide more detail for submissions with the disambiguation flag set to true\. 

17

instrumentation

C

May be provided if the disambiguation flag is set to True\. 

Array of three\-digit Instrument Codes and/or Standard Instrumentation Types as per standard CWR lookup tables\. 

\.

17

derivedWorkType

O

Derived Work Type\. One of ModifiedVersion, Excerpt or Composite\.    If not provided, then this isn't a derived work

Submission containing a derivedWorkType value won’t be matched against existing ISWCs with the same metadata but which don’t have the same derivedWorkType\.  Multiple submissions with the same metadata \(title and creators\) and the same derivedWorkType values can be disambiguated from each other using the disambiguation flag above, if each submission represents a unique work\.  E\.G\. Multiple “Different Arrangements” of the same work\.    

derivedFromIswcs

C

Must be provided if derivedWorkType is set\. 

Array of iswc’s or titles of the original work that this submission is derived from\. \.  E\.G\. If a derived work is derived from two different works where one’s ISWC is known and the other isn’t then this field value would look as follows:

"derivedFromIswcs": \[\{"iswc": "T1231231234"\},\{"title": "Another title"\}\],

19

originalTitle

M

Original Work Title\.

16

otherTitles

O

Other titles associated with musical composition \(aka alternative titles\)\.  Title must include a title type and optionally, many include a language code \(ISO 639\-1\)\.

interestedParties

M

Interested party associated with musical composition, such as: composers, lyricist, translator, and original publishers\.  Interested Party includes a mandatory nameNumber and role fields\.  In addition, it includes optional name and baseNumber fields\. 

11,12,16

additionalIdentifiers

O

Optional list of additional identifiers that can be supplied with the submission\.  Currently limited to ISRCs but will be extended over time to incorporate additional identifiers\.   Example usage:

"additionalIdentifiers": \{"isrcs": \["AB1231212345","IE1231212345"\]\}

20

### <a id="_Toc135998145"></a>Transaction Level Validation

Ref

Description

Business Rules Document Ref

11

If work submission does not contain IP with creator role, transaction is rejected\. \(TR\)

IV/07

12

Reject submissions with certain configured IPs, ignore certain Ips in matching and deal with PD IPs 

IV/24, IV/25, IV/29

16

If the matching criteria not satisfied, transaction is rejected\. \(TR\)

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

20

If present must contain one or more ISRCs\.  ISRCs must be a valid format without separators\.  E\.G\. “IE1231212345”\.  In the future additional types of identifier may be supported\.  These identifiers will be initially used for reconciliation only\. 

### 		<a id="_Toc135998146"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

### <a id="_Toc135998147"></a>Sample

   "addSubmissions": \[

        \{

            "submissionId": 1,

            "workcode": "SONY10001",

            "disambiguation": false,

            "originalTitle": "string",

            "interestedParties": \[

                \{

                    "nameNumber": 375001586,

                    "role": "CA"

                \}

            \],

            "additionalIdentifiers": \{

                "isrcs": \["AB1231212345","IE1231212345"\]

            \}

        \},

        \{

            "submissionId": 2,

            "workcode": "SONY10002",

            "disambiguation": false,

            "originalTitle": "string",

            "otherTitles": \[

                \{

                    "title": "string",

                    "type": "CT"

                \}

            \],

            "interestedParties": \[

                \{

                    "nameNumber": 375001500,

                    "role": "CA"

                \}

            \]

        \}

    \]

## <a id="_Toc135998148"></a>FindSubmission

Field

Req

Field Description

Field Rules Reference

submissionId

M

Submitter provided unique id for the transaction within the file\.   This id can be used to tie an acknowledgement record back to its original transaction

workcode

M

The publisher’s identifier for the work\.  This information will be saved against the ISWC returned in the ISWC database and can be used by the society to cross validate ISWCs allocated by publishers with their subsequent CWR submissions\. 

disambiguation

O

Flag\. Indicates if this submission should be disambiguated from other known ISWCs with similar metadata\.

If set to true, then a disambiguationReason must also be provided\. 

disambiguationReason

C

Reason for disambiguation\.  Must be provided if the disambiguation flag is set to true\. 

See Appendix B for list of codes and descriptions

18

disambiguateFrom

C

Array of ISWCs\.  Must be provided if the disambiguation flag is set to true\. 

18

derivedWorkType

O

Derived Work Type\. One of ModifiedVersion, Excerpt or        Composite\.    If not provided, then this isn't a derived work

Submission containing a derivedWorkType value won’t be matched against existing ISWCs with the same metadata but which don’t have the same derivedWorkType\.  Multiple submissions with the same metadata \(title and creators\) and the same derivedWorkType values can be disambiguated from each other using the disambiguation flag above, if each submission represents a unique work\.  E\.G\. Multiple “Different Arrangements” of the same work\.    

derivedFromIswcs

C

Must be provided if derivedWorkType is set\. 

Array of iswc’s or titles of the original work that this submission is derived from\. 

19

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

11,12,16

additionalIdentifiers

O

Optional list of additional identifiers that can be supplied with the submission e\.g\. Agency Work Codes\.   
  
These work codes can be used for matching purposes where the user does not have IP information for all creator IPs\. If supplied, the work code is used for matching before defaulting to title and creator metadata as per normal in the event of no match being found\.  
If a match is made by workcode, all creator IP will be returned in the ACK file\.

20

Open Items:  The existing resolution service includes the ISWC as an optional input field\.  Should this be included here\.   If so, what is the expected behaviour?  Ans: The Agency ISWC allocation process cross matches the metadata on the ISWC provided in the ISWC database with the metadata in the submission and if they are different returns the current matching one so in this usage the result would be the same as not providing it\.

### <a id="_Toc135998149"></a>Transaction Level Validation

Ref

Description

Business Rules Document Ref

11

If work submission does not contain IP with creator role, transaction is rejected\. \(TR\)

IV/07

12

Reject submissions with certain configured IPs, ignore certain IPs in matching and deal with PD IPs 

IV/24, IV/25, IV/29

16

If The matching criteria not satisfied, transaction is rejected\. \(TR\)

MAT/01, MAT/02, MAT/03, MAT/04, MAT/42, MAT/40, MAT/39, MAT/09, MAT/10, MAT/41, MAT/43, MAT/30, MAT/31, MAT/39

18

If the disambiguation flag is set to True, then a disambiguationReason and disambiguateFrom values must be provided\.

IV/40, PV/30

19

If the derivedWorkType field is set, then the derivedFromIswcs field should be provided\.   The derivedFromISwcs field should contain the list of ISWCs that the work is derived from where available and where the ISWCs are not known it should contain the list of title\(s\)\. 

IV/34, IV/36, IV/38

20

If present must contain one or more ISRCs\.  ISRCs must be a valid format without separators\.  E\.G\. “IE1231212345”\.  In the future additional types of identifier may be supported\.  These identifiers will be initially used for reconciliation only\. 

### <a id="_Toc135998150"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

### <a id="_Toc135998151"></a>Sample

    "findSubmissions": \[

        \{

            "submissionId": 3,

            "workcode": "SONY10003",

            "disambiguation": false,

            "originalTitle": "You raise me up",

            "interestedParties": \[

                \{

                    "nameNumber": 87000986,

                    "role": "CA"

                \},

                \{

                    "nameNumber": 87632644,

                    "role": "CA"                    

                \}

            \]

        \}

    \]

## <a id="_Toc135998152"></a>Acknowledgement Record Format

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

workcode

O

Represents the publishers identifier for the work associated with the outcome of the original transaction\.   

originalTitle

O

Represents the original title associated with the ISWC returned as the outcome of the transaction\. 

processingDateTime

M

The date/time this transaction or file was formally processed by the recipient \(typically the ISWC database\)

9

transactionStatus

M

The status or outcome of the original transaction\.  One of: FullyAccepted  or Rejected\.  Note: “PartiallyAccepted” is also a valid response but based in the currently implemented rules will never be returned\. 

10

otherTitles

O

Other titles associated with the musical composition\.  

interestedParties

O

Interested parties associated with the musical composition

13

workInfo

C

Other society work information associated with the returned ISWC\. 

errorMessages

C

Details of any errors that occurred with the transaction\.   

11

### <a id="_Toc135998153"></a>Transaction Level Validation

Ref

Description

Business Rules Document Ref

11

Only one acknowledgment is allowed per transaction\. \(TR\)

### <a id="_Toc135998154"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

1\.

Must match the creation date/time of the original file that contained the transaction that this acknowledgement applies to as per the fileHeader of that original file\. 

2\.

The originalSubmissionId must exist within the file referred to by originalFileCreationDateTime\. \(TR\)

3\.

originalTransactionType must match the transaction referred to by originalSubmissionId above \(TR\)

7\.

If provided, preferred ISWC must be a valid ISWC Number\. \(TR\)

IV/14

9\.

Must be a valid date/time \(TR\)

10\.

Must be one of: FullyAccepted, PartiallyAccepted or Rejected

11\.

Will be provided if the transaction has a transactionStatus of “PartiallyAccepted” or “Rejected”

13

If Interested Party information is returned it will include only nameNumber, name and role fields\.  It will never include baseNumber\. 

### <a id="_Toc135998155"></a>Sample

\{

    "$schema": "\./iswc\_publisher\_schema\.json",

    "fileHeader": \{

        "submittingAgency": "128",

        "submittingSourcedb": 128,

        "submittingPublisher": \{

            "name": "Sony/ATV Music Publishing LLC",

            "nameNumber": 269021863,

            "email": "info@spanishpoint\.ie"

        \},

        "fileCreationDateTime": "2019\-10\-01T18:25:43\.511Z",

        "receivingAgency": "315"

    \},

    "acknowledgements": \[

        \{

            "submissionId": 1000,

            "originalFileCreationDateTime": "2019\-10\-01T18:25:43\.511Z",

            "originalSubmissionId": 1,

            "originalTransactionType": "AddSubmission",

            "preferredIswc": "T0302332075",

            "workcode": "SONY10001",

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

            "workcode": "SONY10002",

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

        \},

        \{

            "submissionId": 1002,

            "originalFileCreationDateTime": "2019\-10\-01T18:25:43\.511Z",

            "originalSubmissionId": 3,

            "originalTransactionType": "FindSubmission",

            "workcode": "SONY10003",

            "preferredIswc": "T9176016301",

            "originalTitle": "you raise me up",

            "processingDate": "2019\-11\-25T18:25:43\.511Z",

            "transactionStatus": "FullyAccepted",

            "workInfo": \[

                \{

                    "agency": "052",

                    "sourcedb": 52,

                    "workcode": "192581DM"

                \},

                \{

                    "agency": "079",

                    "sourcedb": 79,

                    "workcode": "00027882687"

                \},

                \{

                    "agency": "090",

                    "sourcedb": 90,

                    "workcode": "00027882687"

                \}

            \]

        \}

    \]

\}

1. <a id="_Toc135998156"></a>Flat File Data Exchange Format 

This chapter describes the new combined allocation and resolution flat file format\.  This format is based on the existing flat file formats supported by the existing allocations and resolution services\.  It includes the additional data elements supported by the new ISWC database\. 

## <a id="_Toc135998157"></a>File Structure

This section describes the overall file structure and the record structure for each type of transaction\.  

#### <a id="_Toc135998158"></a>File Headers and Delimiters

All files will be __tab delimited__ and will contain a single record type which corresponds to one of the three supported transactions types:

- AddSubmission 

This represents an ISWC Allocation transaction\.  Sent from a publisher to their ISWC Agency / Society\.

- FindSubmission

This represents an ISWC Resolution transaction\.  Sent from a publisher to CISAC\.

  

- Acknowledgement 

This represents the outcome of a previously sent AddSubmission or FindSubmission transaction file\.  Sent from an ISWC Agency/Society or CISAC to a publisher\.  

Column __headers should be excluded__ from the file\.

Where __repeating information is to be provided in a single field__ \(e\.g\. the disambiguateFrom field below\) then the __pipe delimiter \(“|”\) should be used__ to separate the different values within that one field\. 

### <a id="_Toc135998159"></a>AddSubmission / Allocation Record 

Each AddSubmission/Allocation record should have the following structure:

Field

Field \#

Req

Field Description

Field Rules Reference

recordType

0

M

AddSubmissions

submissionId

1

M

Submitter provided unique id for the transaction within the file\.   This id can be used to tie an acknowledgement record back to its original transaction

submittingAgency

2

M

Zero filled 3 digit Agency / Society code that the allocation transactions are being sent through\.  

submittingSourcedb

3

M

Hub through which the Agency / Society sends data to the ISWC database\.   In most cases this will be same as the submittingAgency value\.

submittingPublisher / name

4

M

Name of submitting publisher

submittingPublisher / nameNumber

5

M

IP Name Number for the submitting publisher

submittingPublisher / role

6

O

One of two valid roles that the submitting publisher has for this submission\.  

21,22

submittingPublisher / email

7

M

Contact email address for publisher

workcode

8

M

The publisher’s identifier for the work\.  This information will be saved against the ISWC returned in the ISWC database and can be used by the society to cross validate ISWCs allocated by publishers with their subsequent CWR submissions\. 

disambiguation

9

O

__Though data for fields 9 – 17 is not mandatory, user should enter an empty tab for each otherwise file will fail validation\.__

Flag\. “true” or “false”\.  Indicates if this submission should be disambiguated from other known ISWCs with similar metadata\.

If set to true, then a disambiguationReason must also be provided\. 

23

disambiguationReason

10

C

Reason for disambiguation\.  Must be provided if the disambiguation flag is set to true\. 

See appendix B for information on codes and descriptions\. 

18

disambiguateFrom

11

C

Array of ISWCs\.  Must be provided if the disambiguation flag is set to true\.  Multiple ISWCs must be “|” delimited\.  Each ISWC must be in the correct format and be a valid existing ISWC\.  I\.E\. Must begin with ‘T’ and be followed with a 10 digit number with no separators\.

18

bvltr

12

O

May be provided if the disambiguation flag is set to True

Background, Logo, Theme, Visual or Rolled Up Cue\.  

17

Performers / firstName

13

C

May be provided if the disambiguation flag is set to True\. 

Known list of performer last name and first name pairs\. Used to provide more detail for submissions with the disambiguation flag set to true\.

Multiple names must be “|” delimited\. 

17

Performers /lastName

14

C

Must be provided if Performer firstName is entered

Known list of performers first names\. Used to provide more detail for submissions with the disambiguation flag set to true\.

Multiple names must be “|” delimited and must be provided in same sequence as lastNames above\. 

17

Instrumentation

15

C

May be provided if the disambiguation flag is set to True\. List of  three\-digit Instrument Codes and/or Standard Instrumentation Types as per standard CWR lookup tables\. Multiple values must be “|” delimited\.  

17

derivedWorkType

16

O

Derived Work Type\. One of ModifiedVersion, Excerpt or Composite\.    If not provided, then this isn't a derived work\.

Submission containing a derivedWorkType value won’t be matched against existing ISWCs with the same metadata but which don’t have the same derivedWorkType\.  Multiple submissions with the same metadata \(title and creators\) and the same derivedWorkType values can be disambiguated from each other using the disambiguation flag above, if each submission represents a unique work\.  E\.G\. Multiple “Different Arrangements” of the same work\.    

derivedFromIswcs

17

C

Must be provided if derivedWorkType is set\. 

Array of iswcs that this submission is derived from\.  Multiple ISWCs must be “|” delimited\.  Each ISWC must be in the correct format and be a valid existing ISWC\.  I\.E\. Must begin with ‘T’ and be followed with a 10 digit number with no separators\. 

19

originalTitle

18

M

Original Work Title\.  Multiple values must be “|” delimited\.  

16

ISRCs

19

O

__Though this field is not mandatory, user should enter an empty tab otherwise file will fail validation\.__

Optional list of ISRCs that can be supplied with the submission\. 

24

interestedParty1 / name

20

M

Name of first interested party associated with musical composition, such as: composers, lyricist, translator, and original publishers\.  

Name should be Lastname first, first name\(s\) second\.  E\.G\. “LENNON JOHN WINSTON” or “MCCARTNEY PAUL JAMES” 

11,12,16

interestedParty1 / nameNumber

21

M

IP name number of first interested party associated with musical composition, such as: composers, lyricist, translator, and original publishers\.  

11,12,16

interestedParty1 / 

role

22

M

Role of first interested party associated with musical composition, such as: composers, lyricist, translator, and original publishers\.   Must be one of the following role codes: "CA", "AR", "SE", "PA", ”ES", "AM", "SA", "C", "AD", "A", "E", "AQ", "SR",           "TR"

11,12,16,25

interestedPartyN / name

M

Name of __N__th<a id="footnote-ref-2"></a>[\[1\]](#footnote-2) interested party associated with musical composition, such as: composers, lyricist, translator, and original publishers\.  

Name should be Lastname first, first name\(s\) second\.  E\.G\. “LENNON JOHN WINSTON” or “MCCARTNEY PAUL JAMES” 

11,12,16

interestedPartyN / nameNumber

M

IP name number of __N__th interested party associated with musical composition, such as: composers, lyricist, translator, and original publishers\.  

11,12,16

interestedPartyN / 

role

M

Role of __N__th interested party associated with musical composition, such as: composers, lyricist, translator, and original publishers\.   Must be one of the following role codes: "CA", "AR", "SE", "PA", ”ES", "AM", "SA", "C", "AD", "A", "E", "AQ", "SR",           "TR"

11,12,16,25

#### <a id="_Toc135998160"></a>Transaction Level Validation

Ref

Description

Business Rules Document Ref

11

If work submission does not contain IP with creator role, transaction is rejected\. \(TR\)

IV/07

12

Reject submissions with certain configured Ips, ignore certain Ips in matching and deal with PD Ips 

IV/24, IV/25, IV/29

16

If The matching criteria not satisfied, transaction is rejected\. \(TR\)

MAT/01, MAT/02, MAT/03, MAT/04, MAT/42, MAT/40, MAT/39, MAT/09, MAT/10, MAT/41, MAT/43, MAT/30, MAT/31, MAT/39

17

If the disambiguation flag is set to True, then additional informational metadata may be provided\.   Information on codes and descriptions is provided in Appendix B\.

IV/40

18

If the disambiguation flag is set to True, then a disambiguationReason and disambiguateFrom values must be provided\.

IV/40, PV/30

19

If the derivedWorkType field is set, then the derivedFromIswcs field should be provided\.   The derivedFromISwcs field should contain the list of ISWCs that the work is derived from\.

IV/34, IV/36, IV/38

20

If present must contain one or more ISRCs\.  ISRCs must be a valid format without separators\.  E\.G\. “IE1231212345”\.  In the future additional types of identifier may be supported\.  These identifiers will be initially used for reconciliation only\. 

22

The submitting publisher, if provided, must be a member of the society/agency designated in the submittingAgency field\. 

If none of the creators in a submission are members of that society/agency for the applicable creator roles, then the submitting publisher role must be provided\.  

I\.E\. In order to allocate an ISWC, the submitting publisher must have an “AM” or “E” role on the work being submitted and be a member of the society that the submission is being made through or one of the creators listed on the submission must be a member of the society that the submission is being made through for a valid creator role\. 

#### <a id="_Toc135998161"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

21

If provided the submitting publisher role must be one of “AM” or “E”\.   

23

Must be “true” or “false”

24

All ISRCs must conform to the expected ISRC format\.  I\.E\. fulfil the following regular expression: \[A\-Z\]\{2\}\[A\-Z\\\\d\]\{3\}\\\\d\{2\}\\\\d\{5\}

25

Must be one of the following role codes: "CA", "AR", "SE", "PA", ”ES", "AM", "SA", "C", "AD", "A", "E", "AQ", "SR",           "TR"

### <a id="_Toc135998162"></a>FindSubmission / Resolution Record 

Each FindSubmission/Resolution record should have the following structure:

Field

Field \#

Req

Field Description

Field Rules Reference

recordType

0

M

FindSubmissions

submissionId

1

M

Submitter provided unique id for the transaction within the file\.   This id can be used to tie an acknowledgement record back to its original transaction

submittingAgency

2

M

Agency / Society code that the resolution transactions are being associated with\.  

submittingSourcedb

3

M

Hub of the Agency / Society that the resolution transactions are being associated with\.  In most cases this will be same as the submittingAgency value\.

submittingPublisher / name

4

M

Name of submitting publisher

submittingPublisher / nameNumber

5

M

IP Name Number for the submitting publisher

submittingPublisher / role

6

O

One of two valid roles that the submitting publisher has for this submission\.  

21

submittingPublisher / email

7

M

Contact email address for publisher

workcode

8

M

The publisher’s identifier for the work\.  This information will be saved against the ISWC returned in the ISWC database and can be used by the society to cross validate ISWCs retrieved by publishers with their subsequent CWR submissions\. 

disambiguation

9

O

__Though data for fields 9 – 13 is not mandatory, user should enter an empty tab for each otherwise file will fail validation\.__

Flag\. Indicates if this submission should be disambiguated from other known ISWCs with similar metadata\.

If set to true, then a disambiguationReason must also be provided\. 

23

disambiguationReason

10

C

Reason for disambiguation\.  Must be provided if the disambiguation flag is set to true\. 

See Appendix B for list of codes and descriptions

18

disambiguateFrom

11

C

Array of ISWCs\.  Must be provided if the disambiguation flag is set to true\.  Multiple ISWCs must be “|” delimited\.  Each ISWC must be in the correct format and be a valid existing ISWC\.  I\.E\. Must begin with ‘T’ and be followed with a 10 digit number with no separators\.

18

derivedWorkType

12

O

Derived Work Type\. One of ModifiedVersion, Excerpt or Composite\.    If not provided, then this isn't a derived work\.

Submission containing a derivedWorkType value won’t be matched against existing ISWCs with the same metadata but which don’t have the same derivedWorkType\.  Multiple submissions with the same metadata \(title and creators\) and the same derivedWorkType values can be disambiguated from each other using the disambiguation flag above, if each submission represents a unique work\.  E\.G\. Multiple “Different Arrangements” of the same work\.    

derivedFromIswcs

13

C

Must be provided if derivedWorkType is set\. 

Array of iswcs that this submission is derived from\.  Multiple ISWCs must be “|” delimited\.

Each ISWC must be in the correct format and be a valid existing ISWC\.  I\.E\. Must begin with ‘T’ and be followed with a 10 digit number with no separators\.

19

originalTitle

14

M

Original Work Title\. Multiple values must be “|” delimited\.  

16

additionalIdentifiers / ISRCs

15

O

__Though this field is not mandatory, user should enter an empty tab otherwise file will fail validation\.__

Optional list of ISRCs that can be supplied with the submission\. 

24

additionalIdentifiers / agencyWorkCodes

16

O

__Though this field is not mandatory, user should enter an empty tab otherwise file will fail validation\.__

Agency work codes, used for matching\. Formatted as a "|" delimited collection of tuple values as follows "\(aa1,ww1\)" where ww1 is the first work code and aa1 the agency code\.  
  
These work codes can be used for matching purposes where the user does not have IP information for all creator IPs\. If supplied, the work code is used for matching before defaulting to title and creator metadata as per normal in the event of no match being found\.

interestedParty1 / name

17

M

Name of first interested party associated with musical composition, such as: composers, lyricist, translator, and original publishers\.  

Name should be Lastname first, first name\(s\) second\.  E\.G\. “LENNON JOHN WINSTON” or “MCCARTNEY PAUL JAMES” 

11,12,16

interestedParty1 / nameNumber

18

M

IP name number of first interested party associated with musical composition, such as: composers, lyricist, translator, and original publishers\.  

11,12,16

interestedParty1 / 

role

19

M

Role of first interested party associated with musical composition, such as: composers, lyricist, translator, and original publishers\.   Must be one of the following role codes: "CA", "AR", "SE", "PA", ”ES", "AM", "SA", "C", "AD", "A", "E", "AQ", "SR",           "TR"

11,12,16,25

interestedPartyN / name

M

Name of __N__th<a id="footnote-ref-3"></a>[\[2\]](#footnote-3) interested party associated with musical composition, such as: composers, lyricist, translator, and original publishers\.  

Name should be Lastname first, first name\(s\) second\.  E\.G\. “LENNON JOHN WINSTON” or “MCCARTNEY PAUL JAMES” 

11,12,16

interestedPartyN / nameNumber

M

IP name number of __N__th interested party associated with musical composition, such as: composers, lyricist, translator, and original publishers\.  

11,12,16

interestedPartyN / 

role

M

Role of __N__th interested party associated with musical composition, such as: composers, lyricist, translator, and original publishers\.   Must be one of the following role codes: "CA", "AR", "SE", "PA", ”ES", "AM", "SA", "C", "AD", "A", "E", "AQ", "SR",           "TR"

11,12,16,25

#### <a id="_Toc135998163"></a>Transaction Level Validation

Ref

Description

Business Rules Document Ref

11

If work submission does not contain IP with creator role, transaction is rejected\. \(TR\)

IV/07

12

Reject submissions with certain configured Ips, ignore certain Ips in matching and deal with PD Ips 

IV/24, IV/25, IV/29

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

If the derivedWorkType field is set, then the derivedFromIswcs field should be provided\.   The derivedFromISwcs field should contain the list of ISWCs that the work is derived from\.

IV/34, IV/36, IV/38

#### <a id="_Toc135998164"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

21

If provided the submitting publisher role must be one of “AM” or “E”\.   

23

Must be “true” or “false”

24

All ISRCs must conform to the expected ISRC format\.  I\.E\. fulfil the following regular expression: \[A\-Z\]\{2\}\[A\-Z\\\\d\]\{3\}\\\\d\{2\}\\\\d\{5\}

25

Must be one of the following role codes: "CA", "AR", "SE", "PA", ”ES", "AM", "SA", "C", "AD", "A", "E", "AQ", "SR",           "TR"

###  <a id="_Toc135998165"></a>Acknowledgment Record 

Each Acknowledgement record should have the following structure:

Field

Field \#

Req

Field Description

Field Rules Reference

recordType

0

M

Acknowledgement

submittingAgency

1

M

Agency / Society code that the resolution transactions are being associated with\.  

submittingSourcedb

2

M

Hub of the Agency / Society that the resolution transactions are being associated with\.  In most cases this will be same as the submittingAgency value\.

submittingPublisher / name

3

M

Name of submitting publisher

submittingPublisher / nameNumber

4

M

IP Name Number for the submitting publisher as provided in 

submittingPublisher / role

5

O

Role of submitting publisher if provided in original submission\.

submittingPublisher / email

6

M

Contact email address of submitting publisher as provided in original submission

originalSubmissionId

7

M

The submissionId of the original transaction that this acknowledgement applied to\.

2,11

preferredIswc

8

O

Preferred ISWC assigned to the original transaction\. 

7

workcode

9

O

Represents the publisher’s identifier for the work associated with the outcome of the original transaction\.   

originalTitle

10

O

Represents the original title for the work associated with the outcome of the original transaction\. 

processingDateTime

11

M

The date/time this transaction or file was formally processed by the recipient \(typically the ISWC database\)

9

transactionStatus

12

M

The status or outcome of the original transaction\.  One of: FullyAccepted or Rejected

10

errorMessages

13

C

Details of any errors that occurred with the transaction\.   

11

workInfo1 / agency

14

C

Identifies the society of the first society work code associated with the returned preferred ISWC\.   Will be provided if a preferred ISWC is assigned and existing society work codes are already linked to that ISWC\.

workInfo1 / sourcedb

15

C

Identifies the hub used for the first society work code associated with the returned preferred ISWC\.  Will be provided if a preferred ISWC is assigned and existing society work codes are already linked to that ISWC\. 

workInfo1 / workcode 

16

C

Identifies the first society work code associated with the returned preferred ISWC\.  Will be provided if a preferred ISWC is assigned and existing society work codes are already linked to that ISWC\. 

workInfoN / agency

C

Identifies the society of the __N__th society or Publisher work code associated with the returned preferred ISWC\.   Will be provided if a preferred ISWC is assigned and existing society work codes are already linked to that ISWC\.

workInfoN / sourcedb

C

Identifies the hub used for the __N__th society work code associated with the returned preferred ISWC\.  Will be provided if a preferred ISWC is assigned and existing society work codes are already linked to that ISWC\. 

workInfoN / workcode 

C

Identifies the __N__th society work code associated with the returned preferred ISWC\.  Will be provided if a preferred ISWC is assigned and existing society work codes are already linked to that ISWC\. 

#### <a id="_Toc135998166"></a>Transaction Level Validation

Ref

Description

Business Rules Document Ref

11

Only one acknowledgment is allowed per transaction\. \(TR\)

#### <a id="_Toc135998167"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

1\.

Must match the creation date/time of the original file that contained the transaction that this acknowledgement applies to as per the fileHeader of that original file\. 

2\.

The originalSubmissionId must exist within the file referred to by originalFileCreationDateTime\. \(TR\)

7\.

If provided, preferred ISWC must be a valid ISWC Number\. \(TR\)

IV/14

9\.

Must be a valid date/time \(TR\)

10\.

Must be one of: FullyAccepted, PartiallyAccepted or Rejected

11\.

Will be provided if the transaction has a transactionStatus of “PartiallyAccepted” or “Rejected”

## <a id="_Toc135998168"></a>File Naming, Transfer and Encoding 

### <a id="_Toc135998169"></a>File Encoding

All files will be encoded in UTF\-8\.

### <a id="_Toc135998170"></a>File Transfer and Location

The standard for transmission will be the Secure File Transfer Protocol \(SFTP\)\.  Each ISWC agency or publisher will push files to an agency and publisher specific secure folder in a central ISWC Database public SFTP site and pull files from that secure folder\. 

### <a id="_Toc135998171"></a>File Naming Convention

ISWCP – identifies an ISWC Publisher file 

YYYY\-MM\-DDTHH\-MM\-SS identifies the creation date and time of the file converted to UTC

SSS – Represents the Agency \(Society\) identifier of the sender of the file

PPP – Represents the Publisher Sender ID code as per CWR Sender IDs involved in this exchange

RRR – Represents the Agency \(Society\) identifier of the receiver of the file

YYNN – Represents an additional optional descriptor for the file\.  E\.G\. Publishers could use the existing convention as used by CWR for this\. 

If Sony/ATV Music Publishing LLC \(Sender ID SA\) send through IMRO \(society code 128\), a file that was created at 18:25 on Nov 25th, 2019 to the ISWC Database Centre \(society code 315\), the filename would be:

iswcp\_2019\-11\-25T18\-25\-43\_128\_SA\_315\_SampleSubmissions\.txt

1. <a id="_Toc135998172"></a>Changes to Agency Solution 

This chapter describes the changes needed to the already developed Agency solution in order to be able to support the new ISWC Publisher JSON format data exchange described above\.  

## <a id="_Toc135998173"></a>ISWC Database Data\-Structure Changes  

This section describes the changes to existing data structures required to support the combined allocation and resolution solution\.

## <a id="_Toc135998174"></a>ISWC Database Data\-Structure Changes  

This section describes the changes to existing data structures required to support the combined allocation and resolution solution\.

###  <a id="_Toc135998175"></a>\[ISWC\]\.\[AdditionalIdentifier\] Table

The current \[ISWC\]\.\[WorkInfo\] table holds up\-to\-date information on each Agency submission, both ISWC eligible and non ISWC eligible submissions, linked to an ISWC\.  Where an agency, using the same agency workcode, provides a secondary submission using the same agency workcode the existing \[WorkInfo\] record with that workcode is updated\.  

In the existing CSI system, allocation requests generate the equivalent of a new \[WorkInfo\] record using an autogenerated agency workcode\.   We propose to continue this approach in the new system\.

In addition, we propose to capture an identifier for the publisher and the publishers provided workcode\.  We will also capture other identifiers for the work being submitted \(initially an optional ISRC\)\.  These identifiers will be held in the new \[AdditionalIdentifier\] table:

Column

Data Type 

Required

Description

AdditionalIdentifierID

Bigint

Y

Autogenerated Primary Key 

WorkInfoID

Bigint

Y

Foreign Key to ISWC\.WorkInfo table

NumberTypeID

Int

Yes

ID for Number\.   Linked to a new \[Lookup\] schema table\.   Each Publisher will map to a unique NumberTypeID value\. Additional types of numbers such as ISRCs will also map to a unique NumberTypeID value\. 

WorkIdentifier

Nvarchar\(20\)

No

Identifier for the specified number type \(either an ISRC or publisher workcode\)r

### <a id="_Toc135998176"></a>\[Lookup\]\.\[NumberType\] Table

Column

Data Type 

Required

Description

NumberTypeID

Int

Yes

ID for a work number type\.  

Each Publisher will map to a unique NumberTypeID value\. Additional types of numbers such as ISRCs will also map to a unique NumberTypeID value\.  Publisher NumberTypeID values should start at 1000

Code

Nvarchar\(10\)

Yes

Code for number\.  Will be a publisher submitter code such as “SA” or “ISRC”

Description

Nvarchar\(80\)

No

Description for number type

### <a id="_Toc135998177"></a>\[Lookup\]\.\[PublisherSubmitterCode\] Table

Lookup data to map submitting publisher IP Name Numbers to a common SubmitterCode so that this SubmitterCode can in turn map to NumberType to capture publisher workcodes in a consistent way\.  See CISAC document for the current mapping info: [https://members\.cisac\.org/CisacPortal/cisacDownloadFile\.do?docId=8558](https://members.cisac.org/CisacPortal/cisacDownloadFile.do?docId=8558)

Column

Data Type 

Required

Description

PublisherSubmitterCodeID

Int

Yes

Autogenerated ID 

Code

Nvarchar\(10\)

Yes

Should correspond to a TypeCode value in the \[NumberType\] lookup table

IPNameNumber

Int

Yes

IP Name Number of submitter 

1. <a id="_Toc31797577"></a><a id="_Toc32668044"></a><a id="_Toc135998178"></a><a id="_Toc355093999"></a><a id="_Toc485799689"></a>Reporting

This chapter describes summarises key reporting requirements for the allocation and resolution solution\.   This chapter is for information purposes only – it does not form part of the scope of this design document\.  The reporting module itself will be defined in a separate specification that will cover the reporting capabilities that will be available for agencies across all centrally maintained ISWC data\.     

Key reporting requirements:

1. Societies will be provided with a report of ISWCs, including meta\-data, in which the society has an interest \(including those which have been requested through a different society\) 
2. Ratio/number of ISWCs created by the ISWC Allocation Service and ISWCs verified when a society work code is assigned and submitted for validation\. 
3. Ratio/Number of requests rejected due to failing technical validation
4. Ratio/Number of requests rejected due to failing business validation 
5. Additional Stats to capture compliancy to SLOs 
6. Number of ISWCs allocated including historical \(comparative report against historical data\)

 

1. For the resolution service, reporting should allow a society to identify which of its publishers are making requests, which works they are requesting ISWCs for and what information is being returned to the publisher

# <a id="_Toc135998179"></a>Appendix A – Open and Closed Items

This appendix provides a tracking list of specific issues/queries raised by CISAC during the specification process and how they were incorporated or excluded from this specification:

__Open and Closed Items__

__ID__

__Description__

__Response__

__Status__

__Next Action By__

1

Open Item:  Is it possible to get an ISWC for a work where the society that allocation request is going through doesn’t have an Administrator, Original Publisher or Creator on the work data\. 

In the existing allocation file formats, the submitter is treated as an original publisher or administrator for each request / line item and therefore the same rules as are in place for society submissions can be implemented for publisher submissions through a society \(allocations\)\.  In the new format we are also asking for the publisher role which would need to be one of administrator or original publisher\.   This is important to bring the implementation of the rules applied for eligibility into alignment with the rules in place for agencies\. 

Design team to validate this with publishers as part of the file format review\.   

__*Updated following workshop with Publishers *__

Incorporation of publisher role is ok with Publishers\.  Also added a default value at header level\. 

* *

Closed

Design Team

2

Open Item: Must all allocation service requests go through societies file shares 

This specification is for a single combined software application to cater for allocations and resolution\.  

It is agnostic as to the process through which files get to or are retrieved from the service\.   The introduction section has been updated to reflect this\.

Closed

John C

3

Open Item: Remove any disambiguation data that is provided though allow it in the format\.  

John C to investigate how this can be done\. 

Ans: This can be done reasonably easily as we can just not pass it through from the file format into the web service\.  When needed going forward we can add it back in again\.  Are we sure we really want to do this though?  It seems simpler to me to just say to the publishers not to use it for now and just monitor it\.  If needed, I’ll add in some additional rules against these fields in the format to define it\.   Next action is for the design team to decide if we need to do this or not\. 

__*Updated following workshop on 10th Feb: *__

Agreed with the design team that either way of doing it is acceptable\.  I\.E\. Through the software or through communicating to publishers that they should not use the feature until notified\. 

I suggest we proceed with the “communicating to publishers” approach\. 

__*Updated following workshop with Publishers *__

In general Publishers were ok with a delayed usage of the disambiguation functionality\.  

Closed

Design Team

4

Open Item:  Support the capture of additional identifiers such as ISRCs through allocation requests\.   

I’ve added in an additional identifiers section that for now just includes ISRCs\.  See 3\.1 and 3\.2 above\.  

Closed

5

John C to track submitters work numbers for both allocation and resolution service\.

This is now reflected in section 3\.2 and the associated developer artefacts

Closed\.

6

Need to test the idea that we can consolidate on a single JSON format with Publishers

Next action is for the design team to take the provisionally agreed specification to publishers and get their feedback\. 

__*Updated following workshop on 10th Feb: *__

As a fall back we can look to support the existing EDI formats for allocations and resolution in addition to the JSON one through with minor changes to collect the required additional data \(publisher role and publisher work number\)

__*Updated following workshop with Publishers *__

EDI based format is not required\. JSON and Flat File formats have been included\.  

Closed

Design Team

7

Currently this spec doesn’t incorporate updating the embedded process as it is currently not in use\.  Need to validate if this is the case\. 

Added section 2\.6 to document the options for societies in implementing an embedded process\.  This specification does not include the redevelopment of the IAS Conversion Tool\.  This tool in not in live use and no society has been identified to carry out testing with\.   I propose that we consider building a replacement for this tool only if there is real demand for it\.  It could be built and tested quickly so if a society wants to use it then it could be developed then\.  Next action is for the design team to confirm that this is acceptable or not\. 

__*Updated following workshop on 10th Feb: *__

Suggest that we revisit this after the publisher consultation\.  If we end up also supporting the existing EDI formats for allocations and resolution, then the existing FastTrack file converter application could still be used

__*Updated following workshop with Publishers *__

EDI based format is not required\. JSON and Flat File formats have been included\.  

Closed

Design Team

8

Need to define the requirements for reporting across 

Design Team to review specific requirements listed by Niamh in chapter 5 and confirm if these are required

Agreed that the overall ISWC Database reporting solution will be covered by a separate specification

Closed

Design Team

9

John C to include list of disambiguation reason codes to the document

Included in Appendix B\.

Closed

# <a id="_Toc135998180"></a>Appendix B – Lookup Codes and Descriptions

This appendix provides a list of lookup codes and descriptions used in the above specification: 

__Disambiguation Reason Codes__

__Code__

__Description__

DIT

Different work which has the same title and creators\. Can be distinguished by adding supplemental information, such as performer or instrumentation details\.

For example:

Work 1\. Title = Happy / Creator = Pharrell Williams

Work 2\. Title = Happy / Creator = Pharrell Williams / Performer = N\.E\.R\.D

DIA

The work has the same title and creators but is a different arrangement of the work

For example:

An arrangement for specific instruments, e\.g\. strings

A different arrangement for a new album

DIE

The work has the same title and creators but is an Excerpt of another work

DIC

The work has the same title and creators but is a different AV Work / Cue 

The BVLTR indicator can be used to provide further information about what type of cue the work is, i\.e\. background, logo, theme, visual or rolled up cue

DIV

The work has the same title and creators but is a different version of a work \(excerpt, modified work e\.g\. instrumental\) with different shares

__Derived Work Types__

__Code__

__Description__

ModifiedVersion

Modified version of a work

Excerpt

Excerpt of a work

Composite

Composite of one or more works 

1. <a id="footnote-2"></a> The name, nameNumber and role columns will be repeated for each Interested Party \(IP\) in a record / submission\.   If a record has two IPs then the three columns will be repeated twice\.  If a record has __N__ IPs then the three columns will be repeated N times\.   [↑](#footnote-ref-2)


2. <a id="footnote-3"></a> The name, nameNumber and role columns will be repeated for each Interested Party \(IP\) in a record / submission\.   If a record has two IPs then the three columns will be repeated twice\.  If a record has __N__ IPs then the three columns will be repeated N times\.   [↑](#footnote-ref-3)



