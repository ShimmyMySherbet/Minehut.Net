# Minehut.Net
This project allows for easy intergration to your minehut account/server. It includes all of the features of the minehut backend API, including the ability to make/verify accounts.

This project was written in Visual Basic, but as it is a .NET library, it makes no differance to using it in C#.

# Documentation
The first part you will need to know, the bulk of this library is in the MinehutAPIClient

## MinehutAPIClient
Most of the features of the minehut API will require logging in, so you can access your servers. To do this, use the Login() method. For most of the functions contain within this class, it is recomended to import the Minehut.Types class.
```C#
using Minehut.Types;
```
### Login
This will authenticate the client to the minehut api, allowing for you to work with your servers.
C#
```C#
MinehutApiClient Minehut = New MinehutApiClient();
Minehut.Login("Email@domain.com", "SuperSecretPassword");
```

### GetServers
Using this function, you can access all of the currently running public minehut servers. These servers are return as an IList of the Server type. (Within Minehut.Types)
```C#


```
