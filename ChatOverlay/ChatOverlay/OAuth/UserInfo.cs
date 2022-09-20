using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MaJiCSoft.ChatOverlay.OAuth
{
    public class UserInfo
    {
        public string Aud { get; set; }
        public string Azp { get; set; }
        public string Exp { get; set; }
        public string Iat { get; set; }
        public string Iss { get; set; }
        public string Sub { get; set; }
        public string Picture { get; set; }
    }
}
