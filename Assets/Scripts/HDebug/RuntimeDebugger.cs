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
                GUILayout.Label($"资源路径: {Application.streamingAssetsPath}", guiStyle);
                GUILayout.Label($"AB包是否存在: {Directory.Exists(Path.Combine(Application.streamingAssetsPath, "AssetBundles"))}", guiStyle);
               // GUILayout.Label($"已加载的AB包数量: {AssetBundleResourceLoader.Instance.LoadedBundleCount}", guiStyle);
                GUILayout.Label($"场景物体数量: {FindObjectsOfType<Transform>().Length}", guiStyle);
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