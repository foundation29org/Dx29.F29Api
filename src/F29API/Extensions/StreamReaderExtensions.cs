using System;

namespace System.IO
{
    static public class StreamReaderExtensions
    {
        static public string GoTo(this StreamReader reader, string str, StringComparison comparisonType = StringComparison.Ordinal)
        {
            string line = reader.ReadLine();
            while (line != null)
            {
                if (String.Equals(line, str, StringComparison.Ordinal))
                    break;
                line = reader.ReadLine();
            }
            return line;
        }
    }
}
