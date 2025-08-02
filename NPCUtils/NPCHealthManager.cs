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
using SimpleHealthBar.UI;

namespace SimpleHealthBar.NPCUtils
{
    public static class NPCHealthManager
    {
        private static MelonLogger.Instance Logger;
        private static NPCManager NPCManager;
        private static NPCHealthBar NPCSelectedBar;
        private static bool IsInitialized = false;

        /// <summary>
        /// Initializes the NPC healthbar manager, setting up the logger and creating the healthbar for the nearest NPC if the NPCManager is available.
        /// </summary>
        /// <param name="logger">The logger instance to use for output.</param>
        public static void Init(MelonLogger.Instance logger)
        {
            Logger = logger;
            IsInitialized = NPCManager.InstanceExists;
            if (IsInitialized)
            {
                NPCManager = NPCManager.Instance;
                NPCSelectedBar = new NPCHealthBar();
                NPCSelectedBar.Init(HUD.Instance.transform);
                Logger.Msg("Nearest NPC Healthbar Initialized!");
            }
            else
                Logger.Error("NPCManager not found, aborting NPC health reporting!");
        }

        /// <summary>
        /// Handles logic when an NPC is selected, updating and showing the healthbar if the selected NPC changes or its health changes.
        /// </summary>
        /// <param name="npc">The NPC that has been selected.</param>
        public static void OnNPCSelected(NPC npc)
        {
            if (IsInitialized)
            {
                if(NPCSelectedBar.GetNPC() != npc)
                {
                    NPCSelectedBar.SetNPC(npc);
                    NPCSelectedBar.UpdateText();
                    NPCSelectedBar.Show();
                }
                else
                {
                    if (NPCSelectedBar.GetDisplayedHealth() != npc.Health.Health)
                    {
                        NPCSelectedBar.UpdateText();
                        NPCSelectedBar.Show();
                    }
                }
            }
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
                float sqrDist = (npc.Movement.FootPosition - Player.Local.CameraPosition).sqrMagnitude;
                if(sqrDist < closestDist)
                {
                    closestDist = sqrDist;
                    closestNPC = npc;
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
            if (NPCSelectedBar.GetNPC() != closestNPC)
            {
                NPCSelectedBar.SetNPC(closestNPC);
                NPCSelectedBar.UpdateText();
                NPCSelectedBar.Show();
            }
            else
            {
                bool update = NPCSelectedBar.GetNPCHealth() != NPCSelectedBar.GetDisplayedHealth();
                if (update)
                {
                    NPCSelectedBar.UpdateText();
                    NPCSelectedBar.Show();
                }
                if (NPCSelectedBar.CheckDistanceChanged())
                {
                    NPCSelectedBar.UpdateText();
                    NPCSelectedBar.Show();
                }
            }
            NPCSelectedBar.Update();
        }

        /// <summary>
        /// Unloads the NPC healthbar manager, clearing references and resetting initialization state.
        /// </summary>
        public static void Unload()
        {
            IsInitialized = false;
            NPCSelectedBar = null;
        }
        /// <summary>
        /// Returns whether the NPC healthbar manager has been initialized.
        /// </summary>
        /// <returns>True if initialized, otherwise false.</returns>
        public static bool Initialized() { return IsInitialized; }
    }
}
