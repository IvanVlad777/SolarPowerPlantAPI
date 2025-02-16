# SolarPlantAPI Configuration Guide

## Setting Up `appsettings.json`

Before running the API, you need to configure `appsettings.json`. This file contains essential settings such as logging, database connection, and authentication.

### Example `appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Warning"
    },
    "LogFilePath": "D:LogsSolarAPIapi_logs.log"
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=SolarPowerDB;User Id=YOUR_DB_USER;Password=YOUR_DB_PASSWORD;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "YourSecureJWTKey!12345678YourSecureJWTKey!12345678",
    "Issuer": "SolarPowerAPI",
    "Audience": "SolarPowerAudience",
    "TokenExpirationMinutes": 180
  }
}
```

### Replace the following values:

| **Key**                                          | **What to Replace?**      | **Example**                                            |
| ------------------------------------------------ | ------------------------- | ------------------------------------------------------ |
| `LogFilePath`                                    | Path to log file          | `"C:\Logs\SolarAPI\api_logs.log"`                      |
| `Server=YOUR_SERVER_NAME`                        | Your SQL Server name      | `"localhost\SQLEXPRESS"`                               |
| `Database=SolarPowerDB`                          | Your database name        | `"SolarPlantDB"`                                       |
| `User Id=YOUR_DB_USER;Password=YOUR_DB_PASSWORD` | Your database credentials | `"sa; Password=StrongPassword123!"`                    |
| `SecretKey`                                      | A secure JWT key          | `"YourSecureJWTKey!12345678YourSecureJWTKey!12345678"` |

### Steps to Apply Configuration:

1. **Create or update `appsettings.json`** in your project folder.
2. **Replace placeholders** with your actual values.

After that your API will be configured correctly.
