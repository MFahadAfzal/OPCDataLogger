Public Class HistoryForm
    Dim varName As String
    Public Sub New(tagName As String)
        varName = tagName
        InitializeComponent()
    End Sub

    Private dataService As New DataService()
    Private Async Sub HistoryForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim foundLabel = CType(Me.Controls.Find("tagTitle", True)(0), Label)
        foundLabel.Text = varName
    End Sub

End Class