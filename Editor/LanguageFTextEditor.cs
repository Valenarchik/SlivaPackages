using TMPro;
using UnityEditor;
using UnityEngine;

namespace Localization
{
    [CustomEditor(typeof(LanguageFText))]
    public class LanguageFTextEditor: Editor
    {
        private LanguageFText scr => (LanguageFText) target;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            if (scr.GetComponent<TextMeshProUGUI>() == null)
            {
                if (GUILayout.Button("Add component - Text Mesh Pro UGUI", GUILayout.Height(23)))
                {
                    Undo.AddComponent<TextMeshProUGUI>(scr.gameObject);
                    EditorUtility.SetDirty(scr.gameObject);
                }
                return;
            }
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("value"), new GUIContent("OFF FOLDOUT"));

            var rect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f));
            
            EditorGUILayout.LabelField("Settings");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("uniqueFontTMP"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("changeOnlyFont"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fontNumber"));
            
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
    }
}