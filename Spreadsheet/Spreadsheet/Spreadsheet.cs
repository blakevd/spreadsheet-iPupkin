using SpreadsheetUtilities;
using System;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        private Dictionary<string, Cell> nonEmptyCells;
        private DependencyGraph cellGraph;

        private Regex validRegexCheck = new Regex(@"^[a-zA-Z]*[0-9]*$");
        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed { get; protected set; }

        /// <summary>
        /// No arguement constructor that sets up the class
        /// </summary>
        public Spreadsheet() : base(s => true, n => n, "default")
        {
            nonEmptyCells = new Dictionary<string, Cell>();
            cellGraph = new DependencyGraph();

        }

        /// <summary>
        /// 3 arguement constructor that sets up the class
        /// </summary>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            nonEmptyCells = new Dictionary<string, Cell>();
            cellGraph = new DependencyGraph();
            Changed = true;
        }

        /// <summary>
        /// 4 arguement constructor that sets up the class using a file path
        /// </summary>
        public Spreadsheet(string filePath, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            nonEmptyCells = new Dictionary<string, Cell>();
            cellGraph = new DependencyGraph();
            makeSpreadsheetFromFile(filePath);
            Changed = true;
        }

        /// <summary>
        /// private helper method that checks if the given string meets the 
        /// requirements to be a valid cell name, returns true if the cell is a valid name
        /// false otherwise
        /// </summary>
        private bool isValidCellName(string name)
        {
            return !(name is null) && validRegexCheck.IsMatch(name);
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            if (isValidCellName(name) && IsValid(name))
            {
                name = Normalize(name);
                if (nonEmptyCells.ContainsKey(name)) // if cell has contents
                    return nonEmptyCells[name].Contents; // return the content value
                else
                    return ""; // otherwise it is just an empty cell
            }
            else
                throw new InvalidNameException();
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            List<string> nonEmptyNames = new List<string>(); // create empty list
            foreach (string name in nonEmptyCells.Keys)
            {
                nonEmptyNames.Add(name); // add all cells that have contents in them 
            }

            return nonEmptyNames;
        }

        /// <summary>
        /// private helper method for setSellContents to get rid of repeating code
        /// sets the cell content and the value
        /// </summary>
        private IList<string> CreateCell(string name, object content)
        {
            List<string> recalculatedList = new List<string>(GetCellsToRecalculate(name));

            if (nonEmptyCells.ContainsKey(name)) // check if cell exists
            {
                if (cellGraph.HasDependents(name)) // if cellGraph had previous reference
                    cellGraph.ReplaceDependents(name, new HashSet<string>()); // replace that reference with nothing

                nonEmptyCells[name].Contents = content; // set new content
                nonEmptyCells[name].Value = content;
            }
            else // otherwise create a new non empty cell
            {
                Cell newCell = new Cell(); // make new cell
                newCell.Contents = content;
                newCell.Value = content;

                nonEmptyCells.Add(name, newCell); // add cell to the nonEmpty list
            }

            Changed = true;
            return recalculatedList;
        }


        /// <summary>
        /// LookUp function for Formula.Evaluate that checks if a variable exists and also recursivley returns
        /// the other varaible if it is a formula
        /// </summary>
        private double LookUp(string name)
        {
            object value = GetCellValue(name);
            return (double)value;
        }

        /// <summary>
        /// private helper method for setSellContents to get rid of repeating code
        /// sets the cell content and the value
        /// </summary>
        private IList<string> CreateFormulaCell(string name, Formula content)
        {
            List<string> recalculatedList = new List<string>(GetCellsToRecalculate(name));

            if (nonEmptyCells.ContainsKey(name)) // check if cell exists
            {
                if (cellGraph.HasDependents(name)) // if cellGraph had previous reference
                    cellGraph.ReplaceDependents(name, new HashSet<string>()); // replace that reference with nothing

                nonEmptyCells[name].Contents = content; // set new content
                nonEmptyCells[name].Value = content.Evaluate(LookUp); // evaluate it
            }
            else // otherwise create a new non empty cell
            {
                Cell newCell = new Cell(); // make new cell
                newCell.Contents = content;
                newCell.Value = content.Evaluate(LookUp); // evaluate it

                nonEmptyCells.Add(name, newCell); // add cell to the nonEmpty list
            }

            Changed = true;
            return recalculatedList;
        }


        /// <summary>
        /// The contents of the named cell becomes number.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, double number)
        {
            return CreateCell(name, number);
        }

        /// <summary>
        /// The contents of the named cell becomes text.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, string text)
        {
            if (text != "")
            {
                return CreateCell(name, text);
            }
            else
                return new List<string>();
        }

        /// <summary>
        /// If changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException, and no change is made to the spreadsheet.
        /// 
        /// Otherwise, the contents of the named cell becomes formula. The method returns a
        /// list consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            // add dependecies to keep track of
            foreach (string t in formula.GetVariables())
            {
                cellGraph.AddDependency(name, t);
            }

            return CreateFormulaCell(name, formula);
        }

        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown,
        ///       and no change is made to the spreadsheet.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a list consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            if (content is null)
                throw new ArgumentNullException();

            if (isValidCellName(name) && IsValid(name))
            {
                name = Normalize(name);

                if (double.TryParse(content, out double parseContent)) // its a number
                {
                    return SetCellContents(name, parseContent);
                }
                else if (content.StartsWith("=")) // its a formula
                {
                    return SetCellContents(name, new Formula(content.Substring(1, content.Length - 1), Normalize, IsValid));
                }
                else // its just text
                {
                    return SetCellContents(name, content);
                }
            }

            throw new InvalidNameException();
        }

        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            List<string> dirDep = new List<string>(cellGraph.GetDependees(name));
            return dirDep;
        }

        /// <summary>
        /// private helper method for the constructor to read from a file and convert its contents
        /// to that of the spreadsheet class
        /// </summary>
        private void makeSpreadsheetFromFile(string filename)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    string cellName = "";
                    string contName = "";

                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "spreadsheet":
                                    Version = reader.GetAttribute("version");
                                    break;
                                case "cell":
                                    break;
                                case "name":
                                    reader.Read();
                                    cellName = reader.Value;
                                    System.Diagnostics.Debug.WriteLine(cellName);
                                    break;
                                case "contents":
                                    reader.Read();
                                    contName = reader.Value;
                                    System.Diagnostics.Debug.WriteLine(contName);
                                    break;
                            }
                        }
                        else
                        {
                            if (reader.Name == "cell") // end of cell
                            {
                                SetContentsOfCell(cellName, contName);
                            }
                        }
                    }
                }
            }
            catch
            {
               throw new SpreadsheetReadWriteException("Could not open or read the file");
            }
        }

        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override string GetSavedVersion(string filename)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            if (reader.Name == "spreadsheet")
                                return reader.GetAttribute("version");
                        }
                    }
                }

                throw new SpreadsheetReadWriteException("Could not find a version in the file");
            }
            catch
            {
               throw new SpreadsheetReadWriteException("Could not open or read from the file");
            }
           
        }

        /// <summary>
        /// private helper method that creates an XML document from all the existing data in 
        /// this spreadsheet class
        /// </summary>
        private void WriteXml(string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            using (XmlWriter writer = XmlWriter.Create(filename, settings))
            {
                writer.WriteStartDocument(); // start the document
                writer.WriteStartElement("spreadsheet"); // <spreadsheet
                writer.WriteAttributeString("version", Version); // version = v>

                foreach (string cell in nonEmptyCells.Keys)
                {
                    writer.WriteStartElement("cell"); // <cell>

                    writer.WriteStartElement("name"); // <name>
                    writer.WriteString(cell); // actual name
                    writer.WriteEndElement(); // </name>

                    writer.WriteStartElement("contents"); // <contents>
                    if (nonEmptyCells[cell].Contents is Formula)
                        writer.WriteString("=" + nonEmptyCells[cell].Contents.ToString());
                    else
                        writer.WriteString(nonEmptyCells[cell].Contents.ToString());
                    writer.WriteEndElement(); // </contents>

                    writer.WriteEndElement(); // </cell>
                }

                writer.WriteEndElement(); // </spreadsheet>
                writer.WriteEndDocument(); // close document
            }
        }

        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>cell name goes here</name>
        /// <contents>cell contents goes here</contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            if (Changed == false)
                throw new SpreadsheetReadWriteException("File could not be opened or properly read from");
            try
            {
                WriteXml(filename);
                Changed = false;
            }
            catch
            {
                throw new SpreadsheetReadWriteException("File could not be opened or properly read from");
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            if (isValidCellName(name) && IsValid(name))
            {
                name = Normalize(name);

                if (nonEmptyCells.ContainsKey(name)) // if cell has contents
                {
                    if (nonEmptyCells[name].Contents is Formula) // if it is a formula
                    {
                        Formula f = (Formula)nonEmptyCells[name].Contents;
                        return f.Evaluate(LookUp); // return the content value
                    }

                    return nonEmptyCells[name].Value; // return the value of the string or double
                }
                else
                    return ""; // otherwise it is just an empty cell
            }
            else
                throw new InvalidNameException();
        }

        /// <summary>
        /// private cell class that holds the contents and value of a cell
        /// </summary>
        class Cell
        {
            private object _contents;
            private object _value;

            public object Contents // Property with get/set
            {
                get => _contents;
                set => _contents = value;
            }

            public object Value
            {
                get => _value;
                set => _value = value;
            }
        }
    }
}