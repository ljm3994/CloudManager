using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nemiro.OAuth;
using googlecloud1.login;
using googlecloud1.Files;
using Nemiro.OAuth.Clients;
using System.Windows.Forms;
namespace googlecloud1.Folder
{
    class DropBoxFolder : AllFolder
    {

        public DriveInfo driveinfo { get; set; }
        List<CloudFiles> file = new List<CloudFiles>();
        public DropBoxFolder(DriveInfo driveinfo)
        {
            this.driveinfo = driveinfo;
        }
        public List<Files.CloudFiles> GetFiles()
        {
            List<CloudFiles> cloude = new List<CloudFiles>();
            foreach (var item in file)
            {
                CloudFiles cl = item;
                cloude.Add(cl);
            }
            return cloude;
        }
        public string GetTumbNail(bool Thumnail, string fileid)
        {
            string re = null;
            if (Thumnail)
            {
                re = string.Format("https://api-content.dropbox.com/1/thumbnails/auto{0}?access_token={1}&size={2}", fileid, driveinfo.token.access_token, "m");
            }
            return re;
        }
        public async Task AddFiles(string id)
        {
            try
            {
                RequestResult result = OAuthUtility.Get("https://api.dropbox.com/1/metadata/auto/", new HttpParameterCollection { { "path", id }, { "access_token", driveinfo.token.access_token }, { "include_media_info", "true" } });
                var map = new ApiDataMapping();
                map.Add("path", "FileID");
                map.Add("path", "FileName");
                map.Add("thumb_exists", "Thumnail");
                map.Add("bytes", "FileSize", typeof(long));
                map.Add("modified", "modifiedDate");
                foreach (var item in result.CollectionItems.Items["contents"].CollectionItems.Items.Values)
                {
                    FileInfo fi = new FileInfo(item, map);
                    fi.driveinfo = driveinfo;
                    if(id == "/")
                    {
                        fi.Path = "DropBox/";
                    }
                    if (fi.Thumnail == "True")
                    {
                        fi.Thumnail = GetTumbNail(true, fi.FileID);
                    }
                    else
                    {
                        fi.Thumnail = GetTumbNail(false, fi.FileID);
                    }
                    if (item["icon"].ToString() == "folder" || item["icon"].ToString() == "folder_app")
                    {
                        fi.IsFile = false;
                        fi.Path += fi.FileID;
                    }
                    else
                    {
                        fi.IsFile = true;
                        fi.FileName = GetFileName(fi.FileName);
                        fi.Extention = GetExtension(fi.FileName);
                    }
                    fi.DownUrl = "https://api-content.dropbox.com/1/files/auto/" + fi.FileID;
                    DropBoxFile items = new DropBoxFile(fi);
                    file.Add(items);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(string.Format("드롭박스 파일 불러오기 오류 : {0}",e));
            }
        }
        public string GetFileName(string Name)
        {
            int index = Name.LastIndexOf("/");
            return Name.Substring(index + 1, Name.Length - (index + 1));
        }
        public string GetExtension(string Name)
        {
            int index = Name.LastIndexOf(".");
            return Name.Substring(index + 1, Name.Length - (index+1));
        }
        public CloudFiles AddFiles(Dictionary<string, object> items)
        {
            UniValue files = UniValue.Create(items);
            var map = new ApiDataMapping();
            map.Add("path", "FileID");
            map.Add("path", "FileName");
            map.Add("thumb_exists", "Thumnail");
            map.Add("bytes", "FileSize", typeof(long));
            map.Add("modified", "modifiedDate");
            FileInfo fi = new FileInfo(files, map);
            fi.driveinfo = driveinfo;
            if (fi.Thumnail == "True")
            {
               fi.Thumnail = GetTumbNail(true, fi.FileID);
            }
            else
            {
               fi.Thumnail = GetTumbNail(false, fi.FileID);
            }
            if (items["icon"].ToString() == "folder")
            {
                fi.IsFile = false;
            }
            else
            {
               fi.IsFile = true;
               fi.FileName = GetFileName(fi.FileName);
               fi.Extention = GetExtension(fi.FileName);
               fi.DownUrl = "https://api-content.dropbox.com/1/files/auto/" + fi.FileID;
            }
            DropBoxFile itemss = new DropBoxFile(fi);
            file.Add(itemss);
            return itemss;
        }


        public string DriveName()
        {
            return driveinfo.token.Drive + "(" + driveinfo.UserID + ")";
        }
        public async Task<CloudFiles> CreateFolder(string foldername, string parentid)
        {
            string method = "POST";
            string uri = "https://api.dropbox.com/1/fileops/create_folder";
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("root", "auto");
            if(parentid == "root")
            {
                parentid = "/";
            }
            parentid += "/" + foldername;
            parameter.Add("path", parentid);
            try
            {
                System.Net.HttpWebResponse rp = await HttpHelper.RequstHttp(method, uri, parameter, driveinfo.token.access_token, null, null, 0, 0, 0);
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
            if (parentid == "root")
            {
                parentid = "/";
            }
            return new FileUpDown.FileUpload(stream, path, stream.Length, parentid, driveinfo, group);
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
        public void RemoveFile(CloudFiles file)
        {
            int index = this.file.IndexOf(file);
            this.file.RemoveAt(index);
        }


        public CloudFiles GetRootFile()
        {
            FileInfo info = new FileInfo();
            info.FileID = "/";
            info.driveinfo = this.driveinfo;
            info.FileName = "root";
            info.IsFile = false;
            CloudFiles file = new GoogleFile(info);
            return file;
        }
    }
}
