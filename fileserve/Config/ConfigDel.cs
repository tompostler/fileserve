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
        /// Delete a file from the configuration. Cascade deletion through links.
        /// </summary>
        public void FileDel()
        {
            Guid id = Tools.GetA.Guid(Resources.GetAGuid);
            if (id == Guid.Empty)
            {
                Console.WriteLine(Resources.ProgramConfigFileDelFail);
                return;
            }

            int index = this.files.FindIndex((f) => f.Id == id);
            if (index == -1)
            {
                Console.WriteLine(Resources.ErrorGuidNotFound);
                return;
            }

            File file = this.files[index];
            this.files.RemoveAt(index);
            foreach (var link in this.links)
            {
                if (link.Value.Contains(id))
                    link.Value.Remove(id);
            }

            Console.WriteLine(Resources.ProgramConfigFileDel, file.WebPath, file.AbsPath);
        }

        /// <summary>
        /// Delete a link from the configuration.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public Tuple<Guid, Guid> LinkDel(Guid userId, Guid fileId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete a user from the configuration. Cascade deletion through links.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User UserDel(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
