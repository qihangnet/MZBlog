using System.Collections.Generic;

namespace IPIP.Net
{
    public class City
    {
        /**
         * @var Reader
         */
        private readonly Reader reader;

        public City(string name)
        {
            reader = new Reader(name);
        }

        public string[] Find(string addr, string language = "CN")
        {
            return reader.Find(addr, language);
        }

        public Dictionary<string, string> FindMap(string addr, string language = "CN")
        {
            var data = reader.Find(addr, language);
            if (data == null)
            {
                return null;
            }
            var fields = reader.SupportFields;
            var m = new Dictionary<string, string>();
            for (int i = 0, l = data.Length; i < l; i++)
            {
                m.Add(fields[i], data[i]);
            }

            return m;
        }

        public CityInfo FindInfo(string addr, string language = "CN")
        {
            var data = reader.Find(addr, language);
            if (data == null)
            {
                return null;
            }

            return new CityInfo(data);
        }

        public bool IsIPv4
        {
            get
            {
                return (reader.MetaData.IPVersion & 0x01) == 0x01;
            }
        }

        public bool IsIPv6
        {
            get
            {
                return (reader.MetaData.IPVersion & 0x02) == 0x02;
            }
        }

        public string[] Fields
        {
            get
            {
                return reader.SupportFields;
            }
        }

        public int BuildTime
        {
            get
            {
                return reader.BuildUTCTime;
            }
        }
    }
}