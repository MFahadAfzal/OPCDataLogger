<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class HistoryForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        FormsPlot1 = New ScottPlot.WinForms.FormsPlot()
        tagTitle = New Label()
        SuspendLayout()
        ' 
        ' FormsPlot1
        ' 
        FormsPlot1.Location = New Point(100, 63)
        FormsPlot1.Name = "FormsPlot1"
        FormsPlot1.Size = New Size(626, 375)
        FormsPlot1.TabIndex = 1
        ' 
        ' tagTitle
        ' 
        tagTitle.Font = New Font("Segoe UI", 26.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        tagTitle.Location = New Point(290, 9)
        tagTitle.Name = "tagTitle"
        tagTitle.Size = New Size(242, 51)
        tagTitle.TabIndex = 2
        tagTitle.Text = "Label1"
        ' 
        ' HistoryForm
        ' 
        AutoScaleDimensions = New SizeF(7.0F, 15.0F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Controls.Add(tagTitle)
        Controls.Add(FormsPlot1)
        Name = "HistoryForm"
        Text = "HistoryForm"
        ResumeLayout(False)
    End Sub
    Friend WithEvents FormsPlot1 As ScottPlot.WinForms.FormsPlot
    Friend WithEvents tagTitle As Label
End Class
