CREATE TABLE [dbo].[Type](
	[TypeID] [int] IDENTITY(1,1) NOT NULL,
	[TypeName] [nvarchar](255) NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_Type] PRIMARY KEY CLUSTERED 
(
	[TypeID] ASC
)
)
CREATE UNIQUE NONCLUSTERED INDEX [IX_TypeName_Version] ON [dbo].[Type] 
(
	[TypeName] ASC,
	[Version] ASC
)

CREATE TABLE [dbo].[Timestamp](
	[DatabaseId] [bigint] NOT NULL,
	[FactId] [bigint] NOT NULL,
	[FKPeerId] [int] NOT NULL,
	[PivotId] [bigint] NOT NULL,
 CONSTRAINT [PK__Timestamp] PRIMARY KEY CLUSTERED 
(
	[FKPeerId] ASC,
	[PivotId] ASC
)
)

CREATE TABLE [dbo].[Share](
	[FKPeerID] [int] NOT NULL,
	[FKFactID] [bigint] NOT NULL,
	[RemoteFactID] [bigint] NOT NULL,
 CONSTRAINT [PK__Share] PRIMARY KEY CLUSTERED 
(
	[FKPeerID] ASC,
	[FKFactID] ASC
)
)

CREATE TABLE [dbo].[Role](
	[RoleID] [int] IDENTITY(1,1) NOT NULL,
	[DeclaringTypeID] [int] NOT NULL,
	[RoleName] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)
)
CREATE UNIQUE NONCLUSTERED INDEX [IX_DeclaringTypeID_RoleName] ON [dbo].[Role] 
(
	[DeclaringTypeID] ASC,
	[RoleName] ASC
)

CREATE TABLE [dbo].[Protocol](
	[ProtocolID] [int] IDENTITY(1,1) NOT NULL,
	[ProtocolName] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_Protocol] PRIMARY KEY CLUSTERED 
(
	[ProtocolID] ASC
),
 CONSTRAINT [UQ__Protocol] UNIQUE NONCLUSTERED 
(
	[ProtocolName] ASC
)
)

CREATE TABLE [dbo].[Predecessor](
	[FKRoleID] [int] NOT NULL,
	[PredecessorID] [bigint] IDENTITY(1,1) NOT NULL,
	[FKFactID] [bigint] NOT NULL,
	[FKPredecessorFactID] [bigint] NOT NULL,
	[IsPivot] [bit] NOT NULL,
 CONSTRAINT [PK__Predecessor] PRIMARY KEY CLUSTERED 
(
	[PredecessorID] ASC
)
)
CREATE NONCLUSTERED INDEX [IX_PredecessorFact_Role] ON [dbo].[Predecessor] 
(
	[FKPredecessorFactID] ASC,
	[FKRoleID] ASC
)

CREATE TABLE [dbo].[Peer](
	[PeerID] [int] IDENTITY(1,1) NOT NULL,
	[FKProtocolID] [int] NOT NULL,
	[PeerName] [nvarchar](1024) NOT NULL,
 CONSTRAINT [PK_Peer] PRIMARY KEY CLUSTERED 
(
	[PeerID] ASC
)
)

CREATE TABLE [dbo].[NamedFact](
	[Name] [nvarchar](255) NOT NULL,
	[FKFactID] [bigint] NOT NULL,
 CONSTRAINT [PK_NamedFact] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)
)

CREATE TABLE [dbo].[Message](
	[PivotId] [bigint] NOT NULL,
	[FactId] [bigint] NOT NULL,
	[PeerId] [int] NOT NULL,
 CONSTRAINT [PK__Message] PRIMARY KEY CLUSTERED 
(
	[PivotId] ASC,
	[FactId] ASC
)
)

CREATE TABLE [dbo].[Fact](
	[FactID] [bigint] IDENTITY(1,1) NOT NULL,
	[Data] [varbinary](1024) NOT NULL,
	[Hashcode] [int] NOT NULL,
	[FKTypeID] [int] NOT NULL,
	[DateAdded] [datetime] NOT NULL,
 CONSTRAINT [PK_Fact] PRIMARY KEY CLUSTERED 
(
	[FactID] ASC
)
)
CREATE NONCLUSTERED INDEX [IX_Type_Hashcode] ON [dbo].[Fact] 
(
	[FKTypeID] ASC,
	[Hashcode] ASC
)
ALTER TABLE [dbo].[Fact] ADD  DEFAULT (getdate()) FOR [DateAdded]

CREATE TABLE [dbo].[Client](
	[ClientGUID] [uniqueidentifier] NOT NULL
)
