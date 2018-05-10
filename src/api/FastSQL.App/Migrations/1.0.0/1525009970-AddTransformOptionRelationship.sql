
-- Name: example-095e5b71-0dbd-4290-8bf9-42aa2e3cfbe0
-- Date: 4/29/2018 8:50:45 PM
-- Author: BaoChau
----------------------------
-- Migration up goes here.
----------------------------
ALTER TABLE core_index_column_transformation ADD [TransformerId] NVARCHAR(255) NOT NULL;
ALTER TABLE core_index_column_transformation ADD [TargetEntityId] UNIQUEIDENTIFIER NOT NULL;
ALTER TABLE core_index_column_transformation ADD [TargetEntityType] INT NOT NULL;
ALTER TABLE core_index_column_transformation DROP COLUMN [EntityId];
ALTER TABLE core_index_column_transformation DROP COLUMN [EntityType];
ALTER TABLE core_index_column_transformation DROP COLUMN [SourceTransformationId];
ALTER TABLE core_index_column_transformation DROP COLUMN [DestinationTransformationId];
ALTER TABLE core_index_column_transformation DROP COLUMN [SourceTransformationFormat];
ALTER TABLE core_index_column_transformation DROP COLUMN [DestinationTransformationFormat];
--Down--
----------------------------
-- Migration down goes here.
----------------------------
ALTER TABLE core_index_column_transformation DROP COLUMN [TransformerId];
ALTER TABLE core_index_column_transformation DROP COLUMN [TargetEntityId];
ALTER TABLE core_index_column_transformation DROP COLUMN [TargetEntityType];
ALTER TABLE core_index_column_transformation ADD [EntityId] UNIQUEIDENTIFIER NOT NULL;
ALTER TABLE core_index_column_transformation ADD [EntityType] INT NOT NULL;
ALTER TABLE core_index_column_transformation ADD [SourceTransformationId] NVARCHAR(255) NOT NULL;
ALTER TABLE core_index_column_transformation ADD [DestinationTransformationId] NVARCHAR(255) NOT NULL;
ALTER TABLE core_index_column_transformation ADD [SourceTransformationFormat] NVARCHAR(255) NULL;
ALTER TABLE core_index_column_transformation ADD [DestinationTransformationFormat] NVARCHAR(255) NULL;