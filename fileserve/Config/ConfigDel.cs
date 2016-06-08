namespace Unlimitedinf.Fileserve.Config
{
    using Json;
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
        /// <param name="id"></param>
        /// <returns></returns>
        public File FileDel(Guid id)
        {
            int index = this.files.FindIndex((f) => f.Id == id);
            if (index == -1)
                return null;

            File file = this.files[index];
            this.files.RemoveAt(index);
            foreach (var link in this.links)
            {
                if (link.Value.Contains(id))
                    link.Value.Remove(id);
            }

            return file;
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
