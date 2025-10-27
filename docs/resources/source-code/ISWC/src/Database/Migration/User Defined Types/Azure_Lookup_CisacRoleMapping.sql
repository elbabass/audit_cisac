CREATE TYPE [Migration].[Azure_Lookup_CisacRoleMapping] AS TABLE (
    [CreatedDate]      DATETIME2 (0) NOT NULL,
    [LastModifiedDate] DATETIME2 (0) NOT NULL,
    [LastModifiedUser] VARCHAR (20)  NOT NULL,
    [RoleType]         VARCHAR (20)  NOT NULL,
    [CisacRoleType]    CHAR (2)      NOT NULL);

