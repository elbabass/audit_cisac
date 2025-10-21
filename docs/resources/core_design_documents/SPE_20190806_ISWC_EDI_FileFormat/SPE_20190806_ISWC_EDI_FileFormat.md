  
![A close up of a sign

Description automatically generated](1.png)

![](2.png)

CISAC

ISWC Database EDI Format  

# <a id="_Toc399421500"></a><a id="_Toc399422154"></a><a id="_Toc485799633"></a><a id="_Toc46834319"></a>Document Control

## <a id="_Toc158527933"></a><a id="_Toc399421501"></a><a id="_Toc399422155"></a><a id="_Toc485799634"></a><a id="_Toc46834320"></a>Change Record

Date

Person

Version/Reference

6th Aug 2019

John Corley, 

Curnan Reidy  

V1\.0 / Initial Draft

2nd Sep 2019

Curnan Reidy

Added NAT, ACK and CSN record types

9th Sep 2019

Curnan Reidy

Incorporated feedback from final design team workshop and prepared document for sign off by SG

17th Sep 2019

John Corley

Updated at Steering Group Meeting and Signed Off

28th Jul 2020

John Corley

Included CWI record in CSN

27 May 2021

John Corley

V1\.5 / Minor corrections based on feedback from APRA

<a id="_Toc158527934"></a><a id="_Toc399421502"></a><a id="_Toc399422156"></a>

## <a id="_Toc485799635"></a><a id="_Toc46834321"></a>Reviewers

<a id="_Toc158527935"></a><a id="_Toc399421503"></a><a id="_Toc399422157"></a><a id="_Toc485799636"></a>

Katrien Tielemans

Bolmar Carrasquilla 

Ed Oshanani 

Bolmar Carrasquilla

Tim Carmichael

Didier Roy 

Hanna Mazur 

José Macarro 

Sylvain Piat 

John Corley

Niamh McGarry

Declan Rudden

Curnan Reidy

## <a id="_Toc46834322"></a>Distribution

Reviewers

## <a id="_Toc158527936"></a><a id="_Toc399421504"></a><a id="_Toc399422158"></a><a id="_Toc485799637"></a><a id="_Toc46834323"></a>Approval

This document was approved electronically via email by the following people on the following dates:

Date/Time

Person

Note

# <a id="_Toc46834324"></a>Table of Contents<a id="_Toc485799638"></a>

[Document Control	2](#_Toc46834319)

[Change Record	2](#_Toc46834320)

[Reviewers	2](#_Toc46834321)

[Distribution	3](#_Toc46834322)

[Approval	3](#_Toc46834323)

[Table of Contents	4](#_Toc46834324)

[1	Introduction	7](#_Toc46834325)

[What does this document contain?	7](#_Toc46834326)

[Who should read this document?	7](#_Toc46834327)

[References	7](#_Toc46834328)

[2	Introduction to New \(Modern\) EDI Format	8](#_Toc46834329)

[2\.1\. Relationship to the existing \(classic\) CSI EDI format	8](#_Toc46834330)

[2\.2\. EDI Terminology	8](#_Toc46834331)

[2\.3\. Properties of EDI Components	9](#_Toc46834332)

[2\.3\.1\. Data Element Properties	9](#_Toc46834333)

[2\.3\.2\. Detail Record Properties	11](#_Toc46834334)

[2\.3\.3\. Transaction Header Properties	11](#_Toc46834335)

[2\.3\.4\. Validation	12](#_Toc46834336)

[2\.3\.5\. Record Prefixes	12](#_Toc46834337)

[2\.3\.6\. Field Level Validation	13](#_Toc46834338)

[2\.4\. File Structure	13](#_Toc46834339)

[2\.4\.1\. File Naming Convention	13](#_Toc46834340)

[2\.4\.2\. Control Records	14](#_Toc46834341)

[2\.4\.3\. Transaction Orchestration	14](#_Toc46834342)

[2\.4\.3\.1\. CAR: ISWC Add Request	14](#_Toc46834343)

[2\.4\.3\.2\. CUR: ISWC Update Request	15](#_Toc46834344)

[2\.4\.3\.3\. CDR: ISWC Delete Request	15](#_Toc46834345)

[2\.4\.3\.4\. CMQ: CSI Meta Data Queries	16](#_Toc46834346)

[2\.4\.3\.5\. CIQ: CSI ISWC Queries	16](#_Toc46834347)

[2\.4\.3\.6\. MER: ISWC Merge 	16](#_Toc46834348)

[2\.4\.4\. Transaction / Record Type Mapping	18](#_Toc46834349)

[2\.4\.4\.1\. CAR Transaction	18](#_Toc46834350)

[2\.4\.4\.2\. CUR Transaction	18](#_Toc46834351)

[2\.4\.4\.3\. CDR Transaction	19](#_Toc46834352)

[2\.4\.4\.4\. CMQ Transaction	19](#_Toc46834353)

[2\.4\.4\.5\. CIQ Transaction	19](#_Toc46834354)

[2\.4\.4\.1\. MER Transaction	20](#_Toc46834355)

[2\.4\.4\.1\. CSN Transaction	20](#_Toc46834356)

[3	New \(Modern\) EDI Format Record Types	21](#_Toc46834357)

[3\.1\. CAR Record Format	21](#_Toc46834358)

[3\.1\.1\. Transaction Level Validation	22](#_Toc46834359)

[3\.1\.2\. Field Level Validation	23](#_Toc46834360)

[3\.2\. CUR Record Format	24](#_Toc46834361)

[3\.2\.1\. Transaction Level Validation	25](#_Toc46834362)

[3\.2\.2\. Field Level Validation	26](#_Toc46834363)

[3\.3\. CDR Record Format	27](#_Toc46834364)

[3\.3\.1\. Transaction Level Validation	28](#_Toc46834365)

[3\.3\.2\. Field Level Validation	29](#_Toc46834366)

[3\.4\. CMQ Record Format	29](#_Toc46834367)

[3\.4\.1\. Transaction Level Validation	30](#_Toc46834368)

[3\.4\.2\. Field Level Validation	31](#_Toc46834369)

[3\.5\. CIQ Record Format	32](#_Toc46834370)

[3\.5\.1\. Transaction Level Validation	33](#_Toc46834371)

[3\.5\.2\. Field Level Validation	33](#_Toc46834372)

[3\.6\. MER Record Format	34](#_Toc46834373)

[3\.6\.1\. Transaction Level Validation	35](#_Toc46834374)

[3\.6\.2\. Field Level Validation	35](#_Toc46834375)

[3\.7\. ACK Record Format	36](#_Toc46834376)

[3\.7\.1\. Transaction Level Validation	37](#_Toc46834377)

[3\.7\.2\. Field Level Validation	38](#_Toc46834378)

[3\.8\. CSN Record Format	38](#_Toc46834379)

[3\.8\.1\. Transaction Level Validation	40](#_Toc46834380)

[3\.8\.2\. Field Level Validation	40](#_Toc46834381)

[3\.9\. WFT Record Format	40](#_Toc46834382)

[3\.9\.1\. Field Level Validation	41](#_Toc46834383)

[3\.10\. CTL Record Format	41](#_Toc46834384)

[3\.10\.1\. Field Level Validation	42](#_Toc46834385)

[3\.11\. NAT Record Format	42](#_Toc46834386)

[3\.11\.1\. Field Level Validation	43](#_Toc46834387)

[3\.12\. CIP Record Format	43](#_Toc46834388)

[3\.12\.1\. Field Level Validation	44](#_Toc46834389)

[3\.13\. DIS Record Format	44](#_Toc46834390)

[3\.13\.1\. Field Level Validation	45](#_Toc46834391)

[3\.14\. DER Record Format	45](#_Toc46834392)

[3\.14\.1\. Field Level Validation	46](#_Toc46834393)

[3\.15\. PER Record Format	46](#_Toc46834394)

[3\.15\.1\. Field Level Validation	47](#_Toc46834395)

[3\.16\. INS Record Format	47](#_Toc46834396)

[3\.16\.1\. Field Level Validation	47](#_Toc46834397)

[3\.17\. MLI Record Format	48](#_Toc46834398)

[3\.17\.1\. Field Level Validation	48](#_Toc46834399)

[3\.18\. CWI Record Format	49](#_Toc46834400)

[3\.19\. Lookup Tables	49](#_Toc46834401)

[3\.19\.1\. Derived Work Type	49](#_Toc46834402)

[3\.19\.2\. BVLTR	50](#_Toc46834403)

[3\.19\.3\. Disambiguation Reason	50](#_Toc46834404)

[3\.19\.4\. Transaction Status	50](#_Toc46834405)

[Appendix A – Open and Closed Items	52](#_Toc46834406)

1. <a id="_Toc399422159"></a><a id="_Toc399421505"></a><a id="_Toc158527937"></a><a id="_Toc53481878"></a><a id="_Toc399422160"></a><a id="_Toc399421506"></a><a id="_Toc158527938"></a><a id="_Toc485799639"></a><a id="_Toc46834325"></a>Introduction

## <a id="_Toc158527939"></a><a id="_Toc399421507"></a><a id="_Toc399422161"></a><a id="_Toc485799640"></a><a id="_Toc46834326"></a>What does this document contain?

It provides a detailed specification and design of the extended EDI file format that will be supported by the ISWC Database\.  It also serves as the specification for the new ISWC Database file processor\. 

## <a id="_Toc158527940"></a><a id="_Toc399421508"></a><a id="_Toc399422162"></a><a id="_Toc485799641"></a><a id="_Toc46834327"></a>Who should read this document?

CISAC development and project management personnel\. Society development and project management personnel\.  Spanish Point development team members\.   

## <a id="_Toc158527942"></a><a id="_Toc399421510"></a><a id="_Toc399422164"></a><a id="_Toc485799643"></a><a id="_Toc46834328"></a>References

Reference

Description

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

CIS\-Net\_BS\-03\_EDIFileFormats\.doc

Existing \(classic\) EDI file format specification \(V2\.00\)

1. <a id="_Toc46834329"></a>Introduction to New \(Modern\) EDI Format

The new modern EDI format extends the existing CSI EDI format\.  It adds support for the following:

- Additional disambiguation data
- Multi\-lingual character sets in work titles
- Merge transactions
- Derived works 

<a id="_Toc468452208"></a><a id="_Toc496111659"></a><a id="_Toc496111662"></a><a id="_Toc496111669"></a><a id="_Toc496111673"></a><a id="_Toc496111677"></a><a id="_Toc496111684"></a><a id="_Toc496111690"></a>

## <a id="_Toc46834330"></a>Relationship to the existing \(classic\) CSI EDI format 

This new EDI format \(V3\.00\) extends the existing classic EDI format \(V2\.00\)\.  The specification for the existing V2\.00 format is available in the Fastrack document ref: CIS\-Net\_BS\-03\_EDIFileFormats\.docx

## <a id="_Toc46834331"></a>EDI Terminology 

This section has been extracted from the CIS Guidelines for Electronic Data Interchange

\(IS/IM/47\)\. The guidelines were developed by the International Modelling Group and approved by the CISAC community for general use by all societies for all types of data transmission\.

__Data Element: __the basic unit of information in the EDI standard\. Data elements \(also referred to as Fields\) contain information that represents a singular fact, for example, song title, date of birth, or production year\. Data elements can be codes, numeric values, or literal descriptions\. Data element definitions must include a description, specifications for data type, size, and if appropriate, a list of code values and definitions of each value\.

__Detail Record: __a combination of functionally related data elements\. An identifier or record type is placed at the beginning of each detail record identifying its purpose\. Detail record definitions must include a description, the list of data elements that are included in the detail record along with an indication of whether each data element is mandatory, optional, or conditional, and the validation rules to insure all data elements are correct\.

__Transaction Header: __a combination of functionally related data elements that is also used to define the start of a business transaction\. An identifier or transaction code is placed at the beginning of each transaction header identifying its purpose\. Transaction header definitions must include a description, the list of data elements that are included in the transaction header record along with an indication of whether each data element is mandatory, optional, or conditional, and the validation rules to insure all data elements are correct\.

__Transaction: __a transaction header that may or may not be followed by any number of supporting detail records\. A transaction contains all the data required to define a specific business transaction

e\.g\. transactions may represent the equivalent of: 

- Work Registrations 
- Agreements
- Interested Party Information

Transaction definitions must include a list of what detail records can be included in the transaction along with an indication of whether the detail record is mandatory, optional, or conditional\. For each of those detail records, the definition must also indicate the repeat frequency for the record \(how many times can this record occur within this transaction\)\.

__Group: __composed of one or more transactions of the same type\. Each group begins with a header record that identifies the type of transaction contained in the group, and ends with a trailer that includes control totals summarizing the content of the file\. Note that a group can contain up to 10,000,000 transactions\.

__File: __composed of one or more groups\. Files are the unit that ends up getting transmitted between CISAC EDI participants\. Each file begins with a header record that identifies the file’s origination and ends with a trailer that includes control totals summarizing the content of the file\.

__Control Records: __provide information about the content of a group or file\. These records ensure that only authorized users of this facility are participating and that the integrity of each transaction is maintained as the file travels through various telecommunication lines\.

## <a id="_Toc46834332"></a>Properties of EDI Components

Files transmitted within this standard must exhibit the following characteristics:

- All data will be ASCII encoded, except for the NAT record which will accept UTF\-8 encoding\.
- Records are variable length and carriage return / line feed \(CR/LF\) delimited
- Fields within the records are fixed length and are not delimited

The standard for transmission will be the Secure File Transfer Protocol \(SFTP\)\.  Each ISWC agency will push files to an agency specific secure folder in a central ISWC Database public SFTP site and pull files from that secure folder\. 

### <a id="_bookmark3"></a><a id="_Toc46834333"></a>Data Element Properties

The following information will be defined for each data element within the CISAC EDI standard:

- __Field: __Indicates the name of the data element
- __Start: __The position in the record where this field starts \(note the first position of the record is
- “1”\)\.
- __Size: __The number of characters that this field will occupy\.
- __Format: __The type of data included in this field\. Data present in a field that is inconsistent

with the rules defined below will cause that record \(and potentially the entire transaction\) to be rejected\. The legend of type abbreviations is:

__Code__

__Description__

__Default Size__

__Rules__

__A__

Alpha or Alphanumeric

None

Any string containing valid ASCII text\. Note that nulls are not acceptable, and all alphabetic characters must be in upper case\. If there is no data to be entered in an alpha field, blanks must be entered\.

__B__

Boolean

1

Field must be equal to <Y>es or <N>o

__F__

Flag

1

Field must be equal to <Y>es, <N>o, or <U>nknown\. Note that the difference between Boolean and Flag is the allowance of an unknown condition for all fields declared as Flag\.

__D__

Date

8

Dates are all formatted as YYYYMMDD\. If there is no data to be entered in a date field, zeroes must be entered\.

__N__

Numeric

None

Numeric fields are to be right justified and zero filled\. If there is an implied decimal point, it will be defined in the record layout\. If there is no data to be entered in a numeric field, zeroes must be entered\.

__T__

Time	or Duration

6

Time/Duration fields are all formatted as HHMMSS\. Time of day is to be expressed in 24 hour format, otherwise known as military time\. If there is no data to be entered in a time or duration field, zeroes must be entered\.

__L__

List or Table Lookup

None

The valid entries for these fields come from a list in the field description or a table\. Note that the values for these tables are to be found in the Lookup Table section of this document\.

- __Req: __Indicates whether or not an entry must be present in this field\. Values in the REQ field will be: 

__M: __Mandatory, meaning this field must be filled out\. If it is not filled out, this record will be rejected and, depending on the record type, the entire transaction may be rejected\.

Note that not all record types are mandatory; however, there are mandatory fields within optional records\.

__C:__ Conditional, meaning this field may be Mandatory depending on other conditions that exist in either the current record or the transaction\. If the condition results in a mandatory field and this field is not present, this record will be rejected and, depending on the record type, the entire transaction may be rejected\.

__O:__ Optional, meaning this field may or may not be entered\.

- __Field Description: __Provides a basic description of the field to be entered\. Also included will be the individual table where valid entries reside for fields with a format type equal to “L”\.

### <a id="_bookmark4"></a><a id="_Toc46834334"></a>Detail Record Properties

The following information will be defined for each detail record within the CISAC EDI standard:

- __Record Description: __Provides a detailed description of the purpose of this record\.
- __Record Format: __Lists the data elements from which this detail record is composed\. Each data element definition within the Record Format section will include all Data Element Properties as listed above\.
- __Record Level Validation: __The validation criteria that will be applied by the recipient to this detail record when received\. Record level validation insures validity of this detail record\.
- __Field Level Validation: __The validation criteria that will be applied by the recipient to this detail record when received\. Field level validation ensures the validity of each data element contained in the detail record\.

### <a id="_bookmark5"></a><a id="_Toc46834335"></a>Transaction Header Properties

The following information will be defined for each transaction header within the CISAC EDI standard:

- __Transaction Description:__ Provides a detailed description of the purpose of this transaction\. 
- __Transaction Format:__ Lists the transaction header and the various detail records \(if any\) from which this transaction is composed\. For each detail record, three additional items are defined:
- Req: indicates whether the detail record/transaction header is Mandatory \(M\), Optional \(O\), or Conditional \(C\)\.
- Max Use: Indicates the number of times this detail record can appear within a transaction\. Values are either 1 meaning the record can only occur once, or M meaning the record can appear as many times as is required\.
- Comment: Used to communicate any additional information which may be helpful to those implementing the transaction\.
- __Record Description:__ Provides a detailed description of the purpose of this transaction header\. 
- __Record Format:__ Lists the data elements from which this detail record is composed\. Each data element definition within the Record Format section will include all Data Element Properties as listed above\.
- __Transaction Level Validation:__ The validation criteria that will be applied by the recipient to this detail record when received\. Transaction level validation ensures validity of this detail record as it relates to the overall transaction\.
- __Field Level Validation:__ The validation criteria that will be applied by the recipient to this detail record when received\. Field level validation ensures the validity of each data element contained in the detail record\.

### <a id="_bookmark6"></a><a id="_Toc46834336"></a>Validation

Subsequent to each detail record or transaction header description, a set of validation criteria will be provided\. These criteria are listed at different potential levels depending on the record being edited\. The levels of validation are File, Group, Transaction, Record, or Field\. As a result of validation, the same levels of detail may be rejected from the data\. Rejections are indicated at the end of the validation criteria with one of the following codes: 

- __ER: __Entire File is rejected
- __GR: __Entire Group is rejected
- __TR: __Entire Transaction is rejected
- __RR: __Entire Record is rejected
- __FR: __Field is rejected and a default value is specified for the field

### <a id="_bookmark7"></a><a id="_Toc46834337"></a>Record Prefixes

Each Transaction Header and Detail Record contains a prefix that identifies both the record and the transaction that is being delivered\. The attached table describes the layout of the prefix area: 

__Field__

__Start__

__Size__

__Fmt__

__Req__

__Field Description__

__Record Type__

1

3

L

M

The three character transaction type or detail record type\. These values reside in the Record Type Table\.

__Transaction Sequence \#__

4

8

N

M

If this is the first transaction within a group, the *Transaction Sequence \# *must be equal to 00000000\. Otherwise, for transaction headers, the *Transaction Sequence \# *must be equal to the previous transaction header’s *Transaction Sequence \# *incremented by 1\. For detail records, the *Transaction*

*Sequence \# *must be equal to the *Transaction Sequence \# *of the previous transaction header\.

__Record Sequence \#__

12

8

N

M

For transaction headers, always set to 00000000\. For detail records, set this field to the *Record Sequence \# *of the previous record written to the file

incremented by 1\.

### <a id="_bookmark8"></a><a id="_Toc46834338"></a>Field Level Validation

1. *Record Type *must be either a valid transaction type or a valid detail record type\. \(ER\)
2. If this is the first transaction header in the group, *Transaction Sequence \# *must be equal to 0\. \(ER\)
3. If this is a transaction header that is not the first transaction header in the group, the Transaction Sequence \# must be equal to the previous transaction’s Transaction Sequence \# incremented by 1\. \(TR\) 
4. If this is a detail record, the Transaction Sequence \# must be equal to the previous record’s Transaction Sequence \#\. \(TR\) 
5. If this is a transaction header record, the *Record Sequence \# *must be equal to zero\. \(ER\)
6. If this is a detail record, the *Record Sequence *\# must be equal to the previous record’s

*Record Sequence \# *incremented by 1\. \(ER\)

1. If the *Transaction Sequence \# *on subsequent transactions are not in sequential order within a group, the entire file will be rejected\. \(ER\)
2. If any detail records belonging to a transaction header do not carry the same *Transaction Sequence \# *as the preceding transaction header, the subordinate records are out of sequence\. In this case, the entire file will be rejected\. \(ER\)
3. Record length must match the record length specified within the specification\. \(ER\)

## <a id="_Toc46834339"></a>File Structure 

### <a id="_Toc46834340"></a>File Naming Convention

ISWC – identifies a ISWC file 

YYYYMMDD – identifies the creation date of the file

HHMMSS – identifies the creation time of the file

RRR – Represents the Agency identifier of the receiver of the file

SSS – Represents the Agency identifier of the sender of the file

CSN – is the file extension

If the file is zipped, it will be named ISWCYYYYMMDDHHMMSSRRRSSS\.zip\. The unzipped file it contains will be named as above with the version number\.

If SOCAN \(society code 101\) sends an uncompressed file that was created at 08:01am on January 1 2007 and conforms to version 3\.0 of the standard, to the CSI Center \(society code 315\), the filename would be:

ISWC20070101080100315101\.CSN

The compressed version of the same file would be named:

ISWC20070101080101315101\.zip

### <a id="_Toc46834341"></a>Control Records

The following record layouts are used to partition and control the submission of files between participants\. Proper control records are required within the file to ensure the integrity of transmission over telecommunication lines, as well as confirming that the data within the file has not been altered as a result of intentional or unintentional tampering with data\.

Control records defined within this version of the standard are:

- __HDR: __Transmission Header
- __GRH: __Group Header
- __GRT: __Group Trailer
- __TRL: __Transmission Trailer

### <a id="_Toc46834342"></a>Transaction Orchestration

The following describes the intended source and recipients for each transaction type along with the expected corresponding transactions:

#### <a id="_Toc196729659"></a><a id="_Toc46834343"></a>CAR: ISWC Add Request 

Agency or Regional Center would like to add a work to the ISWC Database:

From

To

Transaction

Description

Agency or Regional Center 

ISWC Database

CAR

Agency delivers a CAR transaction notifying the ISWC Database of a work to be added\.

ISWC Database

Agency or Regional Center 

ACK

ISWC Database acknowledges receipt of the work and indicates whether or not it has been added\.

ISWC Database

Agency or Regional Center 

CSN

If the work is added to the ISWC Database, notification transactions are delivered to agencies other than the submitting agency listed against that ISWC \(if any\)\. 

#### <a id="_Toc196729660"></a><a id="_Toc46834344"></a>CUR: ISWC Update Request 

Agency or Regional Center would like to update a work that already exists in the ISWC Database:

__From__

__To__

__Transaction__

__Description__

Agency or Regional Center

ISWC Database

CUR

Agency delivers a CUR transaction notifying the ISWC Database of a work to be updated\.

ISWC Database

Agency or Regional Center

ACK

ISWC Database acknowledges receipt of the work and indicates whether it has been updated\.

ISWC Database

Agency or Regional Center

CSN

If the work is updated in the ISWC Database, notification transactions are delivered to agencies other than the submitting agency listed against that ISWC \(if any\)\. This will include workflow tasks if any were generated\.

#### <a id="_Toc196729661"></a><a id="_Toc46834345"></a>CDR: ISWC Delete Request 

Agency or Regional Center would like to delete a work that already exists in the ISWC Database:

__From__

__To__

__Transaction__

__Description__

Agency or Regional Center

ISWC Database

CDR

Agency delivers a CDR transaction notifying the ISWC Database of a work to be logically deleted\.

ISWC Database

Agency or Regional Center 

ACK

ISWC Database acknowledges receipt of the work and indicates whether or not it has been logically deleted\.

ISWC Database

Agency or Regional Center

CSN

If the work is logically deleted in the ISWC Database, notification transactions are delivered to agencies other than the submitting agency listed on the ISWC \(if any\)

#### <a id="_Toc196729662"></a><a id="_Toc46834346"></a>CMQ: CSI Meta Data Queries 

Agency or Regional Center would like to query work meta data info by the preferred ISWC number that already exists in the ISWC Database:

__From__

__To__

__Transaction__

__Description__

Agency or Regional Center 

ISWC Database

CMQ

Agency delivers a CMQ transaction to query against the ISWC Database\.

ISWC Database

Agency or Regional Center

ACK

ISWC Database acknowledges receipt of the query request and responds with the work meta data information 

#### <a id="_Toc196729663"></a><a id="_Toc46834347"></a>CIQ: CSI ISWC Queries 

Agency or Regional Center would like to query the preferred ISWC information by the work’s identifier or archived ISWC that already exists in the ISWC Database: 

__From__

__To__

__Transaction__

__Description__

Agency or Regional Center

ISWC Database

CIQ

Agency delivers a CIQ transaction to query against the ISWC Database

ISWC Database

Agency or Regional Center

ACK

ISWC Database acknowledges receipt of the query request and responds with the work ISWC information\.

#### <a id="_Toc46834348"></a>MER: ISWC Merge 

Agency or Regional Center would like to merge two or more existing preferred ISWCs in the ISWC Database: 

__From__

__To__

__Transaction__

__Description__

Agency or Regional Center

ISWC Database

MER

Agency delivers a MER transaction to merge existing preferred ISWCs in the ISWC Database

ISWC Database

Agency or Regional Center

ACK

ISWC Database acknowledges receipt of the merge transaction and indicates whether or not it has been carried out successfully\.

ISWC Database

Agency or Regional Center

CSN

If the preferred ISWCs are merged, notification transactions are delivered to agencies other than the submitting agency listed on the ISWC \(if any\)

### <a id="_Toc46834349"></a>Transaction / Record Type Mapping

The following table lists the transaction header and detail record types that are expected for each transaction type:

#### <a id="_Toc46834350"></a>CAR Transaction

Record Type

Name

Req

Max Use

Comments

CAR 

ISWC Database Add Request

M

1

CTL

Work Title

O

M

CIP

Interested Parties Set

M

M

Must have at least one CIP record

DIS

Disambiguation ISWCs Set

C

M

These records should only be provided if the transaction header record \(e\.g\. CAR\) has its Disambiguation field set to T \(True\)\.

DER

Derived from works Set

C

M

These records should only be provided if the transaction header record \(e\.g\. CAR\) has a non\-blank “Derived Work Type” field value\.

PER

Performer Set

C

M

These records may be provided if the transaction header record \(e\.g\. CAR\) has its Disambiguation field set to T \(True\)\.

INS

Instrumentation Set 

C

M

These records may be provided if the transaction header record \(e\.g\. CAR\) has its Disambiguation field set to T \(True\)\.

#### <a id="_Toc46834351"></a>CUR Transaction

Record Type

Name

Req

Max Use

Comments

CUR

ISWC Database Update Request

M

1

CTL

Work Title

O

M

CIP

Interested Parties Set

M

M

Must have at least one CIP record

DIS

Disambiguation ISWCs Set

C

M

These records should only be provided if the transaction header record \(e\.g\. CAR\) has its Disambiguation field set to T \(True\)\.

DER

Derived from works Set

C

M

These records should only be provided if the transaction header record \(e\.g\. CAR\) has a non\-blank “Derived Work Type” field value\.

PER

Performer Set

C

M

These records may be provided if the transaction header record \(e\.g\. CAR\) has its Disambiguation field set to T \(True\)\.

INS

Instrumentation Set 

C

M

These records may be provided if the transaction header record \(e\.g\. CAR\) has its Disambiguation field set to T \(True\)\.

#### <a id="_Toc46834352"></a>CDR Transaction

Record Type

Name

Req

Max Use

Comments

CDR

ISWC Database Delete Request

M

1

CTL

Work Title

O

M

CIP

Interested Parties Set

O

M

#### <a id="_Toc46834353"></a>CMQ Transaction

Record Type

Name

Req

Max Use

Comments

CMQ

ISWC Meta Data Query

M

1

CTL

Work Title

O

M

These are not expected and will be ignored if provided\.

CIP

Interested Parties Set

O

M

These are not expected and will be ignored if provided\.

#### <a id="_Toc46834354"></a>CIQ Transaction

Record Type

Name

Req

Max Use

Comments

CIQ

ISWC Query

M

1

CTL

Work Title

O

M

CIP

Interested Parties Set

O

M

#### <a id="_Toc46834355"></a>MER Transaction

Record Type

Name

Req

Max Use

Comments

MER

Merge Transaction

M

1

MLI

Merge List Item 

M

M

#### <a id="_Toc46834356"></a>CSN Transaction

Record Type

Name

Req

Max Use

Comments

CSN

Notification

M

1

CWI

Work Information

M

M

CIP

Interested Parties Set

M

M

Must have at least one CIP record

CTL

Work Title

O

M

DER

Derived from works Set

C

M

INS

Instrumentation Set 

C

M

PER

Performer Set

O

M

Known Performer information

MER

Merge Info

O

M

Merge information

1. <a id="_Toc46834357"></a>New \(Modern\) EDI Format Record Types

The following section shows the expected format and validation rules for each record type:  

## <a id="_Toc46834358"></a>CAR Record Format

Field

Start

Size

Fmt

Req

Field Description

Field Rules Reference

*Record Type*

1

3

A

M

CAR \(ISWC Add Request\)\.  

1

*Transaction Sequence \#*

4

8

N

M

See record Prefixes section above\.

*Record Sequence \#*

12

8

N

M

See record Prefixes section above\.

*Work Title*

20

100

A

M

Original Work Title

6

*Agency Code*

120

3

A

M

Agency Code representing the society that submitted the file\.

Values for this field reside in the Agency Code Table\.

3

*Agency Work Code*

123

20

A

M

Agency Work Code

The combination of *Agency Code*, *Agency Work Code* and *Source DB Code* is the information that allows identifying a musical work in the database of the society that contributed it to the ISWC Database

4

*Source DB Code* 

143

3

L

M

Source Database indicates the Source Database that fetches the submitted work\. 

Values for this field reside in the Agency Code Table\.

5

*Preferred ISWC*

146

11

A

O

Preferred ISWC  

See rule PV/24\.

8,9,10, 11,17

*Archived ISWC \(Submitted ISWC\)*

157

11

A

O

The submitted ISWC, which after validation by the ISWC generating center has been determined to be a duplicate of another, ISWC previously assigned by another ISWC agency\. 

12,13,

14, 15

*Derived Work Type*

168

20

L

O

Derived Work Type\- if not provided then this isn't a derived work\.   Derived Work Types table below holds list of possible values\.  

18

*Disambiguation*

188

1

B

O

Indicates if this submission should be disambiguated from specific existing Preferred ISWCs\.  If not provided, then assumed to be set to F \(False\)\.

*Disambiguation Reason*

189

20

L

C

Must be provided if the Disambiguation flag is set to T \(True\)\.  Disambiguation Reasons table below holds list of possible values\.

19

*bvltr*

209

1

L

C

May be provided if the Disambiguation flag is set to T \(True\)\.  BVLTR table below holds list of possible values\.

20

### <a id="_Toc46834359"></a>Transaction Level Validation

Ref

Description

Business Rules Document Ref

1

Only one CAR/CUR/CDR/CMQ/CIQ record is allowed per transaction\. \(TR\)

IV/01

4

For Add/Update/ ISWC Query transactions \(CAR, CUR, CIQ\) at least one CIP record must be included in each transaction\. \(TR\)

IV/02

9

For Add/Update/ ISWC Query transactions \(CAR, CUR, CIQ\) at least one Title must be included in each transaction\. \(TR\)

IV/05

10

If work contains more than one original title, transaction is rejected\. \(TR\)

IV/06

11

If work submission does not contain IP with creator role, transaction is rejected\. \(TR\)

IV/07

16

If The matching criteria not satisfied, transaction is rejected\. \(TR\)

MAT/01, MAT/02, MAT/03, MAT/04, MAT/42, MAT/40, MAT/39, MAT/09, MAT/10, MAT/41, MAT/43, MAT/30, MAT/31, MAT/39

### <a id="_Toc46834360"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

1

Record Type must be entered and must match a valid request type\. It must be one of the service types that CSI provides to societies\. They are CAR, CUR, CDR, CMQ, MER and CIQ\. \(ER\)

IV/09

3

Agency Code must be entered and must match an entry in the Agency Code Table\. It is a mandatory field for CAR, CUR and CDR\. \(TR\)

IV/10

4

Agency Work Code must be entered\. It is a mandatory field for CAR, CUR and CDR\. \(TR\)

IV/11

5

Source DB Code must be entered and must match an entry in the Agency Code Table\.  It is a mandatory field for CAR, CUR and CDR\. \(TR\)

IV/12

8,9,10

If provided the Preferred ISWC must have length of 11 characters, must begin with a “T” and must match the following pattern: \[T\]\[0\-9\]\{10\}  \(TR\)

IV/14

11

If provided, the Preferred ISWC must match valid “check digit” as per Annex B of ISO 15707 standard document\.

IV/15

12,13,14

If provided the Archived ISWC must have length of 11 characters, must begin with a “T” and must match the following pattern: \[T\]\[0\-9\]\{10\}  \(TR\)

IV/14

15

If provided, the Archived ISWC must match valid “check digit” as per Annex B of ISO 15707 standard document\. \(TR\)

IV/15

17

If provided, the Preferred ISWC must exist in the ISWC database as a Preferred ISWC \(TR\)

PV/24

18

If provided the Derived Work Type must match one of the values in the Derived Work Types table below\. \(TR\) 

19

If provided the Disambiguation Reason must match one of the values in the Disambiguation Reasons table below\.

20

If provided the BVLTR value must match one of the values in the BVLTR table below\.

## <a id="_Toc46834361"></a>CUR Record Format

Field

Start

Size

Fmt

Req

Field Description

Field Rules Reference

*Record Type*

1

3

A

M

CUR \(ISWC Update Request\)\.  

1

*Transaction Sequence \#*

4

8

N

M

See record Prefixes section above\. 

*Record Sequence \#*

12

8

N

M

See record Prefixes section above\.

*Work Title*

20

100

A

M

Original Work Title

6

*Agency Code*

120

3

A

M

Agency Code representing the society that submitted the file\.

Values for this field reside in the Agency Code Table\.

3

*Agency Work Code*

123

20

A

M

Agency Work Code

The combination of *Agency Code*, *Agency Work Code* and *Source DB Code* is the information that allows identifying a musical work in the database of the society that contributed it to the ISWC Database

4

*Source DB Code* 

143

3

L

M

Source Database indicates the Source Database that fetches the submitted work\. 

Values for this field reside in the Agency Code Table\.

5

*Preferred ISWC*

146

11

A

M

Preferred ISWC  

See rule PV/24\.

8,9,10, 11,17

*Archived ISWC \(Submitted ISWC\)*

157

11

A

O

The submitted ISWC, which after validation by the ISWC generating center has been determined to be a duplicate of another, ISWC previously assigned by another ISWC agency\. 

12,13,

14, 15

*Derived Work Type*

168

20

L

O

Derived Work Type\- if not provided then this isn't a derived work\.   Derived Work Types table below holds list of possible values\.  

18

*Disambiguation*

188

1

B

O

Indicates if this submission should be disambiguated from specific existing Preferred ISWCs\.  If not provided, then assumed to be set to F \(False\)\.

*Disambiguation Reason*

189

20

L

C

Must be provided if the Disambiguation flag is set to T \(True\)\.  Disambiguation Reasons table below holds list of possible values\.

19

*bvltr*

209

1

L

C

May be provided if the Disambiguation flag is set to T \(True\)\.  BVLTR table below holds list of possible values\.

20

### <a id="_Toc46834362"></a>Transaction Level Validation

Ref

Description

Business Rules Document Ref

1

Only one CAR/CUR/CDR/CMQ/CIQ record is allowed per transaction\. \(TR\)

IV/01

4

For Add/Update/ ISWC Query transactions \(CAR, CUR\) at least one CIP record must be included in each transaction\. \(TR\)

IV/02

9

For Add/Update/ ISWC Query transactions \(CAR, CUR\) at least one Title must be included in each transaction\. \(TR\)

IV/05

10

If work contains more than one original title, transaction is rejected\. \(TR\)

IV/06

11

If work submission does not contain IP with creator role, transaction is rejected\. \(TR\)

IV/07

16

If The matching criteria not satisfied, transaction is rejected\. \(TR\)

MAT/01, MAT/02, MAT/03, MAT/04, MAT/42, MAT/40, MAT/39, MAT/09, MAT/10, MAT/41, MAT/43, MAT/30, MAT/31, MAT/39

### <a id="_Toc46834363"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

1

Record Type must be entered and must match a valid request type\. It must be one of the service types that CSI provides to societies\. They are CAR, CUR, CDR, CMQ, MER and CIQ\. \(ER\)

IV/09

3

Agency Code must be entered and must match an entry in the Agency Code Table\. It is a mandatory field for CAR, CUR and CDR\. \(TR\)

IV/10

4

Agency Work Code must be entered\. It is a mandatory field for CAR, CUR and CDR\. \(TR\)

IV/11

5

Source DB Code must be entered and must match an entry in the Agency Code Table\.  It is a mandatory field for CAR, CUR and CDR\. \(TR\)

IV/12

7

Preferred ISWC is a mandatory field for CUR, CDR and CMQ\. \(TR\)

IV/13

8,9,10

If provided the Preferred ISWC must have length of 11 characters, must begin with a “T” and must match the following pattern: \[T\]\[0\-9\]\{10\}  \(TR\)

IV/14

11

If provided, the Preferred ISWC must match valid “check digit” as per Annex B of ISO 15707 standard document\.

IV/15

12,13,14

If provided the Archived ISWC must have length of 11 characters, must begin with a “T” and must match the following pattern: \[T\]\[0\-9\]\{10\}  \(TR\)

IV/14

15

If provided, the Archived ISWC must match valid “check digit” as per Annex B of ISO 15707 standard document\. \(TR\)

IV/15

17

If provided, the Preferred ISWC must exist in the ISWC database as a Preferred ISWC \(TR\)

PV/24

18

If provided the Derived Work Type must match one of the values in the Derived Work Types table below\. \(TR\) 

19

If provided the Disambiguation Reason must match one of the values in the Disambiguation Reasons table below\.

20

If provided the BVLTR value must match one of the values in the BVLTR table below\.

## <a id="_Toc46834364"></a>CDR Record Format

Field

Start

Size

Fmt

Req

Field Description

Field Rules Reference

*Record Type*

1

3

A

M

CDR \(ISWC Delete Request\)\.  

1

*Transaction Sequence \#*

4

8

N

M

See record Prefixes section above\.

*Record Sequence \#*

12

8

N

M

See record Prefixes section above\.

*Work Title*

20

100

A

M

Original Work Title

6

*Agency Code*

120

3

A

M

Agency Code representing the society that submitted the file\.

Values for this field reside in the Agency Code Table\.

3

*Agency Work Code*

123

20

A

M

Agency Work Code

The combination of *Agency Code*, *Agency Work Code* and *Source DB Code* is the information that allows identifying a musical work in the database of the society that contributed it to the ISWC Database

4

*Source DB Code* 

143

3

L

M

Source Database indicates the Source Database that fetches the submitted work\. 

Values for this field reside in the Agency Code Table\.

5

*Preferred ISWC*

146

11

A

M

Preferred ISWC  

See rule PV/24\.

8,9,10, 11,17

*Archived ISWC \(Submitted ISWC\)*

157

11

A

O

Not required for a delete transaction\.

*Derived Work Type*

168

20

L

O

Not required for a delete transaction\.

*Disambiguation*

188

1

B

O

Not required for a delete transaction\.

*Disambiguation Reason*

189

20

L

C

Not required for a delete transaction\.

*bvltr*

209

1

L

C

Not required for a delete transaction\.

*Deletion reason*

210

25

A

M

Deletion reason for the transaction

### <a id="_Toc46834365"></a>Transaction Level Validation

Ref

Description

Business Rules Document Ref

1

Only one CAR/CUR/CDR/CMQ/CIQ record is allowed per transaction\. \(TR\)

IV/01

16

If The matching criteria not satisfied, transaction is rejected\. \(TR\)

MAT/01, MAT/02, MAT/03, MAT/04, MAT/42, MAT/40, MAT/39, MAT/09, MAT/10, MAT/41, MAT/43, MAT/30, MAT/31, MAT/39

### <a id="_Toc46834366"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

1

Record Type must be entered and must match a valid request type\. It must be one of the service types that CSI provides to societies\. They are CAR, CUR, CDR, CMQ, MER and CIQ\. \(ER\)

IV/09

3

Agency Code must be entered and must match an entry in the Agency Code Table\. It is a mandatory field for CAR, CUR and CDR\. \(TR\)

IV/10

4

Agency Work Code must be entered\. It is a mandatory field for CAR, CUR and CDR\. \(TR\)

IV/11

5

Source DB Code must be entered and must match an entry in the Agency Code Table\.  It is a mandatory field for CAR, CUR and CDR\. \(TR\)

IV/12

7

Preferred ISWC is a mandatory field for CUR, CDR and CMQ\. \(TR\)

IV/13

8,9,10

If provided the Preferred ISWC must have length of 11 characters, must begin with a “T” and must match the following pattern: \[T\]\[0\-9\]\{10\}  \(TR\)

IV/14

11

If provided, the Preferred ISWC must match valid “check digit” as per Annex B of ISO 15707 standard document\.

IV/15

17

If provided, the Preferred ISWC must exist in the ISWC database as a Preferred ISWC \(TR\)

PV/24

## <a id="_Toc46834367"></a>CMQ Record Format

Field

Start

Size

Fmt

Req

Field Description

Field Rules Reference

*Record Type*

1

3

A

M

CMQ \(ISWC Meta Data Queries\)\.  

1

*Transaction Sequence \#*

4

8

N

M

See record Prefixes section above\. 

*Record Sequence \#*

12

8

N

M

See record Prefixes section above\.

*Work Title*

20

100

A

O

Original Work Title

*Agency Code*

120

3

A

O

Agency Code representing the society that submitted the file\.

Values for this field reside in the Agency Code Table\.

3

*Agency Work Code*

123

20

A

O

Agency Work Code

The combination of *Agency Code*, *Agency Work Code* and *Source DB Code* is the information that allows identifying a musical work in the database of the society that contributed it to the ISWC Database

*Source DB Code* 

143

3

L

O

Source Database indicates the Source Database that fetches the submitted work\. 

Values for this field reside in the Agency Code Table\.

5

*Preferred ISWC*

146

11

A

M

Preferred ISWC  

See rule PV/24\.

7,8,9,10, 11,17

*Archived ISWC \(Submitted ISWC\)*

157

11

A

O

Not required for a meta data query transaction\.

*Derived Work Type*

168

20

L

O

Not required for a meta data query transaction\.

*Disambiguation*

188

1

B

O

Not required for a meta data query transaction\.

*Disambiguation Reason*

189

20

L

C

Not required for a meta data query transaction\.

*bvltr*

209

1

L

C

Not required for a meta data query transaction\.

### <a id="_Toc46834368"></a>Transaction Level Validation

Ref

Description

Business Rules Document Ref

1

Only one CAR/CUR/CDR/CMQ/CIQ record is allowed per transaction\. \(TR\)

IV/01

16

If the matching criteria not satisfied, transaction is rejected\. \(TR\)

MAT/01, MAT/02, MAT/03, MAT/04, MAT/42, MAT/40, MAT/39, MAT/09, MAT/10, MAT/41, MAT/43, MAT/30, MAT/31, MAT/39

### <a id="_Toc46834369"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

1

Record Type must be entered and must match a valid request type\. It must be one of the service types that CSI provides to societies\. They are CAR, CUR, CDR, CMQ, MER and CIQ\. \(ER\)

IV/09

3

If Agency Code is entered, it must match an entry in the Agency Code Table\. 

IV/10

5

If Source DB Code is entered, it must match an entry in the Agency Code Table\.  

IV/12

7

Preferred ISWC is a mandatory field for CUR, CDR and CMQ\. \(TR\)

IV/13

8,9,10

If provided the Preferred ISWC must have length of 11 characters, must begin with a “T” and must match the following pattern: \[T\]\[0\-9\]\{10\}  \(TR\)

IV/14

11

If provided, the Preferred ISWC must match valid “check digit” as per Annex B of ISO 15707 standard document\.

IV/15

17

If provided, the Preferred ISWC must exist in the ISWC database as a Preferred ISWC \(TR\)

PV/24

## <a id="_Toc46834370"></a>CIQ Record Format

Field

Start

Size

Fmt

Req

Field Description

Field Rules Reference

*Record Type*

1

3

A

M

CIQ \(ISWC Query\)\.  

1

*Transaction Sequence \#*

4

8

N

M

See record Prefixes section above\. 

*Record Sequence \#*

12

8

N

M

See record Prefixes section above\.

*Work Title*

20

100

A

M

Original Work Title

*Agency Code*

120

3

A

O

Agency Code representing the society that submitted the file\.

Values for this field reside in the Agency Code Table\.

*Agency Work Code*

123

20

A

O

Agency Work Code

The combination of *Agency Code*, *Agency Work Code* and *Source DB Code* is the information that allows identifying a musical work in the database of the society that contributed it to the ISWC Database

*Source DB Code* 

143

3

L

O

Source Database indicates the Source Database that fetches the submitted work\. 

Values for this field reside in the Agency Code Table\.

*Preferred ISWC*

146

11

A

O

Preferred ISWC  

See rule PV/24\.

8,9,10, 11,17

*Archived ISWC \(Submitted ISWC\)*

157

11

A

O

Not required for an ISWC query transaction\.

12,13,14,

15

*Derived Work Type*

168

20

L

O

Not required for an ISWC query transaction\.

*Disambiguation*

188

1

B

O

Not required for an ISWC query transaction\.

*Disambiguation Reason*

189

20

L

O

Not required for an ISWC query transaction\.

*bvltr*

209

1

L

O

Not required for an ISWC query transaction\.

### <a id="_Toc46834371"></a>Transaction Level Validation

Ref

Description

Business Rules Document Ref

1

Only one CAR/CUR/CDR/CMQ/CIQ record is allowed per transaction\. \(TR\)

IV/01

16

If the matching criteria not satisfied, transaction is rejected\. \(TR\)

MAT/01, MAT/02, MAT/03, MAT/04, MAT/42, MAT/40, MAT/39, MAT/09, MAT/10, MAT/41, MAT/43, MAT/30, MAT/31, MAT/39

### <a id="_Toc46834372"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

1

Record Type must be entered and must match a valid request type\. It must be one of the service types that CSI provides to societies\. They are CAR, CUR, CDR, CMQ, MER and CIQ\. \(ER\)

IV/09

8,9,10

If provided the Preferred ISWC must have length of 11 characters, must begin with a “T” and must match the following pattern: \[T\]\[0\-9\]\{10\}  \(TR\)

IV/14

11

If provided, the Preferred ISWC must match valid “check digit” as per Annex B of ISO 15707 standard document\.

IV/15

12,13,14

If provided the Archived ISWC must have length of 11 characters, must begin with a “T” and must match the following pattern: \[T\]\[0\-9\]\{10\}  \(TR\)

IV/14

15

If provided, the Archived ISWC must match valid “check digit” as per Annex B of ISO 15707 standard document\. \(TR\)

IV/15

17

If provided, the Preferred ISWC must exist in the ISWC database as a Preferred ISWC \(TR\)

PV/24

## <a id="_Toc46834373"></a>MER Record Format

Field

Start

Size

Fmt

Req

Field Description

Field Rules Reference

*Record Type*

1

3

A

M

MER \(Merge ISWCs Request\)\.  

1

*Transaction Sequence \#*

4

8

N

M

See record Prefixes section above\. 

*Record Sequence \#*

12

8

N

M

See record Prefixes section above\.

*Work Title*

20

100

A

O

Original Work Title\.  Not required for merge transaction

*Agency Code*

120

3

A

M

Agency Code representing the society that submitted the file\.

Values for this field reside in the Agency Code Table\.

3

*Agency Work Code*

123

20

A

M

Agency Work Code

The combination of *Agency Code*, *Agency Work Code* and *Source DB Code* is the information that allows identifying a musical work in the database of the society that contributed it to the ISWC Database

4

*Source DB Code* 

143

3

L

M

Source Database indicates the Source Database that fetches the submitted work\. 

Values for this field reside in the Agency Code Table\.

5

*Preferred ISWC*

146

11

A

M

Preferred ISWC that identifies the Preferred ISWC to be merged into\. 

8,9,10, 11,17

*Archived ISWC \(Submitted ISWC\)*

157

11

A

O

Not required for a merge transaction\. 

*Derived Work Type*

168

20

L

O

Not required for a merge transaction\.  

*Disambiguation*

188

1

B

O

Not required for a merge transaction\.  

*Disambiguation Reason*

189

20

L

C

Not required for a merge transaction\.  

*bvltr*

209

1

L

C

Not required for a merge transaction\.  

### <a id="_Toc46834374"></a>Transaction Level Validation

Ref

Description

Business Rules Document Ref

1

Only one MER record is allowed per transaction\. \(TR\)

IV/01

16

If The matching criteria not satisfied, transaction is rejected\. \(TR\)

MAT/01, MAT/02, MAT/03, MAT/04, MAT/42, MAT/40, MAT/39, MAT/09, MAT/10, MAT/41, MAT/43, MAT/30, MAT/31, MAT/39

### <a id="_Toc46834375"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

1

Record Type must be entered and must match a valid request type\. It must be one of the service types that ISWC provides to societies\. They are CAR, CUR, CDR, CMQ, CIQ and MER\. \(ER\)

IV/09

3

Agency Code must be entered and must match an entry in the Agency Code Table\. It is a mandatory field for CAR, CUR and CDR\. \(TR\)

IV/10

4

Agency Work Code must be entered\. It is a mandatory field for CAR, CUR and CDR\. \(TR\)

IV/11

5

Source DB Code must be entered and must match an entry in the Agency Code Table\.  It is a mandatory field for CAR, CUR and CDR\. \(TR\)

IV/12

8,9,10

If provided the Preferred ISWC must have length of 11 characters, must begin with a “T” and must match the following pattern: \[T\]\[0\-9\]\{10\}  \(TR\)

IV/14

11

If provided, the Preferred ISWC must match valid “check digit” as per Annex B of ISO 15707 standard document\.

IV/15

## <a id="_Toc46834376"></a>ACK Record Format

The Acknowledgement record indicates whether the transaction was accepted and provides a status on the transaction that the acknowledgement has been generated for\.  The acknowledgment will include any error or warning messages associated with the original transaction\.

Field

Start

Size

Fmt

Req

Field Description

Field Rules Reference

*Record Type*

1

3

A

M

ACK \(Acknowledgement Record\)\.  

*Transaction Sequence \#*

4

8

N

M

See record Prefixes section above\. 

*Record Sequence \#*

12

8

N

M

See record Prefixes section above\.

*Creation Date*

20

8

D

M

The Creation Date of the original file that contained the transaction that this transaction applies to\.

1,2

*Creation Time*

28

6

T

M

The Creation Time of the original file that contained the transaction that this transaction applies to\.

1,2

*Original Group Number*

34

5

N

M

The Group Number within which the original transaction that this transaction applies to was located\.  Note that if the transaction is a result of a HDR or TRL record problem, set this field to zeroes\.

2

*Original Transaction Sequence \#*

39

8

N

M

The Transaction Sequence \# of the original transaction that this transaction applies to\. Note that if the ACK is a result of a HDR or TRL record problem, set this field to zeroes\.

2

*Original Transaction Type*

47

3

L

M

The Transaction Type of the original transaction that this transaction applies to\. Note that if the ACK is a result of a HDR or TRL record problem, set this field to HDR or TRL \(whichever is applicable\)\.

3

*Work Title*

50

100

A

O

Work Title

*Agency Code*

150

3

N

O

Agency Code representing the society that submitted the file\.

Values for this field reside in the Agency Code Table\.

*Agency Work Code*

153

20

A

O

Agency Work Code

The combination of *Agency Code*, *Agency Work Code* and *Source DB Code* is the information that allows identifying a musical work in the database of the society that contributed it to the ISWC Database

*Source DB Code* 

173

3

N

O

Source Database indicates the Source Database that fetches the submitted work\. 

Values for this field reside in the Agency Code Table\.

*Preferred ISWC*

176

11

A

O

Preferred ISWC Number 

7

*Processing Date*

187

8

D

M

The date this transaction or file was formally processed by the recipient\.

9

*Transaction Status*

195

2

L

M

The current status of this transaction\.  Values for this field reside in the Transaction Status Table\.

10

### <a id="_Toc46834377"></a>Transaction Level Validation

Ref

Description

Business Rules Document Ref

1

Only one ACK record is allowed per transaction\. \(TR\)

### <a id="_Toc46834378"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

1\.

The combination of Creation Date and Creation Time must match the same fields found on the HDR record of a file generated by the submitter\. \(TR\)

2\.

The combination of Original Group Number and Transaction Sequence \# must be valid within the file referred to by Creation Date and Creation Time\. \(TR\)

3\.

Original Transaction Type must match the transaction referred to by the combination of Creation Date, Creation Time, Original Group \#, and Original Transaction Sequence \#\. \(TR\)

7\.

If provided, preferred ISWC must be a valid ISWC Number\. \(TR\)

IV/14

9\.

Processing Date must be a valid date\. \(TR\)

10\.

Transaction Status must match an entry in the Transaction Status table\. \(TR\)

## <a id="_Toc46834379"></a>CSN Record Format

The Notification record is generated as a notification to agencies that are potentially impacted by the submission of a CAR/CUR/CDR/MER from a submitter agency\.

Field

Start

Size

Fmt

Req

Field Description

Field Rules Reference

*Record Type*

1

3

A

M

CSN \(Acknowledgement Record\)\.  

*Transaction Sequence \#*

4

8

N

M

See record Prefixes section above\. 

*Record Sequence \#*

12

8

N

M

See record Prefixes section above\.

*Creation Date*

20

8

D

M

The generation date of the file\.

1,2

*Creation Time*

28

6

T

M

The generation time of the file\.

1,2

*Original Group Number*

34

5

N

M

Set this field to zeroes\.

2

*Original Transaction Sequence \#*

39

8

N

M

Set this field to zeroes\.

2

*Original Transaction Type*

47

3

L

M

Set this field to zeroes\.

3

*Work Title*

50

100

A

M

Work Title

8

*Agency Code*

150

3

N

M

Agency Code representing the society that submitted the file\.

Values for this field reside in the Agency Code Table\.

4

*Agency Work Code*

153

20

A

M

Agency Work Code

The combination of *Agency Code*, *Agency Work Code* and *Source DB Code* is the information that allows identifying a musical work in the database of the society that contributed it to the ISWC Database

5

*Source DB Code* 

173

3

N

M

Source Database indicates the Source Database that fetches the submitted work\. 

Values for this field reside in the Agency Code Table\.

6

*Preferred ISWC*

176

11

A

M

Preferred ISWC Number 

7

*Processing Date*

187

8

D

M

The date this transaction or file was formally processed by the recipient\.

9

*Transaction Status*

195

2

L

M

The current status of this transaction\.  Values for this field reside in the Transaction Status Table\.

10

*Workflow Task Id*

197

10

N

O

If this is a CSN generated from a new Workflow Task, this field will be populated with the identifier for the Workflow Task

*Workflow Status*

207

1

N

O

If this is a CSN generated from a new Workflow Task, this field will be populated with the Workflow Task Status: 0 \(Outstanding\), 1 \(Approved\), 2 \(Rejected\), 3 \(Cancelled\)

12

### <a id="_Toc46834380"></a>Transaction Level Validation

Ref

Description

Business Rules Document Ref

1

Only one CSN record is allowed per transaction\. \(TR\)

### <a id="_Toc46834381"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

1\.

The combination of Creation Date and Creation Time must match the same fields found on the HDR record of a file generated by the submitter\. \(TR\)

2\.

The combination of Original Group Number and Transaction Sequence \# must be valid within the file referred to by Creation Date and Creation Time\. \(TR\)

3\.

Original Transaction Type must match the transaction referred to by the combination of Creation Date, Creation Time, Original Group \#, and Original Transaction Sequence \#\. \(TR\)

4\.

Agency Code must be entered and must match an entry in the Agency Code Table\. \(TR\) 

IV/10

5\.

Agency Work Code must be entered\. \(TR\) 

IV/11

6\.

Source DB Code must be entered and must match an entry in the Agency Code Table\. \(TR\) 

IV/12

7\.

Preferred ISWC must be a valid ISWC Number\. \(TR\)

IV/14

8\.

Work Title \(i\.e\. Original Title\) must be entered \(TR\)

9\.

Processing Date must be a valid date\. \(TR\)

10\.

Transaction Status must match an entry in the Transaction Status table\. \(TR\)

12\.

Workflow Status match an entry in the Workflow Task Status Table

## <a id="_Toc46834382"></a>WFT Record Format

The Workflow Task \(WFT\) record allows an Agency to update the workflow status of a Workflow Task\. Multiple WFT records are allowed per transaction since there may be multiple outstanding workflow tasks per Agency\.

Field

Start

Size

Fmt

Req

Field Description

Field Rules Reference

*Record Type*

1

3

A

M

WFT \(Workflow Task Record\)\.  

*Transaction Sequence \#*

4

8

N

M

See record Prefixes section above\. 

*Record Sequence \#*

12

8

N

M

See record Prefixes section above\.

*Workflow Task Id*

20

10

N

M

Identifier for the Workflow Task

*Workflow Type*

30

1

N

M

1 \(Update Approval\), 2 \(Merge Approval\)

1

*Status*

31

1

N

M

0 \(Outstanding\), 1 \(Approved\), 2 \(Rejected\), 3 \(Cancelled\)

2

### <a id="_Toc46834383"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

1

Workflow Type must be entered and must match an entry in the Workflow Type Table

2

Status must be entered and must match an entry in the Workflow Task Status Table

## <a id="_Toc46834384"></a>CTL Record Format

The Work Title record describes the title information of a specific work submission\. Each work can have an original title and many alternate titles\.  Multiples CTL records are allowed in one transaction as there may be more than one title fulfilling each specific work\.  Note: The original title must be supplied in the transaction header record\. 

Field

Start

Size

Fmt

Req

Field Description

Field Rules

Record Type

1

3

A

M

CTL \(Work Title\)\. 

Transaction Sequence \#

4

8

N

M

See record Prefixes section above\. 

Record Sequence \#

12

8

N

M

See record Prefixes section above\.

 Work Title

20

100

A

M

Work Title

1,2

Title Type

120

2

L

M

The work title type of the work\.

Values for this field reside in the Title Type Table\.

1,2

### <a id="_Toc46834385"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

1

For Add/Update/ ISWC Query transactions \(CAR, CUR, CIQ\) at least one Original Title must be included in each transaction\.

IV/05

2

If work submission contains more than one original title, transaction is rejected\. \(TR\)

IV/06

3

Title Code must be in the list of valid CISAC Title Codes

IV/31

## <a id="_Toc46834386"></a>NAT Record Format

A new record type will be added called Non\-Roman Alphabet Title\. The record identifies titles in other alphabets for this work\. The language code is used to identify the alphabet\. This record can be used to describe alternate titles\. Multiple NAT records are allowed in one transaction as there may be more than one alternate title for each specific work\. The Character Set field in the HDR record is used to signify if there are any NAT records in the file\. Note: The original title must be supplied in the transaction header record\. 

Field

Start

Size

Fmt

Req

Field Description

Field Rules

Record Type

1

3

A

M

NAT \(Non\-Roman Alphabet Title\)\. 

Transaction Sequence \#

4

8

N

M

See record Prefixes section above\. 

Record Sequence \#

12

8

N

M

See record Prefixes section above\.

Title Type

20

2

L

M

The work title type of the work\.

Values for this field reside in the Title Type Table\.

1, 2, 3

Language Code

22

2

L

O

The language code for the work title \(ISO 639\-1\)\.

 Work Title

24

100

A

M

Work Title

1, 2

### <a id="_Toc46834387"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

1

For Add/Update/ ISWC Query transactions \(CAR, CUR, CIQ\) at least one Original Title must be included in each transaction\.

IV/05

2

If work submission contains more than one original title, transaction is rejected\. \(TR\)

IV/06

3

Title Code must be in the list of valid CISAC Title Codes

IV/31

## <a id="_Toc46834388"></a>CIP Record Format

The Interested Parties \(CIP\) record describes the Interested Parties information for a specific work submission and an interested party is defined as a person or a body who has contributed in some way to a creation or acquired rights in it\.  I\.E\. The creators and __original__ publishers\. 

Multiple CIP records are allowed in one transaction as there may be more than one interested party \(IP\) fulfilling each specific work\.

Field

Start

Size

Fmt

Req

Field Description

Field Rules

Record Type

1

3

A

M

CIP \(Interested Parties\)  

It indicates the information regarding Interested Parties

 

Transaction Sequence \#

4

8

N

M

See record Prefixes section above\. 

Record Sequence \#

12

8

N

M

See record Prefixes section above\.

IP Name Number 

20

11

N

C

IP Name Number: Within the IP base record, identifying each of the names of that person \- one number per name\. 

 

1, 2, 3, 4

IP Base Number

31

13

A

O 

IP Base Number: Identifying the natural or legal person\.  The field is an optional field

IP Role

44

2

L

M

Musical works interested party roles

Values for this field reside in the <a id="_Toc379969919"></a><a id="_Toc388696333"></a><a id="_Toc500222892"></a><a id="_Toc149390406"></a>Writer Designation Table or <a id="_Toc379969911"></a><a id="_Toc388696325"></a><a id="_Toc500222882"></a><a id="_Toc149390391"></a>Publisher Type Table\.

6,7

### <a id="_Toc46834389"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

1

IP Name Number must be entered if there is no IP Base Number\. \(TR\)

2

IP Name Number must be entered for CAR, CUR and CIQ transaction \(TR\)

3

IP Name Number must be numeric value\. \(TR\)

4

IP Name Number must be status 1 or 2 and exist in the IPI database\. \(TR\)

IV/21

6

IP Role must be entered\. \(TR\)

IV/22

7

IP Role must be one of the codes in the table of Writer Designation Table or Publisher Type Table\. \(TR\)

IV/22

## <a id="_Toc46834390"></a>DIS Record Format

The disambiguation \(DIS\) record describes the list of works to disambiguate from a specific work submission\. Multiple DIS records are allowed in one transaction as there may be more than one disambiguated ISWC for each submission\.  These records should only be provided if the transaction header record \(e\.g\. CAR\) has its Disambiguation field set to T \(True\):

Field

Start

Size

Fmt

Req

Field Description

Field Rules

*Record Type*

1

3

A

M

DIS \(Disambiguated Works\)  

It indicates the ISWCs that the current work submission should be disambiguated from\. 

*Transaction Sequence \#*

4

8

N

M

See record Prefixes section above\. 

*Record Sequence \#*

12

8

N

M

See record Prefixes section above\.

*ISWC*

20

11

A

M

ISWC that should be disambiguated from the current transaction\.  Must be a Preferred ISWC in the ISWC database\.

See rule PV/24\.

1,2,3,4

### <a id="_Toc46834391"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

1

If provided the Preferred ISWC must have length of 11 characters, must begin with a “T” and must match the following pattern: \[T\]\[0\-9\]\{10\}  \(TR\)

IV/14

2

If provided, the Preferred ISWC must match valid “check digit” as per Annex B of ISO 15707 standard document\.

IV/15

3

If provided, the Preferred ISWC must exist in the ISWC database as a Preferred ISWC \(TR\)

PV/24

4

The Disambiguation ISWCs that are provided must exist as valid Preferred ISWCs in the ISWC database\.

PV/30

## <a id="_Toc46834392"></a>DER Record Format

The derived from \(DER\) record describes the list of works that the current work submission transaction are derived from\. Multiple DER records are allowed in one transaction as there may be more than one work that another work is derived from\. These records should only be provided if the transaction header record \(e\.g\. CAR\) has a non\-blank “Derived Work Type” field value:

Field

Start

Size

Fmt

Req

Field Description

Field Rules

*Record Type*

1

3

A

M

DER \(Derived from\)  

It indicates the works that the current work submission is derived from  

*Transaction Sequence \#*

4

8

N

M

See record Prefixes section above\. 

*Record Sequence \#*

12

8

N

M

See record Prefixes section above\.

*ISWC*

20

11

A

O

ISWC that the current work submission is derived from\.  If provided must be a Preferred ISWC in the ISWC database\.

See rule PV/24\.

1,2,3,4

*Title*

31

100

A

O

Title of the work that the current work submission is derived from\. 

4

### <a id="_Toc46834393"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

1

If provided the Preferred ISWC must have length of 11 characters, must begin with a “T” and must match the following pattern: \[T\]\[0\-9\]\{10\}  \(TR\)

IV/14

2

If provided, the Preferred ISWC must match valid “check digit” as per Annex B of ISO 15707 standard document\.

IV/15

3

If provided, the Preferred ISWC must exist in the ISWC database as a Preferred ISWC \(TR\)

PV/24

4

If an ISWC is not provided, then a title must be provided

IV/34, IV/36, IV/38

## <a id="_Toc46834394"></a>PER Record Format

The performer \(PER\) record describes the list of performers associated with the current work submission\. Multiple PER records are allowed in one transaction as there may be more than one performer of a work\. These records should only be provided if the transaction header record \(e\.g\. CAR\) has its Disambiguation field set to T \(True\):

Field

Start

Size

Fmt

Req

Field Description

Field Rules

*Record Type*

1

3

A

M

PER \(performer\)  

It indicates the performer associated with the current work submission\.  

*Transaction Sequence \#*

4

8

N

M

See record Prefixes section above\. 

*Record Sequence \#*

12

8

N

M

See record Prefixes section above\.

*First Name*

20

50

A

O

First name of performer

1

*Last Name*

70

50

A

M

Last name of performer or name of band

1

### <a id="_Toc46834395"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

1

Performer information must contain a Second Name or a Second Name and a First Name

IV/40

## <a id="_Toc46834396"></a>INS Record Format

The instrumentation \(INS\) record describes the list if instrumentation associated with the current work submission\. Multiple INS records are allowed in one transaction as there may be more than one instrument used in a work\. These records should only be provided if the transaction header record \(e\.g\. CAR\) has its Disambiguation field set to T \(True\):

Field

Start

Size

Fmt

Req

Field Description

Field Rules

*Record Type*

1

3

A

M

INS \(instrumentation\)  

It indicates the instrumentation used on the work\. 

*Transaction Sequence \#*

4

8

N

M

See record Prefixes section above\. 

*Record Sequence \#*

12

8

N

M

See record Prefixes section above\.

*Code *

20

23

L

M

Standard Instrument code

1

### <a id="_Toc46834397"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

1

Must be a valid instrumentation code as per CISAC SR12\-0569R3

IV/40

## <a id="_Toc46834398"></a>MLI Record Format

The merge list \(MLI\) record describes the list of preferred ISWCs to merge with the target work identified in the MER transaction header record\. 

Field

Start

Size

Fmt

Req

Field Description

Field Rules

*Record Type*

1

3

A

M

MLI \(Merge List\)  

*Transaction Sequence \#*

4

8

N

M

See record Prefixes section above\. 

*Record Sequence \#*

12

8

N

M

See record Prefixes section above\.

*ISWC*

20

11

A

M

ISWC that should be merged with the Preferred ISWC identified in the transaction header\.  Must be a Preferred ISWC in the ISWC database\.

See rule PV/24\.

1,2,3

### <a id="_Toc46834399"></a>Field Level Validation

Ref

Description

Business Rules Document Ref

1

If provided the Preferred ISWC must have length of 11 characters, must begin with a “T” and must match the following pattern: \[T\]\[0\-9\]\{10\}  \(TR\)

IV/14

2

If provided, the Preferred ISWC must match valid “check digit” as per Annex B of ISO 15707 standard document\.

IV/15

3

If provided, the Preferred ISWC must exist in the ISWC database as a Preferred ISWC \(TR\)

PV/24

## <a id="_Toc46834400"></a>CWI Record Format

The Work Information \(CWI\) record describessociety work information associated with the transaction and is included in ACK and CSN transactions\.  

Field

Start

Size

Fmt

Req

Field Description

Field Rules

*Record Type*

1

3

A

M

CWI

*Transaction Sequence \#*

4

8

N

M

See record Prefixes section above\. 

*Record Sequence \#*

12

8

N

M

See record Prefixes section above\.

*Agency Code*

20

3

L

M

Agency Code representing the society that submitted the file\.

Values for this field reside in the Agency Code Table\.

*Society Work Code*

23

20

A

M

Agency Work Code

The combination of *Agency Code*, *Agency Work Code* and *Source DB Code* is the information that allows identifying a musical work in the database of the society that contributed it to the ISWC Database

*Source DB Code* 

43

3

L

M

*Source Database indicates the Source Database that fetches the submitted work\. *

*Values for this field reside in the Agency Code Table\.*

*Preferred ISWC*

46

11

A

O

*Preferred ISWC*

*Archived ISWC *

57

11

A

O

*Archived ISWC*

## <a id="_Toc46834401"></a>Lookup Tables

This section includes the new lookup tables introduced as part of this specification:

### <a id="_Toc46834402"></a>Derived Work Type

Code

Description

ModifiedVersion

Modified Version 

Excerpt

Excerpt

Composite

Composite

### <a id="_Toc46834403"></a>BVLTR

Code

Description

Background

Background

Logo

Logo

Theme

Theme

Visual

Visual

RolledUpCue

Rolled Up Cue

### <a id="_Toc46834404"></a>Disambiguation Reason

Code

Description

DIT

Different work with same title

DIA

Arrangement of work

DIE

Excerpt of another work

DIC

Cuesheet work

DIP

Different performer

DIV

Different version of work \(excerpt, modified work e\.g\. instrumental\) with different shares

### <a id="_Toc46834405"></a>Transaction Status

Code

Description

RJ

Rejected

FA

Fully Accepted

PA

Partially Accepted

__ __

<a id="_Toc355093999"></a><a id="_Toc485799689"></a>

# <a id="_Toc46834406"></a>Appendix A – Open and Closed Items

This appendix provides a tracking list of specific issues/queries raised by CISAC during the specification process and how they were incorporated or excluded from this specification:

__Open and Closed Items__

__ID__

__Description__

__Response__

__Status__

__Next Action By__

1

The ACK Transaction Status table values are not in the original EDI specification\. These values are to be added to a new Lookup table section 3\.16

Didier to provide the list of these:

ACK transaction codes are rejected \(RJ\), fully accepted \(FA\) or partially accepted \(PA\)

Closed

2

Didier to document which societies submit to CSI using EDI messages currently

[Didier Roy \(Guest\): Societies/nodes submitting with EDI are : WID \(53 societies… ](https://teams.microsoft.com/l/message/19:a78725c0b9b4485b86bc40ee740311b2@thread.skype/1566975524893?tenantId=4197e6e7-fe92-417f-8cd8-0997d263db36&groupId=eefca450-9bf9-49ac-b171-01fb643f2cb3&parentMessageId=1566975524893&teamName=CISAC%20SP%20ISWC&channelName=Design&createdTime=1566975524893)

posted in CISAC SP ISWC / Design at Aug 28, 2019 7:58 AM

 

Closed

3

SG to consider additional scope item of adding support for an additional modern format \(e\.g\. JSON\) as well as this extension to the positional file envisaged in this specification\.  This could form a template for modernization of other CISAC file formats\. 

The additional effort for this would be: 

10 days – design & documentation

25 days – build & QA 

Open

Steering Group 

4

SG to consider the lengthening of work titles from 60 to 256 characters in general in this specification\.   Sylvan M has proposed the lengthening of the title field in a future CWR file format also\. 

Proposed to go with 100 characters

Open

Steering Group

