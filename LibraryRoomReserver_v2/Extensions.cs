using HtmlAgilityPack;
using System.IO;
using System.Linq;
using System.Text;
using System;

namespace LibraryRoomReserver_v2
{
    static class Extensions
    {
        public static readonly Encoding encoder = Encoding.UTF8;
        static string result = "";
        static HtmlDocument html = new HtmlDocument();
        public static string GetToken(this Stream htmlDoc)
        {
            // Streamから文字を読み込む
            // XSS & CSRF対策のname=org.apache.struts.taglib.html.TOKENのinputのvalueを読み込む
            using (StreamReader streamReader = new StreamReader(htmlDoc, encoder))
            {
                result = streamReader.ReadToEnd();
            }
            html.LoadHtml(result);

            var node = html.DocumentNode
                .Descendants("input")
                .Where(x => x.Attributes["name"] != null)
                .Where(x => x.Attributes["name"].Value == "org.apache.struts.taglib.html.TOKEN")
                .FirstOrDefault();
            if (node != null)
            {
                return node
                    .Attributes["value"]
                    .Value;
            }
            else
            {
                string path = Environment.CurrentDirectory + @"\error.txt";
                using (StreamWriter streamWriter = new StreamWriter(path, false, Encoding.UTF8))
                {
                    StringBuilder log = new StringBuilder();
                    log.Append("-------------------------------------------------------------------\n");
                    log.Append("                                ERROR                              \n");
                    log.Append("-------------------------------------------------------------------\n");
                    log.Append("<Summary>\n");
                    log.Append("NullExceptionError\n");
                    log.Append("<Detail>\n");
                    log.Append("Any input tag with name attribute couldn't be found.\n");
                    log.Append("<Date>\n");
                    log.Append(DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString() + "\n");
                    log.Append("<Last Fetched HTML Source>\n");
                    log.Append(result);
                    streamWriter.Write(log);
                }
                Console.WriteLine("[ERROR] Null Exception. Couldn't find specified node.\nYou can find more details in {0}", path);
                Environment.Exit(-1);
                return result;
            }
        }
    }
}
