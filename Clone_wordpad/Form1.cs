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
    public partial class Form1 : Form
    {
        List<string> _FontsName = new List<string>();
        List<float> _FontSize = new List<float>();
        public Form1()
        {
            InitializeComponent();
            InitializeFonts();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            tabControl1.Size = new Size(this.Width, 126);
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

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void FontColorbutton_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    //RichTextBoxEditor.SelectionColor = colorDialog.Color;
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
                    //RichTextBoxEditor.SelectionBackColor = colorDialog.Color;
                    FontBackColorbutton.FlatAppearance.BorderColor = colorDialog.Color;
                }
            }
        }
    }
}
