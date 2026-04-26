using MultiSide.shared;

namespace MultiSide
{
    public static class ModListChecker
    {
        public static readonly string Channel = "multiside.modlist";

        public static void Init()
        {
            if (NetworkRegistry.Provider == null) return;
            NetworkRegistry.Provider.OnReceived += OnReceived;
            NetworkRegistry.Provider.OnPlayerJoined += OnPlayerJoined;
        }

        private static void OnPlayerJoined(int actor)
        {
            NetworkRegistry.Provider?.SendTo(actor, Channel, HelperFunctions.GetModList());
        }

        private static void OnReceived(int actor, string channel, object data)
        {
            if (channel != Channel) return;
            string[] theirMods = (string[])data;
            string[] myMods = HelperFunctions.GetModList();

            string[] theyHaveMissing = myMods.Except(theirMods).ToArray();
            string[] iAmMissing = theirMods.Except(myMods).ToArray();

            if (!theyHaveMissing.Any() && !iAmMissing.Any()) return;

            ModController.CoolLogger.Warning($"Mod mismatch with player {actor}!");
            if (iAmMissing.Any())
                ModController.CoolLogger.Warning($"  You are missing: {string.Join(", ", iAmMissing)}");
            if (theyHaveMissing.Any())
                ModController.CoolLogger.Warning($"  They are missing: {string.Join(", ", theyHaveMissing)}");
        }
    }
}
