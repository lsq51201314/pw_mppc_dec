using System;
using System.Windows.Forms;

namespace MppcDec
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string text = TextBox1.Text.Trim();
                if (text.Length > 0)
                {
                    MppcUnpacker mu = new MppcUnpacker();
                    DataStream muds = new DataStream();
                    byte[] mudata = ByteEx.HexToByte(TextBox1.Text);
                    mu.Encode(mudata, 0, mudata.Length, muds);
                    TextBox2.Text = ByteEx.ByteToHex(muds.GetBytes());
                }
                else
                {
                    throw new Exception("原始十六进制数据为空。");
                }
            }
            catch(Exception ex)
            {
                TextBox2.Text = ex.Message;
            }
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = CheckBox1.Checked;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            TextBox1.Text = TextBox2.Text = "";
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            string text = TextBox2.Text.Trim();
            if (text.Length > 0)
            {
                try
                {
                    Clipboard.SetText(text);
                }
                catch
                {
                    MessageBox.Show("复制结果失败！");
                }
            }
        }
    }
}
