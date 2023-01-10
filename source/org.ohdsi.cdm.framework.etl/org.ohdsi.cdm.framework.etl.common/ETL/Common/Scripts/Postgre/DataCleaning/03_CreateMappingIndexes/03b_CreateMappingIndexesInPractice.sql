-- Practice
-- alter table {sc}.practice add constraint pk_practice primary key (pracid, region);				-- already exists
create index IF NOT EXISTS idx_practice_region on {sc}.practice(region);