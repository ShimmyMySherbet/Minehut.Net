Imports Minehut
Imports Minehut.Types
Module Module1
    Dim Minehut As New MinehutAPIClient
    Private WithEvents MinehutEventProvider As MinehutEventProvider
    Dim Server1ID As String = ""
    Sub Main()
        Minehut.Login("ShimmyMySherbet2@gmail.com", "Pets5%%%556")
        Console.WriteLine("Logged in.")
        Server1ID = Minehut.GetSelfServers(1)
        Minehut.StartService(Server1ID)



    End Sub

End Module
