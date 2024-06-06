using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LogSystem
{
    public class LogSettingsEditor : SettingsProvider
    {
        private const string SettingsPath = "Project/Log Settings";
        private SerializedObject _settingsSerializedObject;

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new LogSettingsEditor();
            return provider;
        }

        private LogSettingsEditor() : base(SettingsPath, SettingsScope.Project)
        {
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            var settings = GetOrCreateSettings();
            _settingsSerializedObject = new SerializedObject(settings);
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.PropertyField(_settingsSerializedObject.FindProperty("logOutputLevel"));
            EditorGUILayout.PropertyField(_settingsSerializedObject.FindProperty("openShippingLog"));
            _settingsSerializedObject.ApplyModifiedProperties();
        }

        private LogSettings GetOrCreateSettings()
        {
            var logSettings = ScriptableObject.CreateInstance(typeof(LogSettings)) as LogSettings;
            var monoScript = MonoScript.FromScriptableObject(logSettings);
            string path = AssetDatabase.GetAssetPath(monoScript);
            string directoryName = Path.GetDirectoryName(path);
            string assetPath = Path.Combine(directoryName!, "LogSettings.asset");
            var settings = AssetDatabase.LoadAssetAtPath<LogSettings>(assetPath);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance(typeof(LogSettings)) as LogSettings;
                AssetDatabase.CreateAsset(settings, assetPath);
                AssetDatabase.SaveAssets();
            }

            return settings;
        }
    }
}