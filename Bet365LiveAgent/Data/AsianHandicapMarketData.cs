using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bet365LiveAgent.Data
{
    class AsianHandicapMarketData
    {
        [JsonIgnore]
        public string HomeIT { get; set; } = string.Empty;
        public string HomeHdp { get; set; } = string.Empty;
        [JsonIgnore]
        public string HomeOD { get; set; } = string.Empty;
        public string HomeOdds
        {
            get
            {
                string[] operands = HomeOD.Split(new char[] { '/' });
                if (operands.Length == 2)
                {
                    double op1 = Convert.ToDouble(operands[0]);
                    double op2 = Convert.ToDouble(operands[1]);
                    return string.Format("{0:0.000}", 1 + op1 / op2);
                }
                else
                    return string.Empty;
            }
        }

        [JsonIgnore]
        public string AwayIT { get; set; } = string.Empty;
        public string AwayHdp { get; set; } = string.Empty;
        [JsonIgnore]
        public string AwayOD { get; set; } = string.Empty;
        public string AwayOdds
        {
            get
            {
                string[] operands = AwayOD.Split(new char[] { '/' });
                if (operands.Length == 2)
                {
                    double op1 = Convert.ToDouble(operands[0]);
                    double op2 = Convert.ToDouble(operands[1]);
                    return string.Format("{0:0.000}", 1 + op1 / op2);
                }
                else
                    return string.Empty;
            }
        }

        public void Update(JObject jObjData)
        {
            string type = jObjData["Type"] == null ? "" : jObjData["Type"].ToString();
            if ("MA".Equals(type))
            {
                foreach (var jPAToken in jObjData["PAData"].ToObject<JObject>())
                {
                    JObject jPAData = jPAToken.Value.ToObject<JObject>();
                    Update(jPAData);
                }
            }
            else if ("PA".Equals(type))
            {
                if (jObjData["OR"] != null)
                {
                    if ("0".Equals(jObjData["OR"].ToString()))
                    {
                        if (jObjData["IT"] != null)
                            HomeIT = jObjData["IT"].ToString();
                        if (jObjData["HA"] != null)
                            HomeHdp = jObjData["HA"].ToString();
                        if (jObjData["OD"] != null)
                            HomeOD = jObjData["OD"].ToString();
                    }
                    else if ("1".Equals(jObjData["OR"].ToString()))
                    {
                        if (jObjData["IT"] != null)
                            AwayIT = jObjData["IT"].ToString();
                        if (jObjData["HA"] != null)
                            AwayHdp = jObjData["HA"].ToString();
                        if (jObjData["OD"] != null)
                            AwayOD = jObjData["OD"].ToString();
                    }
                }
            }
        }
    }
}
