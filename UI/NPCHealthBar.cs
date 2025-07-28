using Il2CppFluffyUnderware.DevTools.Extensions;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppTMPro;
using MelonLoader;
using SimpleHealthBar.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleHealthBar.UI
{
    class NPCHealthBar
    {

        /*
         * Begin Variable Definition
         */
        private MelonLogger.Instance Logger;
        private GameObject NPCHealthBarBase;
        private Slider NPCHealthBarSlider;
        private Image NPCHealthBarFillImage;
        private TextMeshProUGUI NPCHealthText;
        private CanvasGroup NPCHealthTextGroup;
        private NPC selectedNPC;
        private float FadeSpeed = 2f;
        private float FadeDelay = 5f;
        private float LastHealthUpdateTime;
        private float CurrentFill;
        private bool HasIninitialized;
        private bool IsOutOfSight;
        /*
         * End Variable Definition
         */

        /// <summary>
        /// Initializes the NPC health bar UI, creating all necessary GameObjects and UI components, and setting their properties.
        /// </summary>
        /// <param name="interactionCanvas">The parent canvas transform for the health bar.</param>
        /// <returns>The initialized NPCHealthBar instance.</returns>
        public NPCHealthBar Init(Transform interactionCanvas)
        {
            Logger = new MelonLogger.Instance(BuildInfo.Name);
            /*
             * Begin creating the base game object for the NPC's health bar.
             * After attatching the object to the interaction canvas, define the slider
             */
            NPCHealthBarBase = new GameObject("NPCHealthBar");
            NPCHealthBarBase.transform.SetParent(interactionCanvas, false);
            NPCHealthBarBase.AddComponent<RectTransform>();
            NPCHealthBarSlider = NPCHealthBarBase.AddComponent<Slider>();

            /*
             * Set the location for the bar to show up
             */
            RectTransform barBase = NPCHealthBarBase.GetComponent<RectTransform>();
            barBase.anchorMin = new Vector2(0.30f, 0f);
            barBase.anchorMax = new Vector2(0.70f, 0f);
            barBase.anchoredPosition = new Vector2(5f, 1000f);
            barBase.sizeDelta = new Vector2(0f, 2f);

            /*
             * Set the initial values for the slider
             */
            NPCHealthBarSlider.minValue = 0f;
            NPCHealthBarSlider.maxValue = 100f;
            NPCHealthBarSlider.value = 0;

            /*
             * Create a new game object for the fill for the bar
             */
            GameObject fill = new GameObject("NPCHealthBarFill");
            fill.transform.SetParent(barBase.transform, false);
            fill.AddComponent<RectTransform>();
            NPCHealthBarFillImage = fill.AddComponent<Image>();
            NPCHealthBarFillImage.color = Color.red;
            RectTransform fillBar = fill.GetComponent<RectTransform>();
            fillBar.anchorMin = Vector2.zero;
            fillBar.anchorMax = Vector2.one;
            fillBar.offsetMin = Vector2.zero;
            fillBar.offsetMax = Vector2.zero;
            NPCHealthBarSlider.fillRect = fillBar;

            /*
             * Begin creating the text display
             */
            GameObject textGroup = new GameObject("NPCHealthBarTextGroup");
            textGroup.transform.SetParent(fill.transform, false);
            NPCHealthTextGroup = textGroup.AddComponent<CanvasGroup>();
            RectTransform textGroupDisplay = textGroup.AddComponent<RectTransform>();
            textGroupDisplay.anchorMin = new Vector2(0f, 1f);
            textGroupDisplay.anchorMax = new Vector2(0f, 1f);
            textGroupDisplay.pivot = new Vector2(0f, 1f);
            textGroupDisplay.anchoredPosition = new Vector2(0f, 0f);
            textGroupDisplay.sizeDelta = new Vector2(0f, 1f);

            GameObject npcHealthText = new GameObject("NPCHealthText");
            npcHealthText.transform.SetParent(textGroupDisplay.transform, false);
            RectTransform npcHealthTextDisplay = npcHealthText.AddComponent<RectTransform>();
            npcHealthTextDisplay.anchorMin = new Vector2(0f, 1f);
            npcHealthTextDisplay.anchorMax = new Vector2(0f, 1f);
            npcHealthTextDisplay.pivot = new Vector2(0f, 1f);
            npcHealthTextDisplay.anchoredPosition = new Vector2(300f, 20f);
            npcHealthTextDisplay.sizeDelta = new Vector2(400f, 20f);
            NPCHealthText = npcHealthText.AddComponent<TextMeshProUGUI>();
            NPCHealthText.alignment = TextAlignmentOptions.Left;
            NPCHealthText.fontSize = 16;
            ApplyTextShadow(NPCHealthText);
            HasIninitialized = true;
            return this;
        }

        /// <summary>
        /// Sets the NPC instance associated with this health bar.
        /// </summary>
        /// <param name="npc">The NPC instance to associate.</param>
        public void SetNPC(NPC npc) { selectedNPC = npc; }

        /// <summary>
        /// Gets the NPC instance associated with this health bar.
        /// </summary>
        /// <returns>The associated NPC instance.</returns>
        public NPC GetNPC() { return selectedNPC; }

        /// <summary>
        /// Applies a shadow effect to the specified TextMeshProUGUI text element for improved readability.
        /// </summary>
        /// <param name="text">The TextMeshProUGUI element to modify.</param>
        public void ApplyTextShadow(TextMeshProUGUI text)
        {
            text.fontMaterial = UnityEngine.Object.Instantiate(text.font.material);
            Material fontMaterial = text.fontMaterial;
            fontMaterial.EnableKeyword("UNDERLAY_ON");
            fontMaterial.SetFloat(ShaderUtilities.ID_UnderlaySoftness, 0.2f);
            fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, 0.8f);
            fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, Color.black);
        }

        /// <summary>
        /// Updates the health bar's fill and text group alpha values based on the current state, distance from the player, and preferences.
        /// Handles fading, visibility, and distance logic.
        /// </summary>
        public void Update()
        {
            if (selectedNPC == null || !HasIninitialized || !Preferences.NPCHealthBarEnabled.Value)
                return;
            float timePassed = Time.time - LastHealthUpdateTime;
            NPCHealthBarSlider.value = Mathf.Lerp(NPCHealthBarSlider.value, CurrentFill, Time.deltaTime * Preferences.FadeDelay.Value);
            float b = 1f;
            b = timePassed > FadeDelay ? 0f : 1f;
            bool fillImage = NPCHealthBarFillImage != null;
            bool fadeBar = Preferences.FadeOutNPCBar.Value;
            bool checkDistance = CheckDistanceFromPlayer();
            if (fillImage && checkDistance)
            {
                Color color = NPCHealthBarFillImage.color;
                color.a = Mathf.Lerp(color.a, b, Time.deltaTime * Preferences.FadeSpeed.Value);
                if(!checkDistance)
                {
                    color.a = Mathf.Lerp(color.a, 0f, Time.deltaTime * Preferences.FadeSpeed.Value);
                }
                if (!fadeBar)
                    color.a = 1f;
                NPCHealthBarFillImage.color = color;
            } 
            else if (fillImage && !checkDistance)
            {
                Color color = NPCHealthBarFillImage.color;
                color.a = Mathf.Lerp(color.a, 0f, Time.deltaTime * Preferences.FadeSpeed.Value);
                if (!fadeBar)
                    color.a = 1f;
                NPCHealthBarFillImage.color = color;
            }
                bool textGroup = NPCHealthTextGroup != null;
            if (GetNPCHealth() == 0f)
            {
                NPCHealthTextGroup.alpha = Mathf.Lerp(NPCHealthTextGroup.alpha, 0f, Time.deltaTime + Preferences.FadeSpeed.Value);
            }
            if (textGroup && fadeBar && checkDistance)
                NPCHealthTextGroup.alpha = Mathf.Lerp(NPCHealthTextGroup.alpha, b, Time.deltaTime * Preferences.FadeSpeed.Value);
            else if (textGroup && fadeBar && !checkDistance)
                NPCHealthTextGroup.alpha = Mathf.Lerp(NPCHealthTextGroup.alpha, 0f, Time.deltaTime * Preferences.FadeSpeed.Value);
            else
                NPCHealthTextGroup.alpha = 1f;
        }

        /// <summary>
        /// Updates the displayed health value and NPC name in the health bar text, and records the update time.
        /// </summary>
        public void UpdateText()
        {
            CurrentFill = selectedNPC.Health.Health;
            var npcHealthText = $"{selectedNPC.fullName} – {Mathf.FloorToInt(GetNPCHealth())} / 100 HP";
            if (CurrentFill == 0f)
            {
                NPCHealthText.text = "";
                return;
            }
            if (NPCHealthText.text != npcHealthText)
            {
                NPCHealthText.text = npcHealthText;
                LastHealthUpdateTime = Time.time;
            }
        }

        /// <summary>
        /// Makes the health bar and its text fully visible if the NPC is within the fade-out distance.
        /// </summary>
        public void Show()
        {
            if (Preferences.NPCHealthBarEnabled.Value)
                return;
            LastHealthUpdateTime = Time.time;
            bool checkDistance = CheckDistanceFromPlayer();
            bool barFill = NPCHealthBarFillImage != null;
            if(barFill && checkDistance)
            {
                Color newColor = NPCHealthBarFillImage.color;
                newColor.a = 1f;
                NPCHealthBarFillImage.color = Color.Lerp(NPCHealthBarFillImage.color, newColor, Preferences.FadeSpeed.Value);
            }
            bool textGroup = NPCHealthTextGroup != null;
            if(textGroup && checkDistance)
            {
                NPCHealthTextGroup.alpha = Mathf.Lerp(NPCHealthTextGroup.alpha, 1f, Preferences.FadeSpeed.Value);
            }
        }

        /// <summary>
        /// Retrieves the current health value of the associated NPC.
        /// </summary>
        /// <returns>The NPC's current health as a float, or 0 if unavailable.</returns>
        public float GetNPCHealth()
        {
            if (selectedNPC != null && selectedNPC.Health != null)
            {
                return selectedNPC.Health.Health;
            }
            else
                return 0f;
        }

        /// <summary>
        /// Gets the currently displayed health value on the health bar.
        /// </summary>
        /// <returns>The displayed health as a float.</returns>
        public float GetDisplayedHealth() { return CurrentFill; }

        /// <summary>
        /// Calculates the squared distance from the NPC to the local player's camera position.
        /// </summary>
        /// <returns>The squared distance as a float.</returns>
        public float GetDistanceFromPlayer()
        {
            return (selectedNPC.Movement.FootPosition - Player.Local.CameraPosition).sqrMagnitude;
        }

        /// <summary>
        /// Checks if the NPC is within the fade-out distance from the player and updates out-of-sight state.
        /// </summary>
        /// <returns>True if the NPC is within the fade-out distance, otherwise false.</returns>
        public bool CheckDistanceFromPlayer()
        {
            bool check = GetDistanceFromPlayer() < Preferences.NPCFadeOutDistance.Value;
            if (!check && !IsOutOfSight)
                IsOutOfSight = true;
            else if (check && IsOutOfSight)
            {
                IsOutOfSight = false;
                LastHealthUpdateTime = Time.time;
            }
                return check;
        }

        /// <summary>
        /// Determines if the NPC has just come back within the fade-out distance after being out of sight.
        /// </summary>
        /// <returns>True if the NPC's distance status has changed to in-range, otherwise false.</returns>
        public bool CheckDistanceChanged()
        {
            bool check = GetDistanceFromPlayer() < Preferences.NPCFadeOutDistance.Value;
            if (check && IsOutOfSight)
            {
                return true;
            }
            return false;
        }
    }
}
