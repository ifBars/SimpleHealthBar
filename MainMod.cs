using System.Collections;
using MelonLoader;
using SimpleHealthBar.Helpers;
using UnityEngine;
using Il2CppScheduleOne.UI.Phone;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppSystem.Xml;
using static MelonLoader.MelonLaunchOptions;
using Il2CppScheduleOne.UI;






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

namespace SimpleHealthBar;

public static class BuildInfo
{
    public const string Name = "SimpleHealthBar";
    public const string Description = "Gives you a health bar above your inventory.";
    public const string Author = "iTidez";
    public const string Version = "1.0.0";
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
            //    Logger.Debug("Main scene loaded, waiting for player");
            //    MelonCoroutines.Start(Utils.WaitForPlayer(DoStuff()));

            //    Logger.Debug("Main scene loaded, waiting for network");
            //    MelonCoroutines.Start(Utils.WaitForNetwork(DoNetworkStuff()));
            this.atMain = true;
        }
        else
            this.atMain = false;
    }

    public override void OnUpdate()
    {
        if (atMain)
        {
            if (this.initialized)
            {
                Player player = Player.Local;
                bool healthBarExists = this.healthBar != null || player != null;
                Phone phone = PlayerSingleton<Phone>.Instance;
                bool phoneOpen = phone != null && phone.IsOpen;
                if (healthBarExists)
                {
                    float health = player.Health.CurrentHealth;
                    bool update = health != this.lastHealth;
                    if (update)
                    {
                        this.lastHealth = health;
                        this.healthBar.UpdateHealth(health);
                        bool showOnDamage = Preferences.ShowOnDamage.Value || phoneOpen;
                        if (showOnDamage)
                            this.healthBar.Show();
                    }
                }
                this.healthBar.Update(phoneOpen);
                this._lastPhoneOpen = phoneOpen;
            }
        }
    }

    private IEnumerator Init()
    {
        Logger.Debug("Initializing health bar");
        this.healthBar = new HealthBarHandler();
        this.healthBar.Init(HUD.Instance.transform, Logger);
        yield return new WaitForSeconds(2f);
        this.initialized = true;
        Logger.Msg("Heathbar Initialized!");
    }

    private HealthBarHandler healthBar;
    private float lastHealth = -1f;
    private bool _lastPhoneOpen;
    private bool atMain = true;
    private bool initialized = false;
}
