using System.Text;

namespace IPIP.Net
{

    public class DistrictInfo
    {

        private readonly string[] data;

        public DistrictInfo(string[] data)
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

        public string DistrictName
        {
            get
            {
                return data[3];
            }
        }

        public string ChinaAdminCode
        {
            get
            {
                return data[4];
            }
        }

        public string CoveringRadius
        {
            get
            {
                return data[5];
            }
        }

        public string Latitude
        {
            get
            {
                return data[6];
            }
        }

        public string Longitude
        {
            get
            {
                return data[7];
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
            sb.Append("district_name:");
            sb.Append(DistrictName);
            sb.Append("\n");
            sb.Append("china_admin_code:");
            sb.Append(ChinaAdminCode);
            sb.Append("\n");
            sb.Append("covering_radius:");
            sb.Append(CoveringRadius);
            sb.Append("\n");
            sb.Append("latitude:");
            sb.Append(Latitude);
            sb.Append("\n");
            sb.Append("longitude:");
            sb.Append(Longitude);

            return sb.ToString();
        }
    }

}
