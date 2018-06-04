
-- Name: example-299566a6-7b90-465a-a1ab-de2b1ddd7ee8
-- Date: 6/4/2018 2:33:22 AM
-- Author: BaoChau
----------------------------
-- Migration up goes here.
----------------------------
CREATE TABLE core_queue_items (
	[Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	[TargetEntityId] UNIQUEIDENTIFIER NOT NULL,
	[TargetEntityType] INT NOT NULL,
	[TargetItemId] NVARCHAR(4000) NOT NULL,
	[Status] INT NOT NULL,
	[MessageId] UNIQUEIDENTIFIER,
	[CreatedAt] INT NOT NULL,
	[UpdatedAt] INT NOT NULL,
	[ExecuteAt] INT NOT NULL,
	[ExecutedAt] INT NOT NULL
);
--Down--
----------------------------
-- Migration down goes here.
----------------------------
DROP TABLE core_queue_items;
