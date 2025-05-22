1. Prerequisites
Backend (.NET)
- .NET 8 SDK
- SQL Server (LocalDB or SQL Server Express)
- Visual Studio 2022+ or VS Code
Frontend (Angular)
- Node.js (v18+ recommended)
- Angular CLI
- Git
- 
2.  Folder Structure
  - /AllTheBeans/ - .Net Web API
  - /all-the-beans-client/ - Angular frontend app
    
3. Backend Setup (.NET API)
- Create a database named AllTheBeansDb and onfigure connection string in appsettings.json
  "ConnectionStrings": {
    "DefaultConnection": "Server={{placeholder}};Database=AllTheBeansDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
- Run the application and the API will be available at https://localhost:7127
- A test user login is available
  Username : testuser
  Password : password
  
4. Frontend Setup (Angular)
- Navigate to the angular project folder  - cd all-the-beans-client
- Install dependencies - npm install
- Update the base URL in src/environments/environment.ts (if needed)
- Run the application - ng serve and the app will be available at https://localhost:4200
- All API endpoints are protected, please login with test credentials to gain access.

   
