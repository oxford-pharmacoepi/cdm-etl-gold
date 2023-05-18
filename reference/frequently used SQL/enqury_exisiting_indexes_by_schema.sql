select
    t.relname as table_name,
    i.relname as index_name,
    a.attname as column_name,
	tt.schemaname as schema_name
from
    pg_class t,
    pg_class i,
    pg_index ix,
    pg_attribute a,
	pg_tables tt
where
    t.oid = ix.indrelid
    and i.oid = ix.indexrelid
    and a.attrelid = t.oid
    and a.attnum = ANY(ix.indkey)
    and t.relkind = 'r'
	and tt.tablename = t.relname
    and tt.schemaname = 'source'
order by
    t.relname,
    i.relname;