<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Button1 = New Button()
        CheckedListBox1 = New CheckedListBox()
        Label1 = New Label()
        Label2 = New Label()
        Label3 = New Label()
        Label4 = New Label()
        Label5 = New Label()
        Label6 = New Label()
        Label7 = New Label()
        SuspendLayout()
        ' 
        ' Button1
        ' 
        Button1.Location = New Point(12, 384)
        Button1.Name = "Button1"
        Button1.Size = New Size(93, 39)
        Button1.TabIndex = 0
        Button1.Text = "Button1"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' CheckedListBox1
        ' 
        CheckedListBox1.FormattingEnabled = True
        CheckedListBox1.Items.AddRange(New Object() {"Counter", "Random", "Sawtooth", "Sinusoid", "Square", "Triangle", "Constant"})
        CheckedListBox1.Location = New Point(12, 12)
        CheckedListBox1.Name = "CheckedListBox1"
        CheckedListBox1.Size = New Size(141, 130)
        CheckedListBox1.TabIndex = 1
        ' 
        ' Label1
        ' 
        Label1.Location = New Point(200, 29)
        Label1.Name = "Label1"
        Label1.Size = New Size(158, 37)
        Label1.TabIndex = 2
        Label1.Text = "Label1"
        ' 
        ' Label2
        ' 
        Label2.Location = New Point(581, 29)
        Label2.Name = "Label2"
        Label2.Size = New Size(158, 37)
        Label2.TabIndex = 3
        Label2.Text = "Label2"
        ' 
        ' Label3
        ' 
        Label3.Location = New Point(200, 89)
        Label3.Name = "Label3"
        Label3.Size = New Size(158, 37)
        Label3.TabIndex = 4
        Label3.Text = "Label3"
        ' 
        ' Label4
        ' 
        Label4.Location = New Point(581, 89)
        Label4.Name = "Label4"
        Label4.Size = New Size(158, 37)
        Label4.TabIndex = 5
        Label4.Text = "Label4"
        ' 
        ' Label5
        ' 
        Label5.Location = New Point(200, 143)
        Label5.Name = "Label5"
        Label5.Size = New Size(158, 37)
        Label5.TabIndex = 6
        Label5.Text = "Label5"
        ' 
        ' Label6
        ' 
        Label6.Location = New Point(581, 143)
        Label6.Name = "Label6"
        Label6.Size = New Size(158, 37)
        Label6.TabIndex = 7
        Label6.Text = "Label6"
        ' 
        ' Label7
        ' 
        Label7.Location = New Point(200, 196)
        Label7.Name = "Label7"
        Label7.Size = New Size(158, 37)
        Label7.TabIndex = 8
        Label7.Text = "Label7"
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7.0F, 15.0F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Controls.Add(Label7)
        Controls.Add(Label6)
        Controls.Add(Label5)
        Controls.Add(Label4)
        Controls.Add(Label3)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Controls.Add(CheckedListBox1)
        Controls.Add(Button1)
        Name = "Form1"
        Text = "Form1"
        ResumeLayout(False)
    End Sub

    Friend WithEvents Button1 As Button
    Friend WithEvents CheckedListBox1 As CheckedListBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label7 As Label

End Class
