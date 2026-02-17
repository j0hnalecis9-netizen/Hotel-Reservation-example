# Hotel Reservation System (ASP.NET Core MVC)

A complete starter project for a **Hotel Reservation System** built with **C# + ASP.NET Core MVC + Entity Framework Core + SQL Server**.

## Features Included

### 1) Customer-Facing Website
- Browse hotel classes (Standard, Deluxe, Suite) with:
  - Price
  - Capacity
  - Amenities
- View current offers and available services on the home page.
- Reservation flow includes:
  - Check-in/check-out date selection
  - Add-ons (Breakfast, Spa, Airport Pickup)
  - Payment method selection (Credit Card, PayPal, Cash)
  - Send receipt by email and/or print receipt options
- Validation for required fields and date rules.

### 2) Manager System
- Manager login using ASP.NET Core Identity.
- Dashboard metrics:
  - Total reservations
  - Daily bookings
  - Monthly bookings
  - Revenue summary
- Reservation management:
  - Update status (Pending/Confirmed/Cancelled/Completed)
  - Delete reservation
- Export reservation list to Excel using **ClosedXML**.

### 3) Database Design
- Entity Framework Core with SQL Server.
- Core models:
  - `HotelClass`
  - `Room`
  - `Reservation`
  - `User` (customer profile)
  - `Payment`
  - `AddOn`
  - `ReservationAddOn` (many-to-many join)
- Relationships configured in `ApplicationDbContext`.

### 4) Optional Additions Implemented
- Basic CSS styling (`wwwroot/css/site.css`)
- Example email receipt service (`ConsoleEmailService`)
- Seed data for hotel classes, rooms, add-ons, and default manager account.

---

## From Codex Workspace to Visual Studio / VS Code (What to do now)

Use this section if Codex generated this project and you now want to run it on your machine.

### A) Copy/Clone the code to your local machine
Choose one method:

1. **Git method (recommended)**
   - Push this repo/branch to GitHub from Codex.
   - On your machine:
     ```bash
     git clone <your-repo-url>
     cd Hotel-Reservation-example/HotelReservationSystem
     ```

2. **ZIP method**
   - Download/export this repository as ZIP.
   - Extract it locally.
   - Open the `HotelReservationSystem` folder.

### B) Open in IDE

#### Visual Studio 2022
1. Open **Visual Studio 2022**.
2. Click **Open a local folder**.
3. Select `HotelReservationSystem`.
4. Wait for restore prompt and click **Restore**.

#### VS Code
1. Open terminal in the project folder.
2. Run:
   ```bash
   code .
   ```
3. Install **C# Dev Kit** when prompted.

### C) Install SDK and tools
1. Install **.NET 8 SDK**.
2. Install **SQL Server / LocalDB**.
3. Install EF CLI tool:
   ```bash
   dotnet tool install --global dotnet-ef
   ```

### D) Restore packages and build
```bash
dotnet restore
dotnet build
```

### E) Configure database connection
In `appsettings.json`, keep or adjust:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HotelReservationDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

If using SQL Server instance instead of LocalDB, replace with your server credentials.

### F) Create database schema
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### G) Run
```bash
dotnet run
```
Open the URL shown in terminal.

### H) Login with seeded manager account
- Email: `manager@hotel.com`
- Password: `Manager@123`

---

## Step-by-Step Setup in Visual Studio Code

## Prerequisites
1. Install **.NET 8 SDK**.
2. Install **SQL Server** (or SQL Express / LocalDB).
3. Install **VS Code** with the **C# Dev Kit** extension.
4. (Optional) Install **SQL Server extension** for VS Code.

## 1. Create Project (if starting from scratch)
```bash
dotnet new mvc -n HotelReservationSystem
cd HotelReservationSystem
```

If you already have this repository, just open it in VS Code:
```bash
code .
```

## 2. Install NuGet Packages
```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package ClosedXML
```

## 3. Create MVC Structure
Create folders/files (already included in this repository):
- `Models/`
- `ViewModels/`
- `Controllers/`
- `Data/`
- `Services/`
- `Views/`
- `wwwroot/css/`

## 4. Configure Database Context and Connection String
1. Update `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HotelReservationDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```
2. In `Program.cs`, ensure:
- `AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(...))`
- Identity is configured
- Authentication/authorization middleware is enabled.

## 5. Create Initial Migration and Update Database
```bash
dotnet tool install --global dotnet-ef

dotnet ef migrations add InitialCreate
dotnet ef database update
```

> On startup, `DbInitializer` seeds initial data and creates a manager user.

## 6. Run the Application Locally
```bash
dotnet run
```
Open browser at:
- `https://localhost:5001` or the URL shown in the terminal.

## 7. Test Customer Functionality
1. Open Home page.
2. Select hotel class and click **Reserve Now**.
3. Fill reservation form with:
   - guest details
   - dates
   - add-ons
   - payment method
4. Submit and verify receipt page.
5. Toggle print option to test print behavior.

## 8. Test Manager Functionality
1. Navigate to `/Manager/Login`.
2. Use seeded manager credentials:
   - Email: `manager@hotel.com`
   - Password: `Manager@123`
3. Verify dashboard metrics.
4. Open reservations list and test:
   - status update
   - delete reservation
   - export to Excel

## 9. GitHub Push and Sync Workflow
```bash
git init
git add .
git commit -m "Initial hotel reservation system"
git branch -M main
git remote add origin <your-github-repo-url>
git push -u origin main
```

For future updates:
```bash
git add .
git commit -m "Describe your change"
git pull --rebase
git push
```

---

## Notes
- Replace `ConsoleEmailService` with SMTP, SendGrid, or MailKit for real email delivery.
- Add payment gateway SDK integration for production-grade payments.
- For advanced reporting, add charts and filtering to Manager dashboard.
