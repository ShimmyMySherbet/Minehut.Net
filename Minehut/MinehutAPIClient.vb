Imports System.Net
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.IO
Imports Minehut.UncleanTypes
Public Class MinehutAPIClient
    Public LoggedIn As Boolean = False
    Private ClientAuthData As AuthData = Nothing
    Private UniversalCookieContainer As New CookieContainer

    Public Shared LoginFailedException As New Exception("Username or password was incorrect, or there was an error connecting to minehut.")
    Public Shared NameAlreadyTakenException As New Exception("The specified name is already taken.")
    ''' <summary>
    '''  Gets all the public servers currently available on the minehut platform.
    ''' </summary>
    ''' <returns>Available minehut servers.</returns>
    Public Function GetServers() As IList(Of Types.Server)
        Dim JsonString As String = HTTPGET("https://api.minehut.com/servers")
        Dim Search As JObject = JObject.Parse(JsonString)
        Dim Results As List(Of JToken) = Search("servers").Children.ToList
        Dim FinalResults As New List(Of Types.Server)
        For Each result In Results
            FinalResults.Add(Types.Server.FromUncleanType(result.ToObject(Of Server)))
        Next
        Return FinalResults
    End Function
    ''' <summary>
    ''' Gets the Authorisation Token used to autheticate to the minehut api.
    ''' </summary>
    ''' <returns>Minehut Authorisation Token</returns>
    Public Function GetAuthToken() As Types.AuthToken
        Dim Autht As New Types.AuthToken With {.SessionID = ClientAuthData.SessionID,
            .Token = ClientAuthData.Token,
            .UserID = ClientAuthData._id}
        Return Autht
    End Function
    ''' <summary>
    ''' Sets the Authorisation Token for this client.
    ''' </summary>
    ''' <param name="Token"></param>
    Public Sub SetAuthToken(Token As Types.AuthToken)
        ClientAuthData = New AuthData With {.SessionID = Token.SessionID, .Token = Token.Token, ._id = Token.UserID}
        ClientAuthData.Servers = GetSelfServers()
    End Sub


    ''' <summary>
    ''' Gets extended information about the specified server.
    ''' </summary>
    ''' <param name="ServerID"></param>
    ''' <returns>Extended information about the specifed server</returns>
    Public Function GetServerByID(ServerID As String) As Types.ExtendedServer
        Dim urld As String = $"https://api.minehut.com/server/{ServerID}"
        Dim JsonString As String = HTTPGET(urld)
        Dim Search As JObject = JObject.Parse(JsonString)
        Dim Results As JObject = Search("server")
        Return Types.ExtendedServer.FromUnclean(Results.ToObject(Of ExtendedServer))
    End Function



    ''' <summary>
    ''' Gets extended information of a server by it's name.
    ''' </summary>
    ''' <param name="ServerName"></param>
    ''' <returns>Extended information about the specifed server</returns>
    Public Function GetServerByName(ServerName As String) As Types.ExtendedServer
        Dim urld As String = $"https://api.minehut.com/server/{ServerName}?byName=true"
        Dim JsonString As String = HTTPGET(urld)
        Dim Search As JObject = JObject.Parse(JsonString)
        Dim Results As JObject = Search("server")
        Return Types.ExtendedServer.FromUnclean(Results.ToObject(Of ExtendedServer))

    End Function

    ''' <summary>
    ''' Gets all plugins currently available on the minehut platform.
    ''' </summary>
    ''' <returns>Minehut server plugins</returns>
    Public Function GetPlugins() As IList(Of Types.Plugin)
        Dim JsonString As String = HTTPGET("https://api.minehut.com/plugins_public")
        Dim Search As JObject = JObject.Parse(JsonString)
        Dim Results As List(Of JToken) = Search("all").Children.ToList
        Dim FinalResults As New List(Of Types.Plugin)
        For Each result In Results
            FinalResults.Add(Types.Plugin.FromUnclean(result.ToObject(Of Plugin)))
        Next
        Return FinalResults
    End Function
    ''' <summary>
    ''' Gets all the servers attached to the current account
    ''' </summary>
    ''' <returns>Server IDs</returns>
    Public Function GetSelfServers() As String()
        Dim usr As Types.User = GetUserByID(ClientAuthData._id)
        Return usr.Servers
    End Function
    ''' <summary>
    ''' Logs into the minehut api, allowing for authenticated methods.
    ''' </summary>
    ''' <param name="Email"></param>
    ''' <param name="Password"></param>
    Public Sub Login(Email As String, Password As String)
        Dim QueryJson As String = "{" & $"""email"":""{Email}"",""password"":""{Password}""" & "}"
        Dim ResponceJson As String = ""
        Dim HttpRequest As HttpWebRequest = HttpWebRequest.Create("https://api.minehut.com/users/login")
        HttpRequest.Headers.Add("method", "POST")
        HttpRequest.CookieContainer = UniversalCookieContainer
        HttpRequest.ContentType = "application/json"
        HttpRequest.ContentLength = QueryJson.Length
        HttpRequest.Method = "POST"
        Dim ErrorThrown As Boolean = False
        Try
            Using streamwriter As StreamWriter = New StreamWriter(HttpRequest.GetRequestStream())
                streamwriter.Write(QueryJson)
                streamwriter.Flush()
                streamwriter.Close()
            End Using
            Using streamReader As StreamReader = New StreamReader(HttpRequest.GetResponse.GetResponseStream())
                ResponceJson = streamReader.ReadToEnd
            End Using
        Catch ex As Exception
            ErrorThrown = True
        End Try
        If Not ErrorThrown Then
            If ResponceJson <> "" Then
                Dim Auth As AuthData = JsonConvert.DeserializeObject(Of AuthData)(ResponceJson)

                LoggedIn = True

                ClientAuthData = Auth
            Else
                Throw New Exception("Auth Client data returned nill.")
            End If
        End If
    End Sub


    ''' <summary>
    ''' Creates a new minehut account.
    ''' </summary>
    ''' <param name="Email"></param>
    ''' <param name="DateOfBirth"></param>
    Public Sub SignUp(Email As String, DateOfBirth As Date)
        POST("https://api.minehut.com/users/confirm_email", $"{"{"}""email"":""{Email}"",""birthday"":""{DateToJSON(DateOfBirth)}""{"}"}")
    End Sub

    Private Function DateToJSON(InD As Date) As String
        Return $"{InD.Year}-{InD.Month}={InD.Day}T{InD.Hour}:{InD.Minute}.{InD.Second}1Z"
    End Function

    ''' <summary>
    ''' Confirms the email address of a recently created minehut account.
    ''' </summary>
    ''' <param name="Password"></param>
    ''' <param name="Code"></param>
    Public Sub ConfirmEmail(Password As String, Code As String)
        Dim QueryJson As String = "{" & $"""email_code"":""{Code}"",""password"":""{Password}""" & "}"
        POST("https://api.minehut.com/users/login/confirm_email", QueryJson)
    End Sub

    ''' <summary>
    ''' Gets information about the specified minehut user.
    ''' </summary>
    ''' <param name="User"></param>
    ''' <returns></returns>
    Public Function GetUserByID(User As String) As Types.User
        Dim Res As String = HTTPGET($"https://api.minehut.com/user/{User}")
        Dim Search As JObject = JObject.Parse(Res)
        Return Types.User.FromUnclean(Search("user").ToObject(Of User))
    End Function
    ''' <summary>
    ''' Creates a new Minehut Minecraft server, attached to the currently logged in account.
    ''' </summary>
    ''' <param name="Name"></param>
    ''' <param name="Platform"></param>
    Public Sub CreateServer(Name As String, Platform As Types.ServerPlatform)
        Dim RequestJson As String = "{" & $"""name"":""{Name}"",""platform"":""{Platform}""" & "}"
        POST("https://api.minehut.com/servers/create", RequestJson)
    End Sub
    ''' <summary>
    ''' Gets the status of the specified server.
    ''' </summary>
    ''' <param name="ServerID"></param>
    ''' <returns>Server status</returns>
    Public Function GetServerStatus(ServerID As String) As Types.Status
        Dim Json As String = HTTPGET($"https://api.minehut.com/server/{ServerID}/status")
        Dim Obj As JObject = JObject.Parse(Json)
        Return Types.Status.FromUnclean(Obj.Item("status").ToObject(Of Status))
    End Function

    ''' <summary>
    ''' Starts the minehut service and minecraft server of the specified ID.
    ''' </summary>
    ''' <param name="ServerID"></param>
    Public Sub StartService(ServerID As String)
        POST($"https://api.minehut.com/server/{ServerID}/start_service", "{}")
    End Sub

    ''' <summary>
    ''' Starts the given minehut minecraft server with the given ID if the service is currently running.
    ''' </summary>
    ''' <param name="ServerID"></param>
    Public Sub StartServer(ServerID As String)
        POST($"https://api.minehut.com/server/{ServerID}/start", "{}")
    End Sub

    ''' <summary>
    ''' Saves and shuts down the specifed minehut minecraft server.
    ''' </summary>
    ''' <param name="ServerID"></param>
    Public Sub ShutdownServer(ServerID As String)
        POST($"https://api.minehut.com/server/{ServerID}/shutdown", "{}")
    End Sub

    ''' <summary>
    ''' Saves the server, and shuts down the service.
    ''' </summary>
    ''' <param name="ServerID"></param>
    Public Sub ShutdownService(ServerID As String)
        POST($"https://api.minehut.com/server/{ServerID}/destroy_service", "{}")
    End Sub

    ''' <summary>
    ''' Repairs all server files in case of corruption.
    ''' </summary>
    ''' <param name="ServerID"></param>
    Public Sub RepairServer(ServerID As String)
        POST($"https://api.minehut.com/server/{ServerID}/repair_files", "{}")
    End Sub

    ''' <summary>
    ''' Resets a server to defaults.
    ''' </summary>
    ''' <param name="ServerID"></param>
    Public Sub ResetServer(ServerID As String)
        POST($"https://api.minehut.com/server/{ServerID}/reset_all", "{}")
    End Sub

    ''' <summary>
    ''' Sends the specified command to the specifed server through console.
    ''' </summary>
    ''' <param name="ServerID"></param>
    ''' <param name="Command"></param>
    Public Sub SendCommand(ServerID As String, Command As String)
        POST($"https://api.minehut.com/server/{ServerID}/send_command", $"{"{"}""command"":""{Command}""{"}"}")
    End Sub


    ''' <summary>
    ''' Changes the IP address/name of the specified server.
    ''' </summary>
    ''' <param name="ServerID"></param>
    ''' <param name="Name"></param>
    Public Sub ChangeServerName(ServerID As String, Name As String)
        Try
            Dim data As String = $"{"{"}""name"":""{Name}""{"}"}"
            Dim HttpRequest As HttpWebRequest = HttpWebRequest.Create($"https://api.minehut.com/server/{ServerID}/change_name")
            HttpRequest.Headers.Add("method", "POST")
            HttpRequest.CookieContainer = UniversalCookieContainer
            HttpRequest.ContentType = "application/json"
            HttpRequest.ContentLength = data.Length
            HttpRequest.Method = "POST"
            If Not IsNothing(ClientAuthData) Then
                HttpRequest.Headers.Add("x-session-id", ClientAuthData._id)
                HttpRequest.Headers.Add("authorization", ClientAuthData.Token)
            End If
            Using streamwriter As StreamWriter = New StreamWriter(HttpRequest.GetRequestStream())
                streamwriter.Write(data)
                streamwriter.Flush()
                streamwriter.Close()
            End Using
            Dim responce As HttpWebResponse = DirectCast(HttpRequest.GetResponse(), HttpWebResponse)
            If responce.StatusCode = 400 Then
                Throw NameAlreadyTakenException
            End If
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' Changes the MOTD of the specified server.
    ''' </summary>
    ''' <param name="ServerID"></param>
    ''' <param name="MOTD"></param>
    Public Sub ChangeServerMOTD(ServerID As String, MOTD As String)
        POST($"https://api.minehut.com/server/{ServerID}/change_motd", $"{"{"}""command"":""{MOTD}""{"}"}")
    End Sub

    ''' <summary>
    ''' Sets whether or not the specified server is visible to the public.
    ''' </summary>
    ''' <param name="ServerID"></param>
    ''' <param name="Visible"></param>
    Public Sub ChangeServerVisibility(ServerID As String, Visible As Boolean)
        POST($"https://api.minehut.com/server/{ServerID}/visibility", $"{"{"}""visibility"":""{Visible}""{"}"}")
    End Sub

    ''' <summary>
    ''' Saves the minecraft world of the specified server.
    ''' </summary>
    ''' <param name="ServerID"></param>
    Public Sub SaveServer(ServerID As String)
        POST($"https://api.minehut.com/server/{ServerID}/save", "{}")
    End Sub

    ''' <summary>
    ''' Deletes and regenerates the default minecraft world of the specified server.
    ''' </summary>
    ''' <param name="Serverid"></param>
    Public Sub ResetServerWorld(Serverid As String)
        POST($"https://api.minehut.com/server/{Serverid}/reset_world", "{}")
    End Sub

    ''' <summary>
    ''' Changes the specified server property of the specified server.
    ''' </summary>
    ''' <param name="ServerID"></param>
    ''' <param name="SelectProperty"></param>
    ''' <param name="Value"></param>
    Public Sub ChangeServerproperty(ServerID As String, SelectProperty As Types.Serverproperties, Value As String)
        POST($"https://api.minehut.com/server/{ServerID}/reset_world", $"{"{"}""field"":""{SelectProperty}"", ""value"":""{Value}""{"}"}")
    End Sub

    ''' <summary>
    ''' Gets all the plugins installed on the specified server.
    ''' </summary>
    ''' <param name="ServerID"></param>
    ''' <returns></returns>
    Public Function GetServerPlugins(ServerID As String) As IList(Of Types.InstalledPlugin)
        Dim responce As String = HTTPGET($"https://api.minehut.com/server/{ServerID}/plugins")
        Dim objec As JObject = JObject.Parse(responce)
        Dim Results As New List(Of Types.InstalledPlugin)
        For Each obj In objec.Item("plugins").Children.ToList
            Dim ob As InstalledPlugin = obj.ToObject(Of InstalledPlugin)
            If ob.State <> "PURCHASED" And ob.State <> "LOCKED" Then
                Results.Add(Types.InstalledPlugin.FromUnclean(ob))
            End If
        Next
        Return Results
    End Function

    ''' <summary>
    ''' Installs the specified plugin on the specified server.
    ''' </summary>
    ''' <param name="ServerID"></param>
    ''' <param name="Plugin"></param>
    Public Sub InstallServerPlugin(ServerID As String, Plugin As String)
        POST($"https://api.minehut.com/server/{ServerID}/install_plugin", $"{"{"}""plugin"":""{Plugin}""{"}"}")
    End Sub

    ''' <summary>
    ''' Uninstalls the specified server plugin from the specified server.
    ''' </summary>
    ''' <param name="ServerID"></param>
    ''' <param name="Plugin"></param>
    Public Sub RemoveServerPlugin(ServerID As String, Plugin As String)
        POST($"https://api.minehut.com/server/{ServerID}/remove_plugin", $"{"{"}""plugin"":""{Plugin}""{"}"}")
    End Sub
    ''' <summary>
    ''' Resets the config of the specified server plugin on the specified server.
    ''' </summary>
    ''' <param name="ServerID"></param>
    ''' <param name="Plugin"></param>
    Public Sub ResetServerPlugin(ServerID As String, Plugin As String)
        POST($"https://api.minehut.com/server/{ServerID}/remove_plugin_data", $"{"{"}""plugin"":""{Plugin}""{"}"}")
    End Sub
    ''' <summary>
    ''' NOT IMPLEMENTED. Gets a list of the server files/folders in the specified directory. (or the root directory if no path is specified)
    ''' </summary>
    ''' <param name="ServerID"></param>
    ''' <param name="Path"></param>
    ''' <returns></returns>
    Public Function GetServerFiles(ServerID As String, Optional Path As String = "") As IList(Of Types.ServerFile)
        If Path = "" Then
            Dim resp As String = HTTPGET($"https://api.minehut.com/file/{ServerID}/list/")
            Dim Obj As JObject = JObject.Parse(resp)
            Dim Res As New List(Of Types.ServerFile)
            For Each item In Obj.Item("files").Children.ToList
                Res.Add(Types.ServerFile.FromUnclean(item.ToObject(Of ServerFile)))
            Next
            Return Res
        Else
            Console.WriteLine("path: " & Path)
            Dim resp As String = HTTPGET($"https://api.minehut.com/file/{ServerID}/list/{Path}")
            Dim Obj As JObject = JObject.Parse(resp)
            Dim Res As New List(Of Types.ServerFile)
            For Each item In Obj.Item("files").Children.ToList
                Res.Add(Types.ServerFile.FromUnclean(item.ToObject(Of ServerFile)))
            Next
            Return Res
        End If
    End Function

    ''' <summary>
    ''' Updates/Overwrites/Creates the specified server file.
    ''' </summary>
    ''' <param name="ServerID"></param>
    ''' <param name="FilePath"></param>
    ''' <param name="FileContents"></param>
    Public Sub UpdateServerFile(ServerID As String, FilePath As String, FileContents As String)
        POST($"https://api.minehut.com/server/{ServerID}/edit/{FilePath}")
    End Sub
    ''' <summary>
    ''' Deletes the specified server file.
    ''' </summary>
    ''' <param name="ServerID"></param>
    ''' <param name="FilePath"></param>
    Public Sub DeleteServerFile(ServerID As String, FilePath As String)
        POST($"https://api.minehut.com/file/{ServerID}/delete/{FilePath}")
    End Sub
    ''' <summary>
    ''' Creates a folder of the specified path.
    ''' </summary>
    ''' <param name="ServerID"></param>
    ''' <param name="FolderName"></param>
    ''' <param name="FolderPath"></param>
    Public Sub CreateServerFolder(ServerID As String, FolderName As String, FolderPath As String)
        POST($"https://api.minehut.com/file/{ServerID}/folder/create", $"{"{"}""name"":""{FolderName}"",""directory"":""{FolderPath}""{"}"}")
    End Sub

    Private Function POST(Uri As String, Optional Data As String = "{}") As String
        Dim HttpRequest As HttpWebRequest = HttpWebRequest.Create(Uri)
        HttpRequest.Headers.Add("method", "POST")
        HttpRequest.CookieContainer = UniversalCookieContainer
        HttpRequest.ContentType = "application/json"
        HttpRequest.ContentLength = Data.Length
        HttpRequest.Method = "POST"

        If Not IsNothing(ClientAuthData) Then
            HttpRequest.Headers.Add("x-session-id", ClientAuthData.SessionID)
            HttpRequest.Headers.Add("Authorization", ClientAuthData.Token)
        End If

        Using streamwriter As StreamWriter = New StreamWriter(HttpRequest.GetRequestStream())
            streamwriter.Write(Data)
            streamwriter.Flush()
            streamwriter.Close()
        End Using
        Using streamReader As StreamReader = New StreamReader(HttpRequest.GetResponse.GetResponseStream())
            Return streamReader.ReadToEnd
        End Using
    End Function

    Private Function HTTPGET(Uri As String) As String
        Dim HttpRequest As HttpWebRequest = HttpWebRequest.Create(Uri)
        HttpRequest.CookieContainer = UniversalCookieContainer
        HttpRequest.Method = "GET"
        If Not IsNothing(ClientAuthData) Then
            HttpRequest.Headers.Add("x-session-id", ClientAuthData.SessionID)
            HttpRequest.Headers.Add("Authorization", ClientAuthData.Token)
        End If
        Using streamReader As StreamReader = New StreamReader(HttpRequest.GetResponse.GetResponseStream())
            Return streamReader.ReadToEnd
        End Using
    End Function









End Class