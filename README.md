# App Name

This is a Gympass-style application, developed to showcase skills in .NET, unit testing, and CI with GitHub.

## Features

### Functional Requirements (RFs)

- [x] User registration
- [x] User authentication
- [x] Retrieving the logged-in user's profile
- [x] Number of check-ins performed
- [x] User's check-in history
- [x] Search for nearby gyms
- [x] Search for gyms by name
- [x] Check-in at gyms
- [x] Check-in validation
- [x] Gym registration

### Business Rules (RNs)

- [x] Users cannot register with a duplicate email
- [x] A user cannot check in twice on the same day
- [x] Check-in is not possible if not within 100m of the gym
- [x] Check-ins can only be validated within 20 minutes of creation
- [x] Only administrators can validate check-ins
- [x] Only administrators can register a gym

### Non-Functional Requirements (RNFs)

- [x] User passwords must be encrypted
- [x] Application data must be persisted in a PostgreSQL database
- [x] All data lists must be paginated with 20 items per page
- [x] User identification via JWT (JSON Web Token)

## Technologies Used

### Project Dependencies

- AutoMapper
- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Design
- Npgsql.EntityFrameworkCore.PostgreSQL
- Swashbuckle.AspNetCore
- System.IdentityModel.Tokens.Jwt
- Other dependencies...

### Docker

Information on using Docker and Docker Compose for the project.

## Configuration and Installation

Detailed instructions on how to configure and install the application.

```bash
# Clone the repository
git clone https://github.com/EmmanuelBarbosaMonteiro/GympassNetCore.git

# Navigate to the project directory
cd GympassNetCore

# Start the database using docker-compose
docker-compose up -d

# Build the .NET project
dotnet build

# Run the .NET project
dotnet run
```
