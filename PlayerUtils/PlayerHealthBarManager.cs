#if MONO
using ScheduleOne.PlayerScripts;
using ScheduleOne.DevUtilities;
using ScheduleOne.UI;
using ScheduleOne.UI.Phone;
#else
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.UI;
using Il2CppScheduleOne.UI.Phone;
using Il2CppScheduleOne.PlayerScripts;
#endif
using MelonLoader;
using SimpleHealthBar.UI;

namespace SimpleHealthBar.PlayerUtils
{
    class PlayerHealthBarManager
    {
        private static MelonLogger.Instance Logger;
        private static PlayerHealthBar PlayerHealthBar;
        private static bool HasInitialized = false;

        /// <summary>
        /// Initializes the player health bar manager, creates the health bar for the local player, and sets up logging.
        /// </summary>
        /// <param name="logger">The logger instance to use for output.</param>
        public static void Init(MelonLogger.Instance logger)
        {
            Logger = logger;
            PlayerHealthBar = new PlayerHealthBar();
#if !MONO
            PlayerHealthBar.SetPlayer(Il2CppScheduleOne.PlayerScripts.Player.Local);
#else
            PlayerHealthBar.SetPlayer(ScheduleOne.PlayerScripts.Player.Local);
#endif
            PlayerHealthBar.Init(HUD.Instance.transform);
            HasInitialized = true;
            Logger.Msg("Player Healthbar Initialized!");
        }

        /// <summary>
        /// Updates the player health bar each frame, refreshing its display and visibility based on the player's health and phone UI state.
        /// </summary>
        public static void OnUpdate()
        {
            if (!HasInitialized)
                return;
            Player player = PlayerHealthBar.GetPlayer();
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

        /// <summary>
        /// Unloads the player health bar manager, clearing references and resetting initialization state.
        /// </summary>
        public static void Unload()
        {
            HasInitialized = false;
            PlayerHealthBar = null;
        }

        /// <summary>
        /// Returns whether the player health bar manager has been initialized.
        /// </summary>
        /// <returns>True if initialized, otherwise false.</returns>
        public static bool Initialized() { return HasInitialized; }
    }
}
