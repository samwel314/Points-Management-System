# 🎯 Points Management System (ASP.NET Core MVC)

**Points Management System** is a web application built with **ASP.NET Core MVC** for managing customer invoices and rewarding loyalty points in a real-world business scenario.

The project focuses mainly on **proper Authorization design**, preventing common security issues such as **IDOR (Insecure Direct Object Reference)**, and applying clean separation of responsibilities.

---

## 🧠 Project Idea

* A client submits a purchase invoice
* An employee or admin reviews the invoice
* If approved, reward points are added to the client account
* Each employee can access and manage invoices **only for their own store**

The system is designed to be **scalable** and to reflect real production-level ASP.NET Core MVC usage.

---

## 🛠 Tech Stack

* **ASP.NET Core MVC**
* **Entity Framework Core**
* **SQL Server**
* **ASP.NET Identity**
* **Claims-based & Resource-based Authorization**
* **Bootstrap (UI)**

---

## ✨ Features

* 👤 Authentication & Authorization (Admin / Employee / Client)
* 🧾 Clients can submit invoices
* ✅ Admin / Employee can review and approve invoices
* ⭐ Reward points are added after approval
* 🔐 IDOR protection using Resource-based Authorization
* 🏬 Multi-store support (employees access only their store data)

---

## 🔐 Authorization Design (Core Focus)

### 1️⃣ Roles (Coarse-Grained Authorization)

* **Admin**: Full system access
* **Employee**: Review invoices related to their store only
* **Client**: Submit invoices and view points

> Roles are used only for high-level access control.

---

### 2️⃣ Claims + Resource-Based Authorization

* Each employee has a **StoreId** stored as a claim
* When accessing an invoice:

  * The system verifies that the invoice belongs to the same store
  * Authorization is handled using `IAuthorizationService`

✔ Prevents unauthorized access
✔ Protects against IDOR attacks
✔ Keeps authorization logic outside controllers

---

### 3️⃣ Authorization Handlers & Service Lifetimes

* Authorization handlers that depend on the database are registered as **Scoped**
* Authorization handlers that do not depend on the database are registered as **Singleton**

> This demonstrates correct service lifetime selection for Authorization Handlers.

---

## ⚙️ Run Locally

### Prerequisites

* .NET SDK
* SQL Server

### Steps

```bash
git clone https://github.com/samwel314/Points-Management-System.git
cd Points-Management-System
dotnet restore
dotnet ef database update
dotnet run



> 📢 This project is used as a practical reference for explaining Authorization Design and IDOR Prevention in technical content on LinkedIn.
