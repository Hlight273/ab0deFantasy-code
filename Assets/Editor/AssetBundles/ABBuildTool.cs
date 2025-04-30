using UnityEditor;
using UnityEngine;
using System.IO;
using HFantasy.Script.Configs;

namespace HFantasy.Editor.AssetBundles
{
    public class ABBuildTool
    {
        [MenuItem("资源工具/编辑器AB包生成（仅测试）")]
        public static void BuildAllABs()
        {
            string outPath = ABPathConfig.GetEditorABRootPath();
            string backupPath = outPath + "_backup";

            try
            {
                // 如果存在旧目录，备份后删除
                if (Directory.Exists(outPath))
                {
                    if (Directory.Exists(backupPath))
                        Directory.Delete(backupPath, true);
                    Directory.Move(outPath, backupPath);
                }

                Directory.CreateDirectory(outPath);

                // 构建 AB
                BuildPipeline.BuildAssetBundles(outPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
                Debug.Log("AB 包已构建 => " + outPath);

                // 成功后删除备份
                if (Directory.Exists(backupPath))
                    Directory.Delete(backupPath, true);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("AB 构建失败 => " + ex.Message);

                // 还原备份
                if (Directory.Exists(backupPath))
                {
                    if (Directory.Exists(outPath))
                        Directory.Delete(outPath, true);
                    Directory.Move(backupPath, outPath);
                    Debug.LogWarning("已还原旧 AB 包目录");
                }
            }
        }

        [MenuItem("资源工具/生成发行AB包并复制进StreamingAssets")]
        public static void BuildAndCopyToStreamingAssets()
        {
            string platformName = EditorUserBuildSettings.activeBuildTarget.ToString();
            string abTempOutput = Path.Combine(Application.dataPath, "../AssetBundleTemp", platformName);
            string streamingTargetPath = Path.Combine(Application.streamingAssetsPath, "AssetBundle", platformName);
            string backupPath = streamingTargetPath + "_backup";

            try
            {
                // 备份 StreamingAssets 目标目录
                if (Directory.Exists(streamingTargetPath))
                {
                    if (Directory.Exists(backupPath))
                        Directory.Delete(backupPath, true);
                    Directory.Move(streamingTargetPath, backupPath);
                }

                Directory.CreateDirectory(streamingTargetPath);

                // 构建临时 AB
                if (!Directory.Exists(abTempOutput))
                    Directory.CreateDirectory(abTempOutput);

                BuildPipeline.BuildAssetBundles(abTempOutput, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
                Debug.Log($"临时 AB 包生成成功 => {abTempOutput}");

                // 复制文件
                foreach (var file in Directory.GetFiles(abTempOutput))
                {
                    if ((Path.GetExtension(file) == ".manifest" &&
                         Path.GetFileNameWithoutExtension(file) != platformName) ||
                        Path.GetFileName(file).Contains(".meta"))
                        continue;

                    string fileName = Path.GetFileName(file);
                    string dest = Path.Combine(streamingTargetPath, fileName);
                    File.Copy(file, dest, true);
                }

                // 复制 Res 文件夹
                string resFolderPath = Path.Combine(abTempOutput, "Res");
                if (Directory.Exists(resFolderPath))
                {
                    string destResFolder = Path.Combine(streamingTargetPath, "Res");
                    Directory.CreateDirectory(destResFolder);

                    foreach (var file in Directory.GetFiles(resFolderPath, "*", SearchOption.AllDirectories))
                    {
                        if (Path.GetFileName(file).Contains(".meta"))
                            continue;

                        string relativePath = file.Substring(resFolderPath.Length + 1);
                        string destPath = Path.Combine(destResFolder, relativePath);

                        Directory.CreateDirectory(Path.GetDirectoryName(destPath));
                        File.Copy(file, destPath, true);
                    }
                }

                AssetDatabase.Refresh();
                Debug.Log($"AB 包已复制到 StreamingAssets => {streamingTargetPath}");

                // 删除备份（构建成功）
                if (Directory.Exists(backupPath))
                    Directory.Delete(backupPath, true);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("生成 AB 并复制失败 => " + ex.Message);

                // 还原备份
                if (Directory.Exists(backupPath))
                {
                    if (Directory.Exists(streamingTargetPath))
                        Directory.Delete(streamingTargetPath, true);
                    Directory.Move(backupPath, streamingTargetPath);
                    Debug.LogWarning("已还原 StreamingAssets 中的旧 AB 包");
                }
            }
        }


    }
}
