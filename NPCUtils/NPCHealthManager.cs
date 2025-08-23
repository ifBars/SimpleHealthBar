#if MONO
using ScheduleOne.PlayerScripts;
using ScheduleOne.UI;
using ScheduleOne.NPCs;
#else
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.UI;
#endif
using MelonLoader;
using SimpleHealthBar.Helpers;
using SimpleHealthBar.UI;
using UnityEngine;

namespace SimpleHealthBar.NPCUtils
{
    public static class NPCHealthManager
    {
        private static NPCManager NPCManager;
        private static HealthBar HealthBar;
        private static NPC CurrentNPC;
        private static bool IsInitialized = false;
        private static bool IsOutOfSight = true;

        /// <summary>
        /// Initializes the NPC healthbar manager, setting up the logger and creating the healthbar for the nearest NPC if the NPCManager is available.
        /// </summary>
        /// <param name="logger">The logger instance to use for output.</param>
        public static void Init(MelonLogger.Instance logger)
        {
            IsInitialized = NPCManager.InstanceExists;
            if (IsInitialized)
            {
                NPCManager = NPCManager.Instance;
#if MONO
                HealthBar = new HealthBar(HealthBarType.NPC, HUD.instance.transform);
#else
                HealthBar = new HealthBar(HealthBarType.NPC, HUD.Instance.transform);
#endif
                ModLogger.Info("NPC Healthbar Initialized!");
            }
            else
                ModLogger.Error("NPCManager not found, aborting NPC health reporting!");
        }

        /// <summary>
        /// Handles logic when an NPC is selected, updating and showing the healthbar if the selected NPC changes or its health changes.
        /// </summary>
        /// <param name="npc">The NPC that has been selected.</param>
        public static void OnNPCSelected(NPC npc)
        {
            if (IsInitialized && npc != null && HealthBar != null)
            {
                if (CurrentNPC != npc)
                {
                    CurrentNPC = npc;
                    HealthBar.UpdateText(npc.fullName);
                    HealthBar.Show();
                }
                else
                {
                    if (HealthBar.GetCurrentHealth() != npc.Health.Health)
                    {
                        HealthBar.UpdateText(npc.fullName);
                        HealthBar.Show();
                    }
                }
            }
        }

        private static float GetDistanceFromPlayer()
        {
            if (CurrentNPC == null || Player.Local == null) return float.MaxValue;
            
#if MONO
            // Mono workaround - exclusively use transform position to avoid MissingMethodException
            Vector3 npcPosition = CurrentNPC.transform.position;
#else
            // IL2CPP - use the standard approach
            Vector3 npcPosition = CurrentNPC.Movement.FootPosition;
#endif
            
            return (npcPosition - Player.Local.CameraPosition).sqrMagnitude;
        }

        private static bool CheckDistanceFromPlayer()
        {
            bool check = GetDistanceFromPlayer() < Preferences.NPCFadeOutDistance.Value;
            if (!check && !IsOutOfSight)
                IsOutOfSight = true;
            else if (check && IsOutOfSight)
            {
                IsOutOfSight = false;
            }
            return check;
        }

        private static bool CheckDistanceChanged()
        {
            bool check = GetDistanceFromPlayer() < Preferences.NPCFadeOutDistance.Value;
            if (check && IsOutOfSight)
                return true;
            return false;
        }

        /*
         * Returns the NPC that is closest in distance to the player
         */
        /// <summary>
        /// Returns the NPC that is closest in distance to the player.
        /// </summary>
        /// <returns>The closest NPC instance, or null if none are found.</returns>
        public static NPC GetClosestNPC()
        {
            NPC closestNPC = null;
            float closestDist = float.MaxValue;
            
            foreach(NPC npc in NPCManager.NPCRegistry)
            {
                if (npc == null) continue;
                
                float sqrDist;
#if MONO
                // Mono workaround - exclusively use transform position to avoid MissingMethodException
                Vector3 npcPosition = npc.transform.position;
#else
                // IL2CPP - use the standard approach
                Vector3 npcPosition = npc.Movement.FootPosition;
#endif
                
                if (Player.Local != null)
                {
                    sqrDist = (npcPosition - Player.Local.CameraPosition).sqrMagnitude;
                    if(sqrDist < closestDist)
                    {
                        closestDist = sqrDist;
                        closestNPC = npc;
                    }
                }
            }

            return closestNPC;
        }



        /// <summary>
        /// Updates the NPC healthbar each frame, ensuring it tracks the closest NPC and updates when health or distance changes.
        /// </summary>
        public static void OnUpdate()
        {
            if (!IsInitialized)
                return;
                
            NPC closestNPC = GetClosestNPC();
            
            if (closestNPC != null && CurrentNPC != closestNPC)
            {
                CurrentNPC = closestNPC;
                float npcHealth = CurrentNPC.Health.Health;
                ModLogger.Debug($"NPC changed to {closestNPC.fullName} with health {npcHealth}");
                HealthBar.SetCurrentHealth(npcHealth);
                HealthBar.UpdateText(closestNPC.fullName);
                HealthBar.Show();
            }
            else if (CurrentNPC != null)
            {
                float currentHealth = CurrentNPC.Health.Health;
                float displayedHealth = HealthBar.GetCurrentHealth();
                bool update = displayedHealth != currentHealth;
                
                if (update)
                {
                    ModLogger.Debug($"NPC health changed from {displayedHealth} to {currentHealth}");
                    HealthBar.SetCurrentHealth(currentHealth);
                    HealthBar.UpdateText(CurrentNPC.fullName);
                    HealthBar.Show();
                }
            }
            
            if (HealthBar != null)
            {
                HealthBar.Update();
            }
        }

        /// <summary>
        /// Unloads the NPC healthbar manager, clearing references and resetting initialization state.
        /// </summary>
        public static void Unload()
        {
            IsInitialized = false;
            HealthBar = null;
        }
        /// <summary>
        /// Returns whether the NPC healthbar manager has been initialized.
        /// </summary>
        /// <returns>True if initialized, otherwise false.</returns>
        public static bool Initialized() { return IsInitialized; }
    }
}
