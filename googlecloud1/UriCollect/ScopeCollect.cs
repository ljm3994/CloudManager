using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace googlecloud1.UriCollect
{
    class ScopeCollect
    {
        static string[] GoogleScope = {"https://www.googleapis.com/auth/drive.file", "https://www.googleapis.com/auth/drive", "https://www.googleapis.com/auth/drive.apps.readonly", "https://www.googleapis.com/auth/drive.readonly"
                                            , "https://www.googleapis.com/auth/drive.metadata.readonly", "https://www.googleapis.com/auth/drive.metadata", "https://www.googleapis.com/auth/drive.install",
                                            "https://www.googleapis.com/auth/drive.appfolder", "https://www.googleapis.com/auth/drive.scripts", "https://www.googleapis.com/auth/userinfo.profile"};

        public static string[] GoogleScope1
        {
            get { return GoogleScope; }
        }
        static string[] OneDriveScope = { "wl.offline_access", "wl.basic", "wl.signin", "onedrive.readwrite", "wl.skydrive_update", "wl.skydrive" };

        public static string[] OneDriveScope1
        {
            get { return OneDriveScope; }
        }
    }
}
