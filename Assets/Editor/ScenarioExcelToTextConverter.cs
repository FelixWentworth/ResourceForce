
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Excel;

public class ScenarioExcelToTextConverter  {

    public enum ScenarioSite { Belfast, Groningen, Valencia, Preston, Nicosia }

    /// <summary>
    /// Converts each sheet in the specified excel file to json and saves them in the output folder.
    /// The name of the processed json file will match the name of the excel sheet. Ignores
    /// sheets whose name begin with '~'. Also ignores columns whose names begin with '~'.
    /// </summary>
    /// <returns><c>true</c>, if excel file was successfully converted to json, <c>false</c> otherwise.</returns>
    /// <param name="filePath">File path.</param>
    /// <param name="outputPath">Output path.</param>
    public bool ConvertExcelFileToText(string filePath, string outputPath, ScenarioSite scenarioSite)
    {
        var outputFileName = "ScenarioInformation_" + scenarioSite;

        Console.WriteLine("Excel To Json Converter: Processing: " + filePath);

        var excelData = GetExcelDataSet(filePath);

        if (excelData == null)
        {
            Console.WriteLine("Excel To Json Converter: Failed to process file: " + filePath);
            return false;
        }
        WriteTextToFile(excelData, outputPath + "/" + outputFileName + ".txt");

        return true;
    }
    /// <summary>
	/// Converts all excel files in the input folder to json and saves them in the output folder.
	/// Each sheet within an excel file is saved to a separate json file with the same name as the sheet name.
	/// Files, sheets and columns whose name begin with '~' are ignored.
	/// </summary>
	/// <param name="inputPath">Input path.</param>
	/// <param name="outputPath">Output path.</param>
	/// <param name="site">The site that the data relates to.</param>
	public void ConvertExcelFilesToJson(string inputPath, string outputPath, ScenarioSite site)
    {
        var excelFiles = GetExcelFileNamesInDirectory(inputPath);
        Console.WriteLine("Excel To Json Converter: " + excelFiles.Count + " excel files found.");


        foreach (var file in excelFiles)
        {
            if (!ConvertExcelFileToText(file, outputPath, site))
            {
                break;
            }
        }
    }

    /// <summary>
	/// Gets all the file names in the specified directory
	/// </summary>
	/// <returns>The excel file names in directory.</returns>
	/// <param name="directory">Directory.</param>
	private List<string> GetExcelFileNamesInDirectory(string directory)
    {
        var directoryFiles = Directory.GetFiles(directory);
        var excelFiles = new List<string>();

        // Regular expression to match against 2 excel file types (xls & xlsx), ignoring
        // files with extension .meta and starting with ~$ (temp file created by excel when fie
        var excelRegex = new Regex(@"^((?!(~\$)).*\.(xlsx|xls$))$");

        foreach (var file in directoryFiles)
        {
            var fileName = file.Substring(file.LastIndexOf('/') + 1);

            if (excelRegex.IsMatch(fileName))
            {
                excelFiles.Add(file);
            }
        }

        return excelFiles;
    }


    /// <summary>
	/// Gets the excel data reader for the specified file.
	/// </summary>
	/// <returns>The excel data reader for file or null if file type is invalid.</returns>
	/// <param name="filePath">File path.</param>
	private IExcelDataReader GetExcelDataReaderForFile(string filePath)
    {
        var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);

        // Create the excel data reader
        IExcelDataReader excelReader;

        // Create regular expressions to detect the type of excel file
        var xlsRegex = new Regex(@"^(.*\.(xls$))");
        var xlsxRegex = new Regex(@"^(.*\.(xlsx$))");

        // Read the excel file depending on it's type
        if (xlsRegex.IsMatch(filePath))
        {
            // Reading from a binary Excel file ('97-2003 format; *.xls)
            excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
        }
        else if (xlsxRegex.IsMatch(filePath))
        {
            // Reading from a OpenXml Excel file (2007 format; *.xlsx)
            excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        }
        else
        {
            Console.WriteLine("Excel To Json Converter: Unexpected files type: " + filePath);
            stream.Close();
            return null;
        }

        // Close the stream
        stream.Close();

        // First row are columns names
        excelReader.IsFirstRowAsColumnNames = true;

        return excelReader;
    }
    /// <summary>
	/// Gets the Excel data from the specified file
	/// </summary>
	/// <returns>The excel data set or null if file is invalid.</returns>
	/// <param name="filePath">File path.</param>
	private string GetExcelDataSet(string filePath)
    {
        // Get the excel data reader with the excel data
        var excelReader = GetExcelDataReaderForFile(filePath);

        return excelReader == null ? null : GetExcelSheetData(excelReader);
    }
    /// <summary>
	/// Gets the Excel data from current spreadsheet
	/// </summary>
	/// <returns>The spreadsheet data table.</returns>
	/// <param name="excelReader">Excel Reader.</param>
	private string GetExcelSheetData(IExcelDataReader excelReader)
    {
        if (excelReader == null)
        {
            Console.WriteLine("Excel To Json Converter: Excel Reader is null. Cannot read data");
            return null;
        }

        var allScenarios = "{";

        // Ignore sheets which start with ~
        var sheetNameRegex = new Regex(@"^~.*$");
        if (sheetNameRegex.IsMatch(excelReader.Name))
        {
            return null;
        }

        // Create the table with the spreadsheet name
        var table = new DataTable(excelReader.Name);
        table.Clear();

        var scenarioNum = "";

        var scenarioString = "";
        // Read the rows and columns
        while (excelReader.Read())
        {
            var rowString = "";
            //var rowNumber = 0;
            for (var i = 0; i < excelReader.FieldCount; i++)
            {
                // If the column is null and this is the first row, skip
                // to next iteration (do not want to include empty columns)
                if (excelReader.IsDBNull(i) &&
                    (excelReader.Depth <= 2 || i > table.Columns.Count - 1))
                {
                    continue;
                }
                if (excelReader.Depth <= 2)
                {
                    // first 2 rows of the data are titles
                    continue;
                }
                var value = excelReader.IsDBNull(i) ? "" : excelReader.GetString(i);
                
                // We are not interested in the first column if it matches the current scenario we are reading data for
                if (value != scenarioNum)
                {
                    // New scenario number
                    if (value.Contains("Scenario_"))
                    {
                        if (scenarioString != "")
                        {
                            scenarioString = scenarioString.Remove(scenarioString.Length - 1, 1);
                            scenarioString += "}";
                            // Add the scenario string to the all scenarios string
                            allScenarios += scenarioString + ",";

                        }

                        scenarioNum = value;
                        scenarioString = "\"" + scenarioNum + "\": {";
                    }
                    else
                    {
                        // Scenario data
                        if (rowString == "")
                        {
                            rowString += "\"" + value + "\"" + ": [";
                            //rowNumber = int.Parse(value);
                        }
                        else
                        {
                            //if (i == 2)
                            //{
                            //    // This value is the title of the incident, store as a string
                            //    rowString += "\"" + scenarioNum.ToUpper() + "_TITLE_" + rowNumber + "\""; ;
                            //}
                            //else if (i == 3)
                            //{
                            //    // this string is the localized string, add the key value as a string
                            //    rowString += "\"" + scenarioNum.ToUpper() + "_INDEX_" + rowNumber + "\"";
                            //}
                            //else if (i == 12)
                            //{
                            //    // Wait feedback
                            //    rowString += "\"" + scenarioNum.ToUpper() + "_WAIT_FEEDBACK_" + rowNumber + "\"";
                            //}
                            //else if (i == 14)
                            //{
                            //    // Officer Feedback
                            //    rowString += "\"" + scenarioNum.ToUpper() + "_OFFICER_FEEDBACK_" + rowNumber + "\"";
                            //}
                            //else if (i == 16)
                            //{
                            //    // Citizen Feedback
                            //    rowString += "\"" + scenarioNum.ToUpper() + "_CITIZEN_FEEDBACK_" + rowNumber + "\"";
                            //}
                            //else
                            //{
                                rowString += "\"" + value + "\"";
                            //}
                        }
                        if (!rowString.EndsWith("[") && rowString != "" && i != 16) // index 10 is the last information we require
                        {
                            rowString += ", ";
                        }
                    }
                }
            }
            if (rowString != "")
            {
                rowString += " ]";
                scenarioString += rowString + ",";
            }
        }
        // Make sure to add the final scenario to the string
        scenarioString = scenarioString.Remove(scenarioString.Length - 1, 1);
        scenarioString += "}";
        allScenarios += scenarioString + ",";

        // Remove the last comma from the all scenarios and close the data file
        allScenarios = allScenarios.Remove(allScenarios.Length - 1, 1);
        allScenarios += "}";
        return allScenarios;
    }


    /// <summary>
    /// Writes the specified text to the specified file, overwriting it.
    /// Creates file if it does not exist.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="filePath">File path.</param>
    private void WriteTextToFile(string text, string filePath)
    {
        System.IO.File.WriteAllText(filePath, text);
    }
}
