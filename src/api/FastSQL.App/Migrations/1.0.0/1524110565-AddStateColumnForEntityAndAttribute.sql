
-- Name: AddStateColumnForEntityAndAttribute
-- Date: 4/19/2018 11:02:45 AM
-- Author: BaoChau
----------------------------
-- Migration up goes here.
----------------------------
ALTER TABLE core_entities ADD State INTEGER;
ALTER TABLE core_attributes ADD State INTEGER;
--Down--
----------------------------
-- Migration down goes here.
----------------------------
ALTER TABLE core_entities DROP COLUMN State;
ALTER TABLE core_attributes DROP COLUMN State;