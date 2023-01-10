-- Lookup
																									-- already have (lookup_id, lookup_type_id, code) as a PK
create index IF NOT EXISTS idx_lookup_lookup_type_id on {sc}.lookup(lookup_type_id);
create index IF NOT EXISTS idx_lookup_code on {sc}.lookup(code);