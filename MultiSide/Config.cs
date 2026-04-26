using Photon.Client;
using Photon.Realtime;

namespace MultiSide
{
    public static class Config
    {
        public static string DefaultRoom = "KoolRoom";
        public static AppSettings settings = new AppSettings
        {
            AppIdRealtime = "301df3f0-b282-4a33-b588-60e22cdf7d87",
            AppVersion = "0.1",
            FixedRegion = "us",
            Protocol = ConnectionProtocol.Udp,
            NetworkLogging = LogLevel.Info
        };
    }
}
