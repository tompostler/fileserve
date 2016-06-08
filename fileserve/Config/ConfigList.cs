namespace Unlimitedinf.Fileserve.Config
{
    using Json;
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal sealed partial class Config
    {
        /// <summary>
        /// Lists all files in the configuration.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public void FileList()
        {
            foreach (File file in this.files)
            {
                Console.WriteLine(Resources.ProgramConfigFileList, file.Id, file.WebPath, file.AbsPath);
            }
        }
    }
}
