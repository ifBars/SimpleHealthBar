# Simple Health Bar - V1.2.5 
[![Get it on GitHub](https://gist.githubusercontent.com/cxmeel/0dbc95191f239b631c3874f4ccf114e2/raw/github_source.svg)](https://github.com/SirTidez/SimpleHealthBar)

---

A lightweight mod that adds a clean, easy-to-read health bar above your inventory in **Schedule I**. Keep track of your player’s health at a glance. No more guessing and checking! Now also includes a health bar for the NPC nearest you!

![Health Bar Preview](https://i.imgur.com/fUTUDkO.jpeg)

---

## Key Features

- **On-Screen Health Display**  
  A subtle health bar appears just above your inventory UI, showing current and maximum health in real time.

- **Auto-Hide & Persistent Modes**  
  Choose to have the bar fade out after a few seconds of full health, or keep it visible at all times.

- **NPC Health Display**  
  Now the NPC nearest to you will have their health displayed at the top of your screen. Keep it around all the time, or have it disappear after a few seconds. It’s all configurable!

- **Easy Configuration**  
  Bar display duration, time, and speed via a simple config file. More coming in a future update.

- **Future Enhancements (WIP)**  
  - Multiplayer health bars  
  - Customizable thresholds and warnings  
  - User-suggested features – drop your ideas in the discussion!

---

## In-Game Configuration

### Mod Manager – Phone App Integration

If you install the [Mod Manager – Phone App (Nexus #397)](https://www.nexusmods.com/schedule1/mods/397), you can browse and tweak all settings directly through your phone’s **Settings** app.

### Manual CFG Config

Settings are stored in your MelonLoader preferences file. To customize without the phone app, open:

```text
<GameFolder>/Schedule I/UserData/MelonPreferences.cfg
```

Locate (or add) the following sections and adjust these values:

```ini
[SimpleHealthBar_HealthBar]
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
```

After editing, save the file and restart Schedule I for changes to take effect.

---

## Requirements

- Main (or beta) branch of Schedule I  
- **[MelonLoader](https://melonwiki.xyz/)**

---

## Installation

1. Ensure **MelonLoader** is installed and working for Schedule I.  
2. Download the latest **SimpleHealthBar-Il2Cpp.dll** from the releases page.  
3. Copy `SimpleHealthBar-Il2Cpp.dll` into your Schedule I’s `Mods/` folder.  
4. Launch Schedule I and enjoy instant health feedback!

---

Got feedback or feature requests? Feel free to open an issue or join the discussion; this mod is a work in progress, and your ideas help shape future updates!

![Join the Discussion](https://i.imgur.com/hR0Gugk.jpeg)
