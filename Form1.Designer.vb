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
        Counter = New Label()
        Random = New Label()
        Sawtooth = New Label()
        Sinusoid = New Label()
        Square = New Label()
        Triangle = New Label()
        Constant = New Label()
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
        ' Counter
        ' 
        Counter.Location = New Point(200, 29)
        Counter.Name = "Counter"
        Counter.Size = New Size(158, 37)
        Counter.TabIndex = 2
        Counter.Text = "Counter: "
        ' 
        ' Random
        ' 
        Random.Location = New Point(581, 29)
        Random.Name = "Random"
        Random.Size = New Size(158, 37)
        Random.TabIndex = 3
        Random.Text = "Random: "
        ' 
        ' Sawtooth
        ' 
        Sawtooth.Location = New Point(200, 89)
        Sawtooth.Name = "Sawtooth"
        Sawtooth.Size = New Size(158, 37)
        Sawtooth.TabIndex = 4
        Sawtooth.Text = "Sawtooth: "
        ' 
        ' Sinusoid
        ' 
        Sinusoid.Location = New Point(581, 89)
        Sinusoid.Name = "Sinusoid"
        Sinusoid.Size = New Size(158, 37)
        Sinusoid.TabIndex = 5
        Sinusoid.Text = "Sinusoi: "
        ' 
        ' Square
        ' 
        Square.Location = New Point(200, 143)
        Square.Name = "Square"
        Square.Size = New Size(158, 37)
        Square.TabIndex = 6
        Square.Text = "Square: "
        ' 
        ' Triangle
        ' 
        Triangle.Location = New Point(581, 143)
        Triangle.Name = "Triangle"
        Triangle.Size = New Size(158, 37)
        Triangle.TabIndex = 7
        Triangle.Text = "Triangle: "
        ' 
        ' Constant
        ' 
        Constant.Location = New Point(200, 196)
        Constant.Name = "Constant"
        Constant.Size = New Size(158, 37)
        Constant.TabIndex = 8
        Constant.Text = "Constant: "
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7.0F, 15.0F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Controls.Add(Constant)
        Controls.Add(Triangle)
        Controls.Add(Square)
        Controls.Add(Sinusoid)
        Controls.Add(Sawtooth)
        Controls.Add(Random)
        Controls.Add(Counter)
        Controls.Add(CheckedListBox1)
        Controls.Add(Button1)
        Name = "Form1"
        Text = "Form1"
        ResumeLayout(False)
    End Sub

    Friend WithEvents Button1 As Button
    Friend WithEvents CheckedListBox1 As CheckedListBox
    Friend WithEvents Counter As Label
    Friend WithEvents Random As Label
    Friend WithEvents Sawtooth As Label
    Friend WithEvents Sinusoid As Label
    Friend WithEvents Square As Label
    Friend WithEvents Triangle As Label
    Friend WithEvents Constant As Label

End Class
