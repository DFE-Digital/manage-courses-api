-- example data
-- you need a ucas_institution before you can run the importer

-- delete from mc_user;

-- the email address has to be valid in the DfE Sign-in test environment
insert into mc_user(id, email, first_name, last_name) values(101, 'Tim.abell+4@digital.education.gov.uk', 'Tim4', 'Abell');

insert into mc_organisation(org_id) values(201);
insert into mc_organisation_user(email, org_id)values('Tim.abell+4@digital.education.gov.uk',201);

-- needs to match the GTTR_INST.xls example rows
insert into ucas_institution(inst_code) values
('C58'),
('C59'),
('152');

insert into mc_organisation_institution(institution_code, org_id) values('C58', 201);
-- select email-- INSERT INTO mc_organisation_user (email, org_id) VALUES (@email, (select id from ));

/*
SELECT u.email, o.org_id, ui.inst_code
from mc_user u
left outer join mc_organisation_user ou on ou.email = u.email
left outer join mc_organisation o on o.org_id = ou.org_id
left outer join mc_organisation_institution oi on oi.org_id = o.org_id
left outer join ucas_institution ui on ui.inst_code = oi.institution_code
where u.id = 101;
*/
