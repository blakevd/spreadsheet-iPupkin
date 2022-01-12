using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpreadsheetUtilities;
using SS;

namespace SpreadsheetGUI
{
    public partial class Form1 : Form
    {
        private Spreadsheet ss;
        private bool isRainbowing;
        private Color defaultRowColColor; //default color for the Row/Col markers
        private Thread rainbowThread;
        private Color defaultButtonColor;
        System.Media.SoundPlayer starMusic;

        private string curFilePath;
        private readonly string ssVersion = "ps6";

        public Form1()
        {
            ss = new Spreadsheet(s => true, s => s.ToUpper(), ssVersion);
            ss.SetContentsOfCell("A1", " ");
            curFilePath = "";

            isRainbowing = false;
            try // make music
            {
                starMusic = new System.Media.SoundPlayer(Resource.starMusic);
            }
            catch { };

            InitializeComponent();
            // add events
            spreadsheetPanel1.SelectionChanged += DisplaySelectedCell;
            spreadsheetPanel1.SelectionChanged += DisplayCellValue;
            spreadsheetPanel1.SelectionChanged += DisplayCellContents;
            spreadsheetPanel1.SetSelection(0, 0);
            DisplaySelectedCell(spreadsheetPanel1);
            // rainbow button stuff
            defaultRowColColor = spreadsheetPanel1.BackColor;
            rainbowThread = new Thread(Rainbowize);
            defaultButtonColor = RainbowButton.BackColor;

        }

        /// <summary>
        /// private helper that helps in displaying the selected cell to the text box in the panel
        /// </summary>
        private void DisplaySelectedCell(SpreadsheetPanel sp)
        {
            int row, col;
            String Name;
            sp.GetSelection(out col, out row);
            Name = Convert.ToChar(col + 65) + "" + (row + 1);
            CurrentSelectedCellName.Text = Name;
            InputCellContents.Focus();
        }

        /// <summary>
        /// private helper in displaying the cell value to the text box
        /// </summary>
        private void DisplayCellValue(SpreadsheetPanel sp)
        {
            int row, col;
            object value;
            sp.GetSelection(out col, out row);
            string cellName = Convert.ToChar((col + 65)) + "" + (row + 1);
            value = ss.GetCellValue(cellName);
            if (value is FormulaError)
                value = "Error";
            CurrentSelectedCellValue.Text = value.ToString();
        }

        /// <summary>
        /// private helper method that displays the cell contents in the cell text box
        /// </summary>
        private void DisplayCellContents(SpreadsheetPanel sp)
        {
            int row, col;
            object contents;
            sp.GetSelection(out col, out row);
            string cellName = Convert.ToChar((col + 65)) + "" + (row + 1);
            contents = ss.GetCellContents(cellName);
            if (contents is Formula)
            {
                contents = '=' + contents.ToString();
            }
            InputCellContents.Text = contents.ToString();

        }

        /// <summary>
        /// private helper method to set the cell contents
        /// </summary>
        private void SetCellContents(SpreadsheetPanel sp)
        {
            InputCellContents.Enabled = false;
            int row, col;
            sp.GetSelection(out col, out row);
            string cellName = Convert.ToChar((col + 65)) + "" + (row + 1); // gets the cell lexographically
            IList<string> dependeeList = ss.SetContentsOfCell(cellName, InputCellContents.Text);
            object val = ss.GetCellValue(cellName);
            if (val is FormulaError)
                val = "Error"; // make the message display error if it is an error

            sp.SetValue(col, row, val.ToString());
            DisplayCellValue(sp);

            // recalculate stuff it depends on
            foreach (string cell in dependeeList)
            {
                int dependeeCol = cell[0] - 65;
                int dependeeRow = int.Parse(cell.Substring(1)) - 1;
                object cVal = ss.GetCellValue(cell);
                if (cVal is FormulaError)
                    cVal = "Error";
                sp.SetValue(dependeeCol, dependeeRow, cVal.ToString());
            }

            // re-enable the button
            InputCellContents.Enabled = true;
        }

        /// <summary>
        /// event for when the enter key is pressed in the text box it sets the cell contents or 
        /// has message poop up if it was an error
        /// </summary>
        private void InputCellContents_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                try
                {
                    SetCellContents(spreadsheetPanel1);
                }
                catch (Exception x)
                {
                    if (x is FormulaFormatException || x is CircularException)
                        MessageBox.Show("The text your entered is an Invalid Formula", "Invalid Formula");
                    InputCellContents.Enabled = true;
                }
            }
        }

        /// <summary>
        /// event for creating a new file
        /// </summary>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // This creates a new window in a new thread
            SpreadsheetApplicationContext.getAppContext().RunForm(new Form1());
        }

        /// <summary>
        /// event for closing current file from the button
        /// </summary>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // DEFINATLEY NEED TO ASK IF SAVED CHANGES HERE LATER
            Close(); // closes the program
        }

        /// <summary>
        /// event for displaying the help button contents
        /// </summary>
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Display a help window
            string msg = "How do I use the spreadsheet?\n" +
                        "First, click on the desired cell and then the box labeled 'cell contents' at the top of the window will automatically have you ready to type. " +
                        "Then type anything you like into that cell. Once you have typed something " +
                        "into the cell, press the 'ENTER' key on your keyboard for it to be put into the cell\n" +
                        "If you want to change selections simply click on another cell on the spreadsheet\n\n" +
                        "What can I put in a cell?\n" +
                        "You can put anything, names, numbers, etc. but if you want a formula you must " +
                        "put an '=' as the first thing you type in. For example, =1 + 2 would show 3 after " +
                        "you put it in a cell.\n\n" +
                        "How do I open or save a file?\n" +
                        "To open a new file simply go to the button at the top left of the screen and go to " +
                        "File => New and this will create a new spreadsheet. To save a file simply go to File => Save or File => Save As\n\n" +
                        "What is the Rainbow Button?\n" +
                        "The rainbow button gives the UI a spicy new look that isn't distracting as it only colors the outer region of the spreadsheet, it also plays cool music too!";

            string caption = "Help";
            MessageBox.Show(msg, caption);
        }

        /// <summary>
        /// event for opening a new file from a path
        /// </summary>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ss.Changed == true)
            {
                // ask if they want to save before opening
                DialogResult result = MessageBox.Show("Would you like to save the current file before opening a new one?", "Save before closing?", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    if (curFilePath != "")
                        saveFile(curFilePath);
                    else
                        saveAsFile();
                }
            }
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "sprd files (*.sprd)|*.sprd|All files (*.*)|*.*";
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        curFilePath = openFileDialog.FileName;
                        ss = new Spreadsheet(curFilePath, s => true, s => s.ToUpper(), ssVersion);
                        ss.SetContentsOfCell("A1", " ");
                        CurrentSelectedCellValue.Clear(); // clear the value
                        InputCellContents.Clear(); // clear the input
                        spreadsheetPanel1.Clear(); // clear the display

                        List<string> cells = new List<string>(ss.GetNamesOfAllNonemptyCells());
                        foreach (string cell in cells)
                        {
                            int dependeeCol = cell[0] - 65;
                            int dependeeRow = int.Parse(cell.Substring(1)) - 1;
                            spreadsheetPanel1.SetValue(dependeeCol, dependeeRow, ss.GetCellValue(cell).ToString());
                        }
                    }
                }
            }
            catch (Exception x)
            {

                MessageBox.Show("There was an error opening the file: " + x.Message, "File Open Error");
            }
        }

        /// <summary>
        /// event for the rainbow button effect and sound
        /// </summary>
        private void RainbowButton_Click(object sender, EventArgs e)
        {
            isRainbowing = !isRainbowing;
            if (isRainbowing == true)
            {
                rainbowThread.Start();
                try
                {
                    starMusic.PlayLooping();
                }
                catch { };
            }
            else //isRainbowing = false;
            {
                try
                {
                    starMusic.Stop();
                }
                catch { };
                rainbowThread.Abort();
                rainbowThread = new Thread(Rainbowize);
                spreadsheetPanel1.BackColor = defaultRowColColor;
                RainbowButton.BackColor = defaultButtonColor;
            }
        }
        /// <summary>
        /// event for using the save as button
        /// </summary>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveAsFile();
        }

        /// <summary>
        /// private helper to save as for a file
        /// </summary>
        private void saveAsFile()
        {
            try
            {
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "sprd files (*.sprd)|*.sprd|All files (*.*)|*.*";
                    saveDialog.DefaultExt = ".sprd";
                    saveDialog.RestoreDirectory = true; // restores to previous dir that was selected
                    saveDialog.Title = "Save Current Spreadsheet";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        saveFile(saveDialog.FileName);
                    }
                }
            }
            catch (Exception x)
            {
                MessageBox.Show("There was an error saving the file: " + x.Message, "File Save Error");
            }
        }

        /// <summary>
        /// Private helper that save the current file
        /// </summary>
        private void saveFile(string filePath)
        {
            try
            {
                ss.Save(filePath);
            }
            catch (SpreadsheetReadWriteException x)
            {
                MessageBox.Show("There was an error saving the file: " + x.Message, "File Save Error");
            }
        }

        /// <summary>
        /// Calls when form is closing and sees if the user wants to save beforehand
        /// </summary>
        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            if (ss.Changed == true)
            {
                // ask if they want to save before opening
                DialogResult result = MessageBox.Show("Would you like to save the current file before closing?", "Save before closing?", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    if (curFilePath != "")
                        saveFile(curFilePath);
                    else
                        saveAsFile();
                }
            }

            starMusic.Stop();
            starMusic.Dispose();
            rainbowThread.Abort();
        }

        /// <summary>
        /// Saves the file under current file path otherwise it prompts for them to save as
        /// </summary>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (curFilePath != "")
                saveFile(curFilePath);
            else
                saveAsFile();
        }

        /// <summary>
        /// private helper that rainbows the cell names and numbers on the side for a cool effect
        /// </summary>
        private void Rainbowize()
        {
            Color[] listOfColors = { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet };
            // get evert cell in a row
            int currentColor = 0;

            // make it a color
            // get every cell in another row make it a color
            // wait 1 second
            // move every color down a row bottom row become top color
            while (true)
            {
                if (currentColor == 7)
                {
                    currentColor = 0;
                }
                spreadsheetPanel1.BackColor = listOfColors[currentColor];
                RainbowButton.BackColor = listOfColors[currentColor];
                currentColor++;

                Thread.Sleep(300);
            }
        }
    }
}

