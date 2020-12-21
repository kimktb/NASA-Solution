using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NASA_Project.Interfaces
{
    public interface INASA_FileService :IDisposable
    {
        Task SaveNASAPhoto(string remoteFileIncludingPath, string localFileIncludingPath);
        List<string> ReadDatesFile();
        void CreateRequiredFolders();
    }
}
