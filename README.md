# Snitch Intelligence System

A comprehensive intelligence and surveillance system built with .NET 8 Web API and React TypeScript.

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
- React 18 with TypeScript
- React Router for navigation
- Axios for API communication
- Modern responsive design

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

## Quick Start

### Prerequisites
- Docker and Docker Compose
- Git

### Development Setup

1. Clone the repository:
```bash
git clone <your-repo-url>
cd snitch-system
```

2. Copy environment configuration:
```bash
cp .env.example .env
# Edit .env with your settings
```

3. Start development environment:
```bash
./start-dev.sh
```

4. Access the application:
- Frontend: http://localhost:3001
- Backend API: http://localhost:5001
- API Documentation: http://localhost:5001/swagger

### Production Setup

1. Configure environment variables in `.env`

2. Start production environment:
```bash
./start-prod.sh
```

3. Access the application:
- Frontend: http://localhost:3000
- Backend API: http://localhost:5000

## Development

### Project Structure
```
snitch-system/
├── backend/              # .NET 8 Web API
│   ├── Controllers/      # API controllers
│   ├── Models/          # Data models
│   ├── Services/        # Business logic
│   └── Data/            # Database context
├── frontend/            # React TypeScript app
│   ├── src/components/  # React components
│   ├── src/services/    # API services
│   └── src/hooks/       # Custom hooks
├── docker-compose.yml   # Production setup
└── docker-compose.dev.yml # Development setup
```

### Available Scripts

- `./start-dev.sh` - Start development environment
- `./start-prod.sh` - Start production environment  
- `./stop.sh` - Stop all services
- `./setup-db.sh` - Initialize database

### API Endpoints

#### Authentication
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh` - Refresh token

#### People Management
- `GET /api/people` - List all people
- `GET /api/people/{id}` - Get person details
- `POST /api/people` - Create person
- `PUT /api/people/{id}` - Update person
- `DELETE /api/people/{id}` - Delete person

#### Reports
- `GET /api/reports` - List all reports
- `GET /api/reports/{id}` - Get report details
- `POST /api/reports` - Submit report
- `PUT /api/reports/{id}` - Update report

#### Alerts
- `GET /api/alerts` - List alerts
- `POST /api/alerts` - Create alert
- `PUT /api/alerts/{id}` - Update alert

#### Analytics
- `GET /api/analytics/dashboard` - Dashboard data
- `GET /api/analytics/reports` - Report analytics

## Environment Variables

See `.env.example` for all available configuration options.

### Required Variables
- `MYSQL_ROOT_PASSWORD` - MySQL root password
- `MYSQL_DATABASE` - Database name
- `MYSQL_USER` - Database user
- `MYSQL_PASSWORD` - Database password
- `JWT_SECRET` - JWT signing key (min 32 characters)

## Database

The system uses MySQL 8.0 with Entity Framework Core migrations.

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
