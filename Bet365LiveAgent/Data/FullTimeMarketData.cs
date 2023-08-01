using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bet365LiveAgent.Data
{
    class FullTimeMarketData
    {
        [JsonIgnore]
        public string HomeIT { get; set; } = string.Empty;        
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
                    return string.Format("{0:0.00}", 1 + op1 / op2);
                }
                else
                    return string.Empty;
            }
        }

        [JsonIgnore]
        public string DrawIT { get; set; } = string.Empty;
        [JsonIgnore]
        public string DrawOD { get; set; } = string.Empty;
        public string DrawOdds
        {
            get
            {
                string[] operands = DrawOD.Split(new char[] { '/' });
                if (operands.Length == 2)
                {
                    double op1 = Convert.ToDouble(operands[0]);
                    double op2 = Convert.ToDouble(operands[1]);
                    return string.Format("{0:0.00}", 1 + op1 / op2);
                }
                else
                    return string.Empty;
            }
        }

        [JsonIgnore]
        public string AwayIT { get; set; } = string.Empty;
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
                    return string.Format("{0:0.00}", 1 + op1 / op2);
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
                        if (jObjData["OD"] != null)
                            HomeOD = jObjData["OD"].ToString();
                    }
                    else if ("1".Equals(jObjData["OR"].ToString()))
                    {
                        if (jObjData["IT"] != null)
                            DrawIT = jObjData["IT"].ToString();
                        if (jObjData["OD"] != null)
                            DrawOD = jObjData["OD"].ToString();
                    }
                    else if ("2".Equals(jObjData["OR"].ToString()))
                    {
                        if (jObjData["IT"] != null)
                            AwayIT = jObjData["IT"].ToString();
                        if (jObjData["OD"] != null)
                            AwayOD = jObjData["OD"].ToString();
                    }
                }
            }
        }
    }
}
