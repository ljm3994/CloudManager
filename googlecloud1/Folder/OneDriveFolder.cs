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
    class OneDriveFolder : AllFolder
    {
        List<CloudFiles> files;
        public DriveInfo driveinfo { get; set; }

        public OneDriveFolder(DriveInfo driveinfo)
        {
            this.driveinfo = driveinfo;
            files = new List<CloudFiles>();
        }
        public async Task AddFiles(string id)
        {
            try
            {
                var parameter = new HttpParameterCollection()
                {
                    {"access_token", driveinfo.token.access_token}
                };
                var result = OAuthUtility.Get(string.Format("https://api.onedrive.com/v1.0/drive/items/{0}/children", id), parameter);
                var map = new ApiDataMapping();
                map.Add("id", "FileID");
                map.Add("name", "FileName");
                map.Add("@content.downloadUrl", "DownUrl");
                map.Add("size", "FileSize", typeof(long));
                map.Add("lastModifiedDateTime", "modifiedDate");
                foreach (var item in result.Result.CollectionItems.Items["value"].CollectionItems.Items.Values)
                {
                    FileInfo fi = new FileInfo(item, map);
                    fi.driveinfo = driveinfo;
                    if(id == "root")
                    {
                        fi.Path = "OneDrive/";
                    }
                    if (fi.Items.CollectionItems.ContainsKey("folder"))
                    {
                        fi.IsFile = false;
                        fi.Path += fi.FileName + "/";
                    }
                    else
                    {
                        fi.IsFile = true;
                        fi.Extention = GetExtension(fi.FileName);
                        fi.DownUrl = item.CollectionItems["@content.downloadUrl"].ToString();
                    }
                    fi.Thumnail = GetTumbNail(fi.FileID);
                    OneDriveFile items = new OneDriveFile(fi);
                    files.Add(items);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(string.Format("원드라이브 파일 불러오기 오류 : {0}", e));
            }
        }
        public string GetTumbNail(string id)
        {
            var parameter = new HttpParameterCollection()
            {
                {"access_token", driveinfo.token.access_token}
            };
            var result = OAuthUtility.Get(string.Format("https://api.onedrive.com/v1.0/drive/items/{0}/thumbnails", id), parameter);
            var map = new ApiDataMapping();
            map.Add("medium", "Thumnail");
            FileInfo fi = null;
            foreach (var item in result.Result.CollectionItems.Items["value"].CollectionItems.Items.Values)
            {
               fi = new FileInfo(item, map);  
            }
            if(fi != null)
            {
                int index = fi.Thumnail.IndexOf("url");
                int last = fi.Thumnail.LastIndexOf(",");
                fi.Thumnail = fi.Thumnail.Substring(index, last - index);
                fi.Thumnail = fi.Thumnail.Substring(7, (fi.Thumnail.Length - 3) - 5);
                return fi.Thumnail;
            }
            else
            {
                return null;
            }
        }
        public CloudFiles AddFiles(Dictionary<string, object> items)
        {
            UniValue file = UniValue.Create((Dictionary<string, object>)items);
            var map = new ApiDataMapping();
            map.Add("id", "FileID");
            map.Add("name", "FileName");
            map.Add("@content.downloadUrl", "DownUrl");
            map.Add("size", "FileSize", typeof(long));
            map.Add("lastModifiedDateTime", "modifiedDate");
            FileInfo fi = new FileInfo(file, map);
            fi.driveinfo = driveinfo;
            if (fi.Items.CollectionItems.ContainsKey("folder"))
            {
                fi.IsFile = false;
            }
            else
            {
                fi.IsFile = true;
                fi.Extention = GetExtension(fi.FileName);
            }
            fi.Thumnail = GetTumbNail(fi.FileID);
            OneDriveFile itemss = new OneDriveFile(fi);
            files.Add(itemss);
            return itemss;
        }
        public string GetExtension(string Name)
        {
            int index = Name.LastIndexOf(".");
            return Name.Substring(index + 1, Name.Length - (index + 1));
        }
        public List<CloudFiles> GetFiles()
        {
            List<CloudFiles> cloude = new List<CloudFiles>();
            foreach (var item in files)
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
            string uri = string.Format("https://api.onedrive.com/v1.0/drive/items/{0}/children", parentid);
            UniValue properties = UniValue.Create(new { name = foldername, folder = new { } });
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

        /// <summary>
        /// 파일을 가져오는 함수
        /// </summary>
        /// <param name="file">부모 파일</param>
        /// <param name="folders"></param>
        public void AddFile(CloudFiles file, AllFolder folders)
        {
            // 부모 파일이 폴더가 아니라 파일이면 함수를 빠져나간다. 재귀함수의 탈출 조건
            if (file.Item.IsFile == true)
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
        public FileUpDown.FileUpload upladfile(System.IO.FileStream stream, string path, string parentid, ListViewGroup group)
        {
            return new FileUpDown.FileUpload(stream, path, stream.Length, parentid, driveinfo, group);
        }


        public void RemoveFile(CloudFiles file)
        {
            int index = this.files.IndexOf(file);
            this.files.RemoveAt(index);
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
