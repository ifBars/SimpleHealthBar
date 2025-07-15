using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.UI;
using Il2CppScheduleOne.UI.Phone;
using MelonLoader;
using SimpleHealthBar.UI;

namespace SimpleHealthBar.PlayerUtils
{
    class PlayerHealthBarManager
    {
        private static MelonLogger.Instance Logger;
        private static PlayerHealthBar PlayerHealthBar;
        private static bool HasInitialized = false;

        public static void Init(MelonLogger.Instance logger)
        {
            Logger = logger;
            PlayerHealthBar = new PlayerHealthBar();
            PlayerHealthBar.SetPlayer(Il2CppScheduleOne.PlayerScripts.Player.Local);
            PlayerHealthBar.Init(HUD.Instance.transform);
            HasInitialized = true;
            Logger.Msg("Player Healthbar Initialized!");
        }

        public static void OnUpdate()
        {
            if (!HasInitialized)
                return;
            Il2CppScheduleOne.PlayerScripts.Player player = PlayerHealthBar.GetPlayer();
            Phone phone = PlayerSingleton<Phone>.Instance;
            bool phoneOpen = phone != null && phone.IsOpen;
            if(PlayerHealthBar != null || player != null)
            {
                if (PlayerHealthBar.GetPlayerHealth() != PlayerHealthBar.GetDisplayedHealth())
                {
                    PlayerHealthBar.UpdateText();
                    PlayerHealthBar.Show();
                }
                PlayerHealthBar.Update(phoneOpen);
            }
        }

        public static void Unload()
        {
            HasInitialized = false;
            PlayerHealthBar = null;
        }

        public static bool Initialized() { return HasInitialized; }
    }
}
