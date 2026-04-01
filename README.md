# Eventify-High-Performance-Event-Management-API
Eventify is a robust, scalable Backend API designed to handle real-time event discovery, user registrations, and ticket management. Built with ASP.NET Core and C#, the system is optimized for high-speed data retrieval and strict data integrity, ensuring a seamless experience for both event organizers and attendees.
Technical Stack
Language: C# 12 / .NET 8

Database: SQL Server

ORM: Dapper (Micro-ORM) – Chosen for maximum performance and fine-grained control over complex SQL queries.

Architecture: Clean Architecture (Separation of Concerns between Domain, Application, and Infrastructure layers).

Design Patterns: Repository Pattern, Unit of Work, and DTO (Data Transfer Objects).

Key Features:
Dynamic Event Discovery: Advanced filtering and searching for events by category, date, and location using optimized SQL indexing.

Secure Booking System: A transaction-safe registration flow that prevents "over-booking" by handling concurrent database requests.

Role-Based Access Control (RBAC): Distinct permissions for 'Admins' (event creation/management) and 'Attendees' (browsing/booking).

Data Integrity: Implementation of SQL Constraints and C# Validations to ensure business rules (e.g., event capacity, valid dates) are never violated.

"I built Eventify to master high-performance data access in C#. I chose Dapper over EF Core because I wanted total control over the execution plan and to ensure the system could handle high-concurrency booking scenarios without data corruption.