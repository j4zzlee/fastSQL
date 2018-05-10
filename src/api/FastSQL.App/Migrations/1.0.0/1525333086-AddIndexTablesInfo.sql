
-- Name: example-cd26ae6d-1490-4219-9f66-414a6c96a0a4
-- Date: 5/3/2018 2:38:06 PM
-- Author: BaoChau
----------------------------
-- Migration up goes here.
----------------------------
ALTER TABLE [core_entities] ADD [SourceViewName] NVARCHAR(60);
ALTER TABLE [core_entities] ADD [OldValueTableName] NVARCHAR(60);
ALTER TABLE [core_entities] ADD [NewValueTableName] NVARCHAR(60);
ALTER TABLE [core_entities] ADD [ValueTableName] NVARCHAR(60);

ALTER TABLE [core_attributes] ADD [SourceViewName] NVARCHAR(60);
ALTER TABLE [core_attributes] ADD [OldValueTableName] NVARCHAR(60);
ALTER TABLE [core_attributes] ADD [NewValueTableName] NVARCHAR(60);
ALTER TABLE [core_attributes] ADD [ValueTableName] NVARCHAR(60);
--Down--
----------------------------
-- Migration down goes here.
----------------------------
ALTER TABLE [core_entities] DROP COLUMN [SourceViewName];
ALTER TABLE [core_entities] DROP COLUMN [OldValueTableName];
ALTER TABLE [core_entities] DROP COLUMN [NewValueTableName];
ALTER TABLE [core_entities] DROP COLUMN [ValueTableName];

ALTER TABLE [core_attributes] DROP COLUMN [SourceViewName];
ALTER TABLE [core_attributes] DROP COLUMN [OldValueTableName];
ALTER TABLE [core_attributes] DROP COLUMN [NewValueTableName];
ALTER TABLE [core_attributes] DROP COLUMN [ValueTableName];