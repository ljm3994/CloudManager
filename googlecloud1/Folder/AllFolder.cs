using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using googlecloud1.Files;
using Nemiro.OAuth;
using System.Windows.Forms;
using googlecloud1.FileUpDown;
namespace googlecloud1.Folder
{
    public interface AllFolder
    {
        Task AddFiles(string id);
        DriveInfo driveinfo { get; set; }
        List<CloudFiles> GetFiles();
        CloudFiles AddFiles(Dictionary<string, object> items);
        string DriveName();
        Task<CloudFiles> CreateFolder(string foldername, string parentid);
        FileUpload upladfile(System.IO.FileStream stream, string path, string parentid, ListViewGroup group);
        void AddFile(CloudFiles file, AllFolder folders);
        void RemoveFile(CloudFiles file);
        CloudFiles GetRootFile();
    }
}
