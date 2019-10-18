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


### GetServers()
Using this function, you can access all of the currently running public minehut servers. These servers are return as an IList of the Server type. (Within Minehut.Types)
```C#
IList<Server> servers = Minehut.GetServers();
```

### GetSelfServers()
This retrives the IDs of your servers, these IDs are used to specify what server you want to effect with other functions/methods.
```C#
String[] MyServers = Minehut.GetSelfServers;
```

### GetPlugins()
This fucntion will retreve all plugins avaliable on minehut.
```C#
IList<Plugin> Plugins = Minehut.GetPlugins();
```

### GetServerByID()
This will get extended information about the specifed server. It requires a server ID, and it will return information about the server.
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
Using this function, you can retrive the files stored as part of ypur minecraft server. Each file is stored as a ServerFile Object (In Minehut.Types). These objects contains:
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
This will return all of the installed plugins of the specified server. They return as an Ilist of InstalledPlugin (In Minehut.Types)
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
This returns a AuthToken object (In  Minehut.types). It encapulates the session ID and Access Token required to authenticate to the minehut API. This is useful if you want to save the session ID/token. It's main use is for the MinehutEventprovider. Examples of this will be seen later in this document

NOTE: You need to have logged into minehut before calling this function.

```C#
AuthToken Auth = Minehut.GetAuthToken();
```

### ChangeServerMOTD()
This method changes the specified server's MOTD (the title message seen on the minecraft server's list)

```C#
String[] MyServers = Minehut.GetSelfServers();
String Sevrer1ID = MyServers[0];

Minehut.ChangeServerMOTD(Server1ID, "Welcome to my epic minecraft server!");
```

### ChangeServerName()
This changes the server's name and IP address. It will throw an error if the name is already taken, so I recomend putting it in a try catch.
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
Using this, you can change any of the server's properties. Specifting the server ID, Server Property (using the ServerProperties Enum, stored in Minehut.Types) and the new property value, you can change any of these properties: Allow Flight, Allow Nether, Annouce Player Achievements, Difficulty, Enable Command Blocks, Force Gamemode, Gamemode, Generate Structures, Generator Settings, Hardcore, Level Name, Level Type, Max Players (10 or 20, can go higher if you have a paid server), PVP, Resource Pack (download url), Spawn Animals, and Spawn Mobs.

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
Installs the specified plugin to a minecraft server. For finding the plugin IDs, it is recomended to use the GetPlugins() function.
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
Uninstalls a plugin form a minecraft server. Pretty similar to InstallServerPlugin(), but instead of installing the plugin, it removes it.

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
Repairs all of the minecraft server files, used to fixed broken/corrupt minecraft server files.

```C#
String[] MyServers = Minehut.GetSelfServers();
String Sevrer1ID = MyServers[0];

Minehut.RepairServer(Server1ID);
```





