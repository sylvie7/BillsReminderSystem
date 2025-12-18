# 🚀 Bill Reminder System  
A Smart ASP.NET Core Web Application for Managing Bills, Sending Email Notifications, and Generating PDF Spending Reports.

---

## 📌 Overview  
The **Bill Reminder System** is a full-featured ASP.NET Core MVC application created to help users track their bills, avoid late payments, and gain insight into their spending.  

Each user can:
- Register & log in securely  
- Add, edit, and delete bills  
- Receive email confirmation when adding bills  
- View a dashboard with reminders  
- Generate PDF reports of their spending  
- Switch between dark and light mode  
- Navigate using a clean, modern sidebar UI  

This system demonstrates strong application architecture, use of services, authentication, reporting, and version control — fulfilling the requirements of a final software engineering project.

---

## ✨ Features

### 🔐 User Authentication
- Register & login with ASP.NET Identity  
- Password validation  
- Each user can only see **their own bills**

### 🧾 Bill Management
- Title  
- Amount + Currency  
- Category  
- Due date  
- Status (Pending / Paid)  
- Validation for clean and safe input  
- Email confirmation sent automatically  

### 📊 Dashboard
Shows:
- Total bills  
- Overdue bills  
- Bills due soon  
- Paid bills  
- List of upcoming unpaid bills  

### 📑 Reports & PDF Export
- Select date range  
- Totals: **Total / Paid / Pending**  
- Category breakdown  
- **Download PDF** (QuestPDF)

### 🎨 User Interface
- Bootswatch Cosmo theme  
- Sidebar navigation  
- Dark / light mode toggle  
- Responsive design  

### 🐳 Docker Support
Includes a Dockerfile for container-based deployment.

---

## 🛠 Technologies Used

| Layer | Technology |
|------|------------|
| Frontend | Razor Views, Bootstrap 5, Bootswatch Cosmo, Custom CSS |
| Backend | ASP.NET Core MVC, C# |
| Authentication | ASP.NET Core Identity |
| Database | SQL Server + Entity Framework Core |
| Email Service | SMTP (Gmail) with EmailSender |
| Reporting | QuestPDF |
| Version Control | Git & GitHub |
| Deployment | Docker |


BillReminderSystem/
│
├── Controllers/
│ ├── BillsController.cs
│ ├── ReportsController.cs
│ ├── AccountController.cs
│ └── HomeController.cs
│
├── Models/
│ ├── Bill.cs
│ ├── ReportViewModel.cs
│ ├── CategorySummary.cs
│
├── Services/
│ └── EmailSender.cs
│
├── Views/
│ ├── Bills/
│ ├── Reports/
│ ├── Home/
│ └── Shared/
│ └── _Layout.cshtml
│
├── wwwroot/
│ ├── css/site.css
│ ├── js/site.js
│ └── register.html (custom registration UI)
│
├── appsettings.json
├── Program.cs
└── Dockerfile

---

## ⚙️ Installation & Setup

### 1️⃣ Clone the repository

```bash
git clone https://github.com/yourusername/BillReminderSystem.git
cd BillReminderSystem
2️⃣ Configure SQL Server

Modify appsettings.json:

"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=BillReminderSystemDb;Trusted_Connection=True;"
}

Run EF Core migrations:

Add-Migration InitialCreate
Update-Database

3️⃣ Configure Email (Gmail SMTP)

"Smtp": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "EnableSsl": true,
  "User": "yourgmail@gmail.com",
  "Password": "YOUR_APP_PASSWORD",
  "From": "Bill Reminder <yourgmail@gmail.com>"
}

⚠️ Important: Use a Gmail App Password, not your normal Gmail password.

*▶️ Running the Application

Build the project

Run in Visual Studio

Go to /register.html to register a user

Log in and explore the dashboard, bills, and reports*




---

## 📂 Project Structure
