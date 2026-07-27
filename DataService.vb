
Imports Microsoft.Data.SqlClient

Public Class DataService

    Private myConn As SqlConnection
    Private myCmd As SqlCommand

    Public Sub New()
        myConn = New SqlConnection("Initial Catalog=OPCDataLogger;Data Source=localhost\SQLEXPRESS;Integrated Security=SSPI;TrustServerCertificate=True;")
    End Sub

    ' Appends a new row to the logs table, and updates currentValues with the
    ' latest value/timestamp for the same tag
    Public Sub LogChange(name As String, value As String, timeAccessed As DateTime)
        myCmd = myConn.CreateCommand
        myCmd.CommandText = "INSERT INTO logs (name, value, timeAccessed) VALUES (@name, @value, @time)"
        myCmd.Parameters.AddWithValue("@name", name)
        myCmd.Parameters.AddWithValue("@value", value)
        myCmd.Parameters.AddWithValue("@time", timeAccessed)

        myConn.Open()
        myCmd.ExecuteNonQuery()

        myCmd.CommandText = "UPDATE currentValues SET value = @value, timeAccessed = @time WHERE name = @name"
        myCmd.ExecuteNonQuery()
        myConn.Close()
    End Sub

    Public Function PullData(name As String) As List(Of Tuple(Of DateTime, Double))
        Dim historyData As New List(Of Tuple(Of DateTime, Double))
        Dim currentTime As DateTime = DateTime.Now
        Dim fiveMinutesAgo As DateTime = DateTime.Now.AddMinutes(-1)

        myCmd = myConn.CreateCommand
        myCmd.CommandText = "SELECT * FROM logs WHERE name = @name AND timeAccessed > @cutOffTime ORDER BY timeAccessed"
        myCmd.Parameters.AddWithValue("@name", name)
        myCmd.Parameters.AddWithValue("@cutOffTime", fiveMinutesAgo)

        myConn.Open()
        Dim reader As SqlDataReader = myCmd.ExecuteReader()
        While reader.Read()
            Dim value As Double = reader.GetDouble(reader.GetOrdinal("value"))
            Dim timeAccessed As DateTime = reader.GetDateTime(reader.GetOrdinal("timeAccessed"))
            historyData.Add(New Tuple(Of DateTime, Double)(timeAccessed, value))
        End While
        reader.Close()
        myConn.Close()
        Return historyData
    End Function
End Class
