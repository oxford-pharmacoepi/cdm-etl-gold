-- Lookuptype
																									-- already have (lookup_type_id, name) as a PK
create index IF NOT EXISTS idx_lookuptype_name on {sc}.lookuptype(name);