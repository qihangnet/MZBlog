using System.Text;

namespace IPIP.Net
{

    public class CityInfo
    {

        private readonly string[] data;

        public CityInfo(string[] data)
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

        public string Latitude
        {
            get
            {
                return data[5];
            }
        }

        public string Longitude
        {
            get
            {
                return data[6];
            }
        }

        public string Timezone
        {
            get
            {
                return data[7];
            }
        }

        public string UtcOffset
        {
            get
            {
                return data[8];
            }
        }

        public string ChinaAdminCode
        {
            get
            {
                return data[9];
            }
        }

        public string IddCode
        {
            get
            {
                return data[10];
            }
        }

        public string CountryCode
        {
            get
            {
                return data[11];
            }
        }

        public string ContinentCode
        {
            get
            {
                return data[12];
            }
        }

        public string IDC
        {
            get
            {
                return data[13];
            }
        }

        public string BaseStation
        {
            get
            {
                return data[14];
            }
        }

        public string CountryCode3
        {
            get
            {
                return data[15];
            }
        }

        public string EuropeanUnion
        {
            get
            {
                return data[16];
            }
        }

        public string CurrencyCode
        {
            get
            {
                return data[17];
            }
        }

        public string CurrencyName
        {
            get
            {
                return data[18];
            }
        }

        public string Anycast
        {
            get
            {
                return data[19];
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
            sb.Append("latitude:");
            sb.Append(Latitude);
            sb.Append("\n");
            sb.Append("longitude:");
            sb.Append(Longitude);
            sb.Append("\n");

            sb.Append("timezone:");
            sb.Append(Timezone);
            sb.Append("\n");

            sb.Append("utc_offset:");
            sb.Append(UtcOffset);
            sb.Append("\n");

            sb.Append("china_admin_code:");
            sb.Append(ChinaAdminCode);
            sb.Append("\n");

            sb.Append("idd_code:");
            sb.Append(IddCode);
            sb.Append("\n");

            sb.Append("country_code:");
            sb.Append(CountryCode);
            sb.Append("\n");

            sb.Append("continent_code:");
            sb.Append(ContinentCode);
            sb.Append("\n");

            sb.Append("idc:");
            sb.Append(IDC);
            sb.Append("\n");

            sb.Append("base_station:");
            sb.Append(BaseStation);
            sb.Append("\n");

            sb.Append("country_code3:");
            sb.Append(CountryCode3);
            sb.Append("\n");

            sb.Append("european_union:");
            sb.Append(EuropeanUnion);
            sb.Append("\n");

            sb.Append("currency_code:");
            sb.Append(CurrencyCode);
            sb.Append("\n");

            sb.Append("currency_name:");
            sb.Append(CurrencyName);
            sb.Append("\n");

            sb.Append("anycast:");
            sb.Append(Anycast);

            return sb.ToString();

        }
    }

}
