using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nemiro.OAuth;
using System.Globalization;
using System.Threading;
using googlecloud1.login;
using System.IO;
namespace googlecloud1.Files
{
    public class FileInfo
    {
        public UniValue Items { get; protected internal set; }
        public string FileID { get; protected internal set; }
        public string FileName { get; protected internal set; }
        public string Extention{get; protected internal set;}
        public string Thumnail { get; protected internal set; }
        public string DownUrl { get; protected internal set; }
        public long FileSize { get; protected internal set; }
        public string Path { get; protected internal set; }
        public bool IsFile { get; protected internal set; }
        public string Description { get; protected internal set; }
        public string modifiedDate { get; protected internal set; }
        public List<CloudFiles> ChildFile { get; protected internal set; }
        public DriveInfo driveinfo { get; protected internal set; }
        public FileInfo()
        {

        }
        public FileInfo(UniValue source, ApiDataMapping mapping)
        {
          if (mapping == null || !source.HasValue) { return; }
          this.Items = source;
          var t = typeof(FileInfo);
          foreach (var p in t.GetProperties())
          {
            var item = mapping.FirstOrDefault(itm => itm.DestinationName.Equals(p.Name, StringComparison.OrdinalIgnoreCase));
            if (item != null && source.ContainsKey(item.SourceName))
            {
              object vr = null;
              UniValue vs = source[item.SourceName];
              if (item.Parse != null)
              {
                vr = item.Parse(vs);
              }
              else
              {
                if (item.Type == typeof(DateTime))
                {
                  var f = new CultureInfo(Thread.CurrentThread.CurrentCulture.Name, true);
                  var formatDateTime = "dd.MM.yyyy HH:mm:ss";
                  if (!String.IsNullOrEmpty(item.Format))
                  {
                    formatDateTime = item.Format;
                  }
                  f.DateTimeFormat.FullDateTimePattern = formatDateTime;
                  f.DateTimeFormat.ShortDatePattern = formatDateTime;
                  DateTime dateValue;
                  if (DateTime.TryParse(vs.ToString(), f, DateTimeStyles.NoCurrentDateDefault, out dateValue))
                  {
                    vr = dateValue;
                  }
                  else
                  {
                    vr = null;
                  }
                }
                else if (item.Type == typeof(bool))
                {
                  vr = Convert.ToBoolean(vs);
                }
                else if (item.Type == typeof(Int16))
                {
                  vr = Convert.ToInt16(vs);
                }
                else if (item.Type == typeof(Int32))
                {
                  vr = Convert.ToInt32(vs);
                }
                else if (item.Type == typeof(Int64))
                {
                  vr = Convert.ToInt64(vs);
                }
                else if (item.Type == typeof(UInt16))
                {
                  vr = Convert.ToUInt16(vs);
                }
                else if (item.Type == typeof(UInt32))
                {
                  vr = Convert.ToUInt32(vs);
                }
                else if (item.Type == typeof(UInt64))
                {
                  vr = Convert.ToUInt64(vs);
                }
                else if (item.Type == typeof(double))
                {
                  vr = Convert.ToDouble(vs);
                }
                else if (item.Type == typeof(Single))
                {
                  vr = Convert.ToSingle(vs);
                }
                else if (item.Type == typeof(decimal))
                {
                  vr = Convert.ToDecimal(vs);
                }
                else if (item.Type == typeof(byte))
                {
                  vr = Convert.ToByte(vs);
                }
                else if (item.Type == typeof(char))
                {
                  vr = Convert.ToChar(vs);
                }
                else if (item.Type == typeof(string))
                {
                  vr = Convert.ToString(vs);
                }
                else
                {
                  vr = Convert.ToString(vs);
                }
              }
              p.SetValue(this, vr, null);
            }
          }
        }
        public override string ToString()
        {
            return this.FileName ?? this.FileID ?? this.GetType().Name;
        }
    }
}
