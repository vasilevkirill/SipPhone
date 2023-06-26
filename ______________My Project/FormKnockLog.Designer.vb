<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Knock
    Inherits System.Windows.Forms.Form

    'Форма переопределяет dispose для очистки списка компонентов.
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

    'Является обязательной для конструктора форм Windows Forms
    Private components As System.ComponentModel.IContainer

    'Примечание: следующая процедура является обязательной для конструктора форм Windows Forms
    'Для ее изменения используйте конструктор форм Windows Form.  
    'Не изменяйте ее в редакторе исходного кода.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        DataGridView1 = New DataGridView()
        Time = New DataGridViewTextBoxColumn()
        Message = New DataGridViewTextBoxColumn()
        CType(DataGridView1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' DataGridView1
        ' 
        DataGridView1.AllowUserToAddRows = False
        DataGridView1.AllowUserToDeleteRows = False
        DataGridView1.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridView1.Columns.AddRange(New DataGridViewColumn() {Time, Message})
        DataGridView1.Location = New Point(12, 12)
        DataGridView1.Name = "DataGridView1"
        DataGridView1.ReadOnly = True
        DataGridView1.RowTemplate.Height = 25
        DataGridView1.Size = New Size(560, 337)
        DataGridView1.TabIndex = 0
        ' 
        ' Time
        ' 
        Time.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
        Time.FillWeight = 5F
        Time.HeaderText = "Time"
        Time.Name = "Time"
        Time.ReadOnly = True
        Time.Width = 58
        ' 
        ' Message
        ' 
        Message.HeaderText = "Message"
        Message.Name = "Message"
        Message.ReadOnly = True
        ' 
        ' Knock
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(584, 361)
        Controls.Add(DataGridView1)
        MaximizeBox = False
        MinimizeBox = False
        MinimumSize = New Size(600, 400)
        Name = "Knock"
        Text = "Логи Кнока (с большой буквы Кнок)"
        CType(DataGridView1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents Time As DataGridViewTextBoxColumn
    Friend WithEvents Message As DataGridViewTextBoxColumn
End Class
