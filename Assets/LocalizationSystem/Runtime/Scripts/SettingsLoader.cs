using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Localization
{
    public static class SettingsLoader
    {
        public static LocalizationSettings LoadSettings()
        {
            var manager = LocalizationManager.Instance == null
                ? Object.FindAnyObjectByType<LocalizationManager>()
                : LocalizationManager.Instance;

            if (manager)
            {
                return manager.settings;
            }
            else
            {
#if UNITY_EDITOR
                return LoadFromAssets();
#else
                return null;
#endif
            }
        }

#if UNITY_EDITOR
        private static LocalizationSettings LoadFromAssets()
        {
            var prefab = (GameObject) AssetDatabase.LoadAssetAtPath(PathHelper.LocalizationManagerPath, typeof(GameObject));
            if (prefab == null)
            {
                return null;
            }

            var manager = prefab.GetComponent<LocalizationManager>();
            if (manager == null)
            {
                return null;
            }

            var settings = manager.settings;
            if (manager == null)
            {
                return null;
            }

            return settings;
        }
#endif
    }
}