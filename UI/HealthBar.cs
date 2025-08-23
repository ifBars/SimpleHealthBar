using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleHealthBar.UI;
using SimpleHealthBar;
using SimpleHealthBar.Helpers;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

#if !MONO
using Il2CppTMPro;
using Il2CppRootMotion.FinalIK;
#else
using TMPro;
#endif

namespace SimpleHealthBar.UI
{
    public class HealthBar : IDisposable
    {
        private GameObject HealthBarObject;
        private Slider HealthSlider;
        private Image FillImage;
        private RectTransform HealthTextDisplay;
        private TextMeshProUGUI HealthText;
        private Vector2 AnchorMin;
        private Vector2 AnchorMax;
        private Vector2 AnchoredPos;
        private float LastHealthUpdateTime;
        private float PauseAccumulated;
        private float PauseStartTime;
        private float CurrentFill;
        private float FadeSpeed = 2f;
        private float FadeDelay = 4f;
        private float FontSize = 12f;
        private bool LastPhoneOpened;
        private bool IsHidden;
        private bool IsSpawned;

        public HealthBarType BarType { get; private set; }
        
        public HealthBar(HealthBarType id, Transform interactionCanvas)
        {
            BarType = id;
            HealthBarObject = new GameObject($"{id.ToString()}HealthBar");
            HealthBarObject.transform.SetParent(interactionCanvas, false);
            RectTransform barBase = HealthBarObject.AddComponent<RectTransform>();
            HealthSlider = HealthBarObject.AddComponent<Slider>();
            barBase.sizeDelta = new Vector2(0f, 2f);

            HealthSlider.minValue = 0f;
            HealthSlider.maxValue = 100f;
            HealthSlider.value = 100f;

            GameObject fill = new GameObject($"{id.ToString()}HealthBarFill");
            fill.transform.SetParent(barBase.transform, false);
            RectTransform fillBar = fill.AddComponent<RectTransform>();
            FillImage = fill.AddComponent<Image>();
            FillImage.color = Color.red;
            fillBar.anchorMin = Vector2.zero;
            fillBar.anchorMax = Vector2.one;
            fillBar.offsetMin = Vector2.zero;
            fillBar.offsetMax = Vector2.zero;
            HealthSlider.fillRect = fillBar;

            GameObject healthTextGroup = new GameObject($"{id.ToString()}TextGroup");
            healthTextGroup.transform.SetParent(fill.transform, false);
            HealthTextDisplay = healthTextGroup.AddComponent<RectTransform>();
            HealthTextDisplay.anchorMin = new Vector2(0f, 1f);
            HealthTextDisplay.anchorMax = new Vector2(0f, 1f);
            HealthTextDisplay.pivot = new Vector2(0f, 1f);
            HealthTextDisplay.sizeDelta = new Vector2(400f, 20f);
            HealthText = healthTextGroup.AddComponent<TextMeshProUGUI>();
            HealthText.alignment = TextAlignmentOptions.Left;
            HealthText.fontSize = FontSize;

            SetupCustomDisplay();

            Shadow textShadow = healthTextGroup.AddComponent<Shadow>();
            textShadow.effectColor = Color.black;
            textShadow.effectDistance = new Vector2(1, -1);
        }

        public HealthBar(HealthBarType id, Transform interactionCanvas, Vector2 anchoredPos)
        {
            BarType = id;
            HealthBarObject = new GameObject($"{id.ToString()}HealthBar");
            HealthBarObject.transform.SetParent(interactionCanvas, false);
            RectTransform barBase = HealthBarObject.AddComponent<RectTransform>();
            HealthSlider = HealthBarObject.AddComponent<Slider>();
            barBase.sizeDelta = new Vector2(0f, 2f);

            HealthSlider.minValue = 0f;
            HealthSlider.maxValue = 100f;
            HealthSlider.value = 0;

            GameObject fill = new GameObject($"{id.ToString()}HealthBarFill");
            fill.transform.SetParent(barBase.transform, false);
            RectTransform fillBar = fill.AddComponent<RectTransform>();
            FillImage = fill.AddComponent<Image>();
            FillImage.color = Color.red;
            fillBar.anchorMin = Vector2.zero;
            fillBar.anchorMax = Vector2.one;
            fillBar.offsetMin = Vector2.zero;
            fillBar.offsetMax = Vector2.zero;
            HealthSlider.fillRect = fillBar;

            GameObject healthTextGroup = new GameObject($"{id.ToString()}TextGroup");
            healthTextGroup.transform.SetParent(fill.transform, false);
            HealthTextDisplay = healthTextGroup.AddComponent<RectTransform>();
            HealthTextDisplay.anchorMin = new Vector2(0f, 1f);
            HealthTextDisplay.anchorMax = new Vector2(0f, 1f);
            HealthTextDisplay.pivot = new Vector2(0f, 1f);
            HealthTextDisplay.sizeDelta = new Vector2(400f, 20f);
            HealthText = healthTextGroup.AddComponent<TextMeshProUGUI>();
            HealthText.alignment = TextAlignmentOptions.Left;
            HealthText.fontSize = FontSize;

            AnchoredPos = anchoredPos;
            SetupCustomDisplay();

            Shadow textShadow = healthTextGroup.AddComponent<Shadow>();
            textShadow.effectColor = Color.black;
            textShadow.effectDistance = new Vector2(1, -1);
        }

        private void SetupCustomDisplay()
        {
            RectTransform barBase = HealthBarObject.GetComponent<RectTransform>();
            switch (BarType)
            {
                case HealthBarType.Player:
                    AnchorMin = new Vector2(0.235f, 0f);
                    AnchorMax = new Vector2(0.775f, 0f);
                    AnchoredPos = new Vector2(0f, 105f);

                    HealthTextDisplay.anchoredPosition = new Vector2(500f, 20f);
                    ModLogger.Debug($"Player Healthbar created at {AnchoredPos.ToString()}");
                    break;
                case HealthBarType.NPC:
                    AnchorMin = new Vector2(0.3f, 0f);
                    AnchorMax = new Vector2(0.7f, 0f);
                    AnchoredPos = new Vector2(5f, 1000f);
                    HealthTextDisplay.anchoredPosition = new Vector2(300f, 20f);
                    ModLogger.Debug($"NPC Healthbar created at {AnchoredPos.ToString()}");
                    break;
                case HealthBarType.Multiplayer:
                    AnchorMin = new Vector2(0.8f, 0f);
                    AnchorMax = new Vector2(0.99f, 0f);

                    HealthText.fontSize = 10;
                    ModLogger.Debug($"Multiplayer Healthbar created at {AnchoredPos.ToString()}");
                    break;
                default:
                    break;
            }
            barBase.anchorMin = AnchorMin;
            barBase.anchorMax = AnchorMax;
            barBase.anchoredPosition = AnchoredPos;
        }

        public void SetSpawned(bool isSpawned) { IsSpawned = isSpawned; }
        public bool GetSpawned() => IsSpawned;

        public void SetAnchoredPosition(Vector2 anchoredPosition)
        {
            AnchoredPos = anchoredPosition;
            AnchoredPos = anchoredPosition;
            RectTransform barBase = HealthBarObject.GetComponent<RectTransform>();
            barBase.anchoredPosition = anchoredPosition;
        }

        public Vector2 GetAnchoredPosition() => AnchoredPos;
        public void SetCurrentHealth(float health) 
        { 
            CurrentFill = health; 
        }
        public float GetCurrentHealth() => CurrentFill;

        public void Update()
        {
            bool fillExists = FillImage != null;
            bool textExists = HealthText != null;

            if (fillExists && textExists)
            {
                // Update the health bar fill and text
                switch (BarType)
                {
                    case HealthBarType.NPC:
                        UpdateNPC();
                        break;
                    default:
                        break;
                }
            }
            else
            {

            }
        }

        public void Update(bool var)
        {
            switch (BarType)
            {
                case HealthBarType.Player:
                    UpdatePlayer(var);
                    break;
                case HealthBarType.Multiplayer:
                    UpdateMultiplayer(var);
                    break;
                default:
                    break;
            }
        }

        private void UpdatePlayer(bool phoneOpen)
        {
            bool fillExists = FillImage != null;
            bool textExists = HealthText != null;
            if (phoneOpen)
            {
                if (!LastPhoneOpened)
                    LastHealthUpdateTime = Time.time;
                LastPhoneOpened = true;
                if (fillExists)
                {
                    Color color = FillImage.color;
                    color.a = Mathf.Lerp(color.a, 1f, Time.deltaTime * 2f);
                    FillImage.color = color;
                }

                if (textExists)
                    HealthText.alpha = Mathf.Lerp(HealthText.alpha, 1f, Time.deltaTime * 2f);
            }
            else
            {
                if (LastPhoneOpened)
                {
                    PauseAccumulated += Time.time - PauseStartTime;
                    LastPhoneOpened = false;
                }

                float timePassed = Time.time - LastHealthUpdateTime;
                float newSliderValue = Mathf.Lerp(HealthSlider.value, CurrentFill, Time.deltaTime * FadeDelay);
                
#if !MONO
                HealthSlider.Set(newSliderValue);
#else
                HealthSlider.value = newSliderValue;
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
                    Color color = FillImage.color;
                    color.a = Mathf.Lerp(color.a, finalAlpha, Time.deltaTime * FadeSpeed);
                    if (!fadeBar)
                        color.a = 1f;
                    FillImage.color = color;
                }

                if (textExists)
                {
                    if (!Preferences.FadeHealthText.Value)
                        HealthText.alpha = 1f;
                    else
                        HealthText.alpha = Mathf.Lerp(HealthText.alpha, finalAlpha, Time.deltaTime * FadeSpeed);
                }
            }
        }

        private void UpdateNPC()
        {
            // Update the health bar fill and text for NPCs
            bool fillExists = FillImage != null;
            bool textExists = HealthText != null;
            
            if (fillExists && textExists)
            {
                // Update the slider value to show current health
                float newSliderValue = Mathf.Lerp(HealthSlider.value, CurrentFill, Time.deltaTime * FadeDelay);
                
#if !MONO
                HealthSlider.Set(newSliderValue);
#else
                HealthSlider.value = newSliderValue;
#endif
                
                // Handle fading logic for NPC health bars
                float timePassed = Time.time - LastHealthUpdateTime;
                float finalAlpha = 1f;
                
                if (Preferences.FadeHealthText.Value)
                {
                    finalAlpha = ((timePassed > FadeDelay) ? 0f : 1f);
                }
                
                bool fadeBar = Preferences.FadeOutBar.Value;
                if (fillExists)
                {
                    Color color = FillImage.color;
                    color.a = Mathf.Lerp(color.a, finalAlpha, Time.deltaTime * FadeSpeed);
                    if (!fadeBar)
                        color.a = 1f;
                    FillImage.color = color;
                }

                if (textExists)
                {
                    if (!Preferences.FadeHealthText.Value)
                        HealthText.alpha = 1f;
                    else
                        HealthText.alpha = Mathf.Lerp(HealthText.alpha, finalAlpha, Time.deltaTime * FadeSpeed);
                }
            }
        }

        private void UpdateMultiplayer(bool isSpawned)
        {
            bool fillExists = FillImage != null;
            bool textExists = HealthText != null;
            if (isSpawned)
            {
                float newSliderValue = Mathf.Lerp(HealthSlider.value, CurrentFill, Time.deltaTime * FadeDelay);
                
#if !MONO
                HealthSlider.Set(newSliderValue);
#else
                HealthSlider.value = newSliderValue;
#endif
                bool fadeBar = Preferences.FadeOutBar.Value;
                if (fillExists)
                {
                    Color color = FillImage.color;
                    color.a = 1f;
                    FillImage.color = Color.Lerp(FillImage.color, color, Time.deltaTime * FadeSpeed);
                }

                if (textExists)
                {
                    HealthText.alpha = Mathf.Lerp(HealthText.alpha, 1f, Time.deltaTime * FadeSpeed);
                }
            }
            else
            {
                Hide();
            }
        }

        public void UpdateText()
        {
            CurrentFill = GetCurrentHealth();
            var healthText = $"{Mathf.FloorToInt(GetCurrentHealth())} / 100 HP";
            if (HealthText.text != healthText)
            {
                HealthText.text = healthText;
                LastHealthUpdateTime = Time.time;
            }
        }

        public void UpdateText(string name)
        {
            CurrentFill = GetCurrentHealth();
            var healthText = $"{name} - {Mathf.FloorToInt(GetCurrentHealth())} / 100 HP";
            if (CurrentFill == 0f)
                HealthText.text = "";
            if (HealthText.text != healthText)
            {
                HealthText.text = healthText;
                LastHealthUpdateTime = Time.time;
            }
        }

        public void Show()
        {
            switch (BarType) {
                case HealthBarType.Player:
                    ShowPlayer();
                    break;
                case HealthBarType.NPC:
                    ShowNPC();
                    break;
                case HealthBarType.Multiplayer:
                    ShowMultiplayer();
                    break;
            }
        }

        public void Hide()
        {
            bool barFill = FillImage != null;
            bool textGroup = HealthText != null;
            if (barFill)
            {
                Color color = FillImage.color;
                color.a = 0f;
                FillImage.color = Color.Lerp(FillImage.color, color, Time.deltaTime * FadeSpeed);
            }

            if (textGroup)
                HealthText.alpha = Mathf.Lerp(HealthText.alpha, 0f, Time.deltaTime * FadeSpeed);
            IsHidden = true;
        }

        public void ShowPlayer()
        {
            if (!Preferences.FadeOutBar.Value)
            {
                LastHealthUpdateTime = Time.time;
                PauseAccumulated = 0f;
                bool barFill = FillImage != null;
                bool textGroup = HealthText != null;
                if (barFill)
                {
                    Color color = FillImage.color;
                    color.a = 1f;
                    FillImage.color = Color.Lerp(FillImage.color, color, FadeSpeed);
                }
                if (textGroup)
                {
                    HealthText.alpha = Mathf.Lerp(HealthText.alpha, 1f, FadeSpeed);
                }
            }
        }

        public void ShowNPC()
        {
            if (!Preferences.NPCHealthBarEnabled.Value)
                return;
            LastHealthUpdateTime = Time.time;
            // Do the distance check in the manager class instead of the healthbar class now
            if (FillImage != null)
            {
                Color color = FillImage.color;
                color.a = 1f;
                FillImage.color = Color.Lerp(FillImage.color, color, FadeSpeed);
            }
            if (HealthText != null)
            {
                HealthText.alpha = Mathf.Lerp(HealthText.alpha, 1f, FadeSpeed);
            }
        }

        public void ShowMultiplayer()
        {
            if (!Preferences.FadeOutBar.Value)
            {
                LastHealthUpdateTime = Time.time;
                bool barFill = FillImage != null;
                bool textGroup = HealthText != null;
                if (barFill)
                {
                    Color color = FillImage.color;
                    color.a = 1f;
                    FillImage.color = Color.Lerp(FillImage.color, color, FadeSpeed);
                }
                if (textGroup)
                {
                    HealthText.alpha = Mathf.Lerp(HealthText.alpha, 1f, FadeSpeed);
                }
            }
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
