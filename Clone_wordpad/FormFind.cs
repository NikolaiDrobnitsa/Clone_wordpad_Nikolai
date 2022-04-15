using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clone_wordpad
{
    public partial class FormFind : Form
    {
        public FormFind()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 frm = (Form1)this.Owner;
            string[] words = textBox1.Text.Split(',');
            foreach (string word in words)
            {
                int startindex = 0;
                while (startindex < frm.RichTextBoxEditor.TextLength)
                {
                    int wordstartIndex = frm.RichTextBoxEditor.Find(word, startindex, RichTextBoxFinds.None);
                    if (wordstartIndex != -1)
                    {
                        frm.RichTextBoxEditor.SelectionStart = wordstartIndex;
                        frm.RichTextBoxEditor.SelectionLength = word.Length;
                        frm.RichTextBoxEditor.SelectionBackColor = Color.Yellow;
                    }
                    else
                        break;
                    startindex += wordstartIndex + word.Length;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 frm = (Form1)this.Owner;
            frm.RichTextBoxEditor.SelectionStart = 0;
            frm.RichTextBoxEditor.SelectAll();
            frm.RichTextBoxEditor.SelectionBackColor = Color.White;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1 frm = (Form1)this.Owner;
            frm.Focus();
            this.Close();
        }
    }
}
