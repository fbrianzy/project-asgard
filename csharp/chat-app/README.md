# ChatApp (TCP)

A minimal TCP chat application consisting of a Server and a Client.

## Run
Terminal 1:
```bash
cd ChatApp/Server
dotnet run -- 5000
```

Terminal 2 (and more):
```bash
cd ChatApp/Client
dotnet run -- 127.0.0.1 5000
```
Type messages, use `/quit` to exit the client.
