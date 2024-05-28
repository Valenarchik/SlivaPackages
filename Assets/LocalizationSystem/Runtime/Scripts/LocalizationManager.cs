using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Localization
{
    [DefaultExecutionOrder(-50)]
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }

        [SerializeField] private LangName lang = LangName.ru;
        [SerializeField, FormerlySerializedAs("settings")] private LocalizationSettings m_settings;
        public static string Lang
        {
            get => Enum.GetName(typeof(LangName), Instance.lang);
            set => Instance.lang = Enum.Parse<LangName>(value);
        }

        public static LangName LangName
        {
            get => Instance.lang;
            set
            {
                Instance.lang = value;
                SwitchLangEvent?.Invoke();
            }
        }

        public LocalizationSettings settings
        {
            get => m_settings;
            set => m_settings = value;
        }

        public static LocalizationSettings Settings => Instance ? Instance.m_settings : null;
        public static event Action SwitchLangEvent;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnValidate()
        {
            Instance = this;
        }
    }
}
