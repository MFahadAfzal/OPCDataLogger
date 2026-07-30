Imports System
Imports System.Diagnostics
Imports System.Threading.Tasks
Imports System.Windows
Imports Opc.Ua

Public Class Form1

    Private client As New OPCClient(Me)
    Private historyWindows As New Dictionary(Of HistoryForm, Integer)
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
        ''opens new graphs window if the item is checked
        If e.NewValue = CheckState.Checked Then
            Dim name = CheckedListBox1.Items(e.Index)
            Dim newWindow As New HistoryForm(name)
            historyWindows(newWindow) = e.Index
            AddHandler newWindow.FormClosed, AddressOf HandleWindowClose
            newWindow.Show()


        End If


    End Sub

    Private Sub HandleWindowClose(sender As Object, e As EventArgs)
        Dim closedForm As HistoryForm = CType(sender, HistoryForm)
        Dim indexToUncheck As Integer = historyWindows(closedForm)
        CheckedListBox1.SetItemChecked(indexToUncheck, False)
        historyWindows.Remove(closedForm)
    End Sub
End Class