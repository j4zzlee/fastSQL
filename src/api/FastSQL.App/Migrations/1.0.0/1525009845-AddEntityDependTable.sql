
-- Name: example-095e5b71-0dbd-4290-8bf9-42aa2e3cfbe0
-- Date: 4/29/2018 8:50:45 PM
-- Author: BaoChau
----------------------------
-- Migration up goes here.
----------------------------
CREATE TABLE core_index_dependency (
	[Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	[EntityId] UNIQUEIDENTIFIER NOT NULL,
	[EntityType] INT NOT NULL,
	[TargetEntityId] UNIQUEIDENTIFIER NOT NULL,
	[TargetEntityType] INT NOT NULL,
	[DependOnStep] INT NOT NULL,
	[StepToExecute] INT NOT NULL,
	[ExecuteImmediately] BIT NOT NULL DEFAULT 0
);
--Down--
----------------------------
-- Migration down goes here.
----------------------------
DROP TABLE core_index_dependency;