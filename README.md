# **Trading app for 3D models**

The project is a web application which allows users to trade 3D virtual models.


---
<br>

## 📋 Table of Contents

- [About the Project](#-about-the-project)
- [Technologies Used](#️-technologies-used)
- [Prerequisites](#-prerequisites)
- [Getting Started](#-getting-started)
- [Project Structure](#-project-structure)
- [Features](#-features)
- [User actions](#-user-actions)
- [Usage](#-usage)
- [Database Setup](#️-database-setup)
- [License](#-license)
- [Contact](#-contact)

---
<br>

## 📖 About the Project

This project is a simple web trading app built as a final project for the *ASP.NET Fundamentals* course. The app is focused on showing the main characteristics of an ASP.NET MVC layered architecture and database management. The main trading subject (3D models) is only an idea and the app will work with images (type *.jpg*) instead of actual 3D model files. The purpose for trading is that when a user buys a product he will be able to download the main trading subject on his PC while the product's creator balance will increase. 

---
<br>

## 🛠️ Technologies Used

| Technology            | Version  | Purpose                          |
|-----------------------|----------|----------------------------------|
| ASP.NET Core MVC      | 8.0      | Web framework                    |
| Entity Framework Core | 8.0      | ORM / Database access            |
| SQL Server            | -        | Database                         |
| Bootstrap             | 5.1      | Frontend styling                 |
| Razor Pages / Views   | -        | Server-side HTML rendering       |

---
<br>

## ✅ Prerequisites

Make sure you have the following tools installed before running the project:

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server)
- [Git](https://git-scm.com/)

---
<br>

## 🚀 Getting Started

Follow these steps in order to run the project locally.

### 0. Open the terminal and navigate to the folder where you want to clone the project

```bash
cd desired-path
```

### 1. Clone the repository

```bash
git clone https://github.com/GeorgRashkov/TradingApp.git
cd TradingApp/TradingApp
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Apply database migrations

```bash
dotnet ef database update
```

### 4. Run the application

```bash
dotnet run
```

Once the application starts, it will be available at a local URL address (e.g. `http://localhost:5074`). Check the terminal output for the exact address.

---
<br>

## 📁 Project Structure

```
TradingApp/
│
├─├─Data                        #Data layer
│  ├─TradingApp.Data                # DbContext, migrations, seeding data
│  ├─TradingApp.Data.Models         # DB Entity models
│
├─├─Services                    #Service layer
│  ├─TradingApp.Services.Core       # Business logic
│
├─├─Web                         #Presentation layer
│  ├─TradingApp                     # Main project folder
│   ├─Controllers/                   # MVC Controllers
│   ├─Views                          # Razor Views (.cshtml)
│   ├─Helpers                        # Helper classes
│   ├─wwwroot                        # Static files (image files of the products)
│   ├─appsettings.json               # App configuration
│   ├─Program.cs                     # App entry point and middleware setup
│  ├─TradingApp.ViewModels          # Models used by the views
│
├─├─TradingApp.GCommon           #Constants and enums used by all layers

```

---
<br>

## ✨ Features

- User registration and login (ASP.NET Identity)
- CRUD operations for the Product entity 
- Uploading and downloading files (.jpg files)
- Input validation (server-side & client-side)

---
<br>

## ⚡ User actions

* Create products
* Upload product image files
* Create sell orders
* Cancel sell orders
* Edit products
* Delete products
* Browse and purchase products
* Track history of completed orders
* Download purchased products

---
<br>

## 💻 Usage


1. The project usage is described in the [Usage.md](Usage.md) file
2. The web pages are described in the [Webpages.md](Webpages.md) file

---
<br>

## 🗄️ Database Setup

The project uses **Entity Framework Core** with a Code-First approach.

Connection string is configured in `appsettings.Development.json`:

```json
 "ConnectionStrings": {
   "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=aspnet-TradingApp;Trusted_Connection=True;MultipleActiveResultSets=true; Encrypt=false"
 }
```

To create the database:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---
<br>


## 📄 License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

---
<br>

## 📬 Contact

**Georg Rashkov** – [https://github.com/GeorgRashkov](https://github.com/GeorgRashkov)

Project Link: [https://github.com/GeorgRashkov/TradingApp](https://github.com/GeorgRashkov/TradingApp)

---
<br>

*Built as part of the **ASP.NET Fundamentals** course.*
