using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bet365LiveAgent.Data
{
    class GoalLineMarketData
    {
        [JsonIgnore]
        public string OverIT { get; set; } = string.Empty;
        public string OverHdp { get; set; } = string.Empty;
        [JsonIgnore]
        public string OverOD { get; set; } = string.Empty;
        public string OverOdds
        {
            get
            {
                string[] operands = OverOD.Split(new char[] { '/' });
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
        public string UnderIT { get; set; } = string.Empty;
        public string UnderHdp { get; set; } = string.Empty;
        [JsonIgnore]
        public string UnderOD { get; set; } = string.Empty;
        public string UnderOdds
        {
            get
            {
                string[] operands = UnderOD.Split(new char[] { '/' });
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
                            OverIT = jObjData["IT"].ToString();
                        if (jObjData["HA"] != null)
                            OverHdp = jObjData["HA"].ToString();
                        if (jObjData["OD"] != null)
                            OverOD = jObjData["OD"].ToString();
                    }
                    else if ("1".Equals(jObjData["OR"].ToString()))
                    {
                        if (jObjData["IT"] != null)
                            UnderIT = jObjData["IT"].ToString();
                        if (jObjData["HA"] != null)
                            UnderHdp = jObjData["HA"].ToString();
                        if (jObjData["OD"] != null)
                            UnderOD = jObjData["OD"].ToString();
                    }
                }
            }            
        }
    }
}
