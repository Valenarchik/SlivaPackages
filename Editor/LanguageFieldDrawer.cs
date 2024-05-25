using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Localization
{
    [CustomPropertyDrawer(typeof(LanguageField<>), true)]
    [CustomPropertyDrawer(typeof(LanguageTextAreaAttribute), true)]
    [CustomPropertyDrawer(typeof(LanguageFieldAttribute), true)]
    public class LanguageFieldDrawer : PropertyDrawer
    {
        private LocalizationSettings settings;
        private GUIStyle red;
        private GUIStyle green;
        private GUIStyle button;

        private bool onFoldout;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Generic)
            {
                return base.GetPropertyHeight(property, label);
            }

            LoadSettings();
            if (settings == null)
            {
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            
            ProcessLabel(label);
            
            var height = 0f;
            
            if (onFoldout)
            {
                height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                if (!property.isExpanded)
                    return height;
            }
            
            height += GetHeaderHeight(property);
            height += GetFieldsHeight(property);
            return height + 5;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Generic)
            {
                EditorGUI.PropertyField(position, property);
                return;
            }

            LoadSettings();
            if (settings == null)
            {
                if (GUI.Button(position.SetHeight(EditorGUIUtility.singleLineHeight), "Initialize Localization"))
                {
                    Tools.InitializeLocalization();
                }
                return;
            }
            
            InitializeStyles();
            ProcessLabel(label);

            EditorGUI.BeginProperty(position, label, property);

            if (onFoldout)
            {
                property.isExpanded = EditorGUI.Foldout(position.SetHeight(EditorGUIUtility.singleLineHeight),
                    property.isExpanded,
                    property.displayName);
                
                if (!property.isExpanded)
                {
                    EditorGUI.EndProperty();
                    return;
                }
                
                EditorGUI.indentLevel += 1;
                position = position.AddY(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            }

            position = DrawHeader(position, property, label);
            DrawFields(position, property, label);
            if (onFoldout)
            {
                EditorGUI.indentLevel -= 1;
            }

            EditorGUI.EndProperty();
        }

        private void DrawFields(Rect position, SerializedProperty property, GUIContent label)
        {
            var textHeight = property.FindPropertyRelative("textHeight");
            var isString = IsString(property);

            if (!IsArray(property))
            {
                EditorGUI.DrawRect(position
                        .AddX(EditorGUI.indentLevel * 15)
                        .SubtractWidth(EditorGUI.indentLevel * 15)
                        .SetHeight(GetFieldsHeight(property)),
                    new Color(0.2f, 0.2f, 0.2f));
            }

            var activeLanguages = LangMethods.GetActiveLanguages(settings);
            EditorGUI.LabelField(position
                    .SetHeight(EditorGUIUtility.singleLineHeight),
                $"{activeLanguages.Length} Languages");
            position = position.AddY(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

            if (isString)
            {
                var leftValue = 20f;
                var rightValue = 500f;
                if (attribute is LanguageTextAreaAttribute textAreaAttribute)
                {
                    leftValue = textAreaAttribute.MinHeight;
                    rightValue = textAreaAttribute.MaxHeight;
                }

                textHeight.floatValue = EditorGUI.Slider(position.SetHeight(EditorGUIUtility.singleLineHeight),
                    "Text height", textHeight.floatValue, leftValue, rightValue);
                position = position.AddY(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            }

            var isFirst = true;
            foreach (var lang in activeLanguages)
            {
                var langString = LangMethods.GetLangName(lang);
                var relativeProperty = property.FindPropertyRelative(langString);

                if (isString)
                {
                    EditorGUI.LabelField(
                        position.AddX(EditorGUI.indentLevel * 15)
                            .SetWidth(40)
                            .SetHeight(textHeight.floatValue),
                        isFirst ? langString + "*" : langString);
                    relativeProperty.stringValue = EditorGUI.TextArea(
                        position.SetHeight(textHeight.floatValue)
                            .AddX(40 + EditorGUI.indentLevel * 15)
                            .SubtractWidth(40 + EditorGUIUtility.standardVerticalSpacing + EditorGUI.indentLevel * 15),
                        relativeProperty.stringValue,
                        EditorStyles.textArea);
                    position = position.AddY(textHeight.floatValue + EditorGUIUtility.standardVerticalSpacing);
                }
                else
                {
                    var relativePropertyHeight = EditorGUI.GetPropertyHeight(relativeProperty);
                    EditorGUI.PropertyField(
                        position.SetHeight(relativePropertyHeight),
                        relativeProperty,
                        new GUIContent(langString)
                    );
                    position = position.AddY(relativePropertyHeight + EditorGUIUtility.standardVerticalSpacing);
                }

                isFirst = false;
            }
        }

        private float GetFieldsHeight(SerializedProperty property)
        {
            var textHeight = property.FindPropertyRelative("textHeight");
            var isString = IsString(property);
            var height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (isString)
            {
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            foreach (var lang in LangMethods.GetActiveLanguages(settings))
            {
                var langString = LangMethods.GetLangName(lang);
                var relativeProperty = property.FindPropertyRelative(langString);

                if (isString)
                {
                    height += textHeight.floatValue + EditorGUIUtility.standardVerticalSpacing;
                }
                else
                {
                    height += EditorGUI.GetPropertyHeight(relativeProperty) + EditorGUIUtility.standardVerticalSpacing;
                }
            }

            return height;
        }

        private float GetHeaderHeight(SerializedProperty property)
        {
            var height = 0f;
            if (!IsString(property))
                return height;
            height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 3;
            height += EditorGUIUtility.standardVerticalSpacing * 2;
            height += 5;
            return height;
        }

        private Rect DrawHeader(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!IsString(property))
                return position;

            GUI.Box(position
                    .SetHeight(GetHeaderHeight(property) - 5)
                    .AddX(EditorGUI.indentLevel * 15)
                    .SubtractWidth(EditorGUI.indentLevel * 15),
                GUIContent.none,
                "HelpBox");

            position = position
                .AddY(EditorGUIUtility.standardVerticalSpacing)
                .AddX(4)
                .AddWidth(-8);

            var identity = property.FindPropertyRelative("identity");
            identity.stringValue = EditorGUI.TextField(
                position
                    .WidthPercent(0.65f)
                    .SetHeight(EditorGUIUtility.singleLineHeight),
                identity.stringValue);

            var helperPosition = position
                .RelativeX(0.65f)
                .AddX(EditorGUIUtility.standardVerticalSpacing)
                .WidthPercent(0.4f)
                .SetHeight(EditorGUIUtility.singleLineHeight);

            if (!string.IsNullOrEmpty(identity.stringValue))
                EditorGUI.LabelField(helperPosition, "ID Translate");
            else
                EditorGUI.LabelField(helperPosition, "ID Translate (necessarily)", red);

            position = position.AddY(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

            if (GUI.Button(position
                    .AddX(EditorGUI.indentLevel * 15)
                    .SubtractWidth(EditorGUI.indentLevel * 15)
                    .WidthPercent(0.5f)
                    .SetHeight(EditorGUIUtility.singleLineHeight), "Import CSV", button))
            {
                var transfers = CSVManager.ImportTransfersByKey(settings, identity.stringValue);
                if (transfers != null)
                    SetLanguages(property, transfers);
            }

            if (GUI.Button(position
                    .AddX(EditorGUI.indentLevel * 15)
                    .SubtractWidth(EditorGUI.indentLevel * 15)
                    .RelativeX(0.5f)
                    .WidthPercent(0.5f)
                    .SetHeight(EditorGUIUtility.singleLineHeight), "Export CSV", button))
            {
                CSVManager.SetIDLineFile(settings, identity.stringValue, GetLanguages(property));
            }

            position = position.AddY(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2);

            if (GUI.Button(position
                        .AddX(EditorGUI.indentLevel * 15)
                        .SubtractWidth(EditorGUI.indentLevel * 15)
                        .SetHeight(EditorGUIUtility.singleLineHeight),
                    "Translate empty fields", button))
            {
                TranslateFields(property);
            }

            position = position.AddY(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

            return position
                .AddX(-4)
                .AddWidth(8)
                .AddY(5);
        }

        private static bool IsString(SerializedProperty property)
        {
            return property.FindPropertyRelative("ru").propertyType == SerializedPropertyType.String;
        }

        private static bool IsArray(SerializedProperty property)
        {
            return property.FindPropertyRelative("ru").propertyType == SerializedPropertyType.Generic;
        }

        private void SetLanguages(SerializedProperty property, string[] values)
        {
            for (var i = 0; i < settings.languages.Count; i++)
            {
                var langProperty = property.FindPropertyRelative(LangMethods.GetLangName(i));
                langProperty.stringValue = values[i];
            }
        }

        private string[] GetLanguages(SerializedProperty property)
        {
            var result = new string[settings.languages.Count];
            for (var i = 0; i < settings.languages.Count; i++)
            {
                var langProperty = property.FindPropertyRelative(LangMethods.GetLangName(i));
                result[i] = langProperty.stringValue;
            }

            return result;
        }

        private void ProcessLabel(GUIContent label)
        {
            onFoldout = label.text != "OFF FOLDOUT";
        }

        private void TranslateFields(SerializedProperty property)
        {
            var languages = GetLanguages(property);
            var activeLanguages = LangMethods.GetLangArr(settings);
            var activeLanguagesIndexes = activeLanguages
                .Select((isActive, i) => (isActive, i))
                .Where(tuple => tuple.isActive)
                .Select(tuple => tuple.i)
                .ToList();
            if (activeLanguagesIndexes.Count == 0)
            {
                return;
            }

            var valueToTranslate = languages[activeLanguagesIndexes[0]];
            if (string.IsNullOrEmpty(valueToTranslate))
            {
                Debug.LogError("Cant translate because main lang is empty");
                return;
            }

            for (var i = 1; i < activeLanguagesIndexes.Count; i++)
            {
                var index = activeLanguagesIndexes[i];
                var lang = LangMethods.GetLangName(index);
                var value = languages[index];
                if (!string.IsNullOrEmpty(value))
                    continue;
                languages[i] = GoogleTranslateUtility.Translate(settings, valueToTranslate, lang);
            }

            SetLanguages(property, languages);
        }

        private LocalizationSettings LoadSettings()
        {
            if (settings == null)
                settings = SettingsLoader.LoadSettings();
            return settings;
        }

        private void InitializeStyles()
        {
            red = new GUIStyle(EditorStyles.label)
            {
                normal =
                {
                    textColor = Color.red
                }
            };

            green = new GUIStyle(EditorStyles.label)
            {
                normal =
                {
                    textColor = Color.green
                }
            };

            button = new GUIStyle(EditorStyles.miniButton);
        }
    }
}