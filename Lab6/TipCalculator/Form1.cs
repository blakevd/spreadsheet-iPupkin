using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TipCalculator
{
    public partial class Form1 : Form
    {
        private double tip;
        private double bill;
        private bool tipBoxActive = false;
        private bool billBoxActive = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void changeTotal()
        {
            if (billBoxActive && tipBoxActive)
            {
                double total = bill + (.01 * tip * bill);
                resultTextBox.Text = total.ToString();
            }
            else
                resultTextBox.Text = "";
        }

        private void billTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Double.TryParse(billTextBox.Text, out double value))
            {
                billBoxActive = true;
                bill = value;
            }
            else
                billBoxActive = false;

            changeTotal();
        }

        private void tipTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Double.TryParse(tipTextBox.Text, out double value))
            {
                tipBoxActive = true;
                tip = value;
            }
            else
                billBoxActive = false;

            changeTotal();
        }
    }
}
