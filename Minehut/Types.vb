Public Class Types
    Public Enum ServerVisibility
        ServerPublic = 1
        ServerPrivate = 0
    End Enum
    Public Enum Serverproperties
        MaxPlayers = 0
        Level_Type = 1
        Level_Name = 2
        generator_settings = 3
        gamemode = 4
        force_gamemode = 5
        pvp = 6
        Spawn_Mobs = 7
        Spawn_Animals = 8
        Allow_Flight = 9
        difficulty = 10
        harcore = 11
        enable_command_block = 12
        announce_player_achievements = 13
        allow_nether = 14
        generate_structures = 15
        resource_pack = 16
    End Enum
    Public Enum ServerPlatform
        All = -1
        Java = 0
        Bedrock = 1
    End Enum
    Public Enum ServerStatus
        SERVICE_OFFLINE = 0
        SERVICE_STARTING = 1
        DOWNLOADING_SERVER = 2
        UNZIPPING_SERVER = 3
        STARTING = 4
        ONLINE = 5
        SAVING = 6
        OFFLINE = 7
    End Enum
    Public Enum PluginState
        Active = 1
        Purchased = 0
        Locked = -1
    End Enum
    Public Class Server
        Public ID As String
        Public PlayerCount As Integer
        Public Online As Boolean
        Public IP As String
        Public Name As String
        Public MOTD As String
        Public MaxPlayers As Integer
        Public Visibility As ServerVisibility
        Public Platform As ServerPlatform
        Public Players() As String
        Public LastOnline As Date
        Public Status As ServerStatus
        Public Updated As Date
        Public lastStatusChange As Date
        Public StartedAt As Date
        Public LastSave As Date
        Public Shared Function FromUncleanType(S As UncleanTypes.Server)
            Dim Serv As New Server With {
                .ID = S._id,
                .IP = S.Name & ".minehut.com",
                .MaxPlayers = S.MaxPlayers,
                .MOTD = S.MOTD,
                .Name = S.Name,
                .Online = S.Online,
                .PlayerCount = S.PlayerCount,
                .Players = S.Players,
                .LastOnline = DateFromUnix(S.LastOnlineAt),
                .LastSave = DateFromUnix(S.LastSave),
                .lastStatusChange = DateFromUnix(S.lastStatusChange),
                .StartedAt = DateFromUnix(S.StartedAt),
                .Updated = DateFromUnix(S.Updated)}
            If S.Visibility Then
                Serv.Visibility = ServerVisibility.ServerPublic
            Else
                Serv.Visibility = ServerVisibility.ServerPrivate
            End If
            If S.Platform.ToLower = "java" Then
                Serv.Platform = ServerPlatform.Java
            Else
                Serv.Platform = ServerPlatform.Bedrock
            End If
            Select Case S.Status
                Case "SERVICE_OFFLINE"
                    Serv.Status = ServerStatus.SERVICE_OFFLINE
                Case "SERVICE_STARTING"
                    Serv.Status = ServerStatus.SERVICE_STARTING
                Case "DOWNLOADING_SERVER"
                    Serv.Status = ServerStatus.DOWNLOADING_SERVER
                Case "UNZIPPING_SERVER"
                    Serv.Status = ServerStatus.UNZIPPING_SERVER
                Case "STARTING"
                    Serv.Status = ServerStatus.STARTING
                Case "ONLINE"
                    Serv.Status = ServerStatus.ONLINE
                Case "SAVING"
                    Serv.Status = ServerStatus.SAVING
                Case "OFFLINE"
                    Serv.Status = ServerStatus.OFFLINE
                Case Else
                    Serv.Status = ServerStatus.SERVICE_OFFLINE
            End Select
            Return Serv
        End Function
    End Class

    Public Class AuthToken
        Public UserID As String
        Public Token As String
        Public SessionID As String
    End Class



    Public Class Plugin
        Public ID As String
        Public Name As String
        Public CreditCost As Long
        Public Platform As ServerPlatform
        Public Description As String
        Public ExtendedDescription As String
        Public Version As String
        Public Disabled As Boolean
        Public Filename As String
        Public ConfigFileName As String
        Public DateCreated As Date
        Public DateUpdated As Date
        Public Shared Function FromUnclean(P As UncleanTypes.Plugin)
            Dim Pl As New Plugin With {
                .ConfigFileName = P.config_file_name,
                .CreditCost = P.credits,
                .DateCreated = DateFromUnix(P.created),
                .DateUpdated = DateFromUnix(P.last_updated),
                .Description = P.desc,
                .Disabled = P.disabled,
                .ExtendedDescription = P.desc_extended,
                .Filename = P.file_name,
                .ID = P._id,
                .Name = P.name,
                .Version = P.version}
            Select Case P.platform.ToLower
                Case "java"
                    Pl.Platform = ServerPlatform.Java
                Case "bedrock"
                    Pl.Platform = ServerPlatform.Bedrock
                Case Else
                    Pl.Platform = ServerPlatform.All
            End Select
            Return Pl
        End Function
    End Class

    Public Class InstalledPlugin
        Public State As PluginState
        Public ID As String
        Public Name As String
        Public CreditCost As Long
        Public Platform As ServerPlatform
        Public Descritpion As String
        Public ExtendedDescription As String
        Public Version As String
        Public Disabled As Boolean
        Public FileName As String
        Public ConfigFileName As String
        Public DateCreated As Date
        Public DateUpdated As Date
        Public Shared Function FromUnclean(I As UncleanTypes.InstalledPlugin)
            Dim clean As New InstalledPlugin With {
                .ConfigFileName = I.config_file_name,
                .CreditCost = I.credits,
                .DateCreated = DateFromUnix(I.created),
                .DateUpdated = DateFromUnix(I.last_updated),
                .Descritpion = I.Descritpion,
                .Disabled = I.disabled,
                .ExtendedDescription = I.desc_extended,
                .FileName = I.file_name,
                .ID = I._id,
                .Name = I.Name,
                .Version = I.version}
            Select Case I.Platform.ToLower
                Case "java"
                    clean.Platform = ServerPlatform.Java
                Case "bedrock"
                    clean.Platform = ServerPlatform.Bedrock
                Case Else
                    clean.Platform = ServerPlatform.All
            End Select
            Select Case I.State
                Case "ACTIVE"
                    clean.State = PluginState.Active
                Case "PURCHASED"
                    clean.State = PluginState.Purchased
                Case "LOCKED"
                    clean.State = PluginState.Locked
            End Select
            Return clean
        End Function

    End Class
    Public Class ExtendedServer
        Public ID As String
        Public OwnerID As String
        Public Name As String
        Public DateCreated As Date
        Public Platform As ServerPlatform
        Public StorageMode As String
        Public Key As String
        Public LastOnline As Date
        Public MOTD As String
        Public Visibility As ServerVisibility
        Public ServerProperties As Dictionary(Of String, String)
        Public Suspended As Boolean
        Public ActivePlugins() As String
        Public PurchasedPlugins() As String
        Public Shared Function FromUnclean(E As UncleanTypes.ExtendedServer)
            Dim ex As New ExtendedServer With {
                .ActivePlugins = E.ActivePlugins,
                .DateCreated = DateFromUnix(E.Creation),
                .ID = E._id,
                .Key = E.Key,
                .LastOnline = DateFromUnix(E.Last_Online),
                .MOTD = E.MOTD,
                .Name = E.Name,
                .OwnerID = E.Owner,
                .PurchasedPlugins = E.purchased_plugins,
                .ServerProperties = E.Server_properties,
                .StorageMode = E.StorageMode,
                .Suspended = E.Suspended}
            If E.Visibility Then
                ex.Visibility = ServerVisibility.ServerPublic
            Else
                ex.Visibility = ServerVisibility.ServerPrivate
            End If
            Select Case E.Platform.ToLower
                Case "java"
                    ex.Platform = ServerPlatform.Java
                Case "bedrock"
                    ex.Platform = ServerPlatform.Bedrock
                Case Else
                    ex.Platform = ServerPlatform.All
            End Select
            Return ex
        End Function
    End Class
    Public Class User
        Public Credits As Long
        Public ID As String
        Public Email As String
        Public EmailVerified As Boolean
        Public EmailSentAt As Long
        Public CreatedAt As Date
        Public Birthday As Date
        Public EmailCode As String
        Public LastPassowrdChange As Date
        Public Token As String
        Public LastLogin As Date
        Public Servers() As String
        Public Shared Function FromUnclean(U As UncleanTypes.User)
            Dim Clean As New User With {
                .CreatedAt = DateFromUnix(U.Created_at),
                .Credits = U.Credits,
                .Email = U.Email,
                .EmailCode = U.Email_Code,
                .EmailSentAt = U.Email_sent_at,
                .EmailVerified = U.Email_Verified,
                .ID = U._id,
                .LastLogin = DateFromUnix(U.Last_Login),
                .LastPassowrdChange = DateFromUnix(U.Last_Password_Change),
                .Servers = U.Servers,
                .Token = U.Token}
            U.Birthday = U.Birthday.Split(" ")(0)
            Clean.Birthday = New Date(U.Birthday.Split("/")(2), U.Birthday.Split("/")(0), U.Birthday.Split("/")(1))
            Return Clean
        End Function
    End Class

    Public Class Status
        Public Online As Boolean
        Public ServiceOnline As Boolean
        Public PlayerCount As Integer
        Public MaxPlayers As Integer
        Public TimeWithNoPlayers As TimeSpan
        Public Players() As String
        Public StartedAt As Date
        Public StoppedAt As Date
        Public Starting As Boolean
        Public Stopping As Boolean
        Public Status As ServerStatus


        Public Shared Function FromUnclean(S As UncleanTypes.Status)
            Dim clean As New Status With {
                .MaxPlayers = S.Max_Players,
                .Online = S.Online,
                .PlayerCount = S.Players.Count,
                .Players = S.Players,
                .ServiceOnline = S.Service_Online,
                .StartedAt = DateFromUnix(S.started_at),
                .Starting = S.starting,
                .StoppedAt = DateFromUnix(S.stopped_at),
                .Stopping = S.stopping,
                .TimeWithNoPlayers = TimeSpan.FromMilliseconds(S.Time_No_Players)}
            Select Case S.status
                Case "SERVICE_OFFLINE"
                    clean.Status = ServerStatus.SERVICE_OFFLINE
                Case "SERVICE_STARTING"
                    clean.Status = ServerStatus.SERVICE_STARTING
                Case "DOWNLOADING_SERVER"
                    clean.Status = ServerStatus.DOWNLOADING_SERVER
                Case "UNZIPPING_SERVER"
                    clean.Status = ServerStatus.UNZIPPING_SERVER
                Case "STARTING"
                    clean.Status = ServerStatus.STARTING
                Case "ONLINE"
                    clean.Status = ServerStatus.ONLINE
                Case "SAVING"
                    clean.Status = ServerStatus.SAVING
                Case "OFFLINE"
                    clean.Status = ServerStatus.OFFLINE
                Case Else
                    clean.Status = ServerStatus.SERVICE_OFFLINE
            End Select
            Return clean
        End Function
    End Class


    Public Class ServerFile
        Public FileName As String
        Public IsDirectory As Boolean
        Public IsProtected As Boolean
        Public Shared Function FromUnclean(S As UncleanTypes.ServerFile)
            Dim newf As New ServerFile With {
                .FileName = S.Name,
                .IsDirectory = S.Directory,
                .IsProtected = S.Blocked}
            Return newf
        End Function
    End Class

End Class
Public Module TimeHelper
    Public Function DateFromUnix(ByVal javaTimeStamp As Double) As DateTime
        Dim dtDateTime As DateTime = New DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)
        dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToLocalTime()
        Return dtDateTime
    End Function
End Module
''' <summary>
''' These types are used within the Minehut Client, are a normally not needed outside of it.
''' </summary>
Public Class UncleanTypes
    Public Class Server
        Public _id As String
        Public PlayerCount As Integer
        Public Online As Boolean
        Public IP As String
        Public Port As Integer
        Public Name As String
        Public MOTD As String
        Public MaxPlayers As Integer
        Public Visibility As Boolean
        Public Platform As String
        Public Players() As String
        Public Starting As Boolean
        Public Stopping As Boolean
        Public LastOnlineAt As Long
        Public Status As String
        Public Updated As Long
        Public lastStatusChange As Long
        Public StartedAt As Long
        Public LastSave As Long
    End Class
    Public Class Plugin
        Public _id As String
        Public name As String
        Public credits As Long
        Public platform As String
        Public desc As String
        Public desc_extended As String
        Public version As String
        Public disabled As String
        Public file_name As String
        Public config_file_name As String
        Public __v As Long
        Public created As Long
        Public last_updated As Long
    End Class
    Public Class InstalledPlugin
        Public State As String
        Public _id As String
        Public Name As String
        Public credits As Long
        Public Platform As String
        Public Descritpion As String
        Public desc_extended As String
        Public version As String
        Public disabled As Boolean
        Public file_name As String
        Public config_file_name As String
        Public __v As String
        Public created As Long
        Public last_updated As Long
    End Class

    Public Class ExtendedServer
        Public _id As String
        Public Owner As String
        Public Name As String
        Public Name_Lower As String
        Public Creation As Long
        Public Platform As String
        Public StorageMode As String
        Public Key As String
        Public __v As Long
        Public Port As Integer
        Public Last_Online As Long
        Public MOTD As String
        Public Visibility As Boolean
        Public Server_properties As Dictionary(Of String, String)
        Public Suspended As Boolean
        Public ActivePlugins() As String
        Public purchased_plugins() As String
    End Class
    Public Class AuthData
        Public _id As String
        Public Token As String
        Public SessionID As String
        Public Servers() As String
    End Class
    Public Class User
        Public Credits As Long
        Public _id As String
        Public Email As String
        Public Email_Verified As Boolean
        Public Email_sent_at As Long
        Public Created_at As Long
        Public Birthday As String
        Public __v As Long
        Public Email_Code As String
        Public Last_Password_Change As Long
        Public Token As String
        Public Last_Login As Long
        Public Servers() As String
    End Class


    Public Class Status
        Public Online As Boolean
        Public Service_Online As Boolean
        Public Play_Count As Integer
        Public Max_Players As Integer
        Public Server_ip As String
        Public Server_port As Integer
        Public Time_No_Players As Integer
        Public Players() As String
        Public started_at As Long
        Public stopped_at As Long
        Public starting As Boolean
        Public stopping As Boolean
        Public status As String
    End Class

    Public Class ServerFile
        Public Name As String
        Public Directory As Boolean
        Public Blocked As Boolean
    End Class
End Class
