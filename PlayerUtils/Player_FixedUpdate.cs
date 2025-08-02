#if MONO
using ScheduleOne.PlayerScripts;
#else
using Il2CppScheduleOne.PlayerScripts;
#endif
using HarmonyLib;
using SimpleHealthBar.NPCUtils;

namespace SimpleHealthBar.PlayerUtils
{
    [HarmonyPatch(typeof(Player))]
    internal class Player_FixedUpdate
    {
        [HarmonyPatch("FixedUpdate")]
        [HarmonyPrefix]
        private static void FixedUpdate(Player __instance)
        {
            NPCHealthManager.OnUpdate();
            PlayerHealthBarManager.OnUpdate();
            MultiplayerHandler.OnUpdate();
        }
    }
}
