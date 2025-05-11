using HFantasy.Script.Configs;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace HFantasy.Editor.ShaderManagement
{
    public class ShaderIncludeProcessor : EditorWindow
    {
        [MenuItem("发行/Shader工具/添加必需Shader")]
        public static void AddRequiredShaders()
        {
            var graphicsSettings = GraphicsSettings.GetGraphicsSettings();
            var serializedObject = new SerializedObject(graphicsSettings);
            var arrayProp = serializedObject.FindProperty("m_AlwaysIncludedShaders");

            //当前已包含的shader列表
            HashSet<Shader> existingShaders = new HashSet<Shader>();
            for (int i = 0; i < arrayProp.arraySize; i++)
            {
                var shader = arrayProp.GetArrayElementAtIndex(i).objectReferenceValue as Shader;
                if (shader != null)
                    existingShaders.Add(shader);
            }

            //自动获取指定目录下所有shader
            var shaderGuids = AssetDatabase.FindAssets("t:Shader", new[] { PathConfig.SHADER_PATH });
            bool modified = false;

            foreach (var guid in shaderGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var shader = AssetDatabase.LoadAssetAtPath<Shader>(assetPath);

                if (shader != null && !existingShaders.Contains(shader))
                {
                    arrayProp.arraySize++;
                    var element = arrayProp.GetArrayElementAtIndex(arrayProp.arraySize - 1);
                    element.objectReferenceValue = shader;
                    modified = true;
                    Debug.Log($"添加Shader: {shader.name}");
                }
            }

            if (modified)
            {
                serializedObject.ApplyModifiedProperties();
                Debug.Log("Shader列表更新完成");
            }
            else
            {
                Debug.Log("所有必需Shader已经包含，无需更新");
            }
        }

        [MenuItem("发行/Shader工具/检查必需Shader")]
        static void CheckRequiredShaders()
        {
            var shaderGuids = AssetDatabase.FindAssets("t:Shader", new[] { PathConfig.SHADER_PATH });
            foreach (var guid in shaderGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var shader = AssetDatabase.LoadAssetAtPath<Shader>(assetPath);

                if (shader == null)
                    Debug.LogError($"找不到Shader: {assetPath}");
                else
                    Debug.Log($"Shader已存在: {shader.name}");
            }
        }
    }
}
