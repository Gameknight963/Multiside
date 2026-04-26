namespace MultiSide.shared
{
    public static class NetworkRegistry
    {
        public static INetworkProvider? Provider { get; private set; }

        public static void Register(INetworkProvider provider)
        {
            Provider = provider;
        }
    }
}
