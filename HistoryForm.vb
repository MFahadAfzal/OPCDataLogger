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
        Dim histData = dataService.PullData(varName)

        Dim values = histData.Select(Function(p) p.Item1).ToList()
        Dim times = histData.Select(Function(p) p.Item2).ToList()
        FormsPlot1.Plot.Add.Scatter(values, times)
        FormsPlot1.Plot.Axes.DateTimeTicksBottom()

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Dim histData = dataService.PullData(varName)
        Dim values = histData.Select(Function(p) p.Item1).ToList()
        Dim times = histData.Select(Function(p) p.Item2).ToList()
        FormsPlot1.Plot.Clear()
        FormsPlot1.Plot.Add.Scatter(values, times)
        FormsPlot1.Plot.Axes.DateTimeTicksBottom()
        FormsPlot1.Refresh()
    End Sub
End Class