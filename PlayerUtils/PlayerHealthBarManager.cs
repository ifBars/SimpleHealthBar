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
using Il2CppCinemachine;
#endif
using MelonLoader;
using SimpleHealthBar.Helpers;
using SimpleHealthBar.UI;
using UnityEngine;

namespace SimpleHealthBar.PlayerUtils
{
    class PlayerHealthBarManager
    {
        private static Player LocalPlayer;
        private static HealthBar HealthBar;
        private static bool HasInitialized = false;

        /// <summary>
        /// Initializes the player health bar manager, creates the health bar for the local player, and sets up logging.
        /// </summary>
        /// <param name="logger">The logger instance to use for output.</param>
        public static void Init(MelonLogger.Instance logger)
        {
            ModLogger.Info("Initializing Player Health Bar");
            //PlayerHealthBar = new PlayerHealthBar();
#if !MONO
            //PlayerHealthBar.SetPlayer(Il2CppScheduleOne.PlayerScripts.Player.Local);
            LocalPlayer = Il2CppScheduleOne.PlayerScripts.Player.Local;
#else
            //PlayerHealthBar.SetPlayer(ScheduleOne.PlayerScripts.Player.Local);
            LocalPlayer = ScheduleOne.PlayerScripts.Player.Local;
#endif
#if MONO
            HealthBar = new HealthBar(HealthBarType.Player, HUD.instance.transform);
#else
            HealthBar = new HealthBar(HealthBarType.Player, HUD.Instance.transform);
#endif
            //PlayerHealthBar.Init(HUD.Instance.transform);
            HasInitialized = true;
            ModLogger.Info("Player Healthbar Initialized!");
        }

        /// <summary>
        /// Updates the player health bar each frame, refreshing its display and visibility based on the player's health and phone UI state.
        /// </summary>
        public static void OnUpdate()
        {
            if (!HasInitialized)
                return;
            Phone phone = PlayerSingleton<Phone>.Instance;
            bool phoneOpen = phone != null && phone.IsOpen;
            
            if (HealthBar != null && LocalPlayer != null)
            {
                float currentHealth = LocalPlayer.Health.CurrentHealth;
                float displayedHealth = HealthBar.GetCurrentHealth();
                
                if (currentHealth != displayedHealth)
                {
                    ModLogger.Debug($"Health changed from {displayedHealth} to {currentHealth}");
                    HealthBar.SetCurrentHealth(currentHealth);
                    HealthBar.UpdateText();
                    HealthBar.Show();
                }
                HealthBar.Update(phoneOpen);
            }
        }

        /// <summary>
        /// Unloads the player health bar manager, clearing references and resetting initialization state.
        /// </summary>
        public static void Unload()
        {
            HasInitialized = false;
            HealthBar = null;
        }

        /// <summary>
        /// Returns whether the player health bar manager has been initialized.
        /// </summary>
        /// <returns>True if initialized, otherwise false.</returns>
        public static bool Initialized() { return HasInitialized; }
    }
}
