using MimeKit;
using System.IO;

namespace CRMHalalBackEnd.Helpers
{
    public static class HtmlFileSend
    {
        public static string HtmlFileSender(string path)
        {
            string pathToFile = System.Web.Hosting.HostingEnvironment.MapPath(path);
            var builder = new BodyBuilder();

            using (StreamReader SourceReader = File.OpenText(pathToFile))
            {

                builder.HtmlBody = SourceReader.ReadToEnd();

            };
            return builder.HtmlBody;
        }
    }
}