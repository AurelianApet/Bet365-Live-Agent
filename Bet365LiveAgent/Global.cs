using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bet365LiveAgent
{
    public enum LOGLEVEL : byte
    {
        FILE = 0,
        NOTICE,
        FULL
    }

    public enum LOGTYPE : byte
    {
        INDATA = 0,
        OUTDATA
    }

    public enum BET365CLIENT_STATUS : byte
    {
        Disconnected = 0,
        Connecting,
        Connected
    }

    public enum BET365AGENT_STATUS : byte
    {
        Stoped = 0,
        Started
    }

    public delegate void WriteLogDelegate(LOGLEVEL logLevel, LOGTYPE logType, string strLog);

    public delegate void Bet365DataReceivedHandler(string strData);

    public delegate void Bet365DataSendHandler(string strData);

    static class Global
    {
        public const string ConfigFileName = "config.xml";

        public static string LogFilePath = $"{Application.StartupPath}\\logs\\";

        public static string LogFileName = "";

        public static WriteLogDelegate WriteLog = null;


        public const string DELIM_RECORD = "\u0001";
        public const string DELIM_FIELD = "\u0002";
        public const string DELIM_HANDSHAKE_MSG = "\u0003";
        public const string DELIM_MSG = "\u0008";
        public const char CLIENT_CONNECT = (char)0;
        public const char CLIENT_POLL = (char)1;
        public const char CLIENT_SEND = (char)2;
        public const char CLIENT_CONNECT_FAST = (char)3;
        public const char INITIAL_TOPIC_LOAD = (char)20;
        public const char DELTA = (char)21;
        public const char CLIENT_SUBSCRIBE = (char)22;
        public const char CLIENT_UNSUBSCRIBE = (char)23;
        public const char CLIENT_SWAP_SUBSCRIPTIONS = (char)26;
        public const char NONE_ENCODING = (char)0;
        public const char ENCRYPTED_ENCODING = (char)17;
        public const char COMPRESSED_ENCODING = (char)18;
        public const char BASE64_ENCODING = (char)19;
        public const char SERVER_PING = (char)24;
        public const char CLIENT_PING = (char)25;
        public const char CLIENT_ABORT = (char)28;
        public const char CLIENT_CLOSE = (char)29;
        public const char ACK_ITL = (char)30;
        public const char ACK_DELTA = (char)31;
        public const char ACK_RESPONSE = (char)32;
        public const char HANDSHAKE_PROTOCOL = (char)35;//'#';
        public const char HANDSHAKE_VERSION = (char)3;
        public const char HANDSHAKE_CONNECTION_TYPE = (char)80;//'P';
        public const char HANDSHAKE_CAPABILITIES_FLAG = (char)1;
        public const string HANDSHAKE_STATUS_CONNECTED = "100";
        public const string HANDSHAKE_STATUS_REJECTED = "111";
    }
}
