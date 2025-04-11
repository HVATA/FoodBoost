# 📋 FoodBoost - Recipe Management Web App

A web application for managing recipes for multiple users. Originally made as a group project for a university course, now being used for personal learning and testing.

---

## 🌟 Features
- User authentication and authorization (Admin & Regular users)
- Recipe management (CRUD operations)
- Favorite recipe management
- Backend API built with ASP.NET Core Web API (FoodAPI)
- Frontend built with Blazor WebAssembly (FoodBlazor)

---

## 🚀 Getting Started

### Prerequisites
- .NET 6.0 SDK or higher
- Visual Studio or Visual Studio Code
- SQL Server (LocalDB or other)
- Entity Framework Core (EF Core)

---

### 🔧 Installation

1. **Clone the repository**
																																																														
	git clone https://github.com/HVATA/FoodBoost.git
    cd FoodBoost

2. **Set up the Database** 
	
	Make sure SQL Server is running.
	Update appsettings.json in the backend project to match your SQL Server connection string.
	Run migrations and update the database. Project will have default data.
    		
	   dotnet ef database update

3. **Run the API Backend (Swagger UI)**
		
	   cd FoodAPI
       dotnet run

	The Swagger UI will be available at: https://localhost:7048/swagger

4. **Run the Blazor Frontend**

	   cd FoodBlazor
       dotnet run

	The frontend will be available at: https://localhost:7042

---

### 🧩 Build and Test

1. **Building**
																																																														
	Open the solution in Visual Studio and build the solution (Ctrl+Shift+B) or use:
    
	   dotnet build FoodBoost.sln

2. **Testing** 
	
	The solution includes automated tests located in the following projects:
	
	  - FoodAPI.Tests (Backend API tests)

	  - FoodBlazor.Tests (Frontend Blazor tests)

	To run all tests from the command line:
    		
	   dotnet test

	You can also run the tests directly from Visual Studio using the Test Explorer.

---

### 🤝 Contributions

This project was originally created and developed by a group of four students as part of a full-stack web development course. 
Development has continued after the course, and the project remains active.

Below are the contributors who have contributed post-course:

HVATA Github: https://github.com/HVATA
valipakka Github: https://github.com/valipakka

---

### 📚 Resources and Inspiration

   - [ASP.NET Core](https://github.com/aspnet/Home)
   - [Visual Studio Code](https://github.com/Microsoft/vscode)
   - [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)

---

### 📜 License

   This project is for personal learning and testing purposes. Not licensed for external use or modification.

---
