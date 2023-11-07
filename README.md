# RecruitmentManagementAPI
API para la gestion de candidatos y sus respctivos ficheros. Cuenta con un rol de recruiter y uno para los candidatos. Que siguen las siuientes reglas:

## Features to implement:
  1. Recruiter CRUD
  2. Candidate CRUD
  3. Document CRUD
  4. As recruiter you can log-in
  5. As recruiter you can create/edit/delete a candidate
  6. As recruiter you can create/edit/delete a document (CSV)
  7. As candidate you can log-in
  8. As candidate you can upload/re-upload document (CSV)

NOTA: 
Se ha permitido que los Recruiters se puedan borrar y editar entre si.

## Futuras mejoras
Las contraseñas no se guardan encriptadas.
Unificar los usuarios en una sola entidad que se relacione consigo misma de manera que se puedan 
ahorrar un controlador, un repositorio y mucha repeticion de codigo. Se implementará en futuras actualizaciones.

## Requisitos Previos
Asegúrate de tener instalados los siguientes elementos antes de utilizar esta API:
- SDK de .NET en su ultima version
- SQLServer
- Postman
- Visual Studio recomendable 

## Instalación y Configuración

1. **Clona el Repositorio:**
   ```bash
   git clone [git@github.com:jeronimoGD/RecruitmentManagementAPI.git](https://github.com/jeronimoGD/RecruitmentManagementAPI.git)

2. Configura tu servidor de SQL Server en la cadena de conexion del fichero appsettings.js
   EJ: "Server=[Your SQL Server ]; Database=RecruitmentManagementDB; TrustServerCertificate=true; Trusted_Connection=true; MultipleActiveResultSets=true"
3. Puedes ejecutar la aplicacion desde la terminal con el comando (dotnet run en el directorio clonado) o desde visual studio cargando la solucion.
4. Utiliza Postman o Swager para probar cada uno de los endpoints.
5. Utiliza el fichero RecruiterManagementAPI_CollectionTests.postman_collection.json. Remmeber to change the IP of your api if needed and to use the
   token you get at login for authentification.
   
## Initial Task breakdown
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

## Endpoints

# Candidate
1. GET /api/CandidateController/GetCandidates
2. GET /api/CandidateController/GetCreatedCandidates
3. POST /api/CandidateController/CreateCandidate
4.POST /api/CandidateController/CandidateLogIn
5.DELETE /api/CandidateController/DeleteCandidate
6.PUT /api/CandidateController/UpdateCandidate

# Document
1. GET /api/DocumentControler/GetDocuments
2. POST /api/DocumentControler/CreateDocument
3. POST /api/DocumentControler/UploadDocument
4. DELETE /api/DocumentControler/DeleteDocument
5. PUT /api/DocumentControler/UpdateDocument

# Recruiter
1. GET /api/RecruiterControler/GetRecruiters
2. POST /api/RecruiterControler/RegisterRecruiter
3. POST /api/RecruiterControler/LogIn
4. DELETE /api/RecruiterControler/DeleteRecruiter
5. PUT /api/RecruiterControler/UpdateRecruiter
