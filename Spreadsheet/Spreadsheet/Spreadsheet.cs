// Written by Manya Bajaj
//
// Version 1
// 
// Change log:
//  (Version 1.1) First attempt at PS5. Made changes to SetCellContents. Added cellValue property
//  (Version 1.2) Attempted Reading a file and writing the xml file. Improved SetContentsOfCell
//  (Version 1.3) Fixed ReadXml method to correctly read from a file and return the version when required
//  (Version 1.4) Final Submission

using System.Text.RegularExpressions;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SS
{
    /// <summary>
    /// This class represents a spreadsheet which consists of an infinite
    /// number of named cells. It contains a cell corresponding to every 
    /// possible cell name and a cell consists of contents and a value 
    /// which can be either a string, a double or a Formula.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// Dictionary mapping names to Cell objects to keep track of non-empty cells
        /// </summary>
        private Dictionary<String, Cell> NonEmptyCells;

        /// <summary>
        /// Dependency graph to keep track of the relationships among cells.
        /// </summary>
        private DependencyGraph Graph;

        public override bool Changed { get; protected set; }

        /// <summary>
        /// Creates an empty spreadsheet.
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            // Create a new dictionary to hold cell mappings
            NonEmptyCells = new Dictionary<string, Cell>();
            // Create a new dependency graph
            Graph = new DependencyGraph();
            Changed = false;
        }

        /// <summary>
        /// Creates an empty spreadsheet allowing the user to provide a validity
        /// delegate, a normalization delegate and a version.
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            // Create a new dictionary to hold cell mappings
            NonEmptyCells = new Dictionary<string, Cell>();
            // Create a new dependency graph
            Graph = new DependencyGraph();
            Changed = false;
        }
        /// <summary>
        /// Creates an empty spreadsheet allowing the user to provide a filepath,
        /// a validity delegate, a normalization delegate and a version.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(String filepath, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            // Create a new dictionary to hold cell mappings
            NonEmptyCells = new Dictionary<string, Cell>();
            // Create a new dependency graph
            Graph = new DependencyGraph();

            // the input  version must match the saved version
            if (!GetSavedVersion(filepath).Equals(version))
            {
                throw new SpreadsheetReadWriteException("The version of the provided file does not match provided version");
            }

            // read the input file
            ReadXml(filepath, false);

            Changed = false;

        }

        public override object GetCellContents(string name)
        {
            CheckCellValidity(name);

            name = Normalize(name);

            // check validity of cell after it has been normalzed as well
            CheckCellValidity(name);

            // return cell contents if the cell is non-empty
            if (NonEmptyCells.ContainsKey(name))
                return NonEmptyCells[name].CellContents;

            // if the cell does not exist in the dictionary,
            // it is an empty cell
            return "";
        }

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return NonEmptyCells.Keys;
        }

        protected override IList<string> SetCellContents(string name, double number)
        {
            Cell cell = new Cell(number);

            // Add the cell to the dictionary of non-empty cells
            AddCell(name, cell);

            // The cell no longer has any dependees since it is just a double number
            Graph.ReplaceDependees(name, new HashSet<string>());

            return new List<string>(GetCellsToRecalculate(name));
        }

        protected override IList<string> SetCellContents(string name, string text)
        {
            Cell cell = new Cell(text);

            // add the cell to the dictionary of non-empty cells
            AddCell(name, cell);

            // if the text is an empty cell, remove the added cell 
            if (text == "")
                NonEmptyCells.Remove(name);

            // The cell no longer has any dependees since it is just a text string
            Graph.ReplaceDependees(name, new HashSet<string>());

            return new List<string>(GetCellsToRecalculate(name));
        }

        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            // Store the cells old dependees 
            HashSet<string> oldDependees = new HashSet<string>(Graph.GetDependees(name));

            // Get the dependees of this cell from the variables in the formula
            Graph.ReplaceDependees(name, formula.GetVariables());
            try
            {
                // If a circular dependency is detected here,
                // the code will skip to the Catch block and CircularException will be thrown
                List<string> cellNames = new List<string>(GetCellsToRecalculate(name));

                // Is this the right way to use the lookup delegate ?
                Cell cell = new Cell(formula, Lookup);

                // Add the cell to the dictionary of non-empty cells
                AddCell(name, cell);

                return cellNames;
            }
            catch (CircularException)
            {
                // return to the original state of dependency graph
                Graph.ReplaceDependees(name, oldDependees);
                throw new CircularException();
            }
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return Graph.GetDependents(name);
        }

        public override object GetCellValue(string name)
        {
            CheckCellValidity(name);
            name = Normalize(name);
            // check validity of cell after it has been normalzed as well
            CheckCellValidity(name);

            if (NonEmptyCells.ContainsKey(name))
                return NonEmptyCells[name].CellValue;
            else
                // the cell value of an empty cell is ""
                return "";
        }

        public override IList<string> SetContentsOfCell(string name, string content)
        {
            if (content == null)
                throw new ArgumentNullException();

            CheckCellValidity(name);

            name = Normalize(name);
            // check validity of cell after it has been normalzed as well
            CheckCellValidity(name);

            IList<string> dependents;

            // avoid index out of bounds exception while checking for '=' in a formula
            if (content == "")
            {
                dependents = SetCellContents(name, content);
            }
            // if content is a double
            else if (Double.TryParse(content, out double number))
            {
                dependents = SetCellContents(name, number);
            }
            // if content is a formula (it begins with '=')
            else if (content[0] == '=')
            {
                // FormulaFormatException will be thrown if it doesn't get parsed
                Formula f = new Formula(content.Substring(1, content.Length - 1), Normalize, IsValid);
                // takes care of Circular dependency 
                dependents = SetCellContents(name, f);
            }
            // if content is a text string
            else
            {
                dependents = SetCellContents(name, content);
            }

            // since the contents of a cell have been set, the spreadsheet has changed
            Changed = true;

            // update the cell value of this cell as well as cell values of its dependents 
            foreach (string dependent in dependents)
            {
                // the cell must be non-empty
                if (NonEmptyCells.ContainsKey(dependent))
                {
                    // update the values of this cell and its dependents 
                    NonEmptyCells[dependent].UpdateValues(Lookup);
                }

            }
            return dependents;
        }

        /// <summary>
        /// Helper method used to add a cell to the dictionary of non-empty cells
        /// </summary>
        /// <param name="cellName"></param> the name of the cell to be added
        /// <param name="cell"></param> the cell to be added
        private void AddCell(string cellName, Cell cell)
        {
            // if the cell name already exists in the dictionary,
            // map the name to this new cell whose contents are number
            if (NonEmptyCells.ContainsKey(cellName))
            {
                NonEmptyCells[cellName] = cell;
            }
            // otherwise if it doesn't already exist, add the name->cell
            // mapping to the dictionary
            else
            {
                NonEmptyCells.Add(cellName, cell);
            }
        }

        /// <summary>
        /// Helper method which checks if the input cell name is valid.
        /// A valid cell name begins with a letter or an underscore 
        /// and is followed by one or  more letters, underscores or digits
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool IsLegalCellName(String s)
        {
            // must pass the basic defination first as well as the delegates validity next. 
            return (Regex.IsMatch(s, @"^[A-Za-z]+[0-9]+$"));
        }

        /// <summary>
        /// Helper method used to throw an exception in case of an invalid cell name 
        /// </summary>
        /// <param name="name"></param> cell name
        private void CheckCellValidity(string name)
        {
            if (name == null || !IsLegalCellName(name) || !IsValid(name))
                throw new InvalidNameException();
        }

        public override string GetSavedVersion(string filename)
        {
            return ReadXml(filename, true);
        }

        public override void Save(string filename)
        {
            // throw an exception if file name is either empty or null
            if (filename == null || filename == "")
                throw new SpreadsheetReadWriteException("File name cannot be null or empty");

            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "  ";

                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);

                    foreach (KeyValuePair<string, Cell> cell in NonEmptyCells)
                    {
                        writer.WriteStartElement("cell");
                        writer.WriteElementString("name", cell.Key);

                        string cellContents = "";
                        object contents = cell.Value.CellContents;
                        // if the cell content is a formula 
                        if (contents is Formula)
                        {
                            cellContents = "=" + contents.ToString();
                        }
                        // else if its a string/ double 
                        else
                        {
                            cellContents = contents.ToString();
                        }
                        writer.WriteElementString("contents", cellContents);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }

            Changed = false;
        }
        /// <summary>
        /// Helper method that reads the contents of an XML file representing a Spreadsheet
        /// </summary>
        /// <param name="filename"></param> name of the xml file
        /// <param name="getVersion"></param> true if only the version is required
        /// <returns></returns>
        private string ReadXml(string filename, bool getVersion)
        {
            try
            {
                // create an Xml reader inside this block, and automatically dispose() it at the end
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    string name = null;
                    string contents = null;
                    bool visited = false;

                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "spreadsheet":
                                    if (getVersion)
                                        return reader["version"];

                                    Version = reader["version"];
                                    break;

                                case "cell":
                                    break;

                                case "name":
                                    reader.Read();
                                    name = reader.Value;
                                    break;

                                case "contents":
                                    reader.Read();
                                    visited = true;
                                    contents = reader.Value;
                                    break;

                                // in case of a misspelt element in the file
                                default:
                                    throw new SpreadsheetReadWriteException("Misspelt element in file");
                            }
                        }
                        // set the contents of cell
                        else
                        {
                            if (reader.Name == "contents")
                                SetContentsOfCell(name, contents);
                        }
                    }
                    // make sure the xml file contains the contents tag
                    if (!visited)
                        throw new SpreadsheetReadWriteException("the contents tag is missing");
                }
            }

            // catch any exception thrown and return the appropriate message
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }
            return Version;
        }


        /// <summary>
        /// The method performs the looking up of a cellName and returns its cell value 
        /// </summary>
        /// <param name="cellName"></param>
        /// <returns></returns>
        private double Lookup(string cellName)
        {
            // if the cell exists and is non-empty
            if (NonEmptyCells.ContainsKey(cellName))
            {
                // if the value of the cell is a double 
                if (GetCellValue(cellName) is double)
                    return (double)GetCellValue(cellName);
                else
                    // throw an exception is the value of the cell is not a double 
                    throw new ArgumentException("The value of this cell is not a double");
            }
            // throw an exception if the cell is empty
            else
            {
                throw new ArgumentException("The cell is empty");
            }
        }
    }

    /// <summary>
    /// This class represents a cell which has contents that can be a double, a string or a formula 
    /// </summary>
    class Cell
    {
        /// <summary>
        /// Getter property of a cell to get the cell contents
        /// </summary>
        public object CellContents { get; }

        /// <summary>
        /// Getter property of a cell to get the cell value 
        /// </summary>
        public object CellValue { get; private set; }

        /// <summary>
        /// Creates a new Cell with cell content being a string
        /// </summary>
        /// <param name="contents"></param> contents of the cell
        public Cell(string contents)
        {
            CellContents = contents;
            CellValue = contents;
        }

        /// <summary>
        /// Creates a new Cell with cell content being a double 
        /// </summary>
        /// <param name="contents"></param> contents of the cell 
        public Cell(double contents)
        {
            CellContents = contents;
            CellValue = contents;
        }

        /// <summary>
        /// Creates a new Cell with cell content being a Formula 
        /// </summary>
        /// <param name="contents"></param> contents of the cell
        /// <param name="lookup"></param> lookup delegate
        public Cell(Formula contents, Func<string, double> lookup)
        {
            CellContents = contents;
            CellValue = contents.Evaluate(lookup);
        }
        /// <summary>
        /// This method updates the cell values of this cell 
        /// </summary>
        /// <param name="lookup"></param>
        public void UpdateValues(Func<string, double> lookup)
        {
            if (CellContents is Formula)
            {
                Formula formula = (Formula)CellContents;
                CellValue = formula.Evaluate(lookup);
            }
        }
    }
}