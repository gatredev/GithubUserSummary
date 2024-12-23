# Github User Summary
## Description
Application reads github commits for user by repository. Synchronization with github data is performed once per minute
### Documentation

    Synchronized data is stored in local sqlite db file configured by **Default** connection string. 
    Distributed cache is stored in local sqlite db file configured by **Cache** connection string.

    To run this solution you will need:
    1. Have installed .net8 SDK
    2. Call "dotent run <username> <repository>" in project folder to run application
 
