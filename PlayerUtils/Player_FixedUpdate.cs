using HarmonyLib;
using SimpleHealthBar.NPCUtils;

namespace SimpleHealthBar.PlayerUtils
{
    [HarmonyPatch(typeof(Il2CppScheduleOne.PlayerScripts.Player))]
    internal class Player_FixedUpdate
    {
        [HarmonyPatch("FixedUpdate")]
        [HarmonyPrefix]
        private static void FixedUpdate(Il2CppScheduleOne.PlayerScripts.Player __instance)
        {
            NPCHealthManager.OnUpdate();
            PlayerHealthBarManager.OnUpdate();
        }
    }
}
