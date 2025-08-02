#if MONO
using ScheduleOne.PlayerScripts;
using TMPro;
#else
using Il2CppScheduleOne.PlayerScripts;
using Il2CppTMPro;
#endif
using MelonLoader;
using SimpleHealthBar.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleHealthBar.UI
{
    class PlayerHealthBar
    {
        /*
         * Begin variable definition
         */
        private MelonLogger.Instance Logger;
        private GameObject PlayerHealthbarBase;
        private Slider PlayerHealthbarSlider;
        private Image PlayerHealthbarImage;
        private TextMeshProUGUI PlayerHealthText;
        private CanvasGroup PlayerHealthTextGroup;
        private Player Player;
        private float LastHealthUpdateTime;
        private float PauseStartTime;
        private float PauseAccumulated;
        private float CurrentFill;
        private bool LastPhoneOpen;

        private float FadeSpeed = Preferences.FadeSpeed.Value;
        private float FadeDelay = Preferences.FadeDelay.Value;
        private float FontSize = Preferences.FontSize.Value;

        /// <summary>
        /// Initializes the player health bar UI, creating all necessary GameObjects and UI components, and setting their properties.
        /// </summary>
        /// <param name="interactionCanvas">The parent canvas transform for the health bar.</param>
        /// <returns>The initialized PlayerHealthBar instance.</returns>
        public PlayerHealthBar Init(Transform interactionCanvas)
        {
            Logger = new MelonLogger.Instance(BuildInfo.Name);
            PlayerHealthbarBase = new GameObject("PlayerHealthBar");
            PlayerHealthbarBase.transform.SetParent(interactionCanvas, false);
            PlayerHealthbarBase.AddComponent<RectTransform>();
            PlayerHealthbarSlider = PlayerHealthbarBase.AddComponent<Slider>();

            RectTransform barBase = PlayerHealthbarBase.GetComponent<RectTransform>();
            barBase.anchorMin = new Vector2(0.235f, 0f);
            barBase.anchorMax = new Vector2(0.775f, 0f);
            barBase.anchoredPosition = new Vector2(0f, 105f);
            barBase.sizeDelta = new Vector2(0f, 2f);

            PlayerHealthbarSlider.minValue = 0f;
            PlayerHealthbarSlider.maxValue = 100f;
            PlayerHealthbarSlider.value = 0;

            GameObject fill = new GameObject("PlayerHealthbarFill");
            fill.transform.SetParent(barBase.transform, false);
            fill.AddComponent<RectTransform>();
            PlayerHealthbarImage = fill.AddComponent<Image>();
            PlayerHealthbarImage.color = Color.red;
            RectTransform fillBar = fill.GetComponent<RectTransform>();
            fillBar.anchorMin = Vector2.zero;
            fillBar.anchorMax = Vector2.one;
            fillBar.offsetMin = Vector2.zero;
            fillBar.offsetMax = Vector2.zero;
            PlayerHealthbarSlider.fillRect = fillBar;

            GameObject playerHealthTextGroup = new GameObject("PlayerHealthTextGroup");
            playerHealthTextGroup.transform.SetParent(fill.transform, false);
            PlayerHealthTextGroup = playerHealthTextGroup.AddComponent<CanvasGroup>();
            RectTransform textGroupDisplay = playerHealthTextGroup.AddComponent<RectTransform>();
            textGroupDisplay.anchorMin = new Vector2(0f, 1f);
            textGroupDisplay.anchorMax = new Vector2(0f, 1f);
            textGroupDisplay.pivot = new Vector2(0f, 1f);
            textGroupDisplay.anchoredPosition = new Vector2(0f, 0f);
            textGroupDisplay.sizeDelta = new Vector2(0f, 0f);

            GameObject playerHealthText = new GameObject("PlayerHealthText");
            playerHealthText.transform.SetParent(playerHealthTextGroup.transform, false);
            RectTransform playerHealthTextDisplay = playerHealthText.AddComponent<RectTransform>();
            playerHealthTextDisplay.anchorMin = new Vector2(0f, 1f);
            playerHealthTextDisplay.anchorMax = new Vector2(0f, 1f);
            playerHealthTextDisplay.pivot = new Vector2(0f, 1f);
            playerHealthTextDisplay.anchoredPosition = new Vector2(500f, 20f);
            playerHealthTextDisplay.sizeDelta = new Vector2(400f, 20f);
            PlayerHealthText = playerHealthText.AddComponent<TextMeshProUGUI>();
            PlayerHealthText.alignment = TextAlignmentOptions.Left;
            PlayerHealthText.fontSize = FontSize;
            ApplyTextShadow(PlayerHealthText);
            Logger.Debug("Player Healthbar initialized");
            return this;
        }

        /// <summary>
        /// Sets the player instance associated with this health bar.
        /// </summary>
        /// <param name="player">The player instance to associate.</param>
        public void SetPlayer(Player player) { Player = player; }

        /// <summary>
        /// Gets the player instance associated with this health bar.
        /// </summary>
        /// <returns>The associated player instance.</returns>
        public Player GetPlayer() { return Player; }

        /// <summary>
        /// Retrieves the current health value of the associated player.
        /// </summary>
        /// <returns>The player's current health as a float.</returns>
        public float GetPlayerHealth() { return Player.Health.CurrentHealth; }

        /// <summary>
        /// Applies a shadow effect to the specified TextMeshProUGUI text element for improved readability.
        /// </summary>
        /// <param name="text">The TextMeshProUGUI element to modify.</param>
        public void ApplyTextShadow(TextMeshProUGUI text)
        {
            text.fontMaterial = UnityEngine.Object.Instantiate<Material>(text.font.material);
            Material fontMaterial = text.fontMaterial;
            fontMaterial.EnableKeyword("UNDERLAY_ON");
            fontMaterial.SetFloat(ShaderUtilities.ID_UnderlaySoftness, 0.2f);
            fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, 0.8f);
            fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, Color.black);
        }

        /// <summary>
        /// Updates the health bar's fill and text group alpha values based on the current state and whether the phone UI is open.
        /// Handles fading and visibility logic.
        /// </summary>
        /// <param name="phoneOpen">Indicates if the phone UI is currently open.</param>
        public void Update(bool phoneOpen)
        {
            bool fillExists = PlayerHealthbarImage != null;
            bool textExists = PlayerHealthTextGroup != null;
            if (phoneOpen)
            {
                if (!LastPhoneOpen)
                    PauseStartTime = Time.time;
                LastPhoneOpen = true;
                if (fillExists)
                {
                    Color color = PlayerHealthbarImage.color;
                    color.a = 1f;
                    PlayerHealthbarImage.color = color;
                }

                if (textExists)
                    PlayerHealthTextGroup.alpha = 1f;
            }
            else
            {
                if (LastPhoneOpen)
                {
                    PauseAccumulated += Time.time - PauseStartTime;
                    LastPhoneOpen = false;
                }
                float timePassed = Time.time - LastHealthUpdateTime;
#if !MONO
                PlayerHealthbarSlider.Set(Mathf.Lerp(PlayerHealthbarSlider.value, CurrentFill, Time.deltaTime * FadeDelay));
#else
                PlayerHealthbarSlider.value = Mathf.Lerp(PlayerHealthbarSlider.value, CurrentFill, Time.deltaTime * FadeDelay);
#endif
                float finalAlpha = 1f;
                if (Preferences.FadeHealthText.Value)
                {
                    bool onDamage = !Preferences.ShowOnDamage.Value;
                    if (onDamage)
                        finalAlpha = 0f;
                    else
                        finalAlpha = ((timePassed > FadeDelay) ? 0f : 1f);
                }
                
                bool fadeBar = Preferences.FadeOutBar.Value;
                if (fillExists)
                {
                    Color color = PlayerHealthbarImage.color;
                    color.a = Mathf.Lerp(color.a, finalAlpha, Time.deltaTime * FadeSpeed);
                    if (!fadeBar)
                        color.a = 1f;
                    PlayerHealthbarImage.color = color;
                }

                if (textExists)
                {
                    if (!Preferences.FadeHealthText.Value)
                        PlayerHealthTextGroup.alpha = 1f;
                    else
                        PlayerHealthTextGroup.alpha = Mathf.Lerp(PlayerHealthTextGroup.alpha, finalAlpha, Time.deltaTime * FadeSpeed);
                }
            }
        }

        /// <summary>
        /// Updates the displayed health value in the health bar text and records the update time.
        /// </summary>
        public void UpdateText()
        {
            CurrentFill = GetPlayerHealth();
            var playerHealthText = $"{Mathf.FloorToInt(GetPlayerHealth())} / 100 HP";
            if (playerHealthText != PlayerHealthText.text)
            {
                PlayerHealthText.text = playerHealthText;
                LastHealthUpdateTime = Time.time;
            }
        }

        /// <summary>
        /// Makes the health bar and its text fully visible, typically after a health update.
        /// </summary>
        public void Show()
        {
            if (!Preferences.FadeOutBar.Value)
            {
                LastHealthUpdateTime = Time.time;
                PauseAccumulated = 0f;
                bool barFill = PlayerHealthbarImage != null;
                bool textGroup = PlayerHealthTextGroup != null;
                if(barFill)
                {
                    Color newColor = PlayerHealthbarImage.color;
                    newColor.a = 1f;
                    PlayerHealthbarImage.color = Color.Lerp(PlayerHealthbarImage.color, newColor, FadeSpeed);
                }
                if(textGroup)
                {
                    PlayerHealthTextGroup.alpha = Mathf.Lerp(PlayerHealthTextGroup.alpha, 1f, FadeSpeed);
                }
            }
        }

        /// <summary>
        /// Gets the currently displayed health value on the health bar.
        /// </summary>
        /// <returns>The displayed health as a float.</returns>
        public float GetDisplayedHealth() { return CurrentFill; }
    }
}
