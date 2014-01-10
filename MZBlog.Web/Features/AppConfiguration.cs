using Nancy;
using System.Configuration;

namespace MZBlog.Web.Features
{
    public class AppConfiguration
    {
        private static readonly DynamicDictionary _config = new DynamicDictionary();

        static AppConfiguration()
        {
            //initializing from configuraiton settings
            foreach (var appKey in ConfigurationManager.AppSettings.AllKeys)
            {
                _config[appKey] = ConfigurationManager.AppSettings[appKey];
            }
        }

        public static dynamic Current
        {
            get { return _config; }
        }

        private AppConfiguration()
        {
        }
    }
}