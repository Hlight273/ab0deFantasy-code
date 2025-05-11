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
        [MenuItem("����/Shader����/��ӱ���Shader")]
        public static void AddRequiredShaders()
        {
            var graphicsSettings = GraphicsSettings.GetGraphicsSettings();
            var serializedObject = new SerializedObject(graphicsSettings);
            var arrayProp = serializedObject.FindProperty("m_AlwaysIncludedShaders");

            //��ǰ�Ѱ�����shader�б�
            HashSet<Shader> existingShaders = new HashSet<Shader>();
            for (int i = 0; i < arrayProp.arraySize; i++)
            {
                var shader = arrayProp.GetArrayElementAtIndex(i).objectReferenceValue as Shader;
                if (shader != null)
                    existingShaders.Add(shader);
            }

            //�Զ���ȡָ��Ŀ¼������shader
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
                    Debug.Log($"���Shader: {shader.name}");
                }
            }

            if (modified)
            {
                serializedObject.ApplyModifiedProperties();
                Debug.Log("Shader�б�������");
            }
            else
            {
                Debug.Log("���б���Shader�Ѿ��������������");
            }
        }

        [MenuItem("����/Shader����/������Shader")]
        static void CheckRequiredShaders()
        {
            var shaderGuids = AssetDatabase.FindAssets("t:Shader", new[] { PathConfig.SHADER_PATH });
            foreach (var guid in shaderGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var shader = AssetDatabase.LoadAssetAtPath<Shader>(assetPath);

                if (shader == null)
                    Debug.LogError($"�Ҳ���Shader: {assetPath}");
                else
                    Debug.Log($"Shader�Ѵ���: {shader.name}");
            }
        }
    }
}
