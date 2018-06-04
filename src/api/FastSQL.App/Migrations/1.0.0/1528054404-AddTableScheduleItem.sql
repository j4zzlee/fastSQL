
-- Name: example-ae089fb0-e38e-46ba-b6ea-4c95b2f8b4d3
-- Date: 6/4/2018 2:33:24 AM
-- Author: BaoChau
----------------------------
-- Migration up goes here.
----------------------------
CREATE TABLE core_schedule_options (
	[Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	[TargetEntityId] UNIQUEIDENTIFIER NOT NULL,
	[TargetEntityType] INT NOT NULL,
	[WorkflowId] NVARCHAR(255) NOT NULL,
	[Interval] INT NOT NULL,
	[Priority] INT NOT NULL,
	[Status] INT NOT NULL
);
--Down--
----------------------------
-- Migration down goes here.
----------------------------
DROP TABLE core_schedule_options;
