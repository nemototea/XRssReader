using System;
using System.Collections.Generic;
using System.Text;

namespace XRssReader.Common
{
    static public class Define
    {
        // RSSフィードのタイトル
        public static string MSG_GETTING = "RSSフィードを取得中 ...";

        // RSSフィードの説明
        public static string MSG_RECIVING = "Recieving ...";

        // HTMLタグを取り除くための文字列
        public const string PATTERN_STR = @"<.*?>|&nbsp;";

        // HTMLタグから画像ファイルのソースを抽出するための文字列
        public const string PATTERN_IMGTAG = @"<img.*?src\s*=\s*[""'](?<uri>.+?)[""'].*?>";
    }
}
