using MelonLoader;

namespace SimpleHealthBar
{
    public static class Preferences
    {
        public static void Init()
        {
            Preferences.Category = MelonPreferences.CreateCategory("SimpleHealthBar_HealthBar", "Bar Settings");
            Preferences.AnimationCategory = MelonPreferences.CreateCategory("SimpleHealthBar_Animation", "Animation Settings");
            Preferences.FadeOutBar = Preferences.Category.CreateEntry<bool>("FadeOutBar", true, "Fade Out Health Bar", "Fades the health bar after a few seconds", false, false, null, null);
            Preferences.FadeHealthText = Preferences.Category.CreateEntry<bool>("FadeOutHealthText", true, "Fade Out Health Text", "Fades out the text display showing your health", false, false, null, null);
            Preferences.ShowOnDamage = Preferences.Category.CreateEntry<bool>("ShowOnDamage", true, "Show Health Bar on Damage", "Shows the health bar when you take damage", false, false, null, null);
            Preferences.FontSize = Preferences.Category.CreateEntry<float>("FontSize", 14f, "Font Size", "Configures the font size of the text label HUD element", false, false, null, null);
            //Animation preferences
            Preferences.FadeDelay = Preferences.AnimationCategory.CreateEntry<float>("FadeDelay", 5f, "Fade Out Delay", "The amount of time in seconds it takes for the bar and text to disappear", false, false, null, null);
            Preferences.FadeSpeed = Preferences.AnimationCategory.CreateEntry<float>("FadeSpeed", 2f, "Fade Speed", "Manages the speed of the fade transition");
            Preferences.NPCHealthBar = MelonPreferences.CreateCategory("SimpleHealthBar_NPCHealthBar", "NPC Health Bar");
            Preferences.NPCHealthBarEnabled = Preferences.NPCHealthBar.CreateEntry<bool>("NPCBarEnabled", true, "NPC Health Bar Enabled", "Enables the health bar for the nearest NPC", false, false, null, null);
            Preferences.FadeOutNPCBar = Preferences.NPCHealthBar.CreateEntry<bool>("FadeOutNPCBar", true, "Fade out NPC Health Bar", "Enables fading out the health bar for the nearest NPC", false, false, null, null);
            Preferences.NPCFadeOutDistance = Preferences.NPCHealthBar.CreateEntry<float>("NPCFadeOutDistance", 100f, "Distance from Player to Fade Out Bar", "Sets the distance from the player the NPC health bar should fade out", false, false, null, null);
        }

        public static MelonPreferences_Category Category;
        public static MelonPreferences_Entry<bool> FadeOutBar;
        public static MelonPreferences_Entry<bool> FadeHealthText;
        public static MelonPreferences_Entry<bool> ShowOnDamage;
        public static MelonPreferences_Category AnimationCategory;
        public static MelonPreferences_Entry<float> FadeDelay;
        public static MelonPreferences_Entry<float> FadeSpeed;
        public static MelonPreferences_Entry<float> FontSize;
        public static MelonPreferences_Category NPCHealthBar;
        public static MelonPreferences_Entry<bool> NPCHealthBarEnabled;
        public static MelonPreferences_Entry<bool> FadeOutNPCBar;
        public static MelonPreferences_Entry<float> NPCFadeOutDistance;
    }
}
