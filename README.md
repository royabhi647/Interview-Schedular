# Interview Scheduler

A web application for scheduling interviews with Google Calendar integration and email notifications.

## Features

- Schedule interviews with automatic Google Meet link generation
- Email notifications to candidates and interviewers
- Real-time dashboard to view all interviews
- Google OAuth authentication
- Responsive design with Material-UI

### [live preview](https://www.loom.com/share/6fa6fdbf3d4f46f78eb4e412fb8ff177)

## Tech Stack

**Backend:** ASP.NET Core 8.0, Entity Framework, SQL Server  
**Frontend:** React, Material-UI, Axios  
**APIs:** Google Calendar API, Google OAuth 2.0

## Setup Instructions

### Prerequisites
- .NET 8.0 SDK
- Node.js 18+
- SQL Server or SQL Server Express

### Clone
1. Clone the repository: `git clone https://github.com/royabhi647/Interview-Schedular.git`

### Backend Setup

1. **Navigate to Server directory**
   ```bash
   cd Server
   ```

2. **Install packages**
   ```bash
   dotnet restore
   ```

3. **Set up database**
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

4. **Configure Google OAuth (Optional)**
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "Google:ClientId" "your-client-id"
   dotnet user-secrets set "Google:ClientSecret" "your-client-secret"
   ```

5. **Run backend**
   ```bash
   dotnet run --urls "http://localhost:5180"
   ```

### Frontend Setup

1. **Navigate to frontend directory**
   ```bash
   cd client
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Start development server**
   ```bash
   npm run dev
   ```

## Usage

1. **Access the application**
   - Frontend: `http://localhost:5173`
   - Backend API: `http://localhost:5180`
   - API Documentation: `http://localhost:5180/swagger`

2. **Schedule an interview**
   - Fill out the interview form
   - System generates Google Meet link
   - Email notifications sent automatically

3. **View interviews**
   - Check the dashboard for all scheduled interviews
   - View interview details and meeting links

## Project Structure

```
├── Server/                 # ASP.NET Core API
│   ├── Controllers/        # API endpoints
│   ├── Data/              # Database context
│   ├── Models/            # Data models
│   └── Services/          # Business logic
└── frontend/              # React application
    ├── src/
    │   ├── components/    # React components
    │   └── App.js        # Main app component
    └── public/
```

## API Endpoints

- `POST /api/Interview` - Create new interview
- `GET /api/Interview` - Get all interviews
- `GET /api/Auth/status` - Check authentication status
- `GET /api/Auth/google-login` - Initiate Google OAuth

## Configuration

Update `appsettings.json` for production:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-connection-string"
  },
  "Google": {
    "ClientId": "your-google-client-id",
    "ClientSecret": "your-google-client-secret"
  }
}
```

## Notes

- For development, Google OAuth uses placeholder links
- Email notifications require SMTP configuration
- Database uses SQL Server LocalDB by default