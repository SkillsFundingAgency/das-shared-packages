CREATE TABLE [dbo].[ClientOutboxData]
(
    [MessageId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY NONCLUSTERED,
    [EndpointName] NVARCHAR(150) NOT NULL,
    [CreatedAt] DATETIME NOT NULL,
    [Dispatched] BIT NOT NULL DEFAULT(0),
    [DispatchedAt] DATETIME NULL,
    [PersistenceVersion] VARCHAR(23) NOT NULL,
    [Operations] NVARCHAR(MAX) NOT NULL
)
GO

CREATE INDEX [IX_CreatedAt_PersistenceVersion] ON [dbo].[ClientOutboxData] ([CreatedAt] ASC, [PersistenceVersion] ASC) WHERE [Dispatched] = 0
GO

CREATE INDEX [IX_DispatchedAt_PersistenceVersion] ON [dbo].[ClientOutboxData] ([DispatchedAt] ASC, [PersistenceVersion] ASC) WHERE [Dispatched] = 1
GO