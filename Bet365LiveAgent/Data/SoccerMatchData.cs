using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bet365LiveAgent.Data
{
    class SoccerMatchData
    {
        private string C1 { get; set; } = string.Empty;
        private string C2 { get; set; } = string.Empty;
        private string T1 { get; set; } = string.Empty;
        private string T2 { get; set; } = string.Empty;
        public string EventID
        {
            get
            {
                return $"{C1}{T1}{C2}{T2}";
            }
        }
        public string FixtureID { get; set; } = string.Empty;
        public string Type { get; set; } = "Soccer";
        public string LeagueName { get; set; } = string.Empty;
        public string HomeName { get; set; } = string.Empty;
        public string AwayName { get; set; } = string.Empty;
        [JsonIgnore]
        public string TU { get; set; } = string.Empty;
        [JsonIgnore]
        public string TM { get; set; } = string.Empty;
        public string Time {
            get
            {
                DateTime dtTU = DateTime.ParseExact($"{TU} GMT+0100", "yyyyMMddHHmmss 'GMT'K", CultureInfo.InvariantCulture);
                var pacificTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
                TimeSpan timeDiff = DateTime.UtcNow.Subtract(dtTU.ToUniversalTime());
                string value = string.Empty;
                if (timeDiff.TotalMinutes > 0)
                    value = $"{timeDiff.Minutes}:{timeDiff.Seconds}";
                
                return value;
            }
        }
        public string Score { get; set; } = string.Empty;
        public string XPos { get; set; } = string.Empty;
        public string YPos { get; set; } = string.Empty;
        [JsonIgnore]
        public string TA { get; set; }  = string.Empty;
        [JsonIgnore]
        public string VC { get; set; } = string.Empty;
        public string Animation
        {
            get
            {
                string value = VC;
                if (value.IndexOf("_") > -1)
                    value = value.Substring(value.IndexOf("_") + 1);
                switch (value)
                {
                    case "1014":
                        value = "Kick Off";
                        break;
                    case "1015":
                        value = "Half Time";
                        break;
                    case "1016":
                        value = "Second Half";
                        break;
                    case "1017":
                        value = "Full Time";
                        break;
                    case "1026":
                        value = "Injury Time";
                        if (!string.IsNullOrWhiteSpace(TA))
                            value = $"{value} - {TA} Mins";
                        break;
                    case "11000":
                        value = $"{HomeName} - Dangerous Attack";
                        break;
                    case "11001":
                        value = $"{HomeName} - Attack";
                        break;
                    case "11002":
                        value = $"{HomeName} - In Possession";
                        break;
                    case "11003":
                        value = $"{HomeName} - Goal";
                        break;
                    case "11004":
                        value = $"{HomeName} - Corner";
                        break;
                    case "11005":
                        value = $"{HomeName} - Yellow Card";
                        break;
                    case "11006":
                        value = $"{HomeName} - Red Card";
                        break;
                    case "11007":
                        value = $"{HomeName} - Goal Kick";
                        break;
                    case "11008":
                        value = $"{HomeName} - Penalty";
                        break;
                    case "11009":
                        value = $"{HomeName} - Dangerous Free Kick";
                        break;
                    case "11010":
                        value = $"{HomeName} - Free Kick";
                        break;
                    case "11011":
                        value = $"{HomeName} - Shot On Target";
                        break;
                    case "11012":
                        value = $"{HomeName} - Shot Off Target";
                        break;
                    case "11013":
                        value = $"{HomeName} - Substitution";
                        break;
                    case "11024":
                        value = $"{HomeName} - Throw In";
                        break;
                    case "11025":
                        value = $"{HomeName} - Injury";
                        break;
                    case "11234":
                        value = $"{HomeName} - Offside";
                        break;
                    case "21000":
                        value = $"{AwayName} - Dangerous Attack";
                        break;
                    case "21001":
                        value = $"{AwayName} - Attack";
                        break;
                    case "21002":
                        value = $"{AwayName} - In Possession";
                        break;
                    case "21003":
                        value = $"{AwayName} - Goal";
                        break;
                    case "21004":
                        value = $"{AwayName} - Corner";
                        break;
                    case "21005":
                        value = $"{AwayName} - Yellow Card";
                        break;
                    case "21006":
                        value = $"{AwayName} - Red Card";
                        break;
                    case "21007":
                        value = $"{AwayName} - Goal Kick";
                        break;
                    case "21008":
                        value = $"{AwayName} - Penalty";
                        break;
                    case "21009":
                        value = $"{AwayName} - Dangerous Free Kick";
                        break;
                    case "21010":
                        value = $"{AwayName} - Free Kick";
                        break;
                    case "21011":
                        value = $"{AwayName} - Shot On Target";
                        break;
                    case "21012":
                        value = $"{AwayName} - Shot Off Target";
                        break;
                    case "21013":
                        value = $"{AwayName} - Substitution";
                        break;
                    case "21024":
                        value = $"{AwayName} - Throw In";
                        break;
                    case "21025":
                        value = $"{AwayName} - Injury";
                        break;
                    case "21234":
                        value = $"{AwayName} - Offside";
                        break;
                    default:
                        value = string.Empty;
                        break;
                }
                return value;
            }
        }


        public FullTimeMarketData FullTime { get; set; } = new FullTimeMarketData();
        public AsianHandicapMarketData AsianHandicap { get; set; } = new AsianHandicapMarketData();
        public GoalLineMarketData GoalLine { get; set; } = new GoalLineMarketData();

        public SoccerTeamData HomeTeam { get; set; } = new SoccerTeamData();
        public SoccerTeamData AwayTeam { get; set; } = new SoccerTeamData();

        public int halfTabFlag { get; set; } = 0;

        public void Update(JObject jObjData)
        {
            if (jObjData["C1"] != null)
                C1 = jObjData["C1"].ToString();
            if (jObjData["C2"] != null)
                C2 = jObjData["C2"].ToString();
            if (jObjData["T1"] != null)
                T1 = jObjData["T1"].ToString();
            if (jObjData["T2"] != null)
                T2 = jObjData["T2"].ToString();
            if (jObjData["FI"] != null)
                FixtureID = jObjData["FI"].ToString();
            if (jObjData["CT"] != null)
                LeagueName = jObjData["CT"].ToString();
            if (jObjData["TU"] != null)
                TU = jObjData["TU"].ToString();
            if (jObjData["TM"] != null)
                TM = jObjData["TM"].ToString();
            if (jObjData["SS"] != null)
                Score = jObjData["SS"].ToString();
            if (jObjData["TA"] != null)
                TA = jObjData["TA"].ToString();
            if (jObjData["VC"] != null)
                VC = jObjData["VC"].ToString();
            if (jObjData["XY"] != null)
            {
                string XYPos = jObjData["XY"].ToString();
                if (!string.IsNullOrWhiteSpace(XYPos))
                {
                    XPos = XYPos.Substring(0, XYPos.IndexOf(","));
                    YPos = XYPos.Substring(XPos.Length + 1);
                }
            }
            if (jObjData["TGData"] != null)
            {
                foreach (var jTGToken in jObjData["TGData"].ToObject<JObject>())
                {
                    JObject jTGData = jTGToken.Value.ToObject<JObject>();
                    if (jTGData["TEData"] != null)
                    {
                        JToken jTkHomeTeam = jTGData["TEData"][$"ML{FixtureID}C1T1_32_0"];
                        if (jTkHomeTeam != null)
                            HomeTeam.Update(jTkHomeTeam.ToObject<JObject>());
                        JToken jTkAwayTeam = jTGData["TEData"][$"ML{FixtureID}C1T2_32_0"];
                        if (jTkAwayTeam != null)
                            AwayTeam.Update(jTkAwayTeam.ToObject<JObject>());
                    }
                }
            }
            if (jObjData["ESData"] != null)
            {
                foreach (var jESToken in jObjData["ESData"].ToObject<JObject>())
                {
                    JObject jESData = jESToken.Value.ToObject<JObject>();
                    if (jESData["SCData"] != null)
                    {
                        if (jESData["SCData"][$"ML{FixtureID}C1ES0_32_0"] != null)
                        {
                            JObject jSCData = jESData["SCData"][$"ML{FixtureID}C1ES0_32_0"].ToObject<JObject>();
                            JToken jTkHomeName = jSCData["SLData"][$"ML{FixtureID}C1ES0-0_32_0"];
                            if (jTkHomeName != null && jTkHomeName["D1"] != null)
                                HomeName = jTkHomeName["D1"].ToString();
                            JToken jTkAwayName = jSCData["SLData"][$"ML{FixtureID}C1ES0-1_32_0"];
                            if (jTkAwayName != null && jTkAwayName["D1"] != null)
                                AwayName = jTkAwayName["D1"].ToString();
                        }

                        if (jESData["SCData"][$"ML{FixtureID}C1ES1_32_0"] != null)
                        {
                            JObject jSCData = jESData["SCData"][$"ML{FixtureID}C1ES1_32_0"].ToObject<JObject>();
                            JToken jTkHomeScore = jSCData["SLData"][$"ML{FixtureID}C1ES1-0_32_0"];
                            if (jTkHomeScore != null && jTkHomeScore["D1"] != null)
                                HomeTeam.Score = jTkHomeScore["D1"].ToString();
                            JToken jTkAwayScore = jSCData["SLData"][$"ML{FixtureID}C1ES1-1_32_0"];
                            if (jTkAwayScore != null && jTkAwayScore["D1"] != null)
                                AwayTeam.Score = jTkAwayScore["D1"].ToString();
                        }


                        if (jESData["SCData"][$"ML{FixtureID}C1ES2_32_0"] != null)
                        {
                            JObject jSCData = jESData["SCData"][$"ML{FixtureID}C1ES2_32_0"].ToObject<JObject>();
                            JToken jTkHomeCorner = jSCData["SLData"][$"ML{FixtureID}C1ES2-0_32_0"];
                            if (jTkHomeCorner != null && jTkHomeCorner["D1"] != null)
                                HomeTeam.Corner = jTkHomeCorner["D1"].ToString();
                            JToken jTkAwayCorner = jSCData["SLData"][$"ML{FixtureID}C1ES2-1_32_0"];
                            if (jTkAwayCorner != null && jTkAwayCorner["D1"] != null)
                                AwayTeam.Corner = jTkAwayCorner["D1"].ToString();
                        }                        

                        if (jESData["SCData"][$"ML{FixtureID}C1ES3_32_0"] != null)
                        {
                            JObject jSCData = jESData["SCData"][$"ML{FixtureID}C1ES3_32_0"].ToObject<JObject>();
                            JToken jTkHomeYellowCard = jSCData["SLData"][$"ML{FixtureID}C1ES3-0_32_0"];
                            if (jTkHomeYellowCard != null && jTkHomeYellowCard["D1"] != null)
                                HomeTeam.YellowCard = jTkHomeYellowCard["D1"].ToString();
                            JToken jTkAwayYellowCard = jSCData["SLData"][$"ML{FixtureID}C1ES3-1_32_0"];
                            if (jTkAwayYellowCard != null && jTkAwayYellowCard["D1"] != null)
                                AwayTeam.YellowCard = jTkAwayYellowCard["D1"].ToString();
                        }                        

                        if (jESData["SCData"][$"ML{FixtureID}C1ES4_32_0"] != null)
                        {
                            JObject jSCData = jESData["SCData"][$"ML{FixtureID}C1ES4_32_0"].ToObject<JObject>();
                            JToken jTkHomeRedCard = jSCData["SLData"][$"ML{FixtureID}C1ES4-0_32_0"];
                            if (jTkHomeRedCard != null && jTkHomeRedCard["D1"] != null)
                                HomeTeam.RedCard = jTkHomeRedCard["D1"].ToString();
                            JToken jTkAwayRedCard = jSCData["SLData"][$"ML{FixtureID}C1ES4-1_32_0"];
                            if (jTkAwayRedCard != null && jTkAwayRedCard["D1"] != null)
                                AwayTeam.RedCard = jTkAwayRedCard["D1"].ToString();
                        }                        

                        if (jESData["SCData"][$"ML{FixtureID}C1ES8_32_0"] != null)
                        {
                            JObject jSCData = jESData["SCData"][$"ML{FixtureID}C1ES8_32_0"].ToObject<JObject>();
                            JToken jTkHomePenalty = jSCData["SLData"][$"ML{FixtureID}C1ES8-0_32_0"];
                            if (jTkHomePenalty != null && jTkHomePenalty["D1"] != null)
                                HomeTeam.Penalty = jTkHomePenalty["D1"].ToString();
                            JToken jTkAwayPenalty = jSCData["SLData"][$"ML{FixtureID}C1ES8-1_32_0"];
                            if (jTkAwayPenalty != null && jTkAwayPenalty["D1"] != null)
                                AwayTeam.Penalty = jTkAwayPenalty["D1"].ToString();
                        }
                    }
                }
            }
            if (jObjData["MAData"] != null)
            {
                if (jObjData["MAData"][$"{FixtureID}C1-1777_32_0"] != null)
                {
                    JObject jMAData = jObjData["MAData"][$"{FixtureID}C1-1777_32_0"].ToObject<JObject>();
                    FullTime.Update(jMAData);
                }
                if (jObjData["MAData"][$"{FixtureID}C1-10147_32_0"] != null)
                {
                    JObject jMAData = jObjData["MAData"][$"{FixtureID}C1-10147_32_0"].ToObject<JObject>();
                    AsianHandicap.Update(jMAData);
                }
                if (jObjData["MAData"][$"{FixtureID}C1-10148_32_0"] != null)
                {
                    JObject jMAData = jObjData["MAData"][$"{FixtureID}C1-10148_32_0"].ToObject<JObject>();
                    GoalLine.Update(jMAData);
                }
            }
        }
    }
}
