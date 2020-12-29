using System;
using System.Collections.Generic;
using System.Text;

namespace MCSkinDownloader
{
    public static class Const
    {
        public static class Regex
        {
            public const string URL_REGEX_PATTERN = @"<a(?=\s)(?!(?:[^>""\']|""[^""]*""|\'[^\']*\')*?(?<=\s)(?:href=""\/profile\/*"")\s*=)(?!\s*\/?>)\s+(?:"".*?""|\'.*?\'|[^>]*?)+>";
            public const string IMAGE_REGEX_PATTERN = @"<canvas(?=\s)(?!(?:[^>""""\']|""""[^""""]*""""|\'[^\']*\')*?(?<=\s)(?:data-skin-hash=""*"")\s*=)(?!\s*\/?>)\s+(?:"""".*?""""|\'.*?\'|[^>]*?)+>";
            public const string ATTR_VALUE = @"(\S+)=[""']?((?:.(?![""']?\s+(?:\S+)=|\s*\/?[>""']))+.)[""']?";
            public const string SUB = "<a href=\"/profile/";
        }

        public const string BASE_URL = "https://namemc.com";
        public const string API_URL = BASE_URL + "/search?q={0}";
        public const string IMAGE_URL = "https://render.namemc.com/skin/3d/body.png?skin={0}&model=slim&theta=-41&phi=14&time=90&width=600&height=800";
        public const string DEFAULT_IMAGE = "/Assets/avalonia-logo.ico";
    }
}
