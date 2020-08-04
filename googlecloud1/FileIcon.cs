using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using googlecloud1.Files;
using googlecloud1.Folder;
namespace googlecloud1
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SHFILEINFO
    {
        public IntPtr hIcon;
        public IntPtr iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    };

    class FileIcon
    {
        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
        [DllImport("User32.dll")]
        public static extern int DestroyIcon(IntPtr hIcon);
        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_DISPLAYNAME = 0x000000200;     // 화면이름을 얻어옵니다
        private const uint SHGFI_TYPENAME = 0x000000400;     // 타입의 이름을 얻어옵니다
        private const uint SHGFI_ATTRIBUTES = 0x000000800;     // get attributes
        private const uint SHGFI_ICONLOCATION = 0x000001000;     // get icon location
        private const uint SHGFI_EXETYPE = 0x000002000;     // return exe type
        private const uint SHGFI_SYSICONINDEX = 0x000004000;     // get system icon index
        private const uint SHGFI_LINKOVERLAY = 0x000008000;     // put a link overlay on icon
        private const uint SHGFI_SELECTED = 0x000010000;     // show icon in selected state
        private const uint SHGFI_ATTR_SPECIFIED = 0x000020000;     // get only specified attributes
        private const uint SHGFI_LARGEICON = 0x000000000;     // get large icon
        private const uint SHGFI_SMALLICON = 0x000000001;     // get small icon
        private const uint SHGFI_OPENICON = 0x000000002;     // get open icon
        private const uint SHGFI_SHELLICONSIZE = 0x000000004;     // get shell size icon
        private const uint SHGFI_PIDL = 0x000000008;     // pszPath is a pidl
        private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;     // use passed dwFileAttribute
        private const uint SHGFI_ADDOVERLAYS = 0x000000020;     // apply the appropriate overlays
        private const uint SHGFI_OVERLAYINDEX = 0x000000040;     // Get the index of the overlay
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        private const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
        public static System.Drawing.Bitmap GetIcon(CloudFiles item)
        {
            SHFILEINFO sfi = new SHFILEINFO();
            if (item.Item.Extention != null)
            {
                IntPtr img1= img1 = SHGetFileInfo("." + item.Item.Extention.ToUpper(), FILE_ATTRIBUTE_NORMAL, ref sfi, (uint)Marshal.SizeOf(sfi), SHGFI_ICON | SHGFI_LARGEICON | SHGFI_USEFILEATTRIBUTES | SHGFI_TYPENAME | SHGFI_SYSICONINDEX);
            }
            else
            {
                IntPtr img1 = img1 = SHGetFileInfo("Doesn't matter", FILE_ATTRIBUTE_DIRECTORY, ref sfi, (uint)Marshal.SizeOf(sfi), SHGFI_ICON | SHGFI_LARGEICON | SHGFI_USEFILEATTRIBUTES | SHGFI_OPENICON | SHGFI_TYPENAME);
            }
            System.Drawing.Icon icon = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(sfi.hIcon).Clone();
            DestroyIcon(sfi.hIcon);
            return icon.ToBitmap();
        }
        public static System.Windows.Forms.ImageList GetImageList(List<AllFolder> allfolder)
        {
            System.Windows.Forms.ImageList imagelist = new System.Windows.Forms.ImageList();
            foreach (var item in allfolder)
            {
                List<CloudFiles> file = item.GetFiles();
                foreach (var fileitem in file)
                {
                    if (fileitem.Item.Thumnail != null)
                    {
                        System.Net.WebClient down = new System.Net.WebClient();
                        System.IO.Stream imgst = down.OpenRead(fileitem.Item.Thumnail);
                        System.Drawing.Bitmap bit = new System.Drawing.Bitmap(imgst);
                        imagelist.Images.Add(bit);
                    }
                    else
                    {
                        imagelist.Images.Add(GetIcon(fileitem));
                    }
                }
            }
            return imagelist;
        }
        public System.Drawing.Image GetCustomIcon(CloudFiles files)
        {
            SHFILEINFO sfi = new SHFILEINFO();
            if (files.Item.Extention != null)
            {
                SHGetFileInfo("." + files.Item.Extention.ToUpper(), FILE_ATTRIBUTE_NORMAL, ref sfi, (uint)Marshal.SizeOf(sfi), SHGFI_ICON | SHGFI_LARGEICON | SHGFI_USEFILEATTRIBUTES | SHGFI_TYPENAME | SHGFI_SYSICONINDEX);
                if (sfi.szTypeName.ToUpper() == " 프로그램")
                {
                    return new System.Drawing.Bitmap(global::googlecloud1.Properties.Resources._1439186290_gdm_xnest);
                }
                else if (files.Item.Extention.ToUpper() == "PDF")
                {
                    return new System.Drawing.Bitmap(global::googlecloud1.Properties.Resources._1439186074_pdf);
                }
                else if (sfi.szTypeName == "이미지")
                {
                    return new System.Drawing.Bitmap(global::googlecloud1.Properties.Resources._1439186415_photos);
                }
                else if (files.Item.Extention.ToUpper() == "ZIP" || files.Item.Extention.ToUpper() == "ALZ")
                {
                    return new System.Drawing.Bitmap(global::googlecloud1.Properties.Resources._1439186516_1);
                }
                else if (files.Item.Extention.ToUpper() == "DLL")
                {
                    return new System.Drawing.Bitmap(global::googlecloud1.Properties.Resources._1439186520_12);
                }
                else if (files.Item.Extention.ToUpper() == "BAT")
                {
                    return new System.Drawing.Bitmap(global::googlecloud1.Properties.Resources._1439186523_14);
                }
                else if (files.Item.Extention.ToUpper() == "AVI" || files.Item.Extention.ToUpper() == "MP4")
                {
                    return new System.Drawing.Bitmap(global::googlecloud1.Properties.Resources._1439186533_11);
                }
                else if (files.Item.Extention.ToUpper() == "DOC" || files.Item.Extention.ToUpper() == "DOCX")
                {
                    return new System.Drawing.Bitmap(global::googlecloud1.Properties.Resources.icon_doc);
                }
                else if(files.Item.Extention.ToUpper() == "PPT" || files.Item.Extention.ToUpper() == "PPTX")
                {
                    return new System.Drawing.Bitmap(global::googlecloud1.Properties.Resources.icon_ppt);
                }
                else if(files.Item.Extention.ToUpper() == "XLS" || files.Item.Extention.ToUpper() == "XLSX")
                {
                    return new System.Drawing.Bitmap(global::googlecloud1.Properties.Resources.icon_xls);
                }
                else
                {
                    return new System.Drawing.Bitmap(global::googlecloud1.Properties.Resources.icon_default);
                }
            }
            else
            {
                return new System.Drawing.Bitmap(global::googlecloud1.Properties.Resources.Folder_Closed_Icon_256);
            }
        }
    }
}
