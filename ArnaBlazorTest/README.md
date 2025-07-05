 # Arna Learning Management System

A mini LMS sub-module built with .NET 8 and Blazor Server for assignment management, quiz taking, and progress tracking.

## 🎯 Project Overview

This is a **mini LMS sub-module** that allows:
- **Learners** to view assignments, take quizzes, and track their progress
- **Managers** to create assignments, manage teams, and view progress reports
- **Quiz System** with 5 questions per assignment, 20 points each (100% total)

## 🚀 Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

## 📦 Getting Started

1. **Clone the repository**
2. **Navigate to the project directory**
3. **Update connection string** in `appsettings.json` if needed
4. **Run the following commands:**

```bash
dotnet restore
dotnet ef database update
dotnet run
```

5. **Access the application** at `https://localhost:5001` or `http://localhost:5000`

## 👥 User Roles & Features

### **Learner Features**
- View list of active assignments
- Take quizzes with 5 questions each
- Track progress and scores
- View assignment history

### **Manager Features**
- Create and manage assignments
- Assign learners to teams
- View team progress reports
- Monitor completion rates and scores

## 🏗️ Project Architecture

```
ArnaBlazorTest/
├── Components/           # Blazor components and pages
│   ├── Layout/          # Navigation and layout components
│   ├── Pages/           # Main application pages
│   │   ├── Assignments/ # Assignment list and detail pages
│   │   ├── Auth/        # Login/Register pages
│   │   ├── Manager/     # Manager-specific pages
│   │   └── Progress/    # Progress tracking pages
│   └── Shared/          # Reusable components
├── Controllers/          # API endpoints
├── Data/                # Database context and configurations
├── Models/              # Domain models
├── Services/            # Business logic implementation
└── wwwroot/             # Static files
```

## 🗄️ Database Schema (ERD)

### **Entity Relationship Diagram**

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│     Users       │     │   UserRoles     │     │   Assignments   │
├─────────────────┤     ├─────────────────┤     ├─────────────────┤
│ Id (PK)         │     │ Id (PK)         │     │ Id (PK)         │
│ Username        │     │ Name            │     │ Title           │
│ Email           │     └─────────────────┘     │ Description     │
│ PasswordHash    │             │               │ MaterialUrl     │
│ RoleId (FK)     │◄────────────┘               │ IsActive        │
│ ManagerId (FK)  │                             │ CreatedAt       │
│ CreatedAt       │                             └─────────────────┘
└─────────────────┘                                     │
         │                                               │
         │ 1:N                                           │ 1:N
         ▼                                               ▼
┌─────────────────┐                             ┌─────────────────┐
│AssignmentProgress│                             │    Questions    │
├─────────────────┤                             ├─────────────────┤
│ Id (PK)         │                             │ Id (PK)         │
│ UserId (FK)     │                             │ QuestionText    │
│ AssignmentId(FK)│                             │ OptionsJson     │
│ Score           │                             │ CorrectAnswer   │
│ Status          │                             │ AssignmentId(FK)│
│ SubmittedAt     │                             └─────────────────┘
│ Answers         │
└─────────────────┘
```

### **Database Tables**

#### **Users**
- `Id` (PK) - Primary key
- `Username` - Unique username
- `Email` - User email
- `PasswordHash` - BCrypt hashed password
- `RoleId` (FK) - Reference to UserRoles
- `ManagerId` (FK) - Self-reference for manager-subordinate relationship
- `CreatedAt` - Account creation timestamp

#### **UserRoles**
- `Id` (PK) - Primary key
- `Name` - Role name (Learner/Manager)

#### **Assignments**
- `Id` (PK) - Primary key
- `Title` - Assignment title
- `Description` - Assignment description
- `MaterialUrl` - Link to learning material
- `IsActive` - Whether assignment is available
- `CreatedAt` - Assignment creation timestamp

#### **Questions**
- `Id` (PK) - Primary key
- `QuestionText` - The question text
- `OptionsJson` - JSON array of answer options
- `CorrectAnswer` - The correct answer
- `AssignmentId` (FK) - Reference to Assignments

#### **AssignmentProgress**
- `Id` (PK) - Primary key
- `UserId` (FK) - Reference to Users
- `AssignmentId` (FK) - Reference to Assignments
- `Score` - Quiz score (0-100)
- `Status` - Progress status (completed/pending)
- `SubmittedAt` - Submission timestamp
- `Answers` - JSON of user answers

### **Key Relationships**

1. **Users → UserRoles** (Many-to-One)
   - Each user has one role (Learner or Manager)

2. **Users → Users** (Self-Reference)
   - Managers can have subordinates (Learners)

3. **Assignments → Questions** (One-to-Many)
   - Each assignment has exactly 5 questions

4. **Users → AssignmentProgress** (One-to-Many)
   - Users can have multiple assignment submissions

5. **Assignments → AssignmentProgress** (One-to-Many)
   - Assignments can have multiple user submissions

## 🔐 Security Features

- **JWT Authentication** - Secure token-based authentication
- **Role-based Authorization** - Different access levels for Learners and Managers
- **Password Hashing** - BCrypt for secure password storage
- **HTTPS Enforcement** - Secure communication

## 🧪 Testing

### **Run All Tests**
```bash
dotnet test
```

### **Run Tests with Details**
```bash
dotnet test --logger "console;verbosity=detailed"
```

### **Test Coverage**
- **Scoring Logic Tests** - 5 comprehensive tests covering:
  - All correct answers (100 points)
  - All incorrect answers (0 points)
  - Partial correct answers
  - Case insensitive answers
  - Edge cases

## 🎯 Quiz System

### **Scoring Rules**
- **5 questions per assignment** (enforced)
- **20 points per correct answer**
- **Total possible score: 100 points (100%)**
- **One submission per assignment per user**
- **Case insensitive answers**

### **Example Scoring**
```
Question 1: Correct → 20 points
Question 2: Correct → 20 points
Question 3: Incorrect → 0 points
Question 4: Correct → 20 points
Question 5: Incorrect → 0 points
Total Score: 60/100 (60%)
```

## 🚀 Development

### **Run in Development Mode**
```bash
dotnet watch run
```

### **Database Migrations**
```bash
# Create new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update
```

### **Seed Data**
The application automatically seeds:
- Test users (learner/manager)
- Sample assignments with 5 questions each
- User roles (Learner/Manager)

## 📋 API Endpoints

### **Authentication**
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration

### **Assignments**
- `GET /api/assignment` - Get all assignments
- `GET /api/assignment/{id}` - Get specific assignment
- `POST /api/assignment` - Create assignment (Manager only)
- `PUT /api/assignment/{id}` - Update assignment (Manager only)
- `DELETE /api/assignment/{id}` - Delete assignment (Manager only)

### **Progress**
- `GET /api/progress/user/{userId}` - Get user progress
- `GET /api/progress/team/{managerId}` - Get team progress

## 🎨 UI Features

- **Responsive Design** - Works on desktop and mobile
- **Role-based Navigation** - Different menus for Learners and Managers
- **Progress Visualization** - Charts and progress bars
- **Real-time Updates** - Blazor Server for live updates

## 🔧 Configuration

### **appsettings.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ArnaBlazorTest;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "Jwt": {
    "Key": "YourSecretKeyHere",
    "Issuer": "ArnaBlazorTest",
    "Audience": "ArnaBlazorTest"
  }
}
```

## 📝 License

This project is for educational purposes.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

---

**Built with ❤️ using .NET 8 and Blazor Server**