using System;
using System.IO;
using System.Linq;

namespace XeroConvertor
{
    public class LloydsTsb
    {
        public static void ConvertFile(string filename)
        {

            // Validation
            if (string.IsNullOrEmpty(filename))
            {
                throw new FileNotFoundException("File name must be supplied");
            }

            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("File does not exist");
            }

            // Get a spangly new file name
            var newFile = Path.Combine(Path.GetDirectoryName(filename),
                           "Converted_" + Path.GetFileName(filename));

            // And process the data
            string[] fileData = File.ReadAllLines(filename);
            File.WriteAllLines(newFile, fileData[0].Contains("!Type") ? QifConvertor(fileData) : CsvConvertor(fileData));
        }

        public static string[] QifConvertor(string[] theFile)
        {

            if (theFile[0] != "!Type:CC")
            {
                throw new FileNotFoundException(
                    "File is not a Lloyds TSB credit card file (data directive '!Type:CC' was not found in the file)");
            }

            var recordBeingProcessed = -1;
            var saveString = "";

            foreach (var record in theFile)
            {
                if (record == "^")
                {
                    // previous record was a transaction - reverse the value
                    saveString = saveString.Substring(1, 1) == "-"
                                     ? saveString.Substring(0, 1) + saveString.Substring(2, saveString.Length - 2)
                                     : saveString.Substring(0, 1) + "-" + saveString.Substring(1, saveString.Length - 1);
                    theFile[recordBeingProcessed] = saveString;
                }
                recordBeingProcessed = recordBeingProcessed + 1;
                saveString = record;
            }
            return theFile;
        }

        public static string[] CsvConvertor(string[] theFile)
        {
            var recordBeingProcessed = 0;

            foreach (var record in theFile)
            {
                // Ignore the first row headers
                if (recordBeingProcessed != 0)
                {
                    var bits = record.Split(',');

                    // bits[4] is the amount field - reverse the value
                    bits[4] = bits[4].Substring(0, 1) == "-"
                        ? bits[4].Substring(1, bits[4].Length - 1)
                        : "-" + bits[4].Substring(0, bits[4].Length);
                    theFile[recordBeingProcessed] = string.Join(",", bits);
                }
                recordBeingProcessed = recordBeingProcessed + 1;
            }
            return theFile;
        }
    }
}
