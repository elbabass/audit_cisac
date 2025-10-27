/****** Object:  Index [PK_Publisher]    Script Date: 02/07/2019 09:31:10 ******/
ALTER TABLE [ISWC].[Publisher] DROP CONSTRAINT [PK_Publisher] WITH ( ONLINE = OFF )
GO

SET ANSI_PADDING ON
GO

/****** Object:  Index [PK_Publisher]    Script Date: 02/07/2019 09:31:10 ******/
ALTER TABLE [ISWC].[Publisher] ADD  CONSTRAINT [PK_Publisher] PRIMARY KEY NONCLUSTERED 
(
	[WorkInfoID] ASC,
	[IPBaseNumber] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]
GO


CREATE CLUSTERED INDEX [ClusteredIndex_on_Publisher_IswcID] ON [ISWC].[Publisher]
(
    [IswcID]
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [part_scheme_PartitionByISWCID]([IswcID])