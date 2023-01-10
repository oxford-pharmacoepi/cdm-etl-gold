--Staff
-- alter table {sc}.staff add constraint pk_staff primary key (staffid);							-- already exists
create index IF NOT EXISTS idx_staff_role on {sc}.staff(role);	