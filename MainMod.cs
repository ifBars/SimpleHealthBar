using System.Collections;
using MelonLoader;
using SimpleHealthBar.Helpers;
using SimpleHealthBar.NPCUtils;
using SimpleHealthBar.PlayerUtils;

#if MONO
using ScheduleOne.PlayerScripts;
#else
using Il2CppScheduleOne.PlayerScripts;
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
    public const string Version = "1.4.0";
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
        
        // Initialize ModLogger with debug preference
        ModLogger.SetDebugEnabled(Preferences.EnableDebugLogging.Value);
        
        ModLogger.Info("SimpleHealthBar initialized successfully");
    }

    /// <summary>
    /// Called whenever a new scene is loaded. Starts initialization if the main scene is loaded,
    /// or unloads all healthbar managers if the menu scene is loaded.
    /// </summary>
    /// <param name="buildIndex">The build index of the loaded scene.</param>
    /// <param name="sceneName">The name of the loaded scene.</param>
    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (sceneName == "Main")
        {
            ModLogger.Debug("Main scene loaded - starting initialization");
            MelonCoroutines.Start(Init());
            
            // Start a safety timer for Mono builds
#if MONO
            MelonCoroutines.Start(SafetyTimer());
#endif
        }
        else if (sceneName == "Menu")
        {
            PlayerHealthBarManager.Unload();
            NPCHealthManager.Unload();
            MultiplayerHandler.Unload();
        }
    }

#if MONO
    public override void OnUpdate()
    {
        PlayerHealthBarManager.OnUpdate();
        NPCHealthManager.OnUpdate();
        MultiplayerHandler.OnUpdate();
    }
#endif

    /// <summary>
    /// Safety timer that ensures initialization happens even if the main Init coroutine fails.
    /// This is particularly important for Mono builds where timing issues might occur.
    /// </summary>
    private IEnumerator SafetyTimer()
    {
        yield return Utils.WaitForSeconds(10f); // Wait 10 seconds
        
        // Check if managers are initialized
        if (!PlayerHealthBarManager.Initialized())
        {
            ModLogger.Warn("Safety timer triggered - managers not initialized, attempting fallback");
            ModLogger.Info("Current player state:");
            Utils.LogPlayerState();
            yield return FallbackInit();
        }
        else
        {
            ModLogger.Info("Safety timer: Managers already initialized successfully");
        }
    }

    /// <summary>
    /// Coroutine that waits for the player to be available, then initializes all healthbar managers.
    /// </summary>
    private IEnumerator Init()
    {
        ModLogger.Info("Starting initialization - waiting for player to spawn...");
        
        // Log initial player state for debugging
        Utils.LogPlayerState();
        
#if MONO
        // Use the more robust player detection for Mono
        yield return Utils.WaitForPlayerSpawned(Utils.ReturnNull());
#else
        // Use standard player detection for IL2CPP
        yield return Utils.WaitForPlayer(Utils.ReturnNull());
#endif
        
        ModLogger.Info("Player detected - initializing health bar managers...");
        PlayerHealthBarManager.Init(Logger);
        NPCHealthManager.Init(Logger);
        MultiplayerHandler.Init(Logger);
        ModLogger.Info("All health bar managers initialized successfully!");
    }

    private void StartManagers()
    {
        PlayerHealthBarManager.Init(Logger);
        NPCHealthManager.Init(Logger);
        MultiplayerHandler.Init(Logger);
    }

    /// <summary>
    /// Fallback initialization method that can be called if the initial Init coroutine fails.
    /// This is particularly useful for Mono builds where timing issues might occur.
    /// </summary>
    private IEnumerator FallbackInit()
    {
        ModLogger.Warn("Using fallback initialization - player detection may have failed");
        
        // Wait a bit longer and try to initialize anyway
        yield return Utils.WaitForSeconds(2f);
        
        // Check if we can find the player now
#if MONO
        if (ScheduleOne.PlayerScripts.Player.Local != null && ScheduleOne.PlayerScripts.Player.Local.gameObject != null)
        {
            ModLogger.Info("Player found in fallback - initializing managers");
            StartManagers();
        }
#else
        if (Il2CppScheduleOne.PlayerScripts.Player.Local != null && Il2CppScheduleOne.PlayerScripts.Player.Local.gameObject != null)
        {
            ModLogger.Info("Player found in fallback - initializing managers");
            StartManagers();
        }
#endif
        else
        {
            ModLogger.Error("Player still not found in fallback - initialization failed");
        }
    }
}