using System;


namespace XeroConvertor
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage is XeroConvertor filename where filename is the full file"
                    + "name including the folder path. If the file or folder names contain spaces, "
                    + "then enclose the file name in quotation marks");
                Console.Read();
                return;
            }

            var fileName = args[0];

            LloydsTsb.ConvertFile(fileName);

        }
    }
}
