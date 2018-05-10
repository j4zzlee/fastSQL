
-- Name: example-cd26ae6d-1490-4219-9f66-414a6c96a0a4
-- Date: 5/3/2018 2:38:06 PM
-- Author: BaoChau
----------------------------
-- Migration up goes here.
----------------------------
CREATE TABLE core_index_pull_histories (
	[TargetEntityId] NVARCHAR(255) NOT NULL,
	[TargetEntityType] INT NOT NULL,
	[PullResultStr] NVARCHAR(MAX) NULL,
	[LastUpdated] INT

	CONSTRAINT PK_core_index_pull_histories PRIMARY KEY CLUSTERED ([TargetEntityId], [TargetEntityType])
);

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
--Down--
----------------------------
-- Migration down goes here.
----------------------------
DROP TABLE core_index_pull_histories;
DROP TABLE core_index_pull_dependencies;