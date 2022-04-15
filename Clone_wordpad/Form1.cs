using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clone_wordpad
{
    public partial class Form1 : Form
    {
        public string OpenedDocumentPath { get; set; } = "Новый документ"; 
        public bool IsOpened { get; set; } = false; 
        List<string> _FontsName = new List<string>();
        List<float> _FontSize = new List<float>();
        public string DefaultSaveDirectory { get; set; } = "c:\\";
        private bool isunsaved = false;
        public bool IsUnsaved
        {
            get
            {
                return isunsaved;
            }
            set
            {
                isunsaved = value;
                UpdatePath();
            }
        }
        public Form1()
        {
            InitializeComponent();
            InitializeFonts();
        }
        private void InitializeFonts()
        {
            FontFamily[] fontList = new System.Drawing.Text.InstalledFontCollection().Families;
            foreach (var item in fontList)
                _FontsName.Add(item.Name);

            FontSelectorComboBox.DataSource = _FontsName;
            FontSelectorComboBox.SelectedIndex = 10;
            for (int i = 1; i < 50; i++)
                _FontSize.Add(i);
            FontSizeComboBox.DataSource = _FontSize;
            FontSizeComboBox.SelectedIndex = 10;
            //FontSizeComboBox.SelectedValue = 1;

        }
        private int GetFontIndex(string name)
        {
            return _FontsName.IndexOf(name);
        }
        private void RichTextBoxEditor_TextChanged(object sender, EventArgs e)
        {
            IsUnsaved = true;
            if (WordWrapCheckBox.Checked == false)
            {
                RichTextBoxEditor.Focus();
            }
           
        }
        private void MaximizeMinimizeButton(object sender, EventArgs e)
        {
            var buttonText = ((System.Windows.Forms.Button)sender).Text;
            if (buttonText == "_") WindowState = FormWindowState.Minimized;
            else WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
        }

        //menu
        ////private void ForeColorPickerMenuItem_Click(object sender, EventArgs e)
        ////{
        ////    using (ColorDialog colorDialog = new ColorDialog())
        ////    {
        ////        if (colorDialog.ShowDialog() == DialogResult.OK)
        ////            ForeColor = colorDialog.Color;
        ////    }
        ////}
        ////private void BackColorPickerMenuItem_Click(object sender, EventArgs e)
        ////{
        ////    using (ColorDialog colorDialog = new ColorDialog())
        ////    {
        ////        if (colorDialog.ShowDialog() == DialogResult.OK)
        ////            BackColor = colorDialog.Color;
        ////    }
        ////}
        private void SaveMenuButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsOpened) 
                {
                    var dirPath = OpenedDocumentPath.Substring(0, OpenedDocumentPath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                    Directory.CreateDirectory(dirPath); 

                    RichTextBoxEditor.SaveFile(OpenedDocumentPath,
                        OpenedDocumentPath.EndsWith(".rtf") ? RichTextBoxStreamType.RichText : RichTextBoxStreamType.PlainText); 
                }
                else 
                {
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.InitialDirectory = DefaultSaveDirectory;
                        saveFileDialog.Filter = "Текст с форматированием (*.rtf)|*.rtf|Простой текст (*.txt)|*.txt|Все файлы (*.*)|*.*";
                        saveFileDialog.FilterIndex = 1;
                        saveFileDialog.RestoreDirectory = true;

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            var dirPath = saveFileDialog.FileName.Substring(0, saveFileDialog.FileName.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                            Directory.CreateDirectory(dirPath); 

                            RichTextBoxEditor.SaveFile(saveFileDialog.FileName,
                                saveFileDialog.FileName.EndsWith(".rtf") ? RichTextBoxStreamType.RichText : RichTextBoxStreamType.PlainText); 

                            OpenedDocumentPath = saveFileDialog.FileName;
                            IsOpened = true;
                            IsUnsaved = false;
                            UpdatePath();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void WindowDrag(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0); 
            }
        }
        private void CloseWindowButton_Click(object sender, EventArgs e)
        {
            if (IsUnsaved)
            {
                DialogResult savePrompt = MessageBox.Show("Вы хотите сохранить ваши изменения?", "Nikolai_WordPad", MessageBoxButtons.YesNoCancel);

                switch (savePrompt)
                {
                    case DialogResult.Cancel:
                        break;
                    case DialogResult.No:
                        Close();
                        break;
                    case DialogResult.Yes:
                        SaveMenuButton_Click(sender, e);
                        if (!IsUnsaved) Close();
                        break;
                }
            }
            else
            {
                Close();
            }
        }
        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RichTextBoxEditor.CanRedo == true)
                if (RichTextBoxEditor.RedoActionName != "Delete")
                    RichTextBoxEditor.Redo();
        }
        private void CancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBoxEditor.Undo();
        }
        #region Перетаскивание и растягивание окна
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private const int
            HTLEFT = 10,
            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17;

        const int _ = 16;
        Rectangle TopCursor { get { return new Rectangle(0, 0, this.ClientSize.Width, _); } }
        Rectangle LeftCursor { get { return new Rectangle(0, 0, _, this.ClientSize.Height); } }
        Rectangle BottomCursor { get { return new Rectangle(0, this.ClientSize.Height - _, this.ClientSize.Width, _); } }
        Rectangle RightCursor { get { return new Rectangle(this.ClientSize.Width - _, 0, _, this.ClientSize.Height); } }
        Rectangle TopLeft { get { return new Rectangle(0, 0, _, _); } }
        Rectangle TopRight { get { return new Rectangle(this.ClientSize.Width - _, 0, _, _); } }
        Rectangle BottomLeft { get { return new Rectangle(0, this.ClientSize.Height - _, _, _); } }
        Rectangle BottomRight { get { return new Rectangle(this.ClientSize.Width - _, this.ClientSize.Height - _, _, _); } }


        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);

            if (message.Msg == 0x84) // WM_NCHITTEST
            {
                var cursor = this.PointToClient(Cursor.Position);

                if (TopLeft.Contains(cursor)) message.Result = (IntPtr)HTTOPLEFT;
                else if (TopRight.Contains(cursor)) message.Result = (IntPtr)HTTOPRIGHT;
                else if (BottomLeft.Contains(cursor)) message.Result = (IntPtr)HTBOTTOMLEFT;
                else if (BottomRight.Contains(cursor)) message.Result = (IntPtr)HTBOTTOMRIGHT;

                else if (TopCursor.Contains(cursor)) message.Result = (IntPtr)HTTOP;
                else if (LeftCursor.Contains(cursor)) message.Result = (IntPtr)HTLEFT;
                else if (RightCursor.Contains(cursor)) message.Result = (IntPtr)HTRIGHT;
                else if (BottomCursor.Contains(cursor)) message.Result = (IntPtr)HTBOTTOM;
            }
        }
        #endregion
        private void UpdatePath()
        {
            FileNameLabel.Text = $"{(IsUnsaved ? "*" : "")}{OpenedDocumentPath} - Nikolai_WordPad";
        }
        private void RichTextBoxEditor_SelectionChanged(object sender, EventArgs e)
        {
            if (RichTextBoxEditor.SelectionFont != null)
            {
                checkBoxBold.Checked = RichTextBoxEditor.SelectionFont.Bold;
                checkBoxItalic.Checked = RichTextBoxEditor.SelectionFont.Italic;
                checkBoxUnderline.Checked = RichTextBoxEditor.SelectionFont.Underline;
                checkBoxStrikeout.Checked = RichTextBoxEditor.SelectionFont.Strikeout;

                FontSelectorComboBox.SelectedIndex = GetFontIndex(RichTextBoxEditor.SelectionFont.FontFamily.Name);
                FontSizeComboBox.SelectedItem = RichTextBoxEditor.SelectionFont.Size;

                FontColorbutton.FlatAppearance.BorderColor = RichTextBoxEditor.SelectionColor;
                FontBackColorbutton.FlatAppearance.BorderColor = RichTextBoxEditor.SelectionBackColor;

            }

        }
        
        

        private void CenterWtireButton_Click(object sender, EventArgs e)
        {
            RichTextBoxEditor.SelectionAlignment = HorizontalAlignment.Center;
            CenterWtireButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(197)))), ((int)(((byte)(197)))));
            LeftWtireButton.BackColor = System.Drawing.Color.Transparent;
            RightWtireButton.BackColor = System.Drawing.Color.Transparent;
            RichTextBoxEditor.Focus();
        }

        private void RightWtireButton_Click(object sender, EventArgs e)
        {

            RichTextBoxEditor.SelectionAlignment = HorizontalAlignment.Right;
            RightWtireButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(197)))), ((int)(((byte)(197)))));
            LeftWtireButton.BackColor = System.Drawing.Color.Transparent;
            CenterWtireButton.BackColor = System.Drawing.Color.Transparent;
            RichTextBoxEditor.Focus();
        }

        private void Pastebutton_Click(object sender, EventArgs e)
        {
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text))
                RichTextBoxEditor.Paste();
        }

        private void Copybutton_Click(object sender, EventArgs e)
        {
            if (RichTextBoxEditor.SelectionLength > 0)
                RichTextBoxEditor.Copy();
        }

        private void SelectAllButton_Click(object sender, EventArgs e)
        {
            RichTextBoxEditor.SelectAll();
            RichTextBoxEditor.SelectionStart = 0;
            RichTextBoxEditor.SelectionLength = RichTextBoxEditor.Text.Length;
            RichTextBoxEditor.Focus();
        }

        private void FontSelectorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RichTextBoxEditor.SelectionFont = new Font
             (
                 FontSelectorComboBox.SelectedItem.ToString(),
                 (float)FontSizeComboBox.SelectedItem,
                 (
                     (checkBoxBold.Checked ? FontStyle.Bold : 0) |
                     (checkBoxItalic.Checked ? FontStyle.Italic : 0) |
                     (checkBoxUnderline.Checked ? FontStyle.Underline : 0) |
                     (checkBoxStrikeout.Checked ? FontStyle.Strikeout : 0)
                 )
             );

        }

        private void plusFontSizeButton_Click(object sender, EventArgs e)
        {
            int number = Int32.Parse(FontSizeComboBox.SelectedItem.ToString());
            int Size_value = ++number;
            string myString = Size_value.ToString();
            
            FontSizeComboBox.Text = myString;
            label1.Text = FontSizeComboBox.SelectedItem.ToString();
            FontSelectorComboBox_SelectedIndexChanged(sender,e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int number = Int32.Parse(FontSizeComboBox.SelectedItem.ToString());
            int Size_value = --number;
            string myString = Size_value.ToString();

            FontSizeComboBox.Text = myString;
            label1.Text = FontSizeComboBox.SelectedItem.ToString();
            FontSelectorComboBox_SelectedIndexChanged(sender, e);
        }

        Form2 Search = new Form2();
        
        private void SearchButton_Click(object sender, EventArgs e)
        {

            Search.ShowDialog();

        }

        private void WordWrapCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (WordWrapCheckBox.Checked == true)
            {
                RichTextBoxEditor.WordWrap = true;
            }
            else
            {
                RichTextBoxEditor.WordWrap = false;
            }
        }

        //public void Procces_Search()
        //{
        //    int start = 0;
        //    int end = RichTextBoxEditor.Text.IndexOf(Search.ReturnData());

        //    label1.Text = Search.ReturnData();

        //    RichTextBoxEditor.SelectAll();
        //    RichTextBoxEditor.SelectionBackColor = Color.White;

        //    while (start < end)
        //    {
        //        RichTextBoxEditor.Find(Search.ReturnData(), start, RichTextBoxEditor.TextLength, RichTextBoxFinds.MatchCase);

        //        RichTextBoxEditor.SelectionBackColor = Color.Yellow;

        //        start = RichTextBoxEditor.Text.IndexOf(Search.ReturnData(), start) + 1;
        //    }
        //}
        private void Boltbutton_Click(object sender, EventArgs e)
        {
            bool check_press_bolt = false;
            if (check_press_bolt == false)
            {
                
            }
        }

        
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            //moveTextBox = this.Size.Width;
            //moveTextBox -= moveTextBox2;
            //moveTextBox = this.RichTextBoxEditor.Location.X;
            //moveTextBox = moveTextBox - this.Size.Width;
            
            
                
            
            //label1.Text = this.Size.Width.ToString();
            tabControl1.Size = new Size(this.Width-8, 126);
            this.RichTextBoxEditor.Size = new System.Drawing.Size(763, this.Size.Height);
            //this.RichTextBoxEditor.Location = new System.Drawing.Point(103 + this.Location.X, 23);
            this.RichTextBoxEditor.Location = new System.Drawing.Point(103 , 23);
        }
        

        private void button2_Click(object sender, EventArgs e)
        {
            if (RichTextBoxEditor.SelectionLength > 0)
                RichTextBoxEditor.Cut();
        }

        private void FontColorbutton_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    RichTextBoxEditor.SelectionColor = colorDialog.Color;
                    FontColorbutton.FlatAppearance.BorderColor = colorDialog.Color;
                }
            }
        }

        private void FontBackColorbutton_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    RichTextBoxEditor.SelectionBackColor = colorDialog.Color;
                    FontBackColorbutton.FlatAppearance.BorderColor = colorDialog.Color;
                }
            }
        }

        private void LeftWtireButton_Click(object sender, EventArgs e)
        {
            LeftWtireButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(197)))), ((int)(((byte)(197)))));
            RichTextBoxEditor.SelectionAlignment = HorizontalAlignment.Left;
            RightWtireButton.BackColor = System.Drawing.Color.Transparent;
            CenterWtireButton.BackColor = System.Drawing.Color.Transparent;
            RichTextBoxEditor.Focus();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RichTextBoxEditor.SelectionLength = 0;
            RichTextBoxEditor.SelectedText = DateTime.Now.ToString();


        }


    }
}
