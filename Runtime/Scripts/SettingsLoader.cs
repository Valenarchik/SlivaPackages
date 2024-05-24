using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Localization
{
    public static class SettingsLoader
    {
        public const string LocalizationManagerPrefab = "Packages/com.slivagroup.localization/Runtime/WorkingData/LocalizationManager.prefab";

        public static LocalizationSettings LoadSettings() 
        {
            var manager = Object.FindAnyObjectByType<LocalizationManager>();

            if (manager)
            {
                return manager.settings;
            }
            else
            {
#if UNITY_EDITOR
                var infoYGFromConfig = SettingsLoader.LoadFromAssets();
                return infoYGFromConfig;
#else
                return null;
#endif
            }
        }
        
#if UNITY_EDITOR
        private static LocalizationSettings LoadFromAssets()
        {
            var prefab = (GameObject)AssetDatabase.LoadAssetAtPath(LocalizationManagerPrefab, typeof(GameObject));
            if (prefab == null)
            {
                Debug.LogError($"Префаб LocalizationManager не был найден по пути: {LocalizationManagerPrefab}");
                return null;
            }

            var manager = prefab.GetComponent<LocalizationManager>();
            if (manager == null)
            {
                Debug.LogError($"На объекте LocalizationManager не был найден компонент LocalizationManager! Префаб объекта расположен по пути: {LocalizationManagerPrefab}");
                return null;
            }

            var settings = manager.settings;
            if (manager == null)
            {
                Debug.LogError($"На компоненте LocalizationManager не определено поле Settings! Префаб LocalizationManager расположен по пути: {LocalizationManagerPrefab}");
                return null;
            }

            return settings;
        }
#endif
    }
}