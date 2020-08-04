using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nemiro.OAuth;
using googlecloud1.login;
using System.IO;
using System.Threading;
using System.Globalization;
namespace googlecloud1
{
    public class DriveInfo
    {
        public Dictionary<string, object> Items { get; protected internal set; }
        public string DisplayName { get; protected internal set; }
        public string UserID { get; protected internal set; }
        public long TotalSize {get; protected internal set;}
        public long UseSize { get; protected internal set; }
        public long EmptySize { get; protected internal set; }
        public long DeleteSize { get; protected internal set; }
        public string Status { get; protected internal set; }
        public string DriveType { get; protected internal set; }
        public string DriveID { get; protected internal set; }
        public string username { get; protected internal set; }
        public Authentication.TokenResult token { get; protected internal set; }
        public DriveInfo(UniValue source, ApiDataMapping mapping)
        {
          if (mapping == null || !source.HasValue) { return; }
          this.Items = source.ToDictionary();
          var t = typeof(DriveInfo);
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
            return this.DisplayName ?? this.DriveID ?? this.GetType().Name;
        }
    }
}
