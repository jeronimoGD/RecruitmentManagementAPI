# RecruitmentManagementAPI

# Requirements:
As developer you need to develop an API using .NET 7 to cover the following features.\
Features to implement:
  1. Recruiter CRUD
  2. Candidate CRUD
  3. Document CRUD
  4. As recruiter you can log-in
  5. As recruiter you can create/edit/delete a candidate
  6. As recruiter you can create/edit/delete a document (CSV)
  7. As candidate you can log-in
  8. As candidate you can upload/re-upload document (CSV)
\Recommended tools:
  1. Visual Studio
  2. Postman
  3. Git
\Guidelines:
  1. Use a public GitHub repository so we can review your code
  2. Use best practices
  3. Comment your decisions
  4. Clean code

# Initial Task breakdown
1. Create API .NET7 template project.
2. Create DB migrations following a code first aproach. (Include migration to create data base and items if not found on start up)
4. Implement Repository and UnitOfWork Patterns
3. Add Recruiter Candidate Document CRUDs
4. Add recruiers log-in authentification
5. Add candidate log-in authentification
6. Add candidate delete and creation only for loged recruiters
8. Add logic to alow cndidates upload CSV documents
8. Add logic to alow recruiters crate, edit or delete CSV documents
9. Create a document with the Postmant petitions to test the API
10. Document API
