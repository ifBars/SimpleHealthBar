#if MONO
using ScheduleOne.PlayerScripts;
using ScheduleOne.UI;
#else
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.UI;
#endif
using MelonLoader;
using SimpleHealthBar.UI;
using UnityEngine;

namespace SimpleHealthBar.PlayerUtils
{
    class MultiplayerHandler
    {
        private static MelonLogger.Instance Logger;
        private static List<MultiplayerHealthbar> PlayerHealthbars = new List<MultiplayerHealthbar>();
        private static bool HasInitialized = false;
        private static bool IsMultiplayer = false;

        /// <summary>
        /// Initializes the multiplayer healthbar system by creating healthbars for all non-local players.
        /// </summary>
        public static void Init(MelonLogger.Instance logger)
        {
            Logger = logger;
            foreach (Player player in Player.PlayerList)
            {
                if(player == null || player.IsLocalPlayer)
                    continue; // Skip local player or null players
                MultiplayerHealthbar healthbar = CreatePlayerHealthbar(player);
                if (healthbar == null)
                {
                    Logger.Error($"Failed to create healthbar for player: {player.name}");
                }
            }
            HasInitialized = true;
            Logger.Msg("Player Healthbar Initialized!");
        }

        /// <summary>
        /// Creates and initializes a healthbar for the specified player, positions it, and adds it to the list.
        /// </summary>
        private static MultiplayerHealthbar CreatePlayerHealthbar(Player player)
        {
            if (player == null || PlayerHealthbars.Any(h => h.GetPlayer() == player) || !player.IsSpawned)
                return null;
            MultiplayerHealthbar healthbar = new();
            float adjustedHeight = ((PlayerHealthbars.Count > 0) ? (float)PlayerHealthbars.Count * 25f : 0f) + 105f;
            healthbar.Init(HUD.Instance.transform, new Vector2(0f, adjustedHeight), player);
            if(healthbar != null)
                PlayerHealthbars.Add(healthbar);
            Logger.Msg($"Creating healthbar for player: {player.name}");
            return healthbar;
        }

        /// <summary>
        /// Updates all multiplayer healthbars each frame, refreshing their display and visibility as needed.
        /// </summary>
        public static void OnUpdate()
        {
            if (!HasInitialized)
                return;
            if (IsMultiplayerMode())
            {
                CheckCreate();
                foreach (MultiplayerHealthbar healthbar in PlayerHealthbars)
                {
                     Player player = healthbar.GetPlayer();
                     if (player != null && healthbar.GetPlayerHealth() != healthbar.GetDisplayedHealth() && player.IsSpawned)
                     {
                         healthbar.UpdateText();
                         healthbar.Show();
                     }
                     healthbar.Update();
                }
            }
        }

        /// <summary>
        /// Ensures healthbars exist for all current players and removes healthbars for players that have left.
        /// </summary>
        public static void CheckCreate()
        {
            // Remove healthbars for players that no longer exist
            PlayerHealthbars.RemoveAll(healthbar => healthbar.GetPlayer().IsOffline);
            UpdateLocation();
            foreach (Player player in Player.PlayerList)
            {
                if (player == null || player.IsLocalPlayer || PlayerHealthbars.Any(h => h.GetPlayer() == player) || !player.IsSpawned)
                    continue; // Skip local player or already existing healthbar
                MultiplayerHealthbar healthbar = CreatePlayerHealthbar(player);
                if (healthbar == null)
                {
                    Logger.Error($"Failed to create healthbar for player: {player.name}");
                }
            }
        }

        /// <summary>
        /// Updates the vertical position of all player healthbars to ensure they are stacked correctly.
        /// </summary>
        public static void UpdateLocation()
        {
            if (!HasInitialized)
                return;
            int index = 0;
            foreach (MultiplayerHealthbar healthbar in PlayerHealthbars)
            {
                if (healthbar != null && healthbar.GetPlayer() != null)
                {
                    float adjustedHeight = (index * 25f) + 105f;
                    healthbar.SetAnchoredPosition(new Vector2(0f, adjustedHeight));
                    index++;
                }
            }
        }

        /// <summary>
        /// Determines if the game is currently in multiplayer mode (more than one player).
        /// </summary>
        public static bool IsMultiplayerMode()
        {
            if (HasInitialized)
            {
                IsMultiplayer = Player.PlayerList.Count > 1;
            }
            return IsMultiplayer;
        }

        /// <summary>
        /// Cleans up and hides all healthbars, clearing the list and resetting initialization state.
        /// </summary>
        public static void Unload()
        {
            HasInitialized = false;
            PlayerHealthbars.Clear();
        }

        /// <summary>
        /// Returns whether the multiplayer healthbar system has been initialized.
        /// </summary>
        public static bool Initialized() { return HasInitialized; }
    }
}
