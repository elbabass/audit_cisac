ALTER TABLE [ISWC].[Creator] DROP CONSTRAINT [FK_Creator_WorkInfo_WorkInfoID]
GO
ALTER TABLE [ISWC].[DerivedFrom] DROP CONSTRAINT [FK_DerivedFrom_WorkInfo_WorkInfoID]
GO
ALTER TABLE [ISWC].[DisambiguationISWC] DROP CONSTRAINT [FK_DisambiguationISWC_WorkInfo_WorkInfoID]
GO
ALTER TABLE [ISWC].[Publisher] DROP CONSTRAINT [FK_Publisher_WorkInfo_WorkInfoID]
GO
ALTER TABLE [ISWC].[Title] DROP CONSTRAINT [FK_Title_WorkInfo_WorkInfoID]
GO
ALTER TABLE [ISWC].[WorkflowInstance] DROP CONSTRAINT [FK_WorkflowInstance_WorkInfo_WorkInfoID]
GO
ALTER TABLE [ISWC].[WorkInfoInstrumentation] DROP CONSTRAINT [FK_WorkInfoInstrumentation_WorkInfo_WorkInfoID]
GO
ALTER TABLE [ISWC].[WorkInfoPerformer] DROP CONSTRAINT [FK_WorkInfoPerformer_WorkInfo_WorkInfoID]
GO


/****** Object:  Index [PK_WorkInfo]    Script Date: 01/07/2019 17:22:53 ******/
ALTER TABLE [ISWC].[WorkInfo] ADD  CONSTRAINT [PK_WorkInfo] PRIMARY KEY NONCLUSTERED 
(
	[WorkInfoID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]
GO

/************* Create the Clustered Index on the Partition Scheme ****/
CREATE CLUSTERED INDEX [ClusteredIndex_on_WorkInfo_IswcID] ON [ISWC].[WorkInfo]
(
    [IswcID]
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [part_scheme_PartitionByISWCID]([IswcID])



ALTER TABLE [ISWC].[Creator]  WITH CHECK ADD  CONSTRAINT [FK_Creator_WorkInfo_WorkInfoID] FOREIGN KEY([WorkInfoID])
REFERENCES [ISWC].[WorkInfo] ([WorkInfoID])
GO
ALTER TABLE [ISWC].[Creator] CHECK CONSTRAINT [FK_Creator_WorkInfo_WorkInfoID]
GO
ALTER TABLE [ISWC].[DerivedFrom]  WITH NOCHECK ADD  CONSTRAINT [FK_DerivedFrom_WorkInfo_WorkInfoID] FOREIGN KEY([WorkInfoID])
REFERENCES [ISWC].[WorkInfo] ([WorkInfoID])
GO
ALTER TABLE [ISWC].[DerivedFrom] CHECK CONSTRAINT [FK_DerivedFrom_WorkInfo_WorkInfoID]
GO
ALTER TABLE [ISWC].[DisambiguationISWC]  WITH NOCHECK ADD  CONSTRAINT [FK_DisambiguationISWC_WorkInfo_WorkInfoID] FOREIGN KEY([WorkInfoID])
REFERENCES [ISWC].[WorkInfo] ([WorkInfoID])
GO
ALTER TABLE [ISWC].[DisambiguationISWC] CHECK CONSTRAINT [FK_DisambiguationISWC_WorkInfo_WorkInfoID]
GO
ALTER TABLE [ISWC].[Publisher]  WITH NOCHECK ADD  CONSTRAINT [FK_Publisher_WorkInfo_WorkInfoID] FOREIGN KEY([WorkInfoID])
REFERENCES [ISWC].[WorkInfo] ([WorkInfoID])
GO
ALTER TABLE [ISWC].[Publisher] CHECK CONSTRAINT [FK_Publisher_WorkInfo_WorkInfoID]
GO
ALTER TABLE [ISWC].[Title]  WITH NOCHECK ADD  CONSTRAINT [FK_Title_WorkInfo_WorkInfoID] FOREIGN KEY([WorkInfoID])
REFERENCES [ISWC].[WorkInfo] ([WorkInfoID])
GO
ALTER TABLE [ISWC].[Title] CHECK CONSTRAINT [FK_Title_WorkInfo_WorkInfoID]
GO
ALTER TABLE [ISWC].[WorkflowInstance]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowInstance_WorkInfo_WorkInfoID] FOREIGN KEY([WorkInfoID])
REFERENCES [ISWC].[WorkInfo] ([WorkInfoID])
GO
ALTER TABLE [ISWC].[WorkflowInstance] CHECK CONSTRAINT [FK_WorkflowInstance_WorkInfo_WorkInfoID]
GO
ALTER TABLE [ISWC].[WorkInfoInstrumentation]  WITH NOCHECK ADD  CONSTRAINT [FK_WorkInfoInstrumentation_WorkInfo_WorkInfoID] FOREIGN KEY([WorkInfoID])
REFERENCES [ISWC].[WorkInfo] ([WorkInfoID])
GO
ALTER TABLE [ISWC].[WorkInfoInstrumentation] CHECK CONSTRAINT [FK_WorkInfoInstrumentation_WorkInfo_WorkInfoID]
GO
ALTER TABLE [ISWC].[WorkInfoPerformer]  WITH NOCHECK ADD  CONSTRAINT [FK_WorkInfoPerformer_WorkInfo_WorkInfoID] FOREIGN KEY([WorkInfoID])
REFERENCES [ISWC].[WorkInfo] ([WorkInfoID])
GO
ALTER TABLE [ISWC].[WorkInfoPerformer] CHECK CONSTRAINT [FK_WorkInfoPerformer_WorkInfo_WorkInfoID]
GO


