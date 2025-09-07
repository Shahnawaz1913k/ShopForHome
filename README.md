ShopForHome E-Commerce Platform
ShopForHome is a full-stack web application for an online home décor store. It provides a complete e-commerce experience for both customers and administrators, built with a modern technology stack.

This project was developed based on the requirements outlined in the "ShopForHome Capstone Project" document.

Features Implemented
For Customers (Users)
Authentication: Secure user registration, login, and logout functionality.

Product Catalog: Users can browse the full list of products.

Advanced Filtering: Products can be filtered by category, price, and rating.

Shopping Cart: A fully reactive shopping cart where users can:

Add products.

Increase or decrease item quantity.

View a real-time summary including subtotal, delivery charges, and grand total.

Persistent State: The user's session and cart are saved, so they remain logged in and their cart is preserved after a page refresh.

Wishlist: Users can add products to a personal wishlist to save them for later.

Dynamic UI: The application features a modern, responsive UI with:

Slide-out panels for the cart and wishlist.

Real-time UI updates without page reloads.

Toast notifications for user actions.

For Site Administrators
Secure Admin Panel: A dedicated admin section protected by role-based authorization.

User Management: Admins have full CRUD (Create, Read, Update, Delete) capabilities for all user accounts.

Product Management: Admins have full CRUD capabilities for the product catalog, managed through an intuitive modal interface.

Category Management: Admins can create, view, and delete product categories.

Bulk Product Upload: Admins can upload a large number of products at once using a CSV file.

Stock Management: A dedicated view to monitor product inventory, with visual highlights for low-stock items.

Sales Reports: A page for generating sales reports (UI and placeholder API).

Discount Coupon Management: Admins can create and assign discount coupons to specific users.


Tech Stack

Backend
Framework: ASP.NET Core Web API

Database: SQL Server with Entity Framework Core

Authentication: JWT (JSON Web Tokens) for secure, stateless authentication

API Documentation: Swagger / OpenAPI for interactive testing and documentation

Libraries: BCrypt.Net (for password hashing), CsvHelper (for bulk upload)

Frontend
Framework: Angular (Standalone Components)

Language: TypeScript

State Management: Angular Signals and RxJS for a fully reactive UI

Styling: CSS3, with a focus on modern, responsive design

Getting Started
Follow these instructions to get the project up and running on your local machine.

Prerequisites
.NET 8 SDK (or newer)

Node.js and npm

Angular CLI

SQL Server (or LocalDB included with Visual Studio)

Backend Setup

Navigate to the backend folder:

cd ShopForHome

Restore dependencies:

dotnet restore

3. Update Connection String: Open appsettings.json and ensure the ConnectionStrings.DefaultConnection value points to your SQL Server instance.

Apply Database Migrations: This command will create the database and all the necessary tables.

dotnet ef database update

dotnet run

The server will typically start on http://localhost:5165 and https://localhost:5026. Check your terminal for the exact URLs.

Good evening! It's 9:21 PM in Noida.

Of course. Creating a README file is a crucial final step to document your project. A good README explains what the project does, what technologies it uses, and how to set it up and run it.

Here is a complete README file for your "ShopForHome" project. You can copy this content into a new file named README.md in the root directory of your source code repository.

ShopForHome E-Commerce Platform
ShopForHome is a full-stack web application for an online home décor store. It provides a complete e-commerce experience for both customers and administrators, built with a modern technology stack.

This project was developed based on the requirements outlined in the "ShopForHome Capstone Project" document.

Features Implemented
For Customers (Users)
Authentication: Secure user registration, login, and logout functionality.

Product Catalog: Users can browse the full list of products.

Advanced Filtering: Products can be filtered by category, price, and rating.

Shopping Cart: A fully reactive shopping cart where users can:

Add products.

Increase or decrease item quantity.

View a real-time summary including subtotal, delivery charges, and grand total.

Persistent State: The user's session and cart are saved, so they remain logged in and their cart is preserved after a page refresh.

Wishlist: Users can add products to a personal wishlist to save them for later.

Dynamic UI: The application features a modern, responsive UI with:

Slide-out panels for the cart and wishlist.

Real-time UI updates without page reloads.

Toast notifications for user actions.

Live/Automated Chat: Integrated chat widget for customer support.

For Site Administrators
Secure Admin Panel: A dedicated admin section protected by role-based authorization.

User Management: Admins have full CRUD (Create, Read, Update, Delete) capabilities for all user accounts.

Product Management: Admins have full CRUD capabilities for the product catalog, managed through an intuitive modal interface.

Category Management: Admins can create, view, and delete product categories.

Bulk Product Upload: Admins can upload a large number of products at once using a CSV file.

Stock Management: A dedicated view to monitor product inventory, with visual highlights for low-stock items.

Sales Reports: A page for generating sales reports (UI and placeholder API).

Discount Coupon Management: Admins can create and assign discount coupons to specific users.

Tech Stack
Backend
Framework: ASP.NET Core Web API

Database: SQL Server with Entity Framework Core

Authentication: JWT (JSON Web Tokens) for secure, stateless authentication

API Documentation: Swagger / OpenAPI for interactive testing and documentation

Libraries: BCrypt.Net (for password hashing), CsvHelper (for bulk upload)

Frontend
Framework: Angular (Standalone Components)

Language: TypeScript

State Management: Angular Signals and RxJS for a fully reactive UI

Styling: CSS3, with a focus on modern, responsive design

Getting Started
Follow these instructions to get the project up and running on your local machine.

Prerequisites
.NET 8 SDK (or newer)

Node.js and npm

Angular CLI

SQL Server (or LocalDB included with Visual Studio)

Backend Setup
Navigate to the backend folder:

Bash

cd ShopForHome.Api
Restore dependencies:

Bash

dotnet restore
Update Connection String: Open appsettings.json and ensure the ConnectionStrings.DefaultConnection value points to your SQL Server instance.

Apply Database Migrations: This command will create the database and all the necessary tables.

Bash

dotnet ef database update
Run the Backend Server:

Bash

dotnet run
The server will typically start on http://localhost:5165 and https://localhost:5026. Check your terminal for the exact URLs.

Frontend Setup
Navigate to the frontend folder:

cd ShopForHome-Frontend

Install dependencies:

npm install

3. Verify API URLs: Open the service files in src/app/services/ (e.g., auth.service.ts, cart.service.ts) and ensure the apiUrl properties match the URL your backend is running on.

4. Run the Frontend Server:
ng serve

5. Access the Application: Open your browser and navigate to http://localhost:4200.


API Documentation

The complete API is documented using Swagger. Once the backend server is running, you can access the interactive API documentation at:

http://<your-backend-url>/swagger (e.g., http://localhost:5165/swagger).
