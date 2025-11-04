/****** Object:  Index [PK_Creator]    Script Date: 02/07/2019 09:27:59 ******/
ALTER TABLE [ISWC].[Creator] DROP CONSTRAINT [PK_Creator] WITH ( ONLINE = OFF )
GO

SET ANSI_PADDING ON
GO

/****** Object:  Index [PK_Creator]    Script Date: 02/07/2019 09:27:59 ******/
ALTER TABLE [ISWC].[Creator] ADD  CONSTRAINT [PK_Creator] PRIMARY KEY NONCLUSTERED 
(
	[WorkInfoID] ASC,
	[IPBaseNumber] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]
GO


CREATE CLUSTERED INDEX [ClusteredIndex_on_Creator_IswcID] ON [ISWC].[Creator]
(
    [IswcID]
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [part_scheme_PartitionByISWCID]([IswcID])
