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
    SimpleHealthBar.BuildInfo.Author,
    SimpleHealthBar.BuildInfo.DownloadLink
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
    public const string Version = "1.2.9";
    public const string DownloadLink = "https://www.nexusmods.com/schedule1/mods/1106";
}

public class SimpleHealthBar : MelonMod
{
    private static MelonLogger.Instance Logger;

    /// <summary>
    /// Called when the mod is initialized. Sets up the logger and initializes user preferences.
    /// </summary>
    public override void OnInitializeMelon()
    {
        Logger = LoggerInstance;
        Preferences.Init();
    }

    /// <summary>
    /// Called whenever a new scene is loaded. Starts initialization if the main scene is loaded,
    /// or unloads all healthbar managers if the menu scene is loaded.
    /// </summary>
    /// <param name="buildIndex">The build index of the loaded scene.</param>
    /// <param name="sceneName">The name of the loaded scene.</param>
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
            MultiplayerHandler.Unload();
        }
    }


    /// <summary>
    /// Coroutine that waits for the player to be available, then initializes all healthbar managers.
    /// </summary>
    private IEnumerator Init()
    {
        yield return Utils.WaitForPlayer(Utils.ReturnNull());
        PlayerHealthBarManager.Init(Logger);
        NPCHealthManager.Init(Logger);
        MultiplayerHandler.Init(Logger);
    }
}