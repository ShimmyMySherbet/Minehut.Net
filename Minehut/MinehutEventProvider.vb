Imports Minehut.Types
Public Class MinehutEventProvider
    Implements IDisposable
    Public Refresh As Integer = 100
    Public Event StatusChanged(Server As String, Status As Types.Status)
    Private Minehut As New MinehutAPIClient
    Private ServerStats As New List(Of ServerStatusCouple)
    Private ScanServers() As String
    Private Disposed As Boolean = False
    Private RefreshThread As New Threading.Thread(AddressOf RefreshLoop)
    Public Sub New(Auth As Types.AuthToken, Refresh As Integer)
        Me.Refresh = Refresh
        Minehut.SetAuthToken(Auth)
        ScanServers = Minehut.GetSelfServers()
        RefreshThread.Start()
    End Sub

    Private Sub RefreshLoop()
        Do Until Disposed
            For Each server In ScanServers
                Try
                    Dim stat As Status = Minehut.GetServerStatus(server)
                    Dim PreStat As IEnumerable(Of ServerStatusCouple) = ServerStats.Where(Function(x)
                                                                                              Return x.Server = server
                                                                                          End Function)
                    If PreStat.Count <> 0 Then
                        If stat.Status <> PreStat(0).Status Then
                            RaiseEvent StatusChanged(server, stat)
                            Dim newp As New ServerStatusCouple With {.Server = server, .Status = stat.Status}
                            ServerStats.Remove(PreStat(0))
                            ServerStats.Add(newp)
                        End If
                    Else
                        RaiseEvent StatusChanged(server, stat)
                        Dim newp As New ServerStatusCouple With {.Server = server, .Status = stat.Status}
                        ServerStats.Add(newp)
                    End If
                Catch ex As Exception
                    ScanServers = Minehut.GetSelfServers()
                End Try
            Next
            Threading.Thread.Sleep(Refresh)
        Loop
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        Disposed = True
        Refresh = Nothing
        Minehut = Nothing
    End Sub
    Private Class ServerStatusCouple
        Public Server As String
        Public Status As ServerStatus
    End Class
End Class
