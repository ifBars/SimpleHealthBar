#if MONO
using ScheduleOne.PlayerScripts;
using ScheduleOne.UI;
#else
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.UI;
using Il2CppSystem.Collections.Generic;

#endif
using MelonLoader;
using SimpleHealthBar.Helpers;
using SimpleHealthBar.UI;
using UnityEngine;

namespace SimpleHealthBar.PlayerUtils
{
    class MultiplayerHandler
    {
        //private static List<MultiplayerHealthbar> PlayerHealthbars = new List<MultiplayerHealthbar>();

        private static System.Collections.Generic.Dictionary<Player, HealthBar> MultiplayerHealthbars = new System.Collections.Generic.Dictionary<Player, HealthBar>();

        private static bool HasInitialized = false;
        private static bool IsMultiplayer = false;

        /// <summary>
        /// Initializes the multiplayer healthbar system by creating healthbars for all non-local players.
        /// </summary>
        public static void Init(MelonLogger.Instance logger)
        {
            foreach (Player player in Player.PlayerList)
            {
                if(player == null || player.IsLocalPlayer)
                    continue; // Skip local player or null players
                HealthBar healthbar = CreatePlayerHealthbar(player);
                
                if (healthbar == null)
                {
                    ModLogger.Error($"Failed to create healthbar for player: {player.name}");
                }
            }
            HasInitialized = true;
            ModLogger.Info("Player Healthbar Initialized!");
        }

        /// <summary>
        /// Creates and initializes a healthbar for the specified player, positions it, and adds it to the list.
        /// </summary>
        private static HealthBar CreatePlayerHealthbar(Player player)
        {
            //if (player == null || PlayerHealthbars.Any(h => h.GetPlayer() == player) || !player.IsSpawned)
            //    return null;
            if (player == null || MultiplayerHealthbars.ContainsKey(player))
                return null;
            //MultiplayerHealthbar healthbar = new();
            //float adjustedHeight = ((PlayerHealthbars.Count > 0) ? (float)PlayerHealthbars.Count * 25f : 0f) + 105f;
            float adjustedHeight = ((MultiplayerHealthbars.Count > 0) ? (float)MultiplayerHealthbars.Count * 25f : 0f) + 105f;
#if MONO
            HealthBar healthbar = new(HealthBarType.Multiplayer, HUD.instance.transform, new Vector2(0f, adjustedHeight));
#else
            HealthBar healthbar = new(HealthBarType.Multiplayer, HUD.Instance.transform, new Vector2(0f, adjustedHeight));
#endif
            if (healthbar != null)
                MultiplayerHealthbars.Add(player, healthbar);
                //PlayerHealthbars.Add(healthbar);
            ModLogger.Debug($"Creating healthbar for player: {player.name}");
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
                foreach (Player p in Player.PlayerList)
                {
                    if (p != null && !p.IsLocalPlayer)
                    {
                        if (MultiplayerHealthbars.ContainsKey(p))
                        {
                            HealthBar healthBar = MultiplayerHealthbars[p];
                            if (healthBar.GetCurrentHealth() != p.Health.CurrentHealth && p.IsSpawned)
                            {
                                healthBar.SetCurrentHealth(p.Health.CurrentHealth);
                                healthBar.UpdateText($"{p.name}");
                                healthBar.Show();
                            }
                            healthBar.Update(p.IsSpawned);
                        }
                        else
                        {
                            float adjustedHeight = ((MultiplayerHealthbars.Count > 0) ? (float)MultiplayerHealthbars.Count * 25f : 0f) + 105f;
#if MONO
                            HealthBar healthBar = new HealthBar(HealthBarType.Multiplayer, HUD.instance.transform, new Vector2(0f, adjustedHeight));
#else
                            HealthBar healthBar = new HealthBar(HealthBarType.Multiplayer, HUD.Instance.transform, new Vector2(0f, adjustedHeight));
#endif
                            MultiplayerHealthbars.Add(p, healthBar);
                            healthBar.SetCurrentHealth(p.Health.CurrentHealth);
                            healthBar.UpdateText($"{p.name}");
                            healthBar.Show();
                            healthBar.Update(p.IsSpawned);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Ensures healthbars exist for all current players and removes healthbars for players that have left.
        /// </summary>
        public static void CheckCreate()
        {
            // Remove healthbars for players that no longer exist
            UpdateLocation();
            foreach (Player player in Player.PlayerList)
            {
                if (player == null || player.IsLocalPlayer)
                    continue;
                if (MultiplayerHealthbars.ContainsKey(player) && player.IsOffline)
                {
                    MultiplayerHealthbars[player].Dispose();
                    MultiplayerHealthbars.Remove(player);
                }
                if (MultiplayerHealthbars.ContainsKey(player) || !player.IsSpawned)
                    continue;
                HealthBar healthbar = CreatePlayerHealthbar(player);
                if (healthbar != null)
                {
                    MultiplayerHealthbars.Add(player, healthbar);
                    healthbar.SetCurrentHealth(player.Health.CurrentHealth);
                    healthbar.UpdateText($"{player.name}");
                    healthbar.Show();
                }
                else
                {
                    ModLogger.Error($"Failed to create healthbar for player: {player.name}");
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
            foreach (HealthBar healthbar in MultiplayerHealthbars.Values)
            {
                if (healthbar != null)
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
            //PlayerHealthbars.Clear();
            MultiplayerHealthbars.Clear();
        }

        /// <summary>
        /// Returns whether the multiplayer healthbar system has been initialized.
        /// </summary>
        public static bool Initialized() { return HasInitialized; }
    }
}
