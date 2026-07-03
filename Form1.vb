Imports System
Imports System.Diagnostics
Imports System.Threading.Tasks
Imports Opc.Ua

Public Class Form1

    Private client As New OPCClient(Me)

    Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Await client.Connect()
            Dim tags = Await client.BrowseTags()
            Await client.Subscribe()
        Catch ex As Exception
            Debug.WriteLine("Error: " & ex.Message)
        End Try
    End Sub

    Private Async Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

    End Sub

    Private Sub CheckedListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CheckedListBox1.SelectedIndexChanged

    End Sub

End Class