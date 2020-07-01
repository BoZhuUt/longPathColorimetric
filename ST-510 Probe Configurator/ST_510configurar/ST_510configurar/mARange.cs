using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ST_510configurar
{
    public partial class mARange : Form
    {
        string unit = "ppb";
        double maxValue = 200;
        public UInt16 mAppb = 200;  //default value,it will be changed in factor_textBox.
        public float mAppbFloat = 0.0F;
        public mARange(string Unit, double MaxValue)
        {
            InitializeComponent();
            unit = Unit;
            maxValue = MaxValue;
        }

        private void mARange_Load(object sender, EventArgs e)
        {
            label2.Text = unit;
            richTextBox1.Text += maxValue.ToString() + " " + unit;
            this.Text = "mA2" + unit;
            setOK_button.Enabled = false;
        }

        private void setOK_button_Click(object sender, EventArgs e)
        {           
                this.DialogResult = DialogResult.OK;
                this.Close();            
        }

        private void cancel_button_Click(object sender, EventArgs e)
        {
            mAppb = 0;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void factor_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar >= 48 && (int)e.KeyChar <= 57) || (int)e.KeyChar == 8) { }//Backspace
            else { e.Handled = true; }
        }

        private void factor_textBox_TextChanged(object sender, EventArgs e)
        {
            UInt16 temp = 0;
            try
            {
                //temp = Convert.ToUInt16(factor_textBox.Text);
                //mAppb = temp;  //public mAppb,form1 can also get mAppb value. 
                mAppbFloat = Convert.ToSingle(factor_textBox.Text);
                mAppb = (UInt16)mAppbFloat;
            }
            catch (OverflowException) { }
            catch (FormatException) { }
            if (temp > 0 && temp <= maxValue) //mARange shouldn't be 0.
            {
                setOK_button.Enabled = true;
            }
            else
            {
                setOK_button.Enabled = false;
            }
        }
    }
}
