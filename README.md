# OEM EV Warranty Management — Backend (ASP.NET)

## Overview
Backend service for OEM EV Warranty Management. Provides APIs for: warranty requests, campaign management, spare‑parts logistics, customer appointments, authentication & image/email integrations, etc.
Serves for Service Center / Manufacturer and connects to Frontend to provide API for web application.

## Links
- Frontend (client UI): https://github.com/nng265/OEMEVManagerment_FE
- Repository: https://github.com/hominhdung9105/OEMEVWarrantyManagement
- Scalar (after running): `/scalar`

## Features

### Service Center & Manufacturer
- Manage warranty requests
- Manage recall campaigns
- Manage spare‑parts logistics (request, approve, import/export)
- Vehicle & customer history
- Dashboard & statistics

### Customer (via Frontend)
- Book service / warranty appointments
- Email confirmations
- Track warranty request status

## Tech Stack
- ASP.NET Web API
- Entity Framework Core
- SQL Server
- JWT Authentication + Google OAuth
- ImageKit SDK
- SMTP Email (Gmail)

## Project Structure (as in repo)
```
OEMEVWarrantyManagement.API/       ← Web API entry
  Controllers/
  Policy/Role/
  OEMEVWarrantyManagement.API.csproj
  OEMEVWarrantyManagement.API.http
  Program.cs

OEMEVWarrantyManagement.Application/
  BackgroundJobs/
  BackgroundServices/
  Dtos/
  IRepository/
  IServices/
  Mapping/
  Services/
  OEMEVWarrantyManagement.Application.csproj

OEMEVWarrantyManagement.Domain/
  Entities/
  OEMEVWarrantyManagement.Domain.csproj

OEMEVWarrantyManagement.Infrastructure/
  BackgroundServices/
  Migrations/
  Persistence/
  Repositories/
  OEMEVWarrantyManagement.Infrastructure.csproj

OEMEVWarrantyManagement.Share/
  Configs/
  Constants/
  Enums/
  Exceptions/
  Middlewares/
  Models/
  Validators/
  OEMEVWarrantyManagement.Share.csproj

OEMEVWarrantyManagement.sln
```

## Setup & Run

### 1. Clone repository
```bash
git clone https://github.com/hominhdung9105/OEMEVWarrantyManagement.git
cd OEMEVWarrantyManagement
```

### 2. Restore dependencies
```bash
dotnet restore
```

### 3. Configure `appsettings.json`
Create `appsettings.json` at repository root with the following template and replace `xxx` with real values:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",

  "AppSettings": {
    "Token": "xxx",
    "Issuer": "xxx",
    "Audience": "xxx"
  },

  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=xxx;User Id=xxx;Password=xxx;TrustServerCertificate=True;"
  },

  "ImageKit": {
    "PublicKey": "xxx",
    "PrivateKey": "xxx",
    "UrlEndpoint": "xxx"
  },

  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "SenderEmail": "xxx",
    "SenderName": "xxx",
    "Password": "xxx",
    "EnableSsl": true,
    "UseDefaultCredentials": false
  },

  "EmailUrlSettings": {
    "AppointmentConfirmationUrl": "http://xxx/confirmappointment",
    "GeneralBookingUrl": "http://xxx/cusappointmentform"
  },

  "Authentication": {
    "Google": {
      "ClientId": "xxx",
      "ClientSecret": "xxx"
    }
  }
}
```

## Connecting with Frontend
- FE repo: https://github.com/nng265/OEMEVManagerment_FE  
- Make sure `VITE_API_BASE_URL` in FE points to the backend URL (e.g. `http://localhost:5000`).

## Notes
- Keep API versioning and DTOs in sync between FE and BE.
- Image upload uses ImageKit — configure keys in `appsettings.json`.
- Email templates and confirmation URLs are configured under `EmailUrlSettings`.

## License
MIT License

Copyright (c) 2025

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is furnished
to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
