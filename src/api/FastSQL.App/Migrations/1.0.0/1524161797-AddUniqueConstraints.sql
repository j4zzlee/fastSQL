
-- Name: AddUniqueConstraints
-- Date: 4/20/2018 1:16:37 AM
-- Author: BaoChau
----------------------------
-- Migration up goes here.
----------------------------
ALTER TABLE core_connections ADD CONSTRAINT UC_connections_name UNIQUE ([Name]);
ALTER TABLE core_entities ADD CONSTRAINT UC_entities_name UNIQUE ([Name]);
ALTER TABLE core_attributes ADD CONSTRAINT UC_attributes_name UNIQUE ([Name]);
--Down--
----------------------------
-- Migration down goes here.
----------------------------
ALTER TABLE core_connections DROP CONSTRAINT UC_connections_name;
ALTER TABLE core_entities DROP CONSTRAINT UC_entities_name;
ALTER TABLE core_attributes DROP CONSTRAINT UC_attributes_name;