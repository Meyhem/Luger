# Luger
Simple, lightweight alternative for centralized logging. 
Provides rich searching capabilities for structures logs while keeping minimal profile.

# First steps
## 1. Choose hosting option
### Windows IIS
Pick [Release](https://github.com/Meyhem/Luger/releases) binary, configure it and start it as IIS site (web.config is included).  
Mind that IIS process must be able to write into StorageDirectory. By default it's **./logs**

### Linux SystemD
Pick [Release](https://github.com/Meyhem/Luger/releases) binary, and run in a unit (write your own unit file // TODO provide in repo).

### Docker
// TODO

## 2. Configure
Luger already preconfigures most values for you in **luger.json**, it's up to you to do the rest.  
This is minimal configuration that you can put into **luger-override.json**.
```json5
{
  "Jwt": {
    "SigningKey": "your-signing-key-here" // <-- Fill in key to be used for JWT sign/verify
  },
  "Luger": {
    "Users": [
      // Create at least one user
      {
        "Id": "admin",
        "Password": "admin",
        // List of buckets this user has access to
        "Buckets": ["project-1"]
      }
    ],
    "Buckets": [
      // Create at least one bucket
      {
        // Bucket identifier (has strong naming policy, only chars "a-z0-9_-")
        "Id": "project-1",
        // Log retention in hours, older are deleted, 0 for infinite retention
        "MaxRetentionHours": 5
      }
    ]
  }
}
```

# Logging providers
**C#** _Microsoft.Extensions.Logging_ [Luger.LoggerProvider](https://github.com/Meyhem/Luger.LoggerProvider)


# Configuration
Luger is configured via json files **luger.json** and **luger-override.json**.  
Preferred place for your configuration is **luger-override.json**.

## Full config structure
```json5
{
  // Listening url for server
  "Urls": "http://0.0.0.0:7931",
  // Default logging options for Luger process, printed to STDOUT
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  // JWT configuration used to authenticate Luger users
  "Jwt": {
    "Audience": "Luger",
    "Issuer": "Luger",
    "ExpiresSeconds": "3600",
    "SigningKey": "your-signing-key-here" // <-- Be sure to provide your own key
  },
  // Luger options
  "Luger": {
    // How often should luger flush logs to files 
    "FlushIntervalSeconds": "5",
    // Path to directory where Luger stores logs
    "StorageDirectory": "./logs",
    // List of users to be allowed to view logs
    "Users": [
      {
        // User
        "Id": "admin",
        // Password
        "Password": "admin",
        // List of buckets this user has access to
        "Buckets": ["project-1"]
      }
    ],
    // List of buckets that Luger will recognize
    "Buckets": [
      {
        // Bucket identifier (has strong naming policy, only chars "a-z0-9_-")
        "Id": "project-1",
        // Log retention in hours, older are deleted, 0 for infinite retention
        "MaxRetentionHours": 5
      }
    ]
  }
}
```

# FAQ
### 1. I'm getting ArgumentException at startup
Mostly caused by invalid configuration, missing user or bucket. Message should guide you. 
