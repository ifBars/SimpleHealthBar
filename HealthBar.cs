using Il2CppScheduleOne.UI;
using Il2CppTMPro;
using MelonLoader;
using SimpleHealthBar.Core;
using SimpleHealthBar.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleHealthBar
{
    class HealthBar
    {
        private static MelonLogger.Instance Logger;
        private static bool HasInitialized = false;
        //public HealthBarHandler(HealthBarBuilder builder,
        //                        HealthBarAnimator animator,
        //                        HealthBarModel model,
        //                        MelonLogger.Instance logger)
        //{
        //    _builder = builder;
        //    _animator = animator;
        //    _model = model;
        //    _logger = logger;
        //}

        //public void Init(Transform hudCanvas)
        //{
        //    var comp = _builder.Build(hudCanvas);
        //    _animator.Fill
        //    _logger.Msg("Healthbar initialized")
        //}

        public void Init()
        {
            var hudCanvas = HUD.Instance.transform;
            Logger = new MelonLogger.Instance(BuildInfo.Name);
            //Create a Game Object called a Health Bar
            this._barGO = new GameObject("HealthBar");
            this._barGO.transform.SetParent(hudCanvas, false);
            this._barGO.AddComponent<RectTransform>();
            this._slider = this._barGO.AddComponent<Slider>();

            // Set the location for the bar to show up
            RectTransform barBase = this._barGO.GetComponent<RectTransform>();
            barBase.anchorMin = new Vector2(0.235f, 0f);
            barBase.anchorMax = new Vector2(0.775f, 0f);
            barBase.anchoredPosition = new Vector2(0f, 105f);
            barBase.sizeDelta = new Vector2(0f, 2f);

            // Set the sliders initial values
            this._slider.minValue = 0f;
            this._slider.maxValue = 100f;
            this._slider.value = 0;

            // Create a new Game Object for the fill for the bar
            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(this._barGO.transform, false);
            fill.AddComponent<RectTransform>();
            this._fillImage = fill.AddComponent<Image>();
            this._fillImage.color = Color.red;

            RectTransform fillBar = fill.GetComponent<RectTransform>();
            fillBar.anchorMin = Vector2.zero;
            fillBar.anchorMax = Vector2.one;
            fillBar.offsetMin = Vector2.zero;
            fillBar.offsetMax = Vector2.zero;
            this._slider.fillRect = fillBar;

            // Begin creating text display
            GameObject textGroup = new GameObject("TextGroup");
            textGroup.transform.SetParent(fill.transform, false);
            this._textGroup = textGroup.AddComponent<CanvasGroup>();
            RectTransform textGroupDisplay = textGroup.AddComponent<RectTransform>();
            textGroupDisplay.anchorMin = new Vector2(0f, 1f);
            textGroupDisplay.anchorMax = new Vector2(0f, 1f);
            textGroupDisplay.pivot = new Vector2(0f, 1f);
            textGroupDisplay.anchoredPosition = new Vector2(0f, 0f);
            textGroupDisplay.sizeDelta = new Vector2(0f, 0f);

            GameObject healthText = new GameObject("HealthText");
            healthText.transform.SetParent(textGroup.transform, false);
            RectTransform healthTextDisplay = healthText.AddComponent<RectTransform>();
            healthTextDisplay.anchorMin = new Vector2(0f, 1f);
            healthTextDisplay.anchorMax = new Vector2(0f, 1f);
            healthTextDisplay.pivot = new Vector2(0f, 01f);
            //healthTextDisplay.anchoredPosition = new Vector2(5f, 20f);
            healthTextDisplay.anchoredPosition = new Vector2(500f, 20f);
            healthTextDisplay.sizeDelta = new Vector2(400f, 20f);
            this._healthText = healthText.AddComponent<TextMeshProUGUI>();
            this._healthText.alignment = TextAlignmentOptions.Left;
            this._healthText.fontSize = FontSize;
            this.ApplyTextShadow(this._healthText);
            //this._builder = new HealthBarBuilder();
            //_builder.Build(hudCanvas);
            HasInitialized = true;
        }

        public void Release()
        {
            this._barGO = null;
        }

        public void Update(bool phoneOpen)
        {
            if (!HasInitialized)
                return;
            if (phoneOpen)
            {
                if (!this._lastPhoneOpen)
                {
                    this._pauseStartTime = Time.time;
                }
                this._lastPhoneOpen = true;
                bool healthFillExists = this._fillImage != null;
                if (healthFillExists)
                {
                    Color color = this._fillImage.color;
                    color.a = 1f;
                    this._fillImage.color = color;
                }

                bool healthTextExists = this._textGroup != null;
                if (healthTextExists)
                {
                    this._textGroup.alpha = 1f;
                }
            }
            else
            {
                bool lastPhoneOpen = this._lastPhoneOpen;
                if (lastPhoneOpen)
                {
                    this._pauseAccumulated += Time.time - this._pauseStartTime;
                    this._lastPhoneOpen = false;
                }
                float num = Time.time - this._lastHealthUpdateTime;
                this._slider.Set(Mathf.Lerp(this._slider.value, this._currentFill, Time.deltaTime * Preferences.FadeDelay.Value));
                float b = 1f;

                bool fadeText = Preferences.FadeHealthText.Value;
                if (fadeText)
                {
                    bool showOnDamage = !Preferences.ShowOnDamage.Value;
                    if (showOnDamage)
                    {
                        b = 0f;
                    }
                    else
                    {
                        b = ((num > Preferences.FadeDelay.Value) ? 0f : 1f);
                    }
                }


                bool fillImage = this._fillImage != null;
                if (fillImage && Preferences.FadeOutBar.Value)
                {
                    Color color = this._fillImage.color;
                    color.a = Mathf.Lerp(color.a, b, Time.deltaTime * Preferences.FadeSpeed.Value);
                    this._fillImage.color = color;
                }

                bool textGroup = this._textGroup != null;
                if (textGroup)
                    this._textGroup.alpha = Mathf.Lerp(this._textGroup.alpha, b, Time.deltaTime * Preferences.FadeSpeed.Value);
            }
        }

        public void UpdateHealth(float currentHealth)
        {
            if (!HasInitialized)
                return;
            this._currentFill = currentHealth;
            //DefaultInterpolatedStringHandler stringHandler = new DefaultInterpolatedStringHandler(6, 2);
            //stringHandler.AppendFormatted<float>(currentHealth);
            //stringHandler.AppendLiteral(" / ");
            //stringHandler.AppendFormatted<float>(100f);
            //stringHandler.AppendLiteral(" HP");

            //string healthText = stringHandler.ToStringAndClear();
            var healthText = $"{Mathf.FloorToInt(currentHealth)} / 100 HP";
            if (this._healthText.text != healthText)
            {
                this._healthText.text = healthText;
                bool showOnDamage = Preferences.ShowOnDamage.Value;

                if (showOnDamage)
                {
                    this._lastHealthUpdateTime = Time.time;
                    this._pauseAccumulated = 0f;
                }
            }
        }

        public void Show()
        {
            if (!HasInitialized)
                return;
            this._lastHealthUpdateTime = Time.time;
            this._pauseAccumulated = 0f;
            bool barFill = this._fillImage != null;
            if (barFill)
            {
                Color origColor = this._fillImage.color;
                Color newColor = origColor;
                newColor.a = 1f;
                this._fillImage.color = Color.Lerp(origColor, newColor, Preferences.FadeSpeed.Value);
            }
            bool textGroup = this._textGroup != null;
            if (textGroup)
            {
                this._textGroup.alpha = Mathf.Lerp(this._textGroup.alpha, 1f, Preferences.FadeSpeed.Value);
            }
        }

        public void ApplyTextShadow(TextMeshProUGUI text)
        {
            text.fontMaterial = UnityEngine.Object.Instantiate<Material>(text.font.material);
            Material fontMaterial = text.fontMaterial;
            fontMaterial.EnableKeyword("UNDERLAY_ON");
            fontMaterial.SetFloat(ShaderUtilities.ID_UnderlaySoftness, 0.2f);
            fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, 0.8f);
            fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, Color.black);
        }

        /*
         * Begin Variable Definitions
         */

        private GameObject _barGO;
        private Slider _slider;
        private Image _fillImage;
        private TextMeshProUGUI _healthText;
        private CanvasGroup _textGroup;
        private float _lastHealthUpdateTime;
        private float _currentFill = 0f;
        private const float LerpSpeed = 5f;
        private const float BarHeight = 2f;
        private const float BarOffsetY = 8f;
        private const float LabelX = 5f;
        private const float HealthY = 38f;
        private const float LabelWidth = 400f;
        private const float LabelHeight = 20f;
        private const int FontSize = 14;
        private bool _lastPhoneOpen = false;
        private float _pauseStartTime = 0f;
        private float _pauseAccumulated = 0f;

        readonly HealthBarAnimator _animator;
        private HealthBarBuilder _builder;
        readonly HealthBarModel _model;
        readonly MelonLogger.Instance _logger;
    }
}
//using MelonLoader;
//using UnityEngine;
//using SimpleHealthBar.Core;
//using SimpleHealthBar.UI;

//namespace SimpleHealthBar
//{
//    /// <summary>
//    /// Coordinates UI construction, state tracking, and animation for the health bar.
//    /// </summary>
//    public class HealthBarHandler
//    {
//        private readonly HealthBarBuilder _builder;
//        private readonly MelonLogger.Instance _logger;

//        private HealthBarComponents _components;
//        private HealthBarModel _model;
//        private HealthBarAnimator _animator;

//        /// <summary>
//        /// Initializes a new instance of the <see cref="HealthBarHandler"/> class.
//        /// </summary>
//        /// <param name="builder">The UI builder for constructing the health bar elements.</param>
//        /// <param name="logger">The MelonLoader logger instance for debug output.</param>
//        public HealthBarHandler(HealthBarBuilder builder, MelonLogger.Instance logger)
//        {
//            _builder = builder;
//            _logger = logger;
//        }

//        /// <summary>
//        /// Initializes the health bar by building the UI, setting initial state, and configuring the animator.
//        /// </summary>
//        /// <param name="hudCanvas">Parent canvas transform under which the bar will be placed.</param>
//        /// <param name="initialHealth">The player's starting health.</param>
//        /// <param name="initialMaxHealth">The player's starting maximum health.</param>
//        public void Init(Transform hudCanvas, float initialHealth, float initialMaxHealth)
//        {
//            _components = _builder.Build(hudCanvas);
//            _model = new HealthBarModel(initialHealth, initialMaxHealth);
//            _animator = new HealthBarAnimator(
//                _components.FillImage,
//                _components.CanvasGroup,
//                Preferences.FadeSpeed.Value
//            );

//            // Set initial text
//            _components.HealthText.text = $"{initialHealth:0}/{initialMaxHealth:0} HP";
//            _logger.Msg("SimpleHealthBar: Initialized health bar UI");
//        }

//        /// <summary>
//        /// Updates the health values and refreshes the displayed text.
//        /// Call this whenever the player's health changes.
//        /// </summary>
//        /// <param name="currentHealth">The player's current health value.</param>
//        /// <param name="maxHealth">The player's maximum health value.</param>
//        public void UpdateHealth(float currentHealth, float maxHealth)
//        {
//            _model.SetHealth(currentHealth, maxHealth);
//            _components.HealthText.text = $"{currentHealth:0}/{maxHealth:0} HP";
//        }

//        /// <summary>
//        /// Called every frame to adjust visibility based on damage and UI events.
//        /// </summary>
//        /// <param name="phoneOpen">Whether the in-game phone/UI is currently open.</param>
//        public void Update(bool phoneOpen)
//        {
//            _model.SetPhoneOpen(phoneOpen);
//            float targetAlpha = _model.GetTargetAlpha(
//                Preferences.FadeDelay.Value,
//                Preferences.ShowOnDamage.Value
//            );
//            _animator.FadeTo(targetAlpha);
//        }
//    }
//}
