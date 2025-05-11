using UnityEngine;
using System.IO;
using System;
using HFantasy.Script.Core.Resource;

namespace HFantasy.Script.Core.HDebug
{
    public class RuntimeDebugger : MonoBehaviour
    {
        private string logPath;
        private GUIStyle guiStyle;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            logPath = Path.Combine(Application.dataPath, "../debug.log");
            Application.logMessageReceived += LogCallback;

            guiStyle = new GUIStyle();
            guiStyle.fontSize = 20;
            guiStyle.normal.textColor = Color.red;
        }

        void Start()
        {
            var materials = Resources.FindObjectsOfTypeAll<Material>();
            foreach (var mat in materials)
            {
                if (mat.shader == null)
                    Debug.LogError($"Material '{mat.name}' has null shader!");
                else
                    Debug.Log($"Material '{mat.name}' uses shader '{mat.shader.name}'");
            }
        }

        void OnGUI()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.D))
            {
                GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));
                GUILayout.Label($"��Դ·��: {Application.streamingAssetsPath}", guiStyle);
                GUILayout.Label($"AB���Ƿ����: {Directory.Exists(Path.Combine(Application.streamingAssetsPath, "AssetBundles"))}", guiStyle);
               // GUILayout.Label($"�Ѽ��ص�AB������: {AssetBundleResourceLoader.Instance.LoadedBundleCount}", guiStyle);
                GUILayout.Label($"������������: {FindObjectsOfType<Transform>().Length}", guiStyle);
                GUILayout.EndArea();
            }
        }

        private void LogCallback(string condition, string stackTrace, LogType type)
        {
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string logMessage = $"[{timeStamp}] {type}: {condition}\n{stackTrace}\n\n";
            File.AppendAllText(logPath, logMessage);
        }

        void OnDestroy()
        {
            Application.logMessageReceived -= LogCallback;
        }
    }
}