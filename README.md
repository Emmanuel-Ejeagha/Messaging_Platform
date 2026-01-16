# Messaging Platform API

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blueviolet)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16+-336791)](https://www.postgresql.org/)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-success)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A production-ready, scalable messaging platform API built with ASP.NET Core 8, implementing Clean Architecture, Domain-Driven Design, and CQRS patterns. Supports one-to-one messaging, group conversations, message threading, and read receipts.

## 🚀 Features

- **Rich Messaging Capabilities**
  - One-to-one conversations
  - Group conversations with roles (Member/Admin/Owner)
  - Message threading and replies
  - Read receipts and unread counts
  - Message editing and soft deletion

- **Production-Ready Architecture**
  - Clean Architecture with clear separation of concerns
  - Domain-Driven Design with rich domain models
  - CQRS pattern using MediatR
  - Repository and Unit of Work patterns
  - Comprehensive validation using FluentValidation

- **Performance & Scalability**
  - PostgreSQL with JSONB support for flexible metadata
  - Efficient indexing strategy for large datasets
  - Pagination and keyset-based navigation
  - Materialized views for complex aggregations
  - Rate limiting and request throttling

- **API Quality**
  - RESTful API with proper HTTP semantics
  - OpenAPI/Swagger documentation
  - RFC 7807 Problem Details for errors
  - Request/Response logging with correlation IDs
  - Health checks and monitoring endpoints

- **Security & Reliability**
  - JWT Bearer authentication support
  - Rate limiting per endpoint
  - CORS configuration
  - Request validation and sanitization
  - Structured logging with Serilog

## 🏗️ Architecture

```
MessagingPlatform/
├── src/
│   ├── MessagingPlatform.Domain/          # Core business entities, value objects
│   ├── MessagingPlatform.Application/     # Use cases, CQRS, business logic
│   ├── MessagingPlatform.Infrastructure/  # Data access, external services
│   └── MessagingPlatform.API/            # Web API, controllers, middleware
└── tests/
    ├── Domain.UnitTests/                  # Domain logic tests
    ├── Application.UnitTests/             # Command/Query handler tests
    ├── Infrastructure.IntegrationTests/    # Database integration tests
    └── API.IntegrationTests/              # API endpoint tests
```

### Core Principles
- **Clean Architecture**: Dependencies flow inward, domain layer has no external dependencies
- **Domain-Driven Design**: Rich domain models with business logic encapsulation
- **CQRS**: Separate command and query models for optimal performance
- **SOLID Principles**: Each class has a single responsibility, open to extension

## 📋 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 16+](https://www.postgresql.org/download/)
- [Git](https://git-scm.com/downloads)

## 🛠️ Installation

### 1. Clone the Repository
```bash
git clone git@github.com:Emmanuel-Ejeagha/Messaging_Platform.git
cd Messaging_Platform
```

#### Using Local PostgreSQL
1. Create a database named `MessagingPlatform`
2. Update connection string in `appsettings.json`

### 3. Configure the Application
```bash
# Copy the development configuration
cp src/MessagingPlatform.API/appsettings.Development.json src/MessagingPlatform.API/appsettings.Development.local.json

# Edit the local configuration if needed
# Update connection strings and other settings
```

### 4. Apply Database Migrations
```bash
# From the solution root
dotnet ef database update --project src/MessagingPlatform.Infrastructure --startup-project src/MessagingPlatform.API
```

### 5. Run the Application
```bash
# Navigate to API project
cd src/MessagingPlatform.API

# Run the application
dotnet run
```

The API will be available at:
- **API**: https://localhost:5001 (or http://localhost:5000)
- **Swagger UI**: https://localhost:5001/api-docs
- **Health Checks UI**: https://localhost:5001/health-ui

## 🧪 Testing

### Running Tests
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/MessagingPlatform.Domain.UnitTests/
dotnet test tests/MessagingPlatform.Application.UnitTests/
dotnet test tests/MessagingPlatform.Infrastructure.IntegrationTests/
dotnet test tests/MessagingPlatform.API.IntegrationTests/
```

### Test Coverage
```bash
# Generate test coverage report (requires coverlet.collector)
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults
```

## 📖 API Documentation

### Authentication
The API uses JWT Bearer authentication. Include the token in the Authorization header:
```
Authorization: Bearer <your-jwt-token>
```

For development/testing, you can use the provided middleware that extracts user ID from:
- `X-User-Id` header, or
- JWT claim `sub` or `ClaimTypes.NameIdentifier`

### Endpoints Overview

#### Conversations
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/conversations` | Get user conversations (paginated) |
| POST | `/api/v1/conversations` | Create new conversation (one-to-one or group) |
| GET | `/api/v1/conversations/{id}` | Get conversation details with recent messages |
| PUT | `/api/v1/conversations/{id}` | Update conversation details |
| DELETE | `/api/v1/conversations/{id}` | Soft delete conversation |

#### Messages
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/conversations/{id}/messages` | Get conversation messages (paginated) |
| POST | `/api/v1/conversations/{id}/messages` | Send new message |
| GET | `/api/v1/conversations/{id}/messages/{messageId}` | Get specific message |
| PUT | `/api/v1/conversations/{id}/messages/{messageId}` | Edit message |
| DELETE | `/api/v1/conversations/{id}/messages/{messageId}` | Soft delete message |

#### Groups
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/groups/{groupId}/participants` | Add participant to group |
| DELETE | `/api/v1/groups/{groupId}/participants/{userId}` | Remove participant from group |
| PUT | `/api/v1/groups/{groupId}/participants/{userId}/role` | Change participant role |

### Example Requests

#### Create One-to-One Conversation
```http
POST /api/v1/conversations
Content-Type: application/json
Authorization: Bearer <token>

{
  "type": 0,
  "otherUserId": "12345678-1234-1234-1234-123456789012"
}
```

#### Send Message
```http
POST /api/v1/conversations/{conversationId}/messages
Content-Type: application/json
Authorization: Bearer <token>

{
  "content": "Hello, how are you?",
  "parentMessageId": null,
  "metadata": {
    "clientId": "web-app-v1",
    "deviceType": "desktop"
  }
}
```

#### Get Messages with Pagination
```http
GET /api/v1/conversations/{conversationId}/messages?pageNumber=1&pageSize=20&includeThreads=true
Authorization: Bearer <token>
```

### Error Responses
The API uses RFC 7807 Problem Details format for errors:
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation error",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "instance": "/api/v1/conversations",
  "timestamp": "2024-01-15T10:30:00Z",
  "traceId": "00-0af7651916cd43dd8448eb211c80319c-00f067aa0ba902b7-01",
  "errors": [
    "Message content is required",
    "Conversation ID must be a valid GUID"
  ]
}
```

## 🏭 Database Schema

### Core Tables
- **Conversations**: Main conversation table with TPH inheritance (OneToOne/Group)
- **Messages**: Individual messages with threading support
- **Participants**: Users in conversations with roles
- **MessageReadReceipts**: Read tracking for messages

### Key Database Features
- **Soft Deletes**: `IsDeleted` flag with audit trail
- **JSONB Support**: Flexible metadata storage in PostgreSQL
- **Efficient Indexing**: Composite indexes for common query patterns
- **Triggers**: Automatic updates (last message timestamp, unread counts)
- **Materialized Views**: Pre-computed statistics for performance

## 🔧 Configuration

### Environment Variables
| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Environment (Development/Production) | `Production` |
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string | - |
| `JWT__Secret` | JWT secret key | - |
| `AllowedOrigins` | CORS allowed origins | `*` |
| `RateLimit__Enable` | Enable rate limiting | `true` |

### App Settings
Key configuration sections in `appsettings.json`:
- **ConnectionStrings**: Database connections
- **Logging**: Log level configuration
- **Serilog**: Structured logging setup
- **IpRateLimiting**: Rate limiting rules
- **HealthChecksUI**: Health monitoring dashboard

## 📊 Monitoring & Observability

### Health Checks
- `/api/health` - Comprehensive health check
- `/api/health/liveness` - Simple liveness probe
- `/api/health/readiness` - Readiness check with dependencies
- `/health-ui` - Health checks dashboard

### Logging
- Structured logging with Serilog
- Console and file sinks
- Correlation IDs for request tracing
- Log levels configurable by environment

### Metrics (Planned)
- Request/response times
- Database query performance
- Error rates and types
- Message volume and throughput



### CI/CD Pipeline
The project includes GitHub Actions workflow for:
- Automated testing
- Docker image building
- Security scanning
- Deployment to staging/production

## 📈 Performance Considerations

### Database Optimization
- **Indexing Strategy**: Composite indexes for common query patterns
- **Query Optimization**: Use of EXISTS vs JOIN where appropriate
- **Materialized Views**: Pre-computed aggregates for complex queries
- **Connection Pooling**: Configured PostgreSQL connection pooling

### API Performance
- **Pagination**: Keyset-based pagination for large datasets
- **Response Compression**: GZIP compression for JSON responses
- **Caching Layer**: Redis caching for frequently accessed data
- **Rate Limiting**: Protection against abuse and DoS attacks

### Scaling Strategies
- **Horizontal Scaling**: Stateless API design supports multiple instances
- **Database Read Replicas**: For read-heavy workloads
- **Message Queue Integration**: For asynchronous processing
- **CDN Integration**: For static assets and media files

## 🔮 Future Enhancements

### Planned Features
- Real-time messaging with SignalR/WebSockets
- File attachments and media sharing
- Message reactions (likes, emojis)
- Message search with Elasticsearch
- Push notifications for mobile
- End-to-end encryption
- Message scheduling
- Chat bots integration

### Technical Improvements
- Distributed tracing with OpenTelemetry
- Advanced caching with Redis
- Event sourcing for audit trail
- API Gateway integration
- Blue-green deployment support
- Chaos engineering tests

## 🤝 Contributing

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Commit** your changes (`git commit -m 'Add amazing feature'`)
4. **Push** to the branch (`git push origin feature/amazing-feature`)
5. **Open** a Pull Request

### Development Guidelines
- Follow Clean Architecture principles
- Write unit tests for new features
- Update API documentation
- Ensure backward compatibility
- Follow the existing code style

### Code Style
- Use C# 12 features where appropriate
- Follow Clean Code principles
- Use meaningful names for classes and methods
- Add XML documentation for public APIs
- Keep methods small and focused

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

- **Documentation**: [GitHub Wiki](https://github.com/yourusername/messaging-platform/wiki)
- **Issues**: [GitHub Issues](https://github.com/yourusername/messaging-platform/issues)
- **Discussions**: [GitHub Discussions](https://github.com/yourusername/messaging-platform/discussions)
- **Email**: support@messagingplatform.com

## 🙏 Acknowledgments

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) by Robert C. Martin
- [Domain-Driven Design](https://domainlanguage.com/ddd/) by Eric Evans
- [MediatR](https://github.com/jbogard/MediatR) for CQRS implementation
- [FluentValidation](https://fluentvalidation.net/) for validation
- [Serilog](https://serilog.net/) for structured logging

---

**Built with ❤️ for scalable messaging applications**

*If you find this project useful, please give it a ⭐ on GitHub!*
```

---

# GitHub Repository Description

```markdown
Production-ready messaging platform API built with ASP.NET Core 8, Clean Architecture, and DDD. Supports one-to-one/group messaging, threading, read receipts, and real-time features. Perfect for building chat applications, customer support systems, or collaborative tools.
```

**Short GitHub Description** (for repo about section):
```
Production-ready messaging platform API with ASP.NET Core 8, Clean Architecture, DDD. Supports 1:1/group messaging, threading, read receipts. Built for scalability and performance.
```

**Tags/keywords** for GitHub repository:
- `aspnet-core`
- `clean-architecture`
- `domain-driven-design`
- `messaging-platform`
- `real-time-chat`
- `cqrs`
- `postgresql`
- `rest-api`
- `dotnet-8`
- `web-api`

**Topics** to add to GitHub repo:
- `aspnet-core`
- `clean-architecture`
- `domain-driven-design`
- `cqrs`
- `messaging-api`
- `chat-application`
- `rest-api`
- `postgresql`
- `dotnet`
- `webapi`

## Repository Features to Enable

1. **GitHub Actions**: CI/CD pipeline for automated testing and deployment
2. **CodeQL**: Security scanning for code vulnerabilities
3. **Dependabot**: Automated dependency updates
4. **Issue Templates**: Standardized bug reports and feature requests
5. **Pull Request Templates**: Consistent code review process
6. **Security Policy**: Clear security reporting guidelines
7. **Contributing Guide**: Instructions for community contributions
8. **Code Owners**: Define maintainers for different parts of the codebase

