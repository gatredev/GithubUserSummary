# Github User Summary
## Description
Application reads github commits for user by repository. Synchronization with github data is performed once per minute
### Documentation

Synchronized data is stored in local sqlite db file configured by **Default** connection string. 
Distributed cache is stored in local sqlite db file configured by **Cache** connection string.
To exceed github ratelimit fill token under Github:Token. 
[Github tokens documentation](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens#creating-a-fine-grained-personal-access-token)

    To run this solution you will need:
    1. Have installed .net8 SDK
    2. Call "dotent run <username> <repository>" in project folder to run application
 
