using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using googlecloud1.Files;
using googlecloud1.login;
using Nemiro.OAuth;
using System.Windows.Forms;
namespace googlecloud1.Folder
{
    class GoogleFolder : AllFolder
    {
        public List<CloudFiles> GoogleFiles {get; set;}
        public GoogleFile gfile { get; set; }
        List<FileInfo> file = new List<FileInfo>();
        public DriveInfo driveinfo { get; set; }
        public GoogleFolder(DriveInfo driveinfo)
        {
            this.driveinfo = driveinfo;;
            GoogleFiles = new List<CloudFiles>();
        }
        public async Task AddFiles(string id)
        {
            try
            {
                GoogleFiles.Clear();
                string query = "'" + id + "' in parents";
                var parameter = new HttpParameterCollection
                {
                    {"q", query},
                    {"access_token", driveinfo.token.access_token}
                };
                var result = OAuthUtility.Get("https://www.googleapis.com/drive/v2/files", parameter);
                var map = new ApiDataMapping();
                map.Add("id", "FileID");
                map.Add("title", "FileName");
                map.Add("fileExtension", "Extention");
                map.Add("downloadUrl", "DownUrl");
                map.Add("thumbnailLink", "Thumnail");
                map.Add("fileSize", "FileSize", typeof(long));
                map.Add("description", "Description");
                map.Add("modifiedDate", "modifiedDate");
                foreach (var item in result.CollectionItems.Items["items"].CollectionItems.Items.Values)
                {
                    FileInfo fi = new FileInfo(item, map);
                    fi.driveinfo = driveinfo;
                    if(id == "root")
                    {
                        fi.Path = "Google/";
                    }
                    if(fi.Extention != null)
                    {
                        fi.DownUrl = "https://www.googleapis.com/drive/v2/files/" + fi.FileID + "?alt=media";
                        fi.IsFile = true;
                    }
                    else
                    {
                        fi.Path += fi.FileName + "/";
                        fi.IsFile = false;
                    }
                    GoogleFile itemss = new GoogleFile(fi);
                    GoogleFiles.Add(itemss);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(string.Format("구글 파일 불러오기 오류 : {0}", e));
            }
        }
        /// <summary>
        /// 파일을 가져오는 함수
        /// </summary>
        /// <param name="file">부모 파일</param>
        /// <param name="folders"></param>
        public void AddFile(CloudFiles file, AllFolder folders)
        {
            // 부모 파일이 폴더가 아니라 파일이면 함수를 빠져나간다. 재귀함수의 탈출 조건
            if (file.Item.IsFile == false)
            {
                return;
            }
            folders.AddFiles(file.Item.FileID);
            file.Item.ChildFile = folders.GetFiles();
            foreach (var item in file.Item.ChildFile)
            {
                //재귀함수로 파일리스트에 파일들을 집어넣어준다.
                AddFile(item, folders);
            }
        }
        public CloudFiles AddFiles(Dictionary<string, object> items)
        {
            UniValue file = UniValue.Create((Dictionary<string, object>)items);
            var map = new ApiDataMapping();
            map.Add("id", "FileID");
            map.Add("title", "FileName");
            map.Add("fileExtension", "Extention");
            map.Add("downloadUrl", "DownUrl");
            map.Add("thumbnailLink", "Thumnail");
            map.Add("fileSize", "FileSize", typeof(long));
            map.Add("modifiedDate", "modifiedDate");
            FileInfo fi = new FileInfo(file, map);
            //fi.Path = "root";
            fi.driveinfo = driveinfo;
            if (fi.Extention != null)
            {
                fi.DownUrl = "https://www.googleapis.com/drive/v2/files/" + fi.FileID + "?alt=media";
                fi.IsFile = true;
            }
            else
            {
                fi.Path += fi.FileName + "/";
                fi.IsFile = false;
            }
            GoogleFile files = new GoogleFile(fi);
            GoogleFiles.Add(files);
            return files;
        }
        public List<CloudFiles> GetFiles()
        {
            List<CloudFiles> cloude = new List<CloudFiles>();
            foreach (var item in GoogleFiles)
            {
                CloudFiles cl = item;
                cloude.Add(cl);
            }
            return cloude;
        }

        public string DriveName()
        {
            return driveinfo.token.Drive + "(" + driveinfo.UserID + ")";
        }
        public async Task<CloudFiles> CreateFolder(string foldername, string parentid)
        {
            string method = "POST";
            string uri = "https://www.googleapis.com/drive/v2/files";
            object parent;
            parent = new object[] { new { id = parentid } };
            UniValue properties = UniValue.Create(new { title = foldername, parents = parent, mimeType = "application/vnd.google-apps.folder" });
            try
            {
                byte[] buf = Encoding.UTF8.GetBytes(properties.ToString());
                System.Net.HttpWebResponse rp = await HttpHelper.RequstHttp(method, uri, null, driveinfo.token.access_token, null, buf, buf.Length, 0, buf.Length);
                Dictionary<string, object> dic = HttpHelper.DerealizeJson(rp.GetResponseStream());
                CloudFiles file = AddFiles(dic);
                return file;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public FileUpDown.FileUpload upladfile(System.IO.FileStream stream, string path, string parentid, ListViewGroup group)
        {
            return new FileUpDown.FileUpload(stream, path, stream.Length, parentid, driveinfo, group);
        }
        public void RemoveFile(CloudFiles file)
        {
            int index = GoogleFiles.IndexOf(file);
            GoogleFiles.RemoveAt(index);
        }


        public CloudFiles GetRootFile()
        {
            FileInfo info = new FileInfo();
            info.FileID = "root";
            info.driveinfo = this.driveinfo;
            info.FileName = "root";
            info.IsFile = false;
            CloudFiles file = new GoogleFile(info);
            return file;
        }
    }
}
