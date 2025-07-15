using Il2CppFluffyUnderware.DevTools.Extensions;
using Il2CppScheduleOne.NPCs;
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
        /*
         * End Variable Definition
         */

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

        public void SetNPC(NPC npc) { selectedNPC = npc; }

        public NPC GetNPC() { return selectedNPC; }

        public void ApplyTextShadow(TextMeshProUGUI text)
        {
            text.fontMaterial = UnityEngine.Object.Instantiate(text.font.material);
            Material fontMaterial = text.fontMaterial;
            fontMaterial.EnableKeyword("UNDERLAY_ON");
            fontMaterial.SetFloat(ShaderUtilities.ID_UnderlaySoftness, 0.2f);
            fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, 0.8f);
            fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, Color.black);
        }

        public void Update()
        {
            if (selectedNPC == null || !HasIninitialized || !Preferences.NPCHealthBarEnabled.Value)
                return;
            float timePassed = Time.time - LastHealthUpdateTime;
            NPCHealthBarSlider.value = Mathf.Lerp(NPCHealthBarSlider.value, CurrentFill, Time.deltaTime * Preferences.FadeDelay.Value);
            //Logger.Debug("Slider set to " + Mathf.Lerp(NPCHealthBarSlider.value, CurrentFill, Time.deltaTime * FadeDelay));
            float b = 1f;
            b = timePassed > FadeDelay ? 0f : 1f;
            bool fillImage = NPCHealthBarFillImage != null;
            bool fadeBar = Preferences.FadeOutNPCBar.Value;
            if (fillImage)
            {
                Color color = NPCHealthBarFillImage.color;
                color.a = Mathf.Lerp(color.a, b, Time.deltaTime * Preferences.FadeSpeed.Value);
                if (!fadeBar)
                    color.a = 1f;
                NPCHealthBarFillImage.color = color;
            }
            bool textGroup = NPCHealthTextGroup != null;
            if (GetNPCHealth() == 0f)
            {
                NPCHealthTextGroup.alpha = Mathf.Lerp(NPCHealthTextGroup.alpha, 0f, Time.deltaTime + Preferences.FadeSpeed.Value);
            }
            if (textGroup && fadeBar)
                NPCHealthTextGroup.alpha = Mathf.Lerp(NPCHealthTextGroup.alpha, b, Time.deltaTime * Preferences.FadeSpeed.Value);
            else
                NPCHealthTextGroup.alpha = 1f;
        }

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

        public void Show()
        {
            if (Preferences.NPCHealthBarEnabled.Value)
                return;
            LastHealthUpdateTime = Time.time;
            bool barFill = NPCHealthBarFillImage != null;
            if(barFill)
            {
                Color newColor = NPCHealthBarFillImage.color;
                newColor.a = 1f;
                NPCHealthBarFillImage.color = Color.Lerp(NPCHealthBarFillImage.color, newColor, Preferences.FadeSpeed.Value);
            }
            bool textGroup = NPCHealthTextGroup != null;
            if(textGroup)
            {
                NPCHealthTextGroup.alpha = Mathf.Lerp(NPCHealthTextGroup.alpha, 1f, Preferences.FadeSpeed.Value);
            }
        }

        public float GetNPCHealth()
        {
            if (selectedNPC != null && selectedNPC.Health != null)
            {
                return selectedNPC.Health.Health;
            }
            else
                return 0f;
        }

        public float GetDisplayedHealth() { return CurrentFill; }
    }
}
