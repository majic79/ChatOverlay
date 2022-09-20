using MaJiCSoft.ChatOverlay.OAuth.Options;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace MaJiCSoft.ChatOverlay.Options
{
    public class TwitchClientOptions {
        public string ApiBase { get; set; }
        public string IRCBase { get; set; }
        public string WSBase { get; set; }

    }
}
