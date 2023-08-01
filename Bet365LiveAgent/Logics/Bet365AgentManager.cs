using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp.Server;
using Bet365LiveAgent.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bet365LiveAgent.Logics
{
    class Bet365AgentManager
    {

        public TelegramBotClient botClient = null;
        private BET365AGENT_STATUS _status = BET365AGENT_STATUS.Stoped;

        private WebSocketServer _webSocketServer = null;
        
        private List<SoccerMatchData> _matches = null;
        private object _lockMatches = new object();

        public Bet365DataReceivedHandler OnBet365DataReceived = null;

        private static Bet365AgentManager _instance = null;
        public static Bet365AgentManager Intance
        {
            get
            {
                if (_instance == null)
                    _instance = new Bet365AgentManager();
                return _instance;
            }
        }

        public Bet365AgentManager()
        {
            botClient = new TelegramBotClient("697407228:AAEjxMDFoZLA9N8DuEiUmaW-sG_lTOHiSZM");
        }

        public void Start()
        {
            try
            {
                //int offset = 0;
                //Update[] updates =  botClient.GetUpdatesAsync(offset).Result;
                //foreach (var update in updates)
                //{
                //    var message = update.Message;

                //    if (message.Text == null)
                //        continue;

                //    var chat = message.Chat;
                //    long chatId = chat.Id;

                //}

                if (_webSocketServer == null || !_webSocketServer.IsListening)
                {
                    Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.OUTDATA, "Bet365AgentManager Started.");
                    _status = BET365AGENT_STATUS.Started;
                    _webSocketServer = new WebSocketServer(Config.Instance.WebSockServerPort, false);
                    _webSocketServer.Start();
                    _matches = new List<SoccerMatchData>();
                    OnBet365DataReceived = ReceiveBet365Data;                    
                }
            }
            catch (Exception ex)
            {
                Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.OUTDATA, ex.ToString());
            }
        }

        public void Stop()
        {
            try
            {
                if (_webSocketServer != null && _webSocketServer.IsListening)
                {
                    Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.OUTDATA, "Bet365AgentManager Stoped.");
                    _status = BET365AGENT_STATUS.Stoped;
                    _webSocketServer.Stop();
                }
                _webSocketServer = null;
                _matches = null;
                OnBet365DataReceived = null;
            }
            catch (Exception ex)
            {
                Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.OUTDATA, ex.StackTrace);
            }
        }

        private void BroadCastMessage(string strMessage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strMessage))
                    return;

                if (_webSocketServer != null && _webSocketServer.IsListening)
                {
                    Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.OUTDATA, strMessage);

                    _webSocketServer.WebSocketServices.Broadcast(strMessage);
                }
            }
            catch (Exception ex)
            {
                Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.OUTDATA, ex.ToString());
            }
        }

        private void ReceiveBet365Data(string strData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strData))
                    return;


                JArray jArrData = ParseBet365Data(strData);
                if  (jArrData.Count > 0)
                {
                    Global.WriteLog(LOGLEVEL.FILE, LOGTYPE.OUTDATA, JsonConvert.SerializeObject(jArrData));
                    ProcessBet365Data(jArrData);
                }
            }
            catch (Exception ex)
            {
                Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.OUTDATA, ex.ToString());
            }
        }

        private JArray ParseBet365Data(string strData)
        {
            JArray jArrResult = new JArray();
            
            try
            {
                string[] packetsData = strData.Split(new string[] { Global.DELIM_MSG }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < packetsData.Length; i++)
                {
                    char packetType = packetsData[i].First();
                    switch (packetType)
                    {
                        case Global.INITIAL_TOPIC_LOAD:
                        case Global.DELTA:
                            string[] recordsData = packetsData[i].Split(new string[] { Global.DELIM_RECORD }, StringSplitOptions.RemoveEmptyEntries);
                            string[] headersData = recordsData[0].Split(new string[] { Global.DELIM_FIELD }, StringSplitOptions.RemoveEmptyEntries);
                            string strTopic = headersData[0].Substring(1);
                            string strMessage = packetsData[i].Substring(recordsData[0].Length + 1);
                            JObject jObjResult = ParseTopicData(packetType, strTopic, strMessage, headersData);
                            if (jObjResult.Count > 0)                            
                                jArrResult.Add(jObjResult);
                            break;
                        case Global.CLIENT_ABORT:
                            Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.INDATA, "Connnection abort!");
                            break;
                        case Global.CLIENT_CLOSE:
                            Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.INDATA, "Connnection close!");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.OUTDATA, ex.ToString());
            }

            return jArrResult;
        }

        private JObject ParseTopicData(char type, string topic, string message, string[] headers)
        {
            JObject jObjResult = new JObject();

            try
            {                                
                char msgType = message.First();
                string msgData = message.Substring(1);
                string[] stemsData = msgData.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                jObjResult["Topic"] = topic;
                jObjResult["Type"] = type;
                jObjResult["MsgType"] = msgType;
                switch (msgType)
                {
                    case 'F':
                    case 'I':
                        {
                            JObject jObjCS = null, jObjCL = null, jObjEV = null, jObjMA = null, jObjCO = null, jObjPA = null, 
                                jObjTG = null, jObjTE = null, jObjES = null, jObjSC = null, jObjSL = null, jObjSG = null, jObjST = null;
                            jObjResult["Data"] = new JObject();
                            for (int i = 0; i < stemsData.Length; i++)
                            {
                                JObject jObjStem = ParseStemData(stemsData[i]);
                                string stemType = jObjStem["Type"] == null ? "-" : jObjStem["Type"].ToString();
                                string stemIT = jObjStem["IT"] == null ? "-" : jObjStem["IT"].ToString();
                                switch (stemType)
                                {
                                    case "IN":
                                    case "CG":
                                        jObjResult["Data"] = jObjStem;
                                        break;
                                    case "CS":
                                        jObjCS = jObjStem;
                                        jObjResult["Data"][stemIT] = jObjStem;
                                        break;
                                    case "CL":
                                        jObjCL = jObjStem;
                                        if (jObjCS == null)
                                            jObjResult["Data"][stemIT] = jObjStem;
                                        else
                                        {
                                            if (jObjCS["CLData"] == null)
                                                jObjCS["CLData"] = new JObject();
                                            jObjCS["CLData"][stemIT] = jObjStem;
                                        }
                                        break;
                                    case "EV":
                                        jObjEV = jObjStem;
                                        if (jObjCL == null)
                                            jObjResult["Data"][stemIT] = jObjStem;
                                        else
                                        {
                                            if (jObjCL["EVData"] == null)
                                                jObjCL["EVData"] = new JObject();
                                            jObjCL["EVData"][stemIT] = jObjStem;
                                        }
                                        break;                                    
                                    case "TG":
                                        jObjTG = jObjStem;
                                        if (jObjEV == null)
                                            jObjResult["Data"][stemIT] = jObjStem;
                                        else
                                        {
                                            if (jObjEV["TGData"] == null)
                                                jObjEV["TGData"] = new JObject();
                                            jObjEV["TGData"][stemIT] = jObjStem;
                                        }
                                        break;
                                    case "TE":
                                        jObjTE = jObjStem;
                                        if (jObjTG == null)
                                            jObjResult["Data"][stemIT] = jObjStem;
                                        else
                                        {
                                            if (jObjTG["TEData"] == null)
                                                jObjTG["TEData"] = new JObject();
                                            jObjTG["TEData"][stemIT] = jObjStem;
                                        }
                                        break;
                                    case "ES":
                                        jObjES = jObjStem;
                                        if (jObjEV == null)
                                            jObjResult["Data"][stemIT] = jObjStem;
                                        else
                                        {
                                            if (jObjEV["ESData"] == null)
                                                jObjEV["ESData"] = new JObject();
                                            jObjEV["ESData"][stemIT] = jObjStem;
                                        }
                                        break;
                                    case "SC":
                                        jObjSC = jObjStem;
                                        if (jObjES == null)
                                            jObjResult["Data"][stemIT] = jObjStem;
                                        else
                                        {
                                            if (jObjES["SCData"] == null)
                                                jObjES["SCData"] = new JObject();
                                            jObjES["SCData"][stemIT] = jObjStem;
                                        }
                                        break;
                                    case "SL":
                                        jObjSL = jObjStem;
                                        if (jObjSC == null)
                                            jObjResult["Data"][stemIT] = jObjStem;
                                        else
                                        {
                                            if (jObjSC["SLData"] == null)
                                                jObjSC["SLData"] = new JObject();
                                            jObjSC["SLData"][stemIT] = jObjStem;
                                        }
                                        break;
                                    case "MA":
                                        jObjMA = jObjStem;
                                        if (jObjEV == null)
                                            jObjResult["Data"][stemIT] = jObjStem;
                                        else
                                        {
                                            if (jObjEV["MAData"] == null)
                                                jObjEV["MAData"] = new JObject();
                                            jObjEV["MAData"][stemIT] = jObjStem;
                                        }
                                        break;
                                    case "CO":
                                        jObjCO = jObjStem;
                                        if (jObjMA == null)
                                            jObjResult["Data"][stemIT] = jObjStem;
                                        else
                                        {
                                            if (jObjMA["COData"] == null)
                                                jObjMA["COData"] = new JObject();
                                            jObjMA["COData"][stemIT] = jObjStem;
                                        }
                                        break;
                                    case "PA":
                                        jObjPA = jObjStem;
                                        if (jObjMA == null)
                                            jObjResult["Data"][stemIT] = jObjStem;
                                        else
                                        {
                                            if (jObjMA["PAData"] == null)
                                                jObjMA["PAData"] = new JObject();
                                            jObjMA["PAData"][stemIT] = jObjStem;
                                        }
                                        break;
                                    case "SG":
                                        jObjSG = jObjStem;
                                        if (jObjEV == null)
                                            jObjResult["Data"][stemIT] = jObjStem;
                                        else
                                        {
                                            if (jObjEV["SGData"] == null)
                                                jObjEV["SGData"] = new JObject();
                                            jObjEV["SGData"][stemIT] = jObjStem;
                                        }
                                        break;
                                    case "ST":
                                        jObjST = jObjStem;
                                        if (jObjSG == null)
                                            jObjResult["Data"][stemIT] = jObjStem;
                                        else
                                        {
                                            if (jObjSG["STData"] == null)
                                                jObjSG["STData"] = new JObject();
                                            jObjSG["STData"][stemIT] = jObjStem;
                                        }
                                        break;
                                    default:
                                        jObjResult["Data"][stemIT] = jObjStem;
                                        break;
                                }
                            }
                        }
                        break;
                    case 'U':
                    case 'D':
                        {
                            jObjResult["Data"] = new JArray();
                            for (int i = 0; i < stemsData.Length; i++)
                            {
                                JObject jObjStem = ParseStemData(stemsData[i]);
                                ((JArray)jObjResult["Data"]).Add(jObjStem);
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.OUTDATA, ex.ToString());
            }

            return jObjResult;
        }

        private JObject ParseStemData(string data)
        {
            JObject jObjResult = new JObject();

            try
            {
                string[] props = data.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < props.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(props[i]))
                        continue;
                    if (props[i].Contains("="))
                    {                        
                        string[] fields = props[i].Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                        if (fields.Length == 2)
                            jObjResult[fields[0]] = fields[1];
                        else
                            jObjResult[fields[0]] = string.Empty;
                    }
                    else if(i == 0)
                    {
                        jObjResult["Type"] = props[i];
                    }
                }
            }
            catch (Exception ex)
            {
                Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.OUTDATA, ex.ToString());
            }
            
            return jObjResult;
        }
        
        private void ProcessBet365Data(JArray jArrData)
        {
            try
            {                
                lock (_lockMatches)
                {
                    List<SoccerMatchData> updatedSoccerMatches = new List<SoccerMatchData>();
                    foreach (JObject jObjData in jArrData)
                    {
                        string topic = jObjData["Topic"].ToString();
                        char type = jObjData["Type"].ToObject<char>();
                        char msgType = jObjData["MsgType"].ToObject<char>();
                        if (type == Global.INITIAL_TOPIC_LOAD)
                        {
                            if (topic.Equals("InPlay_32_0"))
                            {
                                JObject jObjCLsData = jObjData["Data"].ToObject<JObject>();
                                foreach (var jCLToken in jObjCLsData)
                                {
                                    JObject jObjCLData = jCLToken.Value.ToObject<JObject>();
                                    if (!"Soccer".Equals(jObjCLData["NA"].ToString()))
                                        continue;
                                    _matches.Clear();
                                    JObject jObjEVsData = jObjCLData["EVData"].ToObject<JObject>();
                                    foreach (var jEVToken in jObjEVsData)
                                    {
                                        JObject jObjEVData = jEVToken.Value.ToObject<JObject>();
                                        SoccerMatchData soccerMatchData = new SoccerMatchData();
                                        soccerMatchData.Update(jObjEVData);
                                        if (!string.IsNullOrWhiteSpace(soccerMatchData.FixtureID))
                                        {
                                            _matches.Add(soccerMatchData);
                                            Task.Delay(_matches.Count * 1500).ContinueWith(t => RequestMatchLiveData(soccerMatchData.EventID));
                                        }
                                    }
                                }
                            }
                            else if (topic.EndsWith("C1_32_0") || topic.EndsWith("M1_32_0"))
                            {
                                JObject jObjEVsData = jObjData["Data"].ToObject<JObject>();
                                foreach (var jEVToken in jObjEVsData)
                                {
                                    JObject jObjEVData = jEVToken.Value.ToObject<JObject>();
                                    string strFI = jObjEVData["FI"].ToString();
                                    SoccerMatchData soccerMatchData = _matches.Find(m => m.FixtureID == strFI);
                                    if (soccerMatchData == null)
                                        soccerMatchData = new SoccerMatchData();

                                    soccerMatchData.Update(jObjEVData);                                
                                    SoccerMatchData updatedSoccerMatch = updatedSoccerMatches.Find(m => m.FixtureID == strFI);
                                    if (updatedSoccerMatch == null)
                                        updatedSoccerMatches.Add(soccerMatchData);
                                    else
                                        updatedSoccerMatch = soccerMatchData;
                                }
                            }
                        }
                        else if (type == Global.DELTA)
                        {
                            SoccerMatchData soccerMatchData = null;
                            if (msgType == 'I')
                            {
                                string[] stemTopics = topic.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                                if (stemTopics.Length > 0)
                                {
                                    bool flag = false;
                                    JObject jObjStemsData = jObjData["Data"].ToObject<JObject>();
                                    string str = jObjData["Data"].ToString();
                                    foreach (var jStemToken in jObjStemsData)
                                    {
                                        JObject jObjStemData = jStemToken.Value.ToObject<JObject>();
                                        string strType = jObjStemData["Type"] == null ? "" : jObjStemData["Type"].ToString();
                                        if (strType == "PA")
                                        {
                                            if (stemTopics[0].EndsWith("C1-1777-1_32_0"))
                                            {
                                                string strFI = stemTopics[0].Replace("C1-1777-1_32_0", "");
                                                soccerMatchData = _matches.Find(m => m.FixtureID == strFI);
                                                if (soccerMatchData != null)
                                                {
                                                    soccerMatchData.FullTime.Update(jObjStemData);
                                                }
                                            }
                                            else if (stemTopics[0].EndsWith("C1-10147-1_32_0"))
                                            {
                                                string strFI = stemTopics[0].Replace("C1-10147-1_32_0", "");
                                                soccerMatchData = _matches.Find(m => m.FixtureID == strFI);
                                                if (soccerMatchData != null)
                                                {
                                                    soccerMatchData.AsianHandicap.Update(jObjStemData);
                                                }
                                            }
                                            else if (stemTopics[0].EndsWith("C1-10148-1_32_0"))
                                            {
                                                string strFI = stemTopics[0].Replace("C1-10148-1_32_0", "");
                                                soccerMatchData = _matches.Find(m => m.FixtureID == strFI);
                                                if (soccerMatchData != null)
                                                {
                                                    soccerMatchData.GoalLine.Update(jObjStemData);
                                                }
                                            }
                                            else if (stemTopics[0].EndsWith("C1-10560-1_32_0"))
                                            {
                                                flag = true;
                                            }
                                        }
                                    }
                                }                                
                            }
                            else if (msgType == 'U')
                            {
                                JArray jArrDeltaData = jObjData["Data"].ToObject<JArray>();
                                if (topic.EndsWith("M1_32_0"))
                                {
                                    string strEventID = topic.TrimEnd("M1_32_0");
                                    soccerMatchData = _matches.Find(m => m.EventID == strEventID);
                                    if (soccerMatchData != null)
                                    {
                                        foreach (var jDeltaToken in jArrDeltaData)
                                        {
                                            if (jObjData["TU"] != null)
                                                soccerMatchData.TU = jObjData["TU"].ToString();
                                            if (jObjData["TM"] != null)
                                                soccerMatchData.TM = jObjData["TM"].ToString();
                                            if (jDeltaToken["SS"] != null)
                                                soccerMatchData.Score = jDeltaToken["SS"].ToString();
                                            if (jDeltaToken["TA"] != null)
                                                soccerMatchData.TA = jDeltaToken["TA"].ToString();
                                            if (jDeltaToken["VC"] != null)
                                                soccerMatchData.VC = jDeltaToken["VC"].ToString();
                                            if (jDeltaToken["XY"] != null)
                                            {
                                                string XYPos = jDeltaToken["XY"].ToString();
                                                if (!string.IsNullOrWhiteSpace(XYPos))
                                                {
                                                    soccerMatchData.XPos = XYPos.Substring(0, XYPos.IndexOf(","));
                                                    soccerMatchData.YPos = XYPos.Substring(soccerMatchData.XPos.Length + 1);
                                                }
                                            }
                                        }                                        
                                    }
                                }
                                else if (topic.EndsWith("C1T1_32_0"))
                                {
                                    string strFixtureID = topic.TrimStart("ML");
                                    strFixtureID = strFixtureID.TrimEnd("C1T1_32_0");
                                    soccerMatchData = _matches.Find(m => m.FixtureID == strFixtureID);
                                    if (soccerMatchData != null)
                                    {
                                        foreach (var jDeltaToken in jArrDeltaData)
                                        {
                                            soccerMatchData.HomeTeam.Update(jDeltaToken.ToObject<JObject>());
                                        }
                                    }
                                }
                                else if (topic.EndsWith("C1T2_32_0"))
                                {
                                    string strFixtureID = topic.TrimStart("ML");
                                    strFixtureID = strFixtureID.TrimEnd("C1T2_32_0");
                                    soccerMatchData = _matches.Find(m => m.FixtureID == strFixtureID);
                                    if (soccerMatchData != null)
                                    {
                                        foreach (var jDeltaToken in jArrDeltaData)
                                        {
                                            soccerMatchData.AwayTeam.Update(jDeltaToken.ToObject<JObject>());
                                        }
                                    }
                                }
                                else if (topic.EndsWith("C1ES1-0_32_0"))
                                {
                                    string strFixtureID = topic.TrimStart("ML");
                                    strFixtureID = strFixtureID.TrimEnd("C1ES1-0_32_0");
                                    soccerMatchData = _matches.Find(m => m.FixtureID == strFixtureID);
                                    if (soccerMatchData != null)
                                    {
                                        foreach (var jDeltaToken in jArrDeltaData)
                                        {
                                            if (jDeltaToken["D1"] != null)
                                                soccerMatchData.HomeTeam.Score = jDeltaToken["D1"].ToString();
                                        }
                                    }
                                }
                                else if (topic.EndsWith("C1ES1-1_32_0"))
                                {
                                    string strFixtureID = topic.TrimStart("ML");
                                    strFixtureID = strFixtureID.TrimEnd("C1ES1-1_32_0");
                                    soccerMatchData = _matches.Find(m => m.FixtureID == strFixtureID);
                                    if (soccerMatchData != null)
                                    {
                                        foreach (var jDeltaToken in jArrDeltaData)
                                        {
                                            if (jDeltaToken["D1"] != null)
                                                soccerMatchData.AwayTeam.Score = jDeltaToken["D1"].ToString();
                                        }
                                    }
                                }
                                else if (topic.EndsWith("C1ES2-0_32_0"))
                                {
                                    string strFixtureID = topic.TrimStart("ML");
                                    strFixtureID = strFixtureID.TrimEnd("C1ES2-0_32_0");
                                    soccerMatchData = _matches.Find(m => m.FixtureID == strFixtureID);
                                    if (soccerMatchData != null)
                                    {
                                        foreach (var jDeltaToken in jArrDeltaData)
                                        {
                                            if (jDeltaToken["D1"] != null)
                                                soccerMatchData.HomeTeam.Corner = jDeltaToken["D1"].ToString();
                                        }
                                    }
                                }
                                else if (topic.EndsWith("C1ES2-1_32_0"))
                                {
                                    string strFixtureID = topic.TrimStart("ML");
                                    strFixtureID = strFixtureID.TrimEnd("C1ES2-1_32_0");
                                    soccerMatchData = _matches.Find(m => m.FixtureID == strFixtureID);
                                    if (soccerMatchData != null)
                                    {
                                        foreach (var jDeltaToken in jArrDeltaData)
                                        {
                                            if (jDeltaToken["D1"] != null)
                                                soccerMatchData.AwayTeam.Corner = jDeltaToken["D1"].ToString();
                                        }
                                    }
                                }
                                else if (topic.EndsWith("C1ES3-0_32_0"))
                                {
                                    string strFixtureID = topic.TrimStart("ML");
                                    strFixtureID = strFixtureID.TrimEnd("C1ES3-0_32_0");
                                    soccerMatchData = _matches.Find(m => m.FixtureID == strFixtureID);
                                    if (soccerMatchData != null)
                                    {
                                        foreach (var jDeltaToken in jArrDeltaData)
                                        {
                                            if (jDeltaToken["D1"] != null)
                                                soccerMatchData.HomeTeam.YellowCard = jDeltaToken["D1"].ToString();
                                        }
                                    }
                                }
                                else if (topic.EndsWith("C1ES3-1_32_0"))
                                {
                                    string strFixtureID = topic.TrimStart("ML");
                                    strFixtureID = strFixtureID.TrimEnd("C1ES3-1_32_0");
                                    soccerMatchData = _matches.Find(m => m.FixtureID == strFixtureID);
                                    if (soccerMatchData != null)
                                    {
                                        foreach (var jDeltaToken in jArrDeltaData)
                                        {
                                            if (jDeltaToken["D1"] != null)
                                                soccerMatchData.AwayTeam.YellowCard = jDeltaToken["D1"].ToString();
                                        }
                                    }
                                }
                                else if (topic.EndsWith("C1ES4-0_32_0"))
                                {
                                    string strFixtureID = topic.TrimStart("ML");
                                    strFixtureID = strFixtureID.TrimEnd("C1ES4-0_32_0");
                                    soccerMatchData = _matches.Find(m => m.FixtureID == strFixtureID);
                                    if (soccerMatchData != null)
                                    {
                                        foreach (var jDeltaToken in jArrDeltaData)
                                        {
                                            if (jDeltaToken["D1"] != null)
                                                soccerMatchData.HomeTeam.RedCard = jDeltaToken["D1"].ToString();
                                        }
                                    }
                                }
                                else if (topic.EndsWith("C1ES4-1_32_0"))
                                {
                                    string strFixtureID = topic.TrimStart("ML");
                                    strFixtureID = strFixtureID.TrimEnd("C1ES4-1_32_0");
                                    soccerMatchData = _matches.Find(m => m.FixtureID == strFixtureID);
                                    if (soccerMatchData != null)
                                    {
                                        foreach (var jDeltaToken in jArrDeltaData)
                                        {
                                            if (jDeltaToken["D1"] != null)
                                                soccerMatchData.AwayTeam.RedCard = jDeltaToken["D1"].ToString();
                                        }
                                    }
                                }
                                else if (topic.EndsWith("C1ES8-0_32_0"))
                                {
                                    string strFixtureID = topic.TrimStart("ML");
                                    strFixtureID = strFixtureID.TrimEnd("C1ES8-0_32_0");
                                    soccerMatchData = _matches.Find(m => m.FixtureID == strFixtureID);
                                    if (soccerMatchData != null)
                                    {
                                        foreach (var jDeltaToken in jArrDeltaData)
                                        {
                                            if (jDeltaToken["D1"] != null)
                                                soccerMatchData.HomeTeam.Penalty = jDeltaToken["D1"].ToString();
                                        }
                                    }
                                }
                                else if (topic.EndsWith("C1ES8-1_32_0"))
                                {
                                    string strFixtureID = topic.TrimStart("ML");
                                    strFixtureID = strFixtureID.TrimEnd("C1ES8-1_32_0");
                                    soccerMatchData = _matches.Find(m => m.FixtureID == strFixtureID);
                                    if (soccerMatchData != null)
                                    {
                                        foreach (var jDeltaToken in jArrDeltaData)
                                        {
                                            if (jDeltaToken["D1"] != null)
                                                soccerMatchData.AwayTeam.Penalty = jDeltaToken["D1"].ToString();
                                        }
                                    }
                                }
                                else if (topic.StartsWith("P") && topic.EndsWith("_32_0"))
                                {
                                    SoccerMatchData soccerMatchDataForPA = _matches.Find(m => m.FullTime.HomeIT == topic || m.FullTime.DrawIT == topic || m.FullTime.AwayIT == topic);
                                    if (soccerMatchDataForPA != null)
                                    {
                                        foreach (var jDeltaToken in jArrDeltaData)
                                        {
                                            if (soccerMatchDataForPA.FullTime.HomeIT == topic)
                                            {
                                                if (jDeltaToken["OD"] != null)
                                                    soccerMatchDataForPA.FullTime.HomeOD = jDeltaToken["OD"].ToString();
                                            }
                                            else if (soccerMatchDataForPA.FullTime.AwayIT == topic)
                                            {
                                                if (jDeltaToken["OD"] != null)
                                                    soccerMatchDataForPA.FullTime.AwayOD = jDeltaToken["OD"].ToString();
                                            }
                                            else
                                            {
                                                if (jDeltaToken["OD"] != null)
                                                    soccerMatchDataForPA.FullTime.DrawOD = jDeltaToken["OD"].ToString();
                                            }
                                        }
                                        soccerMatchData = soccerMatchDataForPA;
                                    }
                                    soccerMatchDataForPA = _matches.Find(m => m.AsianHandicap.HomeIT == topic || m.AsianHandicap.AwayIT == topic);
                                    if (soccerMatchDataForPA != null)
                                    {
                                        foreach (var jDeltaToken in jArrDeltaData)
                                        {
                                            if (soccerMatchDataForPA.AsianHandicap.HomeIT == topic)
                                            {
                                                if (jDeltaToken["HA"] != null)
                                                    soccerMatchDataForPA.AsianHandicap.HomeHdp = jDeltaToken["HA"].ToString();
                                                if (jDeltaToken["OD"] != null)
                                                    soccerMatchDataForPA.AsianHandicap.HomeOD = jDeltaToken["OD"].ToString();
                                            }
                                            else
                                            {
                                                if (jDeltaToken["HA"] != null)
                                                    soccerMatchDataForPA.AsianHandicap.AwayHdp = jDeltaToken["HA"].ToString();
                                                if (jDeltaToken["OD"] != null)
                                                    soccerMatchDataForPA.AsianHandicap.AwayOD = jDeltaToken["OD"].ToString();
                                            }
                                        }
                                        soccerMatchData = soccerMatchDataForPA;
                                    }
                                    soccerMatchDataForPA = _matches.Find(m => m.GoalLine.OverIT == topic || m.GoalLine.UnderIT == topic);
                                    if (soccerMatchDataForPA != null)
                                    {
                                        foreach (var jDeltaToken in jArrDeltaData)
                                        {
                                            if (soccerMatchDataForPA.GoalLine.OverIT == topic)
                                            {
                                                if (jDeltaToken["HA"] != null)
                                                    soccerMatchDataForPA.GoalLine.OverHdp = jDeltaToken["HA"].ToString();
                                                if (jDeltaToken["OD"] != null)
                                                    soccerMatchDataForPA.GoalLine.OverOD = jDeltaToken["OD"].ToString();
                                            }
                                            else
                                            {
                                                if (jDeltaToken["HA"] != null)
                                                    soccerMatchDataForPA.GoalLine.UnderHdp = jDeltaToken["HA"].ToString();
                                                if (jDeltaToken["OD"] != null)
                                                    soccerMatchDataForPA.GoalLine.UnderOD = jDeltaToken["OD"].ToString();
                                            }
                                        }
                                        soccerMatchData = soccerMatchDataForPA;
                                    }
                                }
                            }
                            else if (msgType == 'D')
                            {
                                if (topic.EndsWith("M1_32_0"))
                                {
                                    string strEventID = topic.TrimEnd("M1_32_0");
                                    SoccerMatchData soccerMatchDataToDel = _matches.Find(m => m.EventID == strEventID);
                                    if (soccerMatchDataToDel != null)
                                    {
                                        Global.WriteLog(LOGLEVEL.FILE, LOGTYPE.INDATA, $"Removed Match Data: EventID={soccerMatchDataToDel.EventID}, FixtureID={soccerMatchDataToDel.FixtureID}, Name='{soccerMatchDataToDel.HomeName} vs {soccerMatchDataToDel.AwayName}'");
                                        _matches.Remove(soccerMatchDataToDel);
                                    }
                                }
                                else if (topic.EndsWith("C1-10560_32_0"))
                                {
                                    string[] stemTopics = topic.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                                    string strFI1 = stemTopics[1].Replace("C1-10560_32_0", "");
                                    soccerMatchData = _matches.Find(m => m.FixtureID == strFI1);
                                    if (soccerMatchData != null)
                                    {
                                        soccerMatchData.halfTabFlag = 1;
                                    }

                                }
                            }

                            if (soccerMatchData != null)
                            {
                                SoccerMatchData updatedSoccerMatch = updatedSoccerMatches.Find(m => m.FixtureID == soccerMatchData.FixtureID);
                                if (updatedSoccerMatch == null)
                                    updatedSoccerMatches.Add(soccerMatchData);
                                else
                                    updatedSoccerMatch = soccerMatchData;
                            }
                        }
                    }
                    if (updatedSoccerMatches.Count > 0)
                    {
                        foreach(SoccerMatchData matchData in updatedSoccerMatches)
                        {
                            string time = matchData.Time;

                            string timeStr = string.Empty;
                            if (time.Contains(":"))
                            {
                                string[] arr = time.Split(':');
                                timeStr = arr[0];
                            }

                            string halfHour = matchData.TM;
                            double matchHour = Int32.Parse(timeStr) + Int32.Parse(halfHour);
                            if(matchHour < 41 && matchData.halfTabFlag == 1)
                            {
                                JObject resultObj = new JObject();
                                resultObj["leagueName"] = matchData.LeagueName;
                                resultObj["homeTeam"] = matchData.HomeName;
                                resultObj["awayTeam"] = matchData.AwayName;
                                resultObj["score"] = matchData.Score;
                                resultObj["status"] = "Half time is finsihed just";

                                string jsonStr = resultObj.ToString();
                                sendMessage(jsonStr);
                            }
                        }
                        BroadCastMessage(JsonConvert.SerializeObject(updatedSoccerMatches));
                    }
                        
                }                
            }
            catch (Exception ex)
            {
                Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.OUTDATA, ex.ToString());
            }
        }

        private void RequestMatchLiveData(string eventId)
        {            
            string strReqMsg = $"{Global.CLIENT_SUBSCRIBE}{Global.NONE_ENCODING}{eventId}C1_32_0{Global.DELIM_RECORD}";
            if (Bet365ClientManager.Intance.OnBet365DataSend != null)
                Bet365ClientManager.Intance.OnBet365DataSend(strReqMsg);

            Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.INDATA, $"Sent MatchLiveData Request 1 >>> {strReqMsg}");

            Task.Delay(500).ContinueWith(_ => {
                strReqMsg = $"{Global.CLIENT_SUBSCRIBE}{Global.NONE_ENCODING}{eventId}M1_32_0{Global.DELIM_RECORD}";
                if (Bet365ClientManager.Intance.OnBet365DataSend != null)
                    Bet365ClientManager.Intance.OnBet365DataSend(strReqMsg);

                Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.INDATA, $"Sent MatchLiveData Request 2 >>> {strReqMsg}");
            });
        }

        private void sendMessage(string text)
        {
            try
            {
                ChatId chatId = new ChatId(891122809);
                Telegram.Bot.Types.Message msg = botClient.SendTextMessageAsync(chatId, text).Result;
            }
            catch(Exception e)
            {

            }
            
           
        }
    }
}
