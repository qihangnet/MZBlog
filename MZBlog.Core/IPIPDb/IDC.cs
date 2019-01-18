using System.Collections.Generic;

namespace IPIP.Net
{
    public class IDC
    {
        /**
         * @var Reader
         */
        private readonly Reader reader;

        public IDC(string name)
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

            var m = new Dictionary<string, string>();

            var fields = reader.SupportFields;

            for (int i = 0, l = data.Length; i < l; i++)
            {
                m.Add(fields[i], data[i]);
            }

            return m;
        }

        public IDCInfo FindInfo(string addr, string language = "CN")
        {
            var data = reader.Find(addr, language);
            if (data == null)
            {
                return null;
            }
            return new IDCInfo(data);
        }

        public bool IsIPv4()
        {
            return (reader.MetaData.IPVersion & 0x01) == 0x01;
        }

        public bool IsIPv6()
        {
            return (reader.MetaData.IPVersion & 0x02) == 0x02;
        }

        public string[] Fields()
        {
            return reader.SupportFields;
        }

        public int BuildTime()
        {
            return reader.BuildUTCTime;
        }
    }
}