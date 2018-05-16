
-- Name: example-cd26ae6d-1490-4219-9f66-414a6c96a0a4
-- Date: 5/3/2018 2:38:06 PM
-- Author: BaoChau
----------------------------
-- Migration up goes here.
----------------------------
DROP TABLE core_index_pull_dependencies;

CREATE TABLE core_pull_dependencies (
	[Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	[TargetEntityId] NVARCHAR(255) NOT NULL,
	[TargetEntityType] INT NOT NULL,
	[DependsOnEntityId] NVARCHAR(255) NOT NULL,
	[DependsOnEntityType] INT NOT NULL,
	[DependsOnItemId] NVARCHAR(255) NOT NULL,
	[CreatedAt] INT NOT NULL,
	[IsProcessed] BIT NOT NULL DEFAULT 0
);
--Down--
----------------------------
-- Migration down goes here.
----------------------------
DROP TABLE core_pull_dependencies;
CREATE TABLE core_index_pull_dependencies (
	[TargetEntityId] NVARCHAR(255) NOT NULL,
	[TargetEntityType] INT NOT NULL,
	[DependsOnEntityId] NVARCHAR(255) NOT NULL,
	[DependsOnEntityType] INT NOT NULL,
	[DependsOnItemId] NVARCHAR(255) NOT NULL

	CONSTRAINT PK_core_index_pull_dependencies PRIMARY KEY CLUSTERED (
		[TargetEntityId],
		[TargetEntityType],
		[DependsOnEntityId],
		[DependsOnEntityType],
		[DependsOnItemId])
);