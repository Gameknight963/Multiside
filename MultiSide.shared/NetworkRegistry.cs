namespace MultiSide.shared
{
    public static class NetworkRegistry
    {
        public static INetworkProvider? Provider { get; private set; }
        public static event Action<INetworkProvider>? OnProviderRegistered;

        public static void Register(INetworkProvider provider)
        {
            Provider = provider;
            OnProviderRegistered?.Invoke(provider);
        }
    }
}
