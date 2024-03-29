# Minehut.Net
This project allows for easy integration to your minehut account/server. It includes all the features of the minehut backend API, including the ability to make/verify accounts.

This project was written in Visual Basic, but as it is a .NET library, it makes no difference to using it in C#.

# Documentation
The first part you will need to know, the bulk of this library is in the MinehutAPIClient

## MinehutAPIClient
Most of the features of the minehut API will require logging in, so you can access your servers. To do this, use the Login() method. For most of the functions contain within this class, it is recommended to import the Minehut.Types class.
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


### GetServers()
Using this function, you can access all of the currently running public minehut servers. These servers are return as an IList of the Server type. (Within Minehut.Types)
```C#
IList<Server> servers = Minehut.GetServers();
```

### GetSelfServers()
This retrieves the IDs of your servers, these IDs are used to specify what server you want to effect with other functions/methods.
```C#
String[] MyServers = Minehut.GetSelfServers;
```

### GetPlugins()
This function will retrieve all plugins available on minehut.
```C#
IList<Plugin> Plugins = Minehut.GetPlugins();
```

### GetServerByID()
This will get extended information about the specified server. It requires a server ID, and it will return information about the server.
```C#
String[] MyServers = Minehut.GetSelfServers();
String Sevrer1ID = MyServers[0];

ExtendedServer ExtendedServerInformation = Minehut.GetServerByID(Server1ID);
```

### GetServerByName()
This function will return the save information as GetServerByID(), but this uses the server name instead of the server ID.
```C#
ExtendedServer ExtendedServerInformation = Minehut.GetServerByID("Skyblock");
```

### GetServerFiles()
Using this function, you can retrieve the files stored as part of your Minecraft server. Each file is stored as a ServerFile Object (In Minehut.Types). These objects contains:
* File Name
* If it's a directory/folder
* If the file/folder is protected

You need to specify the server ID, and optionally you can also specify the path. By default, it will return files stored in the server's root directory.

To get the root directory files:
```C#
String[] MyServers = Minehut.GetSelfServers();
String Sevrer1ID = MyServers[0];

Ilist<ServerFile> RootFiles = Minehut.GetServerFiles(Server1ID);
```

To get the files of the specified folder:
```C#
String[] MyServers = Minehut.GetSelfServers();
String Sevrer1ID = MyServers[0];

Ilist<ServerFile> EssentialsConfigFiles = Minehut.GetServerFiles(Server1ID, "Plugins\Essentials");
```

### GetServerPlugins()
This will return all of the installed plugins of the specified server. They return as an IList of InstalledPlugin (In Minehut.Types)
```C#
String[] MyServers = Minehut.GetSelfServers();
String Sevrer1ID = MyServers[0];

Ilist<InstalledPlugin> InstalledPlugins = Minehut.GetServerPlugins(Server1ID);
```
### GetServerStatus
This returns the status of the given server. Useful to see how many people are online, who is online, the state of the server, ect. Returns as a Status object (In Minehut.Types)
```C#
String[] MyServers = Minehut.GetSelfServers();
String Sevrer1ID = MyServers[0];

Status MyServerStatus = Minehut.GetServerStatus(Server1ID);
```

### GetAuthToken()
This returns a AuthToken object (In  Minehut.types). It encapsulates the session ID and Access Token required to authenticate to the minehut API. This is useful if you want to save the session ID/token. It's main use is for the MinehutEventprovider. Examples of this will be seen later in this document

NOTE: You need to have logged into minehut before calling this function.

```C#
AuthToken Auth = Minehut.GetAuthToken();
```

### ChangeServerMOTD()
This method changes the specified server's MOTD (the title message seen on the Minecraft server's list)

```C#
String[] MyServers = Minehut.GetSelfServers();
String Sevrer1ID = MyServers[0];

Minehut.ChangeServerMOTD(Server1ID, "Welcome to my epic minecraft server!");
```

### ChangeServerName()
This changes the server's name and IP address. It will throw an error if the name is already taken, so I recommend putting it in a try catch.
```C#
String[] MyServers = Minehut.GetSelfServers();
String Sevrer1ID = MyServers[0];

try
{

  Minehut.ChangeServerName(Server1ID, "MySky");
  
  Console.WriteLine("Server name changed!");
}
catch (ArgumentNullException ex)
{
   Console.WriteLine("OOPS! that name is already taken!");
}
```

### ChangeServerProperty()
Using this, you can change any of the server's properties. Specifying the server ID, Server Property (using the ServerProperties Enum, stored in Minehut.Types) and the new property value, you can change any of these properties: Allow Flight, Allow Nether, Announce Player Achievements, Difficulty, Enable Command Blocks, Force Gamemode, Gamemode, Generate Structures, Generator Settings, Hardcore, Level Name, Level Type, Max Players (10 or 20, can go higher if you have a paid server), PVP, Resource Pack (download url), Spawn Animals, and Spawn Mobs.

```C#
String[] MyServers = Minehut.GetSelfServers();
String Sevrer1ID = MyServers[0];

Minehut.ChangeServerProperty(Server1ID, Serverproperties.difficulty, "normal");
Minehut.ChangeServerProperty(Server1ID, Serverproperties.Level_Name, "world1");
Minehut.ChangeServerProperty(Server1ID, Serverproperties.MaxPlayers, 20);
```

### ChangeServerVisibility()
This allows you to make the server Public or Private. Specify the server ID and if it is to be public or private.
True = Public
False = Private

```C#
String[] MyServers = Minehut.GetSelfServers();
String SuperSecretServersID = MyServers[0];

Minehut.ChangeServerVisibility(SuperSecretServersID, False);
```

### InstallServerPlugin()
Installs the specified plugin to a Minecraft server. For finding the plugin IDs, it is recommended to use the GetPlugins() function.
For example, this is the ID for the Essentials plugin: 5a42ba4846246d33fa64c625

```C#
String[] MyServers = Minehut.GetSelfServers();
String Sevrer1ID = MyServers[0];

IList<Plugin> PublicPlugins = Minehut.GetPlugins();
String RandomPluginID = PublicPlugins[50].ID;

//installs a the random plugin
Minehut.InstallServerPlugin(Server1ID, RandomPluginID);

//installs a the Essentials plugin
Minehut.InstallServerPlugin(Server1ID, "5a42ba4846246d33fa64c625");
```

### RemoveServerPlugin()
Uninstalls a plugin form a Minecraft server. Pretty similar to InstallServerPlugin(), but instead of installing the plugin, it removes it.

```C#
String[] MyServers = Minehut.GetSelfServers();
String Sevrer1ID = MyServers[0];

IList<InstalledPlugins> MyInstalledPlugins = Minehut.GetServerPlugins(Server1ID);
String RandomInstalledPluginID = MyInstalledPlugins[4].ID;

//uninstalls a the random plugin
Minehut.RemoveServerPlugin(Server1ID, RandomPluginID);

//uninstalls a the Essentials plugin
Minehut.RemoveServerPlugin(Server1ID, "5a42ba4846246d33fa64c625");
```

### RepairServer()
Repairs all of the Minecraft server files, used to fixed broken/corrupt Minecraft server files.

```C#
String[] MyServers = Minehut.GetSelfServers();
String Sevrer1ID = MyServers[0];

Minehut.RepairServer(Server1ID);
```

### ResetServer()
Deletes all server files of the specified server, resetting it back to factory defaults and deleting all worlds, plugins ect.

NOTE: This CANNOT be undone. The files will be permanently lost, so it is recommended to back up the server files before doing this.
```C#
Minehut.ResetServer(ServerID);
```

### ResetServerPlugin()
This resets the config for the given plugin on the given server. Useful if the config file is completely broken.

```C#
Minehut.ResetServerPlugin(ServerID, PluginID);
```

### ResetServerWorld()
This wipes and regenerates the default server world. Remember, this wipes the server, everything will be lost.

```C#
Minehut.ResetServerWorld(ServerID);
```

### SaveServer()
Saves the server's world. Same as /Save.

```C#
Minehut.SaveServer(ServerID);
```

### SendCommand
This sends the specified command to the server via console. All commands run as server, and therefore with operator privileges.

```C#
minehut.SendCommand(ServerID, "Op ShimmyMySherbet")
minehut.SendCommand(ServerID, "Broadcast Server shutting down in 5 minutes!")
```
### StartService()
Starts the service and server for the specified Minecraft server.

```C#
Minehut.StartService(ServerID);
```

### StartServer()
Starts the Minecraft server. NOTE: Only use this if the service is already online, it's normally best just to use StartService().

```C#
Minehut.StartServer(ServerID);
```

### ShutdownService()
Saves, then shuts down the Minecraft server and it's service. Use this if you don't plan on starting the server up again within the next 5 min.

```C#
Minehut.ShutdownService(ServerID);
```

### ShutdownServer()
Saves, then shuts down the server, but not the service. So you can quickly start the server back up again. Useful for applying property changes or plugin changes.

```C#
Minehut.ShutdownServer(ServerID);
```

### SignUp
Allows you to sign up for a minehut account. It is recommended just to use the minehut website for this though.

```C#
Date DateOfBirth = New Date(2000, 9, 15);

Minehut.SignUp("Email@Domain.com", DateOfBirth);
```
This will trigger minehut to send the verification code to the specified email.

### ConfirmEmail
This allows you to confirm your minehut account using the email code sent to your email address (see above)

```C#
Minehut.ConfirmEmail("SuperSecretPassword124", "EmailverificationCode");

