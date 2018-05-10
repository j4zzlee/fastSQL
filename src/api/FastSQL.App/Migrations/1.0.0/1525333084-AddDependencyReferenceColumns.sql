
-- Name: example-79d2f83b-852e-4aed-951c-f82c30dea3c5
-- Date: 5/3/2018 2:38:04 PM
-- Author: BaoChau
----------------------------
-- Migration up goes here.
----------------------------
ALTER TABLE [core_index_dependency] ADD [ForeignKeys] NVARCHAR(255);
ALTER TABLE [core_index_dependency] ADD [ReferenceKeys] NVARCHAR(255);
--Down--
----------------------------
-- Migration down goes here.
----------------------------
ALTER TABLE [core_index_dependency] DROP COLUMN [ForeignKeys];
ALTER TABLE [core_index_dependency] DROP COLUMN [ReferenceKeys];
