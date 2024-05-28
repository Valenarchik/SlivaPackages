using TMPro;
using UnityEngine;

namespace Localization
{
    public sealed class LanguageText : MonoBehaviour
    {
        [SerializeField] private LanguageField<string> value;

        [SerializeField] private TMP_FontAsset uniqueFontTMP;
        [SerializeField] private bool changeOnlyFont;
        [SerializeField] private int fontNumber;

        private TMP_Text textMPComponent;
        private int baseFontSize;
        private LocalizationSettings settings;

        public bool ChangeOnlyFont
        {
            get => changeOnlyFont;
            set => changeOnlyFont = value;
        }

        public int FontNumber
        {
            get => fontNumber;
            set => fontNumber = value;
        }

        private void Awake()
        {
            settings = LocalizationManager.Settings;
            textMPComponent = GetComponent<TextMeshProUGUI>();
            baseFontSize = Mathf.RoundToInt(textMPComponent.fontSize);
        }

        private void OnEnable()
        {
            SwitchLanguage();
            LocalizationManager.SwitchLangEvent += SwitchLanguage;
        }

        private void OnDisable()
        {
            LocalizationManager.SwitchLangEvent -= SwitchLanguage;
        }

        private void SwitchLanguage()
        {
            if (!changeOnlyFont)
                AssignTranslate(value);
            ChangeFont();
            FontSizeCorrect();
        }

        private void AssignTranslate(string translation)
        {
            textMPComponent.text = translation;
        }

        private void ChangeFont()
        {
            if (uniqueFontTMP)
            {
                textMPComponent.font = uniqueFontTMP;
                return;
            }

            var currentFont = LangMethods.GetCurrentFont(LocalizationManager.LangName, fontNumber, settings);
            if (currentFont)
            {
                textMPComponent.font = currentFont;
            }
        }

        void FontSizeCorrect()
        {
            var correct = LangMethods.GetCurrentFontSizeCorrect(LocalizationManager.LangName, fontNumber, settings);
            textMPComponent.fontSize = baseFontSize + correct;
        }
    }
}