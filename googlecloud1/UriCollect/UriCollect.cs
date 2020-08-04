using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace googlecloud1.UriCollect
{
    public class UriCollection
    {
        public const string GoogleAuthorizationUrl = "https://accounts.google.com/o/oauth2/auth";
        public const string GoogleinstalledAppRedirectUri = "urn:ietf:wg:oauth:2.0:oob";
        public const string GoogleTokenUri = "https://accounts.google.com/o/oauth2/token";
        public const string OneDriveAuthorizationUri = "https://login.live.com/oauth20_authorize.srf";
        public const string OneDriveinstalledAppRedirectUri = "https://login.live.com/oauth20_desktop.srf";
        public const string OneDriveTokenUri = "https://login.live.com/oauth20_token.srf";
        public const string DropBoxAuthorizationUri = "https://www.dropbox.com/1/oauth2/authorize";
        public const string DropBoxTokenUri = "https://api.dropbox.com/1/oauth2/token";
    }
}
