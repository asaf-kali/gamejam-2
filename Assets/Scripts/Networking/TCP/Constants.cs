using UnityEngine.Analytics;

public class Constants
{
    public const int PORT = 8052;
    public const short BUFFER_SIZE = 256;
    public static readonly string SessionID = AnalyticsSessionInfo.sessionId.ToString();
}