
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Localization
{
    public static class Tools
    {
        [MenuItem("Tools/Localization/Initialize")]
        public static void InitializeLocalization()
        {
            CreateDirectoryIfNotExist();
            var managerPrefab = LoadLocalisationManagerPrefab();
            
            var manager = Object.FindAnyObjectByType<LocalizationManager>(FindObjectsInactive.Include);
            if (manager!= null)
            {
                if (manager.settings == null)
                {
                    manager.settings = LoadSettings();
                    EditorUtility.SetDirty(manager);
                }
                return;
            }
            var instantiate = PrefabUtility.InstantiatePrefab(managerPrefab);
            EditorUtility.SetDirty(instantiate);
            Undo.RegisterCreatedObjectUndo(instantiate, "Create Localization Manager");
        }

        private static void CreateDirectoryIfNotExist()
        {
            var path = $"{Application.dataPath}/{PathHelper.LocalizationRoot}";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private static LocalizationSettings LoadSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<LocalizationSettings>(PathHelper.LocalizationSettingsPath);
            if (settings != null)
                return settings;
            settings = ScriptableObject.CreateInstance<LocalizationSettings>();
            AssetDatabase.CreateAsset(settings, PathHelper.LocalizationSettingsPath);
            AssetDatabase.Refresh();
            return settings;
        }

        private static GameObject LoadLocalisationManagerPrefab()
        {
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(PathHelper.LocalizationManagerPath);
            if (go != null)
            {
                var manager = go.GetComponent<LocalizationManager>();
                if (manager == null)
                {
                    manager = go.AddComponent<LocalizationManager>();
                    EditorUtility.SetDirty(go);
                }
                var settings = manager.settings;
                if (settings == null)
                {
                    settings = LoadSettings();
                    manager.settings = settings;
                    EditorUtility.SetDirty(manager);
                }

                return go;
            }
            else
            { 
                go = new GameObject("LocalizationManager");
                var manager = go.AddComponent<LocalizationManager>();
                var settings = LoadSettings();
                manager.settings = settings;
                PrefabUtility.SaveAsPrefabAsset(go, PathHelper.LocalizationManagerPath);
                Object.DestroyImmediate(go);
                return AssetDatabase.LoadAssetAtPath<GameObject>(PathHelper.LocalizationManagerPath);
            }
        }
    }
}
