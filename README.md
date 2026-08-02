# Adda 💬

A modern, full-stack social networking and dating web application featuring real-time chat, member directory, profile customization, photo moderation, and role-based administration.

Built with **Angular 21** on the frontend and **ASP.NET Core 10** on the backend.

---

## ✨ Features

- 👤 **User Profiles & Customization**: Comprehensive profile management with age calculation, bio, location details, interest tags, and photo gallery.
- 💬 **Real-time Messaging**: Instant direct messaging powered by SignalR web sockets with unread indicators and message thread history.
- 💖 **Member Directory & Bookmarks**: Browse and filter members by age, gender, and last active status with one-click profile bookmarking.
- 📸 **Photo Management & Moderation**: Multi-photo upload powered by Cloudinary with admin/moderator approval workflow for public photos.
- 🛡️ **Role-based Access Control (RBAC)**: Fine-grained user access management with `Admin`, `Moderator`, and `Member` roles.
- 🎨 **Dark Glassmorphic UI**: High-contrast, premium aesthetic built with custom CSS design tokens, custom responsive cards, toast notifications (`ngx-toastr`), and Google Fonts (*Outfit* and *Plus Jakarta Sans*).

<img width="1889" height="933" alt="image" src="https://github.com/user-attachments/assets/c42c28cb-db1e-4168-8512-4effb7ec501b" />


---

## 🛠️ Technology Stack

### **Frontend**
- **Framework**: Angular 21 (Standalone Components)
- **State & Routing**: Angular Router with Route Resolvers & Auth Guards
- **Real-time Communication**: `@microsoft/signalr`
- **UI & Styling**: Custom Dark Glassmorphic Design System, Bootstrap 5, FontAwesome 6
- **Notifications**: `ngx-toastr` (`NotificationService`)
- **File Uploads**: `ng2-file-upload`
- **UI Components**: `ngx-bootstrap` (Datepicker, Dropdowns, Pagination, Modals), `ngx-scrollbar`

### **Backend**
- **Framework**: ASP.NET Core 10 Web API (C# 13)
- **Database**: Entity Framework Core with MySQL / SQLite support
- **Authentication**: JWT Bearer Tokens with Identity Role authorization
- **Mapping**: Mapster for high-performance DTO mapping
- **Cloud Storage**: Cloudinary API integration for image uploads
- **Real-time WebSockets**: SignalR Chat Hub (`ChatHub`)

---

## 🚀 Getting Started

### Prerequisites

Ensure you have the following installed:
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (v18+ or v20+)
- [npm](https://www.npmjs.com/) (v9+)

---

### Backend Setup (`Adda.API`)

1. **Navigate to the backend project**:
   ```bash
   cd backend/Adda.API
   ```

2. **Configure App Settings**:
   Ensure `appsettings.json` contains your database connection string and Cloudinary/JWT secrets:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=adda;User=root;Password=yourpassword;"
     },
     "CloudinarySettings": {
       "CloudName": "your_cloud_name",
       "ApiKey": "your_api_key",
       "ApiSecret": "your_api_secret"
     },
     "JwtSettings": {
       "Secret": "YourSuperSecretJwtKeyHere"
     }
   }
   ```

3. **Apply Database Migrations**:
   ```bash
   dotnet ef database update
   ```

4. **Run the API**:
   ```bash
   dotnet run
   ```
   The backend API will start at `https://localhost:44392` (or configured port).

---

### Frontend Setup (`frontend`)

1. **Navigate to the frontend directory**:
   ```bash
   cd frontend
   ```

2. **Install Dependencies**:
   ```bash
   npm install --legacy-peer-deps
   ```

3. **Start Development Server**:
   ```bash
   npm start
   ```

4. **Open in Browser**:
   Navigate to `http://localhost:4200/`

---

## 🧪 Testing

### Running Frontend Tests (Karma / Jasmine)
```bash
cd frontend
npx ng test --watch=false
```

### Running Backend Tests (xUnit)
```bash
cd backend
dotnet test
```

---

## 📁 Repository Structure

```
adda/
├── backend/
│   ├── Adda.API/                 # ASP.NET Core 10 Web API
│   │   ├── Controllers/          # API Endpoints (Auth, Users, Messages, Admin)
│   │   ├── Data/                 # DataContext & Database Seeders
│   │   ├── Dtos/                 # Data Transfer Objects
│   │   ├── Helpers/              # Mapster Configurations & Helpers
│   │   ├── Hubs/                 # SignalR Chat Hubs
│   │   ├── Models/               # Entity Models (User, Photo, Message, Role)
│   │   ├── Repositories/         # Repository pattern implementations
│   │   └── Services/             # Business Logic & Auth Services
│   └── Adda.API.Tests/           # Integration & Unit Tests
│
└── frontend/                     # Angular 21 Single Page App
    ├── src/
    │   ├── app/
    │   │   ├── admin/            # Role & Photo management components
    │   │   ├── guards/           # Route Authorization Guards
    │   │   ├── members/          # Member directory, card, detail & edit views
    │   │   ├── messages/         # Direct messaging & thread views
    │   │   ├── resolver/         # Angular Route Resolvers
    │   │   └── services/         # API, Notification, Auth & Chat Services
    │   └── styles.css            # Dark Glassmorphism CSS Design Tokens
```

---

## 📄 License

This project is open-source and available under the [MIT License](LICENSE).
