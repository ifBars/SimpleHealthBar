using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.UI;
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
            }

            NPCSelectedBar.Update();
        }
        
        public static void Unload()
        {
            IsInitialized = false;
            NPCSelectedBar = null;
        }
        public static bool Initialized() { return IsInitialized; }
    }
}
