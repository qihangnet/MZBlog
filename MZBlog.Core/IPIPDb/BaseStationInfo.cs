using System.Text;

namespace IPIP.Net
{
    public class BaseStationInfo
    {
        private readonly string[] data;

        public BaseStationInfo(string[] data)
        {
            this.data = data;
        }

        public string CountryName
        {
            get
            {
                return data[0];
            }
        }

        public string RegionName
        {
            get
            {
                return data[1];
            }
        }

        public string CityName
        {
            get
            {
                return data[2];
            }
        }

        public string OwnerDomain
        {
            get
            {
                return data[3];
            }
        }

        public string IspDomain
        {
            get
            {
                return data[4];
            }
        }

        public string BaseStation
        {
            get
            {
                return data[5];
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("country_name:");
            sb.Append(CountryName);
            sb.Append("\n");
            sb.Append("region_name:");
            sb.Append(RegionName);
            sb.Append("\n");
            sb.Append("city_name:");
            sb.Append(CityName);
            sb.Append("\n");
            sb.Append("owner_domain:");
            sb.Append(OwnerDomain);
            sb.Append("\n");
            sb.Append("isp_domain:");
            sb.Append(IspDomain);
            sb.Append("\n");
            sb.Append("base_station:");
            sb.Append(BaseStation);

            return sb.ToString();
        }
    }

}
