[size=5]Simple Health Bar - V1.2.5[/size]
[hr][/hr]
A lightweight mod that adds a clean, easy-to-read health bar above your inventory in [b]Schedule I[/b]. Keep track of your player’s health at a glance. No more guessing and checking! Now also includes a health bar for the NPC nearest you!

[img]https://i.imgur.com/fUTUDkO.jpeg[/img]

[hr][/hr]
[size=4]Key Features[/size]
[list][*][b]On-Screen Health Display[/b]
A subtle health bar appears just above your inventory UI, showing current and maximum health in real time.


[*][b]Auto-Hide & Persistent Modes[/b]
Choose to have the bar fade out after a few seconds of full health, or keep it visible at all times.


[*][b]NPC Health Display[/b]
Now the NPC nearest to you will have their health displayed at the top of your screen. Keep it around all the time, or have it disappear after a few seconds. It’s all configurable!


[*][b]Easy Configuration[/b]
Bar display duration, time, and speed via a simple config file. More coming in a future update.


[*][b]Future Enhancements (WIP)[/b]

[list][*]Multiplayer health bars
[*]Customizable thresholds and warnings
[*]User-suggested features – drop your ideas in the discussion!
[/list]

[/list]
[hr][/hr]
[size=4]In-Game Configuration[/size]
[b]Mod Manager – Phone App Integration[/b]
If you install the [url=https://www.nexusmods.com/schedule1/mods/397]Mod Manager – Phone App (Nexus #397)[/url], you can browse and tweak all settings directly through your phone’s [b]Settings[/b] app.

[b]Manual CFG Config[/b]
Settings are stored in your MelonLoader preferences file. To customize without the phone app, open:

[CODE=text]<GameFolder>/Schedule I/UserData/MelonPreferences.cfg
[/CODE]
Locate (or add) the following sections and adjust these values:

[CODE=ini][SimpleHealthBar_HealthBar]
# Fades the health bar after a few seconds
FadeOutBar = true
# Fades out the text display showing your health
FadeOutHealthText = true
# Shows the health bar when you take damage
ShowOnDamage = true
# Configures the font size of the text label HUD element
FontSize = 12.0

[SimpleHealthBar_Animation]
# The amount of time in seconds it takes for the bar and text to disappear
FadeDelay = 10.0
# Manages the speed of the fade transition
FadeSpeed = 5.0

[SimpleHealthBar_NPCHealthBar]
# Enables the health bar for the nearest NPC
NPCBarEnabled = true
# Enables fading out the health bar for the nearest NPC
FadeOutNPCBar = false
[/CODE]
After editing, save the file and restart Schedule I for changes to take effect.

[hr][/hr]
[size=4]Requirements[/size]
[list][*]Main (or beta) branch of Schedule I
[*][b][url=https://melonwiki.xyz/]MelonLoader[/url][/b]
[/list]
[hr][/hr]
[size=4]Installation[/size]
1. Ensure [b]MelonLoader[/b] is installed and working for Schedule I.
2. Download the latest [b]SimpleHealthBar-Il2Cpp.dll[/b] from the releases page.
3. Copy [icode]SimpleHealthBar-Il2Cpp.dll[/icode] into your Schedule I’s [icode]Mods/[/icode] folder.
4. Launch Schedule I and enjoy instant health feedback!
[hr][/hr]
Got feedback or feature requests? Feel free to open an issue or join the discussion; this mod is a work in progress, and your ideas help shape future updates!

[img]https://i.imgur.com/hR0Gugk.jpeg[/img]

