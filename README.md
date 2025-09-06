# StudyHub Backend 🎓

A real-time collaborative study platform backend built with .NET 8, featuring live chat, study rooms, and user management capabilities.

## 🚀 Features

- **Real-time Chat** - Live messaging in study rooms using SignalR WebSockets
- **Study Rooms** - Create, join, and manage collaborative study spaces
- **Authentication & Authorization** - JWT-based security with role-based access (Admin/Student)
- **User Management** - Registration, login, and user profiles
- **Scalable Architecture** - Redis backplane for SignalR scaling across multiple instances
- **Health Monitoring** - Built-in health checks for database and Redis connectivity
- **Containerized Deployment** - Complete Docker setup with PostgreSQL and Redis
- **API Documentation** - Swagger/OpenAPI integration for interactive API docs

## 🏗️ Architecture

This project follows **Clean Architecture** principles with clear separation of concerns:

```
StudyHub.Core/           # Domain Layer
├── Entities/           # Domain entities (User, Room, ChatMessage)
├── DTOs/              # Data transfer objects
├── Services/          # Business logic services
├── Interfaces/        # Repository and service contracts
└── Constants/         # Application constants

StudyHub.Infrastructure/  # Data Access Layer
├── Data/              # EF Core DbContext and migrations
├── Repositories/      # Data access implementations
└── Config/           # Infrastructure configuration

StudyHub.Presentation/   # API Layer
├── Controllers/       # REST API endpoints
├── Hubs/             # SignalR hubs for real-time communication
└── Program.cs        # Application startup and configuration
```

## 🛠️ Technology Stack

- **.NET 8** - Latest .NET runtime with ASP.NET Core
- **Entity Framework Core** - ORM with PostgreSQL database
- **SignalR** - Real-time web functionality with Redis backplane
- **JWT Authentication** - Secure token-based authentication
- **PostgreSQL** - Primary database for persistent data
- **Redis** - Caching and SignalR backplane for scaling
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation
- **Docker** - Containerization and orchestration

## 🚀 Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker & Docker Compose](https://docs.docker.com/get-docker/)

### Option 1: Docker Compose (Recommended)

1. **Clone the repository**
   ```bash
   git clone https://github.com/darshan601/StudyHub_Backend.git
   cd StudyHub_Backend
   ```

2. **Start all services using compose.yaml**
   ```bash
   docker compose up --build
   ```

3. **Access the application**
   - API: http://localhost:5151
   - Swagger UI: http://localhost:5151/swagger
   - Health Check: http://localhost:5151/health

### Option 2: Local Development

1. **Setup Database & Redis**
   ```bash
   # Start PostgreSQL and Redis using compose.yaml
   docker compose up postgres redis -d
   ```

2. **Configure Connection Strings**
   ```bash
   # Update appsettings.Development.json or use environment variables
   export ConnectionStrings__DefaultConnection="Host=localhost;Database=StudyDb;Username=postgres;Password=secret"
   export ConnectionStrings__Redis="localhost:6379"
   export Jwt__Key="your-super-secret-jwt-key-here"
   ```

3. **Run Database Migrations**
   ```bash
   cd StudyHub.Presentation
   dotnet ef database update --project ../StudyHub.Infrastructure
   ```

4. **Start the API**
   ```bash
   dotnet run --project StudyHub.Presentation
   ```

## 📚 API Documentation

### Authentication Endpoints

#### POST `/api/auth/register`
Register a new user account.

**Request Body:**
```json
{
  "userName": "john_doe",
  "password": "SecurePassword123!"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "guid-refresh-token"
}
```

#### POST `/api/auth/login`
Authenticate user and get access token.

**Request Body:**
```json
{
  "userName": "john_doe",
  "password": "SecurePassword123!"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "guid-refresh-token"
}
```

#### GET `/api/auth/debug-claims` 🔒
Debug endpoint to view current user's JWT claims (requires authentication).

### Room Management Endpoints

#### POST `/api/rooms` 🔒👑
Create a new study room (Admin only).

**Headers:**
```
Authorization: Bearer {token}
```

**Request Body:**
```json
{
  "slug": "math-101",
  "title": "Mathematics 101 Study Group"
}
```

**Response:**
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "slug": "math-101",
  "title": "Mathematics 101 Study Group",
  "ownerId": "456e7890-e89b-12d3-a456-426614174000"
}
```

#### GET `/api/rooms/{id}` 🔒
Get room information by ID.

**Headers:**
```
Authorization: Bearer {token}
```

**Response:**
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000"
}
```

#### POST `/api/rooms/{id}/join` 🔒
Join a study room.

**Headers:**
```
Authorization: Bearer {token}
```

**Response:** `204 No Content`

#### POST `/api/rooms/{id}/leave` 🔒
Leave a study room.

**Headers:**
```
Authorization: Bearer {token}
```

**Response:** `204 No Content`

#### GET `/api/rooms/mine` 🔒
Get all rooms the current user is a member of.

**Headers:**
```
Authorization: Bearer {token}
```

**Response:**
```json
[
  {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "slug": "math-101",
    "title": "Mathematics 101 Study Group",
    "ownerId": "456e7890-e89b-12d3-a456-426614174000"
  }
]
```

### Health Check Endpoint

#### GET `/api/health`
Basic health check endpoint.

**Response:**
```json
{
  "status": "healthy",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

#### GET `/health`
Comprehensive health check with database and Redis status.

**Response:**
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0234567",
  "entries": {
    "npgsql": {
      "status": "Healthy",
      "duration": "00:00:00.0123456"
    },
    "redis": {
      "status": "Healthy", 
      "duration": "00:00:00.0098765"
    }
  }
}
```

## 🌐 WebSocket API (SignalR)

Connect to the SignalR hub at `/chat` for real-time messaging functionality.

### Connection

**Endpoint:** `ws://localhost:5151/chat`

**Authentication:** Include JWT token as query parameter
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chat", {
        accessTokenFactory: () => userToken
    })
    .build();
```

### Client Methods (Send to Server)

#### `JoinRoom(roomId)`
Join a chat room to receive messages.

**Parameters:**
- `roomId` (string): GUID of the room to join

**Example:**
```javascript
await connection.invoke("JoinRoom", "123e4567-e89b-12d3-a456-426614174000");
```

#### `LeaveRoom(roomId)`
Leave a chat room.

**Parameters:**
- `roomId` (string): GUID of the room to leave

**Example:**
```javascript
await connection.invoke("LeaveRoom", "123e4567-e89b-12d3-a456-426614174000");
```

#### `SendMessage(roomId, content)`
Send a message to a room.

**Parameters:**
- `roomId` (string): GUID of the target room
- `content` (string): Message content (max 2000 characters)

**Example:**
```javascript
await connection.invoke("SendMessage", "123e4567-e89b-12d3-a456-426614174000", "Hello everyone!");
```

#### `GetHistory(roomId, count?)`
Retrieve chat history for a room.

**Parameters:**
- `roomId` (string): GUID of the room
- `count` (number, optional): Number of messages to retrieve (default: 50)

**Example:**
```javascript
await connection.invoke("GetHistory", "123e4567-e89b-12d3-a456-426614174000", 100);
```

### Server Methods (Receive from Server)

#### `ReceiveMessage(message)`
Receive a new chat message.

**Message Object:**
```javascript
{
  id: "789e0123-e89b-12d3-a456-426614174000",
  roomId: "123e4567-e89b-12d3-a456-426614174000", 
  userId: "456e7890-e89b-12d3-a456-426614174000",
  content: "Hello everyone!",
  timeStamp: "2024-01-15T10:30:00Z"
}
```

**Example:**
```javascript
connection.on("ReceiveMessage", (message) => {
    console.log(`${message.userId}: ${message.content}`);
});
```

#### `SystemMessage(message)`
Receive system notifications (user joins/leaves).

**Example:**
```javascript
connection.on("SystemMessage", (message) => {
    console.log(`System: ${message}`);
});
```

#### `MessageHistory(messages)`
Receive chat history response.

**Messages Array:**
```javascript
[
  {
    id: "789e0123-e89b-12d3-a456-426614174000",
    roomId: "123e4567-e89b-12d3-a456-426614174000",
    userId: "456e7890-e89b-12d3-a456-426614174000", 
    content: "Hello everyone!",
    timeStamp: "2024-01-15T10:30:00Z"
  }
]
```

**Example:**
```javascript
connection.on("MessageHistory", (messages) => {
    messages.forEach(msg => console.log(`${msg.userId}: ${msg.content}`));
});
```

### JavaScript Client Example

```javascript
const signalR = require("@microsoft/signalr");

// Create connection
const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5151/chat", {
        accessTokenFactory: () => "your-jwt-token-here"
    })
    .build();

// Set up event handlers
connection.on("ReceiveMessage", (message) => {
    console.log(`[${message.timeStamp}] ${message.userId}: ${message.content}`);
});

connection.on("SystemMessage", (message) => {
    console.log(`System: ${message}`);
});

connection.on("MessageHistory", (messages) => {
    console.log("Chat History:");
    messages.forEach(msg => 
        console.log(`[${msg.timeStamp}] ${msg.userId}: ${msg.content}`)
    );
});

// Start connection
async function start() {
    try {
        await connection.start();
        console.log("Connected to SignalR hub");
        
        // Join a room
        await connection.invoke("JoinRoom", "123e4567-e89b-12d3-a456-426614174000");
        
        // Get chat history
        await connection.invoke("GetHistory", "123e4567-e89b-12d3-a456-426614174000", 50);
        
        // Send a message
        await connection.invoke("SendMessage", "123e4567-e89b-12d3-a456-426614174000", "Hello from JavaScript!");
        
    } catch (err) {
        console.error("Connection failed: ", err);
    }
}

start();
```

## ⚙️ Configuration

### Environment Variables

| Variable | Description | Default Value | Required |
|----------|-------------|---------------|----------|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string | - | ✅ |
| `ConnectionStrings__Redis` | Redis connection string | - | ✅ |
| `Jwt__Key` | JWT signing key (min 32 chars) | - | ✅ |
| `Jwt__Issuer` | JWT token issuer | `StudyPlatform` | ✅ |
| `Jwt__Audience` | JWT token audience | `StudyPlatformUsers` | ✅ |
| `Admin__Username` | Default admin username | `admin` | ❌ |
| `Admin__Password` | Default admin password | `admin123!` | ❌ |
| `AllowedOrigins__0` | CORS allowed origin | `http://localhost:5173` | ❌ |
| `ASPNETCORE_ENVIRONMENT` | Environment name | `Development` | ❌ |

### appsettings.json Structure

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=StudyDb;Username=postgres;Password=secret",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "Key": "super_secret_very_long_key_here_12345",
    "Issuer": "StudyPlatform", 
    "Audience": "StudyPlatformUsers"
  },
  "Admin": {
    "Username": "admin",
    "Password": "admin123!"
  },
  "AllowedOrigins": [
    "http://localhost:5173"
  ],
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information"
    },
    "WriteTo": [
      {"Name": "Console"},
      {"Name": "File", "Args": {"path": "logs/log-.txt", "rollingInterval": "Day"}}
    ]
  }
}
```

## 🐳 Docker Deployment

The application includes a complete Docker setup with multi-stage builds and health checks defined in `compose.yaml`. This Docker Compose file orchestrates all the necessary services for the application.

### Services Overview

The `compose.yaml` file defines the following services:

- **api** - Main .NET application (port 5151)
- **postgres** - PostgreSQL database (port 5432)
- **redis** - Redis cache/message broker (port 6379)
- **migrations** - Database migration service (runs once on startup)

### Docker Commands

```bash
# Start all services defined in compose.yaml
docker compose up --build

# Start in background
docker compose up -d --build

# View logs
docker compose logs -f api

# Stop all services
docker compose down

# Reset database (removes volumes)
docker compose down -v
```

### Production Deployment

For production deployment, update the environment variables in `compose.yaml`:

```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Production
  - ConnectionStrings__DefaultConnection=Host=prod-db;Database=StudyDb;Username=app_user;Password=strong_password
  - Jwt__Key=your-production-jwt-key-minimum-32-characters
  - Admin__Password=secure_admin_password
```

## 🔧 Development Workflow

### Setting Up Development Environment

1. **Install Prerequisites**
   ```bash
   # Install .NET 8 SDK
   dotnet --version  # Should be 8.0.x
   
   # Install Entity Framework tools
   dotnet tool install --global dotnet-ef
   ```

2. **Clone and Setup**
   ```bash
   git clone https://github.com/darshan601/StudyHub_Backend.git
   cd StudyHub_Backend
   
   # Start dependencies using compose.yaml
   docker compose up postgres redis -d
   
   # Restore packages
   dotnet restore
   ```

3. **Database Migrations**
   ```bash
   # Create new migration
   dotnet ef migrations add MigrationName --project StudyHub.Infrastructure --startup-project StudyHub.Presentation
   
   # Apply migrations
   dotnet ef database update --project StudyHub.Infrastructure --startup-project StudyHub.Presentation
   
   # Remove last migration
   dotnet ef migrations remove --project StudyHub.Infrastructure --startup-project StudyHub.Presentation
   ```

4. **Running the Application**
   ```bash
   # Development mode with hot reload
   dotnet watch run --project StudyHub.Presentation
   
   # Standard run
   dotnet run --project StudyHub.Presentation
   ```

### Code Structure Guidelines

- **Entities** - Pure domain objects with no dependencies
- **DTOs** - Data transfer objects for API contracts
- **Services** - Business logic implementation
- **Repositories** - Data access abstractions
- **Controllers** - HTTP API endpoints
- **Hubs** - SignalR real-time communication

### Adding New Features

1. **Add Entity** (if needed)
   ```csharp
   // StudyHub.Core/Entities/NewEntity.cs
   public class NewEntity
   {
       public Guid Id { get; set; }
       // ... properties
   }
   ```

2. **Add Repository Interface**
   ```csharp
   // StudyHub.Core/Interfaces/INewRepository.cs
   public interface INewRepository
   {
       Task<NewEntity?> GetByIdAsync(Guid id);
       // ... methods
   }
   ```

3. **Implement Repository**
   ```csharp
   // StudyHub.Infrastructure/Repositories/NewRepository.cs
   public class NewRepository : INewRepository
   {
       // ... implementation
   }
   ```

4. **Add Service**
   ```csharp
   // StudyHub.Core/Services/NewService.cs
   public class NewService
   {
       // ... business logic
   }
   ```

5. **Add Controller**
   ```csharp
   // StudyHub.Presentation/Controllers/NewController.cs
   [ApiController]
   [Route("api/[controller]")]
   public class NewController : ControllerBase
   {
       // ... endpoints
   }
   ```

6. **Register Dependencies**
   ```csharp
   // StudyHub.Infrastructure/Config/ServiceCollectionExtensions.cs
   services.AddScoped<INewRepository, NewRepository>();
   services.AddScoped<NewService>();
   ```

## 📝 Default Credentials

When the application starts, it automatically creates:

- **Admin User**
  - Username: `admin`
  - Password: `admin123!`
  - Role: `admin`

- **Sample Room**
  - Slug: `math101`
  - Title: `Math 101`
  - Owner: Admin user

## 🔍 Troubleshooting

### Common Issues

1. **Database Connection Failed**
   ```
   Solution: Ensure PostgreSQL is running and connection string is correct
   docker compose up postgres -d
   ```

2. **Redis Connection Failed**
   ```
   Solution: Ensure Redis is running
   docker compose up redis -d
   ```

3. **JWT Authentication Failed**
   ```
   Solution: Check JWT configuration and ensure key is at least 32 characters
   ```

4. **Migration Errors**
   ```bash
   # Reset database
   dotnet ef database drop --project StudyHub.Infrastructure --startup-project StudyHub.Presentation
   dotnet ef database update --project StudyHub.Infrastructure --startup-project StudyHub.Presentation
   ```

5. **CORS Issues**
   ```
   Solution: Update AllowedOrigins in appsettings.json to include your frontend URL
   ```

### Debug Modes

Enable verbose logging for troubleshooting:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore.Authentication": "Debug",
      "Microsoft.AspNetCore.Authorization": "Debug"
    }
  }
}
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes following the code structure guidelines
4. Run tests and ensure the build passes
5. Commit your changes (`git commit -m 'Add amazing feature'`)
6. Push to the branch (`git push origin feature/amazing-feature`)
7. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🎯 Future Enhancements

- [ ] **Background Workers** - Redis Streams for durable message processing
- [ ] **File Sharing** - Upload and share study materials
- [ ] **Video Chat** - WebRTC integration for video calls
- [ ] **Study Sessions** - Scheduled study sessions with notifications
- [ ] **User Profiles** - Extended user information and avatars
- [ ] **Message Reactions** - React to messages with emojis
- [ ] **Room Categories** - Organize rooms by subject/category
- [ ] **Moderation Tools** - Admin tools for content moderation
- [ ] **Analytics** - Usage analytics and reporting
- [ ] **Mobile App** - React Native or Flutter mobile client

---

## 📞 Support

For questions or support, please:
- Open an issue on GitHub
- Contact the development team
- Check the troubleshooting section above

**Happy Studying! 📚✨**