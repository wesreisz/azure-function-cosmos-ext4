Following these tutorials:
There is alot of noise out there for azure functions and cosmosdb. You must
be using the right extension, sdk, dotnet version for all the pieces to work.
Make sure you're using the 4.0.0 extension. This is the correct information
about how to connect: 
https://devblogs.microsoft.com/cosmosdb/under-the-hood-of-the-new-azure-functions-extension-for-azure-cosmos-db/


You can test is with: curl http://localhost:7071/api/HttpTrigger -d '{"name":"Justin Reisz"}'
