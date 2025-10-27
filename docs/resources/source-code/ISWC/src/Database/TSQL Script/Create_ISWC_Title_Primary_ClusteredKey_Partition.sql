/****** Object:  Index [PK_Title]    Script Date: 02/07/2019 09:18:54 ******/
ALTER TABLE [ISWC].[Title] DROP CONSTRAINT [PK_Title] WITH ( ONLINE = OFF )
GO

/****** Object:  Index [PK_Title]    Script Date: 02/07/2019 09:18:54 ******/
ALTER TABLE [ISWC].[Title] ADD  CONSTRAINT [PK_Title] PRIMARY KEY NONCLUSTERED 
(
	[TitleID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]
GO


CREATE CLUSTERED INDEX [ClusteredIndex_on_Title_IswcID] ON [ISWC].[Title]
(
    [IswcID]
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [part_scheme_PartitionByISWCID]([IswcID])