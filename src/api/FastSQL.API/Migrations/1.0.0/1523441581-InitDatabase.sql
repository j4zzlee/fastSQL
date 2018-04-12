
-- Name: InitDatabase
-- Date: 4/11/2018 5:13:01 PM
-- Author: BaoChau
----------------------------
-- Migration up goes here.
----------------------------
CREATE TABLE core_options (
	[Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	[EntityId] UNIQUEIDENTIFIER NOT NULL,
	[EntityType] INT NOT NULL,
	[Key] NVARCHAR(255) NOT NULL,
	[Value] NVARCHAR(MAX)
);
CREATE TABLE core_option_groups (
	[Name] NVARCHAR(255) PRIMARY KEY,
	[DisplayName] NVARCHAR(255) NOT NULL
);
CREATE TABLE core_rel_option_option_group (
	[OptionId] UNIQUEIDENTIFIER NOT NULL,
	[GroupName] NVARCHAR(255) NOT NULL
)
CREATE TABLE core_connections (
	[Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	[Name] NVARCHAR(255) NOT NULL,
	[Description] NVARCHAR(MAX),
	[ProviderId] NVARCHAR(255)
);
CREATE TABLE core_entities (
	[Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	[Name] NVARCHAR(255) NOT NULL,
	[Description] NVARCHAR(MAX),
	[ProcessorId] NVARCHAR(255),
	[SourceConnectionId] UNIQUEIDENTIFIER,
	[DestinationConnectionId] UNIQUEIDENTIFIER
);
CREATE TABLE core_attributes (
	[Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	[Name] NVARCHAR(255) NOT NULL,
	[Description] NVARCHAR(MAX),
	[ProcessorId] NVARCHAR(255),
	[SourceConnectionId] UNIQUEIDENTIFIER,
	[DestinationConnectionId] UNIQUEIDENTIFIER,
	[EntityId] UNIQUEIDENTIFIER NOT NULL
);
--Down--
----------------------------
-- Migration down goes here.
----------------------------
DROP TABLE core_options;
DROP TABLE core_option_groups;
DROP TABLE core_connections;
DROP TABLE core_rel_option_option_group;
DROP TABLE core_entities;
DROP TABLE core_attributes;
