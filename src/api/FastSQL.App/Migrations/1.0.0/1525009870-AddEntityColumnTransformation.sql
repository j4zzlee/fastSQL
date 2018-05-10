
-- Name: example-095e5b71-0dbd-4290-8bf9-42aa2e3cfbe0
-- Date: 4/29/2018 8:50:45 PM
-- Author: BaoChau
----------------------------
-- Migration up goes here.
----------------------------
CREATE TABLE core_index_column_transformation (
	[Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	[EntityId] UNIQUEIDENTIFIER NOT NULL,
	[EntityType] INT NOT NULL,
	[ColumnName] NVARCHAR(255) NOT NULL,
	[SourceTransformationId] NVARCHAR(255) NOT NULL,
	[DestinationTransformationId] NVARCHAR(255) NOT NULL,
	[SourceTransformationFormat] NVARCHAR(255) NULL,
	[DestinationTransformationFormat] NVARCHAR(255) NULL
);
--Down--
----------------------------
-- Migration down goes here.
----------------------------
DROP TABLE core_index_column_transformation;