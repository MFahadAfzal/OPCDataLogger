Imports System
Imports System.Diagnostics
Imports System.Threading.Tasks
Imports System.Windows
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


    Private Sub CheckedListBox1_SelectedIndexChanged_1(sender As Object, e As EventArgs) Handles CheckedListBox1.SelectedIndexChanged
    End Sub

    Private Sub CheckedListBox1_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles CheckedListBox1.ItemCheck
        Dim name = CheckedListBox1.Items(e.Index)
        Dim newWindow As New HistoryForm(name)
        newWindow.Show()
    End Sub
End Class