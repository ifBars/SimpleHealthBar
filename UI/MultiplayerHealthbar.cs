using Il2CppScheduleOne.PlayerScripts;
using Il2CppTMPro;
using MelonLoader;
using SimpleHealthBar.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleHealthBar.UI
{
    class MultiplayerHealthbar
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
        private Player PlayerInstance;
        private float LastHealthUpdateTime;
        private float CurrentFill;
        private Vector2 AnchorMin;
        private Vector2 AnchorMax;
        private Vector2 AnchorPos;
        private bool IsHidden;

        private float FadeSpeed = Preferences.FadeSpeed.Value;
        private float FadeDelay = Preferences.FadeDelay.Value;

        /// <summary>
        /// Initializes the multiplayer healthbar for a specific player, creating and configuring all UI elements.
        /// </summary>
        /// <param name="interactionCanvas">The parent canvas transform for the healthbar.</param>
        /// <param name="anchorPos">The anchored position for the healthbar UI.</param>
        /// <param name="player">The player instance this healthbar represents.</param>
        /// <returns>The initialized MultiplayerHealthbar instance, or null if the player is null.</returns>
        public MultiplayerHealthbar Init(Transform interactionCanvas, Vector2 anchorPos, Player player)
        {
            Logger = new MelonLogger.Instance(BuildInfo.Name);
            AnchorPos = anchorPos;
            PlayerInstance = player;

            if (PlayerInstance == null)
                return null;
            PlayerHealthbarBase = new GameObject(player.name+"HealthBar");
            PlayerHealthbarBase.transform.SetParent(interactionCanvas, false);
            PlayerHealthbarBase.AddComponent<RectTransform>();
            PlayerHealthbarSlider = PlayerHealthbarBase.AddComponent<Slider>();

            RectTransform barBase = PlayerHealthbarBase.GetComponent<RectTransform>();
            //barBase.anchorMin = new Vector2(0.235f, 0f);
            //barBase.anchorMax = new Vector2(0.775f, 0f);
            //barBase.anchoredPosition = new Vector2(0f, 105f);
            //barBase.anchorMin = AnchorMin;
            //barBase.anchorMax = AnchorMax;
            barBase.anchorMin = new Vector2(0.8f, 0f);
            barBase.anchorMax = new Vector2(0.99f, 0f);
            barBase.anchoredPosition = AnchorPos;
            barBase.sizeDelta = new Vector2(0f, 2f);

            PlayerHealthbarSlider.minValue = 0f;
            PlayerHealthbarSlider.maxValue = 100f;
            PlayerHealthbarSlider.value = 0;

            GameObject fill = new GameObject(player.name + "HealthbarFill");
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

            GameObject playerHealthTextGroup = new GameObject(player.name + "HealthTextGroup");
            playerHealthTextGroup.transform.SetParent(fill.transform, false);
            PlayerHealthTextGroup = playerHealthTextGroup.AddComponent<CanvasGroup>();
            RectTransform textGroupDisplay = playerHealthTextGroup.AddComponent<RectTransform>();
            textGroupDisplay.anchorMin = new Vector2(0f, 1f);
            textGroupDisplay.anchorMax = new Vector2(0f, 1f);
            textGroupDisplay.pivot = new Vector2(0f, 1f);
            textGroupDisplay.anchoredPosition = new Vector2(0f, 0f);
            textGroupDisplay.sizeDelta = new Vector2(0f, 0f);

            GameObject playerHealthText = new GameObject(player.name + "HealthText");
            playerHealthText.transform.SetParent(playerHealthTextGroup.transform, false);
            RectTransform playerHealthTextDisplay = playerHealthText.AddComponent<RectTransform>();
            playerHealthTextDisplay.anchorMin = new Vector2(0f, 1f);
            playerHealthTextDisplay.anchorMax = new Vector2(0f, 1f);
            playerHealthTextDisplay.pivot = new Vector2(0f, 1f);
            playerHealthTextDisplay.anchoredPosition = new Vector2(0f, 20f);
            playerHealthTextDisplay.sizeDelta = new Vector2(400f, 20f);
            PlayerHealthText = playerHealthText.AddComponent<TextMeshProUGUI>();
            PlayerHealthText.alignment = TextAlignmentOptions.Left;
            PlayerHealthText.fontSize = 10f;
            ApplyTextShadow(PlayerHealthText);
            IsHidden = false;
            Logger.Debug("Player Healthbar initialized");
            return this;
        }

        // Generate a method that sets the anchored posistion of the PlayerHealthbarBase
        /// <summary>
        /// Sets the anchored position of the healthbar UI element.
        /// </summary>
        /// <param name="anchoredPosition">The new anchored position.</param>
        public void SetAnchoredPosition(Vector2 anchoredPosition)
        {
            AnchorPos = anchoredPosition;
            RectTransform barBase = PlayerHealthbarBase.GetComponent<RectTransform>();
            barBase.anchoredPosition = AnchorPos;
        }

        /// <summary>
        /// Gets the current anchored position of the healthbar UI element.
        /// </summary>
        /// <returns>The anchored position as a Vector2.</returns>
        public Vector2 GetAnchoredPosition() { return AnchorPos; }

        /// <summary>
        /// Retrieves the current health value of the associated player.
        /// </summary>
        /// <returns>The player's current health as a float.</returns>
        public float GetPlayerHealth() { return PlayerInstance.Health.CurrentHealth; }

        /// <summary>
        /// Applies a shadow effect to the specified TextMeshProUGUI text element for better readability.
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
        /// Updates the healthbar's fill and text group alpha values to reflect the current health and visibility state.
        /// </summary>
        public void Update()
        {
            bool fillExists = PlayerHealthbarImage != null;
            bool textExists = PlayerHealthTextGroup != null;

            if (CheckIsSpawned())
            {
                PlayerHealthbarSlider.Set(Mathf.Lerp(PlayerHealthbarSlider.value, CurrentFill, Time.deltaTime * FadeDelay));
                bool fadeBar = Preferences.FadeOutBar.Value;
                if (fillExists)
                {
                    Color color = PlayerHealthbarImage.color;
                    color.a = 1f;
                    PlayerHealthbarImage.color = color;
                }

                if (textExists)
                {
                    PlayerHealthTextGroup.alpha = 1f;
                }
            } 
            else
            {
                Hide();
            }
        }


        /// <summary>
        /// Updates the displayed health value and player name in the healthbar text, and records the update time.
        /// </summary>
        public void UpdateText()
        {
            CurrentFill = GetPlayerHealth();
            var playerHealthText = $"{ PlayerInstance.NameLabel.ShownText } {Mathf.FloorToInt(GetPlayerHealth())} / 100 HP";
            if (playerHealthText != PlayerHealthText.text)
            {
                PlayerHealthText.text = playerHealthText;
                LastHealthUpdateTime = Time.time;
            }
        }

        /// <summary>
        /// Makes the healthbar and its text fully visible, typically after a health update.
        /// </summary>
        public void Show()
        {
            if (CheckIsSpawned())
            {
                LastHealthUpdateTime = Time.time;
                bool barFill = PlayerHealthbarImage != null;
                bool textGroup = PlayerHealthTextGroup != null;
                if (barFill)
                {
                    Color newColor = PlayerHealthbarImage.color;
                    newColor.a = 1f;
                    PlayerHealthbarImage.color = Color.Lerp(PlayerHealthbarImage.color, newColor, FadeSpeed);
                }
                if (textGroup)
                {
                    PlayerHealthTextGroup.alpha = Mathf.Lerp(PlayerHealthTextGroup.alpha, 1f, FadeSpeed);
                }
            }
            else
            {
                Hide();
            }
        }

        /// <summary>
        /// Checks if the player is spawned and updates the visibility of the healthbar accordingly.
        /// </summary>
        public bool CheckIsSpawned()
        {
            bool isSpawned = PlayerInstance?.IsSpawned ?? false;
            IsHidden = !isSpawned;
            return isSpawned;
        }

        /// <summary>
        /// Hides the healthbar and its text by setting their alpha values to zero.
        /// </summary>
        public void Hide()
        {
             bool barFill = PlayerHealthbarImage != null;
             bool textGroup = PlayerHealthTextGroup != null;
             if (barFill)
             {
                 Color newColor = PlayerHealthbarImage.color;
                 newColor.a = 0f;
                 PlayerHealthbarImage.color = newColor;
             }
             if (textGroup)
             {
                 PlayerHealthTextGroup.alpha = 0f;
             }
             IsHidden = true;
        }

        /// <summary>
        /// Gets the currently displayed health value on the healthbar.
        /// </summary>
        /// <returns>The displayed health as a float.</returns>
        public float GetDisplayedHealth() { return CurrentFill; }

        /// <summary>
        /// Gets the player instance associated with this healthbar.
        /// </summary>
        /// <returns>The Player instance.</returns>
        public Player GetPlayer() { return PlayerInstance; }
    }
}
