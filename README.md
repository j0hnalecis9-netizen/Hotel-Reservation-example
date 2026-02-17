# Hotel Reservation System (ASP.NET Core MVC + SQL Server)

This project is a **beginner-friendly, full-stack hotel reservation system** you can open in **Visual Studio 2022**.

Project folder: `HotelReservationSystem/`

## ✅ Included Features

### 1) Customer Website
- Hotel classes: Economy, Standard, Deluxe, Suite, Presidential Suite.
- Class cards show image, description, price/night, services, and special offers.
- Reservation form includes:
  - Check-in / check-out
  - Number of guests
  - Add-ons (extra bed, breakfast, spa, tour package)
  - Payment method simulation (Card, GCash, PayPal, Cash on Arrival)
- Auto total price calculation on save.
- Receipt page with booking reference + summary.
- Download PDF receipt.
- Print receipt.
- Email sending is simulated by storing the target email in `Receipts.EmailSentTo`.

### 2) Manager/Admin Dashboard
- Secure manager role login.
- Dashboard metrics: total reservations, daily/monthly bookings, revenue.
- Reservation management:
  - View all
  - Update status
  - Delete reservation
  - Optional filtering scaffold in controller
- Room management:
  - Add/edit room details
  - Update price
  - Set availability
- Add-ons/offers management: add/edit/delete add-ons.
- Reports: export bookings to Excel.

### 3) Database Requirements
- Entity Framework Core with SQL Server.
- Tables/entities included: Users, Roles, HotelClasses, Rooms, Services, AddOns, Reservations, Payments, Receipts.
- Proper PK/FK relationships.
- Double booking prevention:
  - Application-level overlap check
  - SQL trigger sample in `SqlScripts/01-create-database.sql`

---

## Step-by-Step Guide (Visual Studio)

## STEP 1 — Install Required Tools
1. Install **Visual Studio 2022 Community**.
2. In Visual Studio Installer, include workloads:
   - **ASP.NET and web development**
   - **Data storage and processing** (for SQL tools)
3. Install **SQL Server** (Developer or Express) and **SQL Server Management Studio (SSMS)**.
4. Install **.NET 8 SDK**.

## STEP 2 — Open/Create the Project in Visual Studio
1. Open Visual Studio.
2. Choose **Open a local folder** and select this repo folder.
3. Open `HotelReservationSystem/HotelReservationSystem.csproj`.
4. Restore NuGet packages (`Build` → `Restore NuGet Packages`).

> If you want to build manually from scratch in Visual Studio:
> - File → New → Project → **ASP.NET Core Web App (Model-View-Controller)**
> - Name: `HotelReservationSystem`
> - Framework: .NET 8
> - Authentication Type: Individual Accounts
> - Then copy these code files into the generated project.

## STEP 3 — Configure SQL Server Connection
1. Open `HotelReservationSystem/appsettings.json`.
2. Set connection string in `ConnectionStrings:DefaultConnection`.

Example for LocalDB:
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HotelReservationDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

Example for SQL Server instance:
```json
"DefaultConnection": "Server=YOUR_SERVER_NAME;Database=HotelReservationDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

## STEP 4 — Create Database (2 options)

### Option A (Recommended): EF Core Migrations
1. Open **Tools → NuGet Package Manager → Package Manager Console**.
2. Run:
```powershell
Add-Migration InitialCreate
Update-Database
```
3. Run the app once; seed data will auto-create roles, manager account, classes, rooms, and add-ons.

### Option B: Run SQL Script Manually
1. Open SSMS.
2. Open and run: `HotelReservationSystem/SqlScripts/01-create-database.sql`.
3. This script creates all required tables + relationships + anti-double-booking trigger.

## STEP 5 — Understand Core Files
- `Program.cs`: service registration, middleware, migration/seed bootstrap.
- `Data/ApplicationDbContext.cs`: EF Core tables + FK + constraints.
- `Data/SeedData.cs`: starter data and manager account.
- `Models/DomainModels.cs`: domain entities and enums.
- `Controllers/*`: booking flow + admin operations.
- `Services/ReceiptService.cs`: PDF receipt generation.
- `Views/*`: Razor UI pages.

## STEP 6 — Run and Test
1. Press `F5` or click **IIS Express** / **https profile**.
2. Register a customer account and create reservation.
3. Test booking flow and PDF download.
4. Login as manager:
   - Email: `manager@hotel.com`
   - Password: `Manager123!`
5. Open `/Admin/Dashboard` and test:
   - status updates
   - room CRUD
   - add-on CRUD
   - Excel export

## STEP 7 — Input Validation and Error Handling
- Data annotations are included in models.
- Reservation date and availability checks are in controller.
- Conflict validation prevents overlapping room booking.
- Global production exception route enabled in Program.

## STEP 8 — Publish the Project
1. Right click project → **Publish**.
2. Target: Folder / IIS / Azure.
3. Choose `Release` and publish.
4. For IIS deployment:
   - Install .NET Hosting Bundle on server.
   - Set connection string in `appsettings.Production.json` or environment variables.
   - Run database migrations on target.

---

## Quick Test Checklist
- [ ] Customer can browse all hotel classes.
- [ ] Customer can reserve room with add-ons and payment simulation.
- [ ] Booking reference is created.
- [ ] Receipt can print and download as PDF.
- [ ] Manager can login and open dashboard.
- [ ] Manager can update reservation status.
- [ ] Manager can edit rooms.
- [ ] Manager can manage add-ons.
- [ ] Excel report downloads correctly.

---

## Notes for Students
- Start by running it with LocalDB (easiest).
- After it works, customize styles and add more reports.
- If migrations fail, delete migration files + DB, then run `Add-Migration` and `Update-Database` again.

