# Luger
Simple, lightweight alternative for centralized logging. 
Provides rich searching capabilities for structures logs while keeping minimal profile.

![Luger demo image](https://raw.githubusercontent.com/Meyhem/Luger/master/.github/luger_ui.png)

- [First steps](#first-steps)
    * [1. Choose hosting option](#1-choose-hosting-option)
        + [Windows IIS](#windows-iis)
        + [Linux SystemD](#linux-systemd)
        + [Docker](#docker)
        + [Docker compose](#docker-compose)
    * [2. Configure](#2-configure)
    * [3. Run](#3-run)
- [Logging providers](#logging-providers)
- [Full config structure](#full-config-structure)
- [FAQ](#faq)
    + [1. I'm getting ArgumentException at startup](#1-im-getting-argumentexception-at-startup)
    + [2. How do I choose release binary](#2-how-do-i-choose-release-binary)
- [Developer](#developer)
    * [Building container](#building-container)

# First steps
## 1. Choose hosting option
### Windows IIS
1. Have IIS installed with AspNetCoreModuleV2 (Part of ASP.NET Core 5.0 Runtime Windows Hosting Bundle Installer)
2. Pick [Release](https://github.com/Meyhem/Luger/releases) binary (Don't pick single\_file ones, IIS doesn't support them)
3. Create IIS Site
4. Copy content of release binary to created IIS site 
5. Configure Luger (see [Configure](#2-configure) section)
6. Set Access rights for IIS User to be able to write to data folder "StorageDirectory"
![Access rights windows](https://raw.githubusercontent.com/Meyhem/Luger/master/.github/windows_iis_setup.png)
7. Start Site  

Mind that IIS process must be able to write into StorageDirectory. By default it's **./luger-data** in application root.

### Linux SystemD
1. Pick [Release](https://github.com/Meyhem/Luger/releases) binary (preferably luger_single_file_runtime_linux-x64.tar.gz)
2. Unpack (```tar -xvf ...```) the archive to /opt/luger_single_file_runtime_linux-x64
3. Prepare unit file ([example here](https://github.com/Meyhem/Luger/blob/master/luger.service)) and copy it to directory where your SystemD units reside e.g. ```/etc/systemd/system/luger.service```
4. Configure Luger (see [Configure](#2-configure) section)
5. ```systemctl enable luger```
6. ```systemctl start luger```
7. ```systemctl status luger```

### Docker
Single command run (good for looking around), contains preconfigured user and bucket.    
username: _admin_  
password: _admin_
```sh 
docker pull meyhem/luger

docker run -p 7931:7931 --env Luger__Users__0__Id="admin" --env Luger__Users__0__Password="admin" --env Luger__Users__0__Buckets__0="bucket" --env Luger__Buckets__0__Id="bucket" --env Jwt__SigningKey="My secred password for JWT" meyhem/luger
```

### Docker-compose
Download [example compose file](https://github.com/Meyhem/Luger/blob/master/docker-compose.yaml) as docker-compose.yml  
It contains preconfigured user and bucket, if you want to access full JSON configuration see provisioning in [Configure](#2-configure) section.

```sh
docker-compose up
```

## 2. Configure
Luger already preconfigures most values for you in **luger.json**, it's up to you to do the rest.  
This is minimal configuration that you can put into **luger-override.json** to get you started, it contains single admin user and one bucket.
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

**Provisioning config override**  
Luger container accepts optional config override at path **/config/luger-override.json** where you can mount your volume with your config override file.

## 3. Run
If running manually out of docker then based on your chosen release binary, there will be Luger.dll and/or Luger.exe present.
You can run the .exe directly, or run dll with ```dotnet Luger.dll```  
Note that you must configure at least one user and bucket first or Luger will refuse to start (ArgumentException)

# Logging providers
**C#** _Microsoft.Extensions.Logging_ [Luger.LoggerProvider](https://github.com/Meyhem/Luger.LoggerProvider)  
**JS** // planned  
**Java** // planned


# Full config structure
Luger is configured via json files **luger.json** and **luger-override.json**.  
Preferred place for your configuration is **luger-override.json**.
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
    "StorageDirectory": "./luger-data",
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
See [Configure](#2-configure) section

### 2. How do I choose release binary 
- **no_runtime** - Package doesn't contain .net runtime, you must install it on your hosting platform and start Luger via ```dotnet Luger.dll```
- **runtime** - larger package, but contain .net runtime, no need to install any
- **single_file** - package with dependencies packed into single file, should be preferred (might be incompatible with very old systems)
- **win-x64** - only for 64-bit windows
- **linux-x64** - only for 64-bit linux
- **linux-arm** - only for 32-bit arm linux
- **linux-arm64** - only for 64-bit arm linux
- **osx-x64** - only for 64-bit osx

### 3. IIS Troubleshooting
Luger logs it's internal logs to stdout by default. To be able to view them on IIS we need to enable stdout logging.
1. Open web.config present at site root
2. Set ```stdoutLogEnabled="true"```  
![IIS enable logging](https://raw.githubusercontent.com/Meyhem/Luger/master/.github/iis_logs_enable.png)
3. Save
4. Restart APP Pool & Site (or exec ```iisreset``` from Admin CLI)
5. Try to access application from browser again
6. _logs_ folder in site root should be created containing exception info (most likely you screwed up configuration, refer to [Configure](#2-configure) section)

# Developer
## Building container
```sh
docker build -t luger:latest .
docker run -p 7931:7931 luger
docker tag luger meyhem/luger:latest
docker push meyhem/luger:latest
```
