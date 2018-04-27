
-- Name: UpdateEntityAndAttributeTableProcessors
-- Date: 4/18/2018 2:28:58 PM
-- Author: BaoChau
----------------------------
-- Migration up goes here.
----------------------------
ALTER TABLE core_entities ADD SourceProcessorId NVARCHAR(255);
ALTER TABLE core_entities ADD DestinationProcessorId NVARCHAR(255);
ALTER TABLE core_entities DROP COLUMN ProcessorId;

ALTER TABLE core_attributes ADD SourceProcessorId NVARCHAR(255);
ALTER TABLE core_attributes ADD DestinationProcessorId NVARCHAR(255);
ALTER TABLE core_attributes DROP COLUMN ProcessorId;
--Down--
----------------------------
-- Migration down goes here.
----------------------------
ALTER TABLE core_entities DROP COLUMN SourceProcessorId;
ALTER TABLE core_entities DROP COLUMN DestinationProcessorId;
ALTER TABLE core_entities ADD ProcessorId NVARCHAR(255);

ALTER TABLE core_attributes DROP COLUMN SourceProcessorId;
ALTER TABLE core_attributes DROP COLUMN DestinationProcessorId;
ALTER TABLE core_attributes ADD ProcessorId NVARCHAR(255);