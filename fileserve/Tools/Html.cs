namespace Unlimitedinf.Fileserve.Tools
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    internal static class Html
    {
        private const int lName = 80;
        private const int lTime = 17;
        private const int lSize = 5;
        private const int lPad = 2;

        /// <summary>
        /// Convert a list of files to an HTML directory page in an apache-esque manor.
        /// </summary>
        /// 
        /// <remarks>
        /// Things in the file list are space separated as follows:
        /// Name is lName, shortened to lName-3 followed by ..> if too long
        /// Last modified is lTime
        /// Size is lSize numbers and 1 unit (e.g. '404.3M')
        /// </remarks>
        /// 
        /// <param name="files">Tuples where T1 is the web path of the file and T2 is for the actual file.</param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string FilesToHtml(List<Tuple<string, FileInfo>> files, string path)
        {
            // Will sort case insensitively on the first part of the tuple (so by filename).
            files.Sort();

            StringBuilder html = new StringBuilder();

            // Open HTML
            html.Append("<!DOCTYPE HTML>\n");
            html.Append("<html>\n");
            html.Append("<head>\n");

            // Title and header: path (aka username)
            html.Append("\t<title>" + path + "</title>\n");
            html.Append("</head>\n");
            html.Append("<body>\n");
            html.Append("\t<h1>Files available for " + path + "</h1>\n");

            //Open file list
            html.Append("\t<pre>\n");
            html.Append("Name".PadRight(lName + lPad) + "Last modified".PadRight(lTime + lPad) + "Size\n");
            html.Append("<hr>");

            // Files
            foreach (var file in files)
            {
                string name = Html.getPrettyName(Uri.UnescapeDataString(file.Item1));
                html.Append($"<a href=\"{file.Item1}\">{name}</a>{"".PadLeft(lName - name.Length)}  ");
                html.Append($"{file.Item2.LastAccessTime.ToString("dd-MMM-yyyy HH:mm")}  ");
                html.Append($"{Html.getPrettySize(file.Item2.Length).PadLeft(lSize + 1)}\n");
            }

            // Close file list
            html.Append("<hr>");
            html.Append("\t</pre>\n");

            // Logout footer
            html.Append("\t<footer>\n");
            html.Append("\t\t<a href=\"logout\">Logout</a>\n");
            html.Append("\t</footer>\n");

            // End HTML
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
        /// <returns></returns>
        private static string getPrettyName(string name)
        {
            if (name.Length <= lName)
                return name;
            return name.Substring(0, lName - 3) + "..>";
        }
    }
}
