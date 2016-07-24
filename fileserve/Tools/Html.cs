namespace Unlimitedinf.Fileserve.Tools
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    internal static class Html
    {
        /// <summary>
        /// Convert a list of files to an HTML directory page in an apache-esque manor.
        /// </summary>
        /// <param name="files">Tuples where T1 is the web path of the file and T2 is for the actual file.</param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string FilesToHtml(List<Tuple<string, FileInfo>> files, string path)
        {
            files.Sort();

            StringBuilder html = new StringBuilder();
            html.Append("<!DOCTYPE HTML>\n");
            html.Append("<html>\n");
            html.Append("<head>\n");
            html.Append("\t<title>Index of " + path + "</title>\n");
            html.Append("</head>\n");
            html.Append("<body>\n");
            html.Append("\t<h1>Index of " + path + "</h1>\n");
            html.Append("\t<pre>\n");

            // Things are space separated as follows:
            // Name is 40, shortened to 37 followed by ..> if too long
            // Last modified is 17
            // Size is 5 numbers and 1 unit (e.g. '##1.3G')

            // Header
            html.Append("Name".PadRight(42) + "Last modified".PadRight(19) + "Size\n");
            html.Append("<hr>");

            // Files
            foreach (var file in files)
            {
                string name = Html.getPrettyName(file.Item1);
                html.Append($"<a href=\"{file.Item1}\">{name}</a>{"".PadLeft(40 - name.Length)}  ");
                html.Append($"{file.Item2.LastAccessTime.ToString("dd-MMM-yyyy HH:mm")}  ");
                html.Append($"{Html.getPrettySize(file.Item2.Length).PadLeft(6)}\n");
            }

            html.Append("\t</pre>\n");
            html.Append("</body>\n");
            html.Append("</html>\n");

            return html.ToString();
        }

        /// <summary>
        /// Convert a size in bytes to a base10 pretty representation of the size. Modified from apache based on
        /// personal preference.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private static string getPrettySize(long size)
        {
            if (size == 0)
                return "";
            
            const int bse = 1024;
            if (size < bse)
                return size.ToString("F1");

            int exponent = (int)Math.Log(size, bse);

            return (size / Math.Pow(bse, exponent)).ToString("F1") + " kMGTPE"[exponent];
        }

        /// <summary>
        /// Converts name to apache-style filename string. If name is longer than length, shorten to length-3 and
        /// append '..>' to name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static string getPrettyName(string name, int length = 40)
        {
            if (name.Length <= length)
                return name;
            return name.Substring(0, length - 3) + "..>";
        }
    }
}
