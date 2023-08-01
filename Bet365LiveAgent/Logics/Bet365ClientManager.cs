using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Bet365LiveAgent.Logics
{
    class Bet365ClientManager
    {
        private Thread _runner = null;

        private BET365CLIENT_STATUS _status = BET365CLIENT_STATUS.Disconnected;

        private HttpClient _httpClient = null;
        private CookieContainer _cookieContainer = null;
        private WebSocket _webSocket = null;
        private string _webSocketHost = null;
        private string _webSocketPort = null;
        private string _cookieToken = null;

        public Bet365DataSendHandler OnBet365DataSend = null;

        private static Bet365ClientManager _instance = null;
        public static Bet365ClientManager Intance
        {
            get
            {
                if (_instance == null)
                    _instance = new Bet365ClientManager();
                return _instance;
            }
        }

        public Bet365ClientManager()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            _cookieContainer = new CookieContainer();
            handler.CookieContainer = _cookieContainer;

            _httpClient = new HttpClient(handler);

            ServicePointManager.DefaultConnectionLimit = 2;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.9");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Linux; Android 6.0; SAMSUNG SM-G930F Build/MMB29K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Mobile Safari/537.36");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");
            _httpClient.DefaultRequestHeaders.ExpectContinue = true;
        }

        public void Start()
        {
            try
            {
                if (_runner == null)
                {
                    Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.INDATA, "Bet365ClientManager Started.");
                    _status = BET365CLIENT_STATUS.Connecting;
                    _runner = new Thread(() => Run());
                    _runner.Start();
                }
            }
            catch (Exception ex)
            {
                Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.INDATA, ex.ToString());
            }            
        }

        public void Stop()
        {
            try
            {
                if (_runner != null)
                {
                    Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.INDATA, "Bet365ClientManager Stoped.");
                    _status = BET365CLIENT_STATUS.Disconnected;
                    _runner.Abort();
                    _runner = null;
                }
                if (_webSocket != null)
                {
                    _webSocket.Close();
                    _webSocket = null;
                }
            }
            catch (Exception ex)
            {
                Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.INDATA, ex.ToString());
            }            
        }

        public void Run()
        {
            try
            {
                Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.INDATA, "Bet365ClientManager Connecting...");
                _status = BET365CLIENT_STATUS.Connecting;

                string strUrl = "https://mobile.bet365.com";
                HttpResponseMessage httpResponse = _httpClient.GetAsync(strUrl).Result;
                httpResponse.EnsureSuccessStatusCode();

                string strResponse = httpResponse.Content.ReadAsStringAsync().Result;
                var cookies = _cookieContainer.GetCookies(new Uri("https://mobile.bet365.com")).Cast<System.Net.Cookie>();
                foreach (var cookie in cookies)
                {
                    if (cookie.Name == "pstk")
                    {
                        _cookieToken = cookie.Value;
                    }
                }

                string strWebSockInfo = "{\"Host\":\"wss://(?<Host>[^\"]*)\",\"Port\":(?<Port>[^\\,]*),\"TransportMethod\":(?<Transport>[^\\,]*),\"DefaultTopic\":\"(?<Default>[^\"]*)\"}";
                Match math = Regex.Match(strResponse, strWebSockInfo);
                _webSocketHost = math.Groups["Host"].Value;
                _webSocketPort = math.Groups["Port"].Value;
                if (string.IsNullOrEmpty(_webSocketHost) || string.IsNullOrEmpty(_webSocketPort))
                {
                    string strLog = "Couldn't get bet365 websocket connection info";
                    Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.INDATA, strLog);
                    return;
                }

                Global.WriteLog(LOGLEVEL.FILE, LOGTYPE.INDATA, $"Websocket Connection Info: Host = {_webSocketHost}, Port = {_webSocketPort}");

                Thread.Sleep(1000);
                string url = "https://mobile.bet365.com/Default.aspx?ot=2&apptype=&appversion=";
                httpResponse = _httpClient.GetAsync(url).Result;
                httpResponse.EnsureSuccessStatusCode();

                strResponse = httpResponse.Content.ReadAsStringAsync().Result;
                string strWsHost1 = "premws-pt11.365lpodds.com";
                string strWsHost2 = "premws-pt12.365lpodds.com";
                string strWsHost3 = "premws-pt13.365lpodds.com";
                string strWsUrl1 = $"wss://{strWsHost1}/zap/?uid={Utils.GenerateRandomNumberString(16)}";
                string strWsUrl2 = $"wss://{strWsHost2}/zap/?uid={Utils.GenerateRandomNumberString(16)}";
                string strWsUrl3 = $"wss://{strWsHost3}/zap/?uid={Utils.GenerateRandomNumberString(16)}";
                
                string strWsHost = string.Empty;
                string strWsUrl = string.Empty;

                if (strResponse.Contains($"wss://{strWsHost1}"))
                {
                    strWsHost = strWsHost1;
                    strWsUrl = strWsUrl1;
                }
                if (strResponse.Contains($"wss://{strWsHost2}"))
                {
                    strWsHost = strWsHost2;
                    strWsUrl = strWsUrl2;
                }
                if (strResponse.Contains($"wss://{strWsHost3}"))
                {
                    strWsHost = strWsHost3;
                    strWsUrl = strWsUrl3;
                }
                List<KeyValuePair<string, string>> webSockCustomHeaders = new List<KeyValuePair<string, string>>();
                webSockCustomHeaders.Add(new KeyValuePair<string, string>("Accept-Encoding", "gzip, deflate, br"));
                webSockCustomHeaders.Add(new KeyValuePair<string, string>("Accept-Language", "en-US,en;q=0.9"));
                webSockCustomHeaders.Add(new KeyValuePair<string, string>("Cache-Control", "no-cache"));
                webSockCustomHeaders.Add(new KeyValuePair<string, string>("Connection", "Upgrade"));                
                webSockCustomHeaders.Add(new KeyValuePair<string, string>("Host", strWsHost));
                webSockCustomHeaders.Add(new KeyValuePair<string, string>("Origin", "https://mobile.bet365.com"));
                webSockCustomHeaders.Add(new KeyValuePair<string, string>("Pragma", "no-cache"));
                webSockCustomHeaders.Add(new KeyValuePair<string, string>("Sec-WebSocket-Version", "13"));
                webSockCustomHeaders.Add(new KeyValuePair<string, string>("Upgrade", "websocket"));
                webSockCustomHeaders.Add(new KeyValuePair<string, string>("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36"));
                _webSocket = new WebSocket(strWsUrl, "zap-protocol-v1");
                _webSocket.EmitOnPing = true;
                _webSocket.CustomHeaders = webSockCustomHeaders;
                _webSocket.Origin = "https://mobile.bet365.com";
                _webSocket.Compression = CompressionMethod.Deflate;                
                _webSocket.Log.Level = LogLevel.Error;
                _webSocket.Log.File = $"{Global.LogFilePath}{Global.LogFileName}";
                _webSocket.OnOpen += Socket_OnOpen;
                _webSocket.OnClose += Socket_OnClose;
                _webSocket.OnError += Socket_OnError;
                _webSocket.OnMessage += Socket_OnHandshake;
                while (true)
                {
                    if (_webSocket != null && _webSocket.ReadyState != WebSocketState.Open)
                    {
                        _webSocket.Connect();
                        Thread.Sleep(5 * 1000);
                    }
                }
            }
            catch(Exception ex)
            {
                Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.INDATA, ex.StackTrace);
            }
        }

        private void Socket_OnOpen(object sender, EventArgs e)
        {
            Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.INDATA, $"Websocket connected");
            
            string handshakePacket = $"{Global.HANDSHAKE_PROTOCOL}{Global.HANDSHAKE_VERSION}{Global.HANDSHAKE_CONNECTION_TYPE}{Global.HANDSHAKE_CAPABILITIES_FLAG}__time,S_{_cookieToken}{(char)0}";
            _webSocket.Send(handshakePacket);

            Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.INDATA, $"Websocket sent handshake : {handshakePacket}");
        }

        private void Socket_OnClose(object sender, CloseEventArgs e)
        {
            Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.INDATA, $"Websocket closed : {e.Reason}");

            OnBet365DataSend = null;

            _status = BET365CLIENT_STATUS.Disconnected;
        }

        private void Socket_OnError(object sender, ErrorEventArgs e)
        {
            Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.INDATA, $"Websocket error : {e.Message}");
        }

        private void Socket_OnHandshake(object sender, MessageEventArgs e)
        {
            Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.INDATA, $"Websocket received handshake : {e.Data}");

            _webSocket.OnMessage -= Socket_OnHandshake;
            _webSocket.OnMessage += Socket_OnMessage;
            OnBet365DataSend = SendBet365Data;

            string configPacket = $"{Global.CLIENT_SUBSCRIBE}{Global.NONE_ENCODING}CONFIG_32_0,InPlay_32_0,LIInPlay_32_0{Global.DELIM_RECORD}";
            _webSocket.Send(configPacket);

            _status = BET365CLIENT_STATUS.Connected;
        }

        private void Socket_OnMessage(object sender, MessageEventArgs e)
        {
            Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.INDATA, e.Data);

            if (Bet365AgentManager.Intance.OnBet365DataReceived != null)
                Bet365AgentManager.Intance.OnBet365DataReceived(e.Data);
        }

        private void SendBet365Data(string strData)
        {
            try
            {
                _webSocket.Send(strData);
            }
            catch (Exception ex)
            {
                Global.WriteLog(LOGLEVEL.FULL, LOGTYPE.INDATA, ex.ToString());
            }
        }
    }
}
