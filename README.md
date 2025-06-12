# Snitch Intelligence System

A comprehensive intelligence and surveillance system built with .NET 8 Web API and React.

## Overview

The Snitch system provides tools for intelligence gathering, person tracking, report management, and analytics. Built for educational and training purposes.

## Technology Stack

### Backend
- .NET 8 Web API
- Entity Framework Core with MySQL
- JWT Authentication
- Swagger/OpenAPI Documentation
- BCrypt for password hashing

### Frontend  
- React 18
- React Router for navigation
- Axios for API communication
- Material-UI for components
- Bootstrap for styling

### Infrastructure
- Docker & Docker Compose
- MySQL 8.0 Database
- Nginx (production)
- Multi-stage builds

## Features

- User registration and authentication
- Person profile management
- Report submission and tracking
- Alert system with notifications
- Analytics dashboard
- Behavioral pattern analysis
- Threat assessment tools
- System audit logging
- OpenAI integration for text analysis
- Hardcoded analysis fallback

## Quick Start

### Prerequisites
- Docker and Docker Compose
- Git
- Node.js 18+ (for frontend development)
- .NET 8 SDK (for backend development)

### Development Setup

1. Clone the repository:
```bash
git clone <your-repo-url>
cd snitch-system
```

2. Backend Setup:
```bash
cd backend
dotnet restore
dotnet run
```

3. Frontend Setup:
```bash
cd frontend
npm install
npm start
```

4. Access the application:
- Frontend: http://localhost:3000
- Backend API: http://localhost:5243
- API Documentation: http://localhost:5243/swagger

## Project Structure
```
snitch-system/
├── backend/              # .NET 8 Web API
│   ├── Controllers/      # API controllers
│   ├── Models/          # Data models
│   ├── Services/        # Business logic
│   ├── Data/            # Database context
│   └── Migrations/      # EF Core migrations
├── frontend/            # React app
│   ├── src/
│   │   ├── components/  # React components
│   │   ├── Services/    # API services
│   │   └── hooks/       # Custom hooks
│   ├── public/          # Static files
│   └── build/           # Production build
├── Dockerfile           # Frontend Dockerfile
└── nginx.conf          # Nginx configuration
```

### Available Scripts

Frontend:
- `npm start` - Start development server
- `npm build` - Build for production
- `npm test` - Run tests

Backend:
- `dotnet run` - Start development server
- `dotnet build` - Build the project
- `dotnet test` - Run tests

### API Endpoints

#### Authentication
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh` - Refresh token

#### People Management
- `GET /api/People` - List all people
- `GET /api/People/{id}` - Get person details
- `POST /api/People` - Create person
- `PUT /api/People/{id}` - Update person
- `DELETE /api/People/{id}` - Delete person

#### Reports
- `GET /api/Reports` - List all reports
- `GET /api/Reports/{id}` - Get report details
- `POST /api/Reports` - Submit report
- `PUT /api/Reports/{id}` - Update report
- `POST /api/Reports/import-csv` - Import reports from CSV

#### Alerts
- `GET /api/Alerts` - List alerts
- `POST /api/Alerts` - Create alert
- `PUT /api/Alerts/{id}` - Update alert

#### Analysis
- `GET /api/AnalysisPreference` - Get analysis preference
- `POST /api/AnalysisPreference` - Set analysis preference

## Environment Variables

### Backend (.env)
```
DB_SERVER=localhost
DB_NAME=snitch_intel
DB_USER=your_db_user
DB_PASSWORD=your_secure_password
JWT_SECRET_KEY=your_secure_jwt_key
JWT_ISSUER=http://localhost:5243
JWT_AUDIENCE=http://localhost:3000
OPENAI_API_KEY=your_openai_api_key
```

### Frontend (.env)
```
REACT_APP_API_BASE_URL=http://localhost:5243/api
```

## Database

The system uses MySQL with Entity Framework Core migrations.

### Models
- **User** - System users with authentication
- **Person** - Subjects of surveillance
- **Report** - Incident reports
- **Alert** - System alerts and notifications
- **BehavioralPattern** - AI-analyzed patterns
- **ThreatAssessment** - Risk evaluations
- **SystemLog** - Audit trail

### Migrations
```bash
# Create migration
cd backend
dotnet ef migrations add MigrationName

# Apply migrations
dotnet ef database update
```

## Security

- JWT token-based authentication
- Password hashing with BCrypt
- CORS configuration
- Input validation and sanitization
- SQL injection prevention
- Audit logging
- Environment variable configuration
- Secure API endpoints

## Contributing

This is a solo development project. For modifications:

1. Work on the `develop` branch
2. Create feature branches for major changes
3. Merge to `main` when ready to release
4. Use semantic commit messages

## License

This project is for educational and training purposes only.

## Support

For issues or questions, refer to the project documentation or create an issue in the repository.
