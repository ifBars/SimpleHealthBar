using System.Collections;
using MelonLoader;
using SimpleHealthBar.Helpers;
using SimpleHealthBar.NPCUtils;
using SimpleHealthBar.PlayerUtils;














#if MONO
using FishNet;
#else
using Il2CppFishNet;
#endif

[assembly: MelonInfo(
    typeof(SimpleHealthBar.SimpleHealthBar),
    SimpleHealthBar.BuildInfo.Name,
    SimpleHealthBar.BuildInfo.Version,
    SimpleHealthBar.BuildInfo.Author
)]
[assembly: MelonColor(1, 255, 0, 0)]
[assembly: MelonGame("TVGS", "Schedule I")]
[assembly: System.Reflection.AssemblyMetadata("NexusModID", "1106")]

namespace SimpleHealthBar;

public static class BuildInfo
{
    public const string Name = "SimpleHealthBar";
    public const string Description = "Gives you a health bar above your inventory.";
    public const string Author = "iTidez";
    public const string Version = "1.2.5";
}

public class SimpleHealthBar : MelonMod
{
    private static MelonLogger.Instance Logger;

    public override void OnInitializeMelon()
    {
        Logger = LoggerInstance;
        Preferences.Init();
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        Logger.Debug($"Scene loaded: {sceneName}");
        if (sceneName == "Main")
        {
            MelonCoroutines.Start(Init());
        }
        else if (sceneName == "Menu")
        {
            PlayerHealthBarManager.Unload();
            NPCHealthManager.Unload();
        }
    }


    private IEnumerator Init()
    {
        yield return Utils.WaitForPlayer(Utils.ReturnNull());
        PlayerHealthBarManager.Init(Logger);
        NPCHealthManager.Init(Logger);
    }
}