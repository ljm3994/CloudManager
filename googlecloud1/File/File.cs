using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.IO;
namespace googlecloud1.Files
{
    public interface CloudFiles
    {
        FileInfo Item { get; set; }
        string GetId();
        bool DoubleClick();
        long GetFileSize();
        Task ChageFileName(string filename);
        Task<bool> FileDelete();
        Task CopyFile(FileInfo parentfile);
    }
}
