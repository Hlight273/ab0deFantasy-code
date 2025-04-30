using UnityEditor;
using UnityEngine;
using System.IO;
using HFantasy.Script.Configs;

namespace HFantasy.Editor.AssetBundles
{
    public class ABBuildTool
    {
        [MenuItem("��Դ����/�༭��AB�����ɣ������ԣ�")]
        public static void BuildAllABs()
        {
            string outPath = ABPathConfig.GetEditorABRootPath();
            string backupPath = outPath + "_backup";

            try
            {
                // ������ھ�Ŀ¼�����ݺ�ɾ��
                if (Directory.Exists(outPath))
                {
                    if (Directory.Exists(backupPath))
                        Directory.Delete(backupPath, true);
                    Directory.Move(outPath, backupPath);
                }

                Directory.CreateDirectory(outPath);

                // ���� AB
                BuildPipeline.BuildAssetBundles(outPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
                Debug.Log("AB ���ѹ��� => " + outPath);

                // �ɹ���ɾ������
                if (Directory.Exists(backupPath))
                    Directory.Delete(backupPath, true);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("AB ����ʧ�� => " + ex.Message);

                // ��ԭ����
                if (Directory.Exists(backupPath))
                {
                    if (Directory.Exists(outPath))
                        Directory.Delete(outPath, true);
                    Directory.Move(backupPath, outPath);
                    Debug.LogWarning("�ѻ�ԭ�� AB ��Ŀ¼");
                }
            }
        }

        [MenuItem("��Դ����/���ɷ���AB�������ƽ�StreamingAssets")]
        public static void BuildAndCopyToStreamingAssets()
        {
            string platformName = EditorUserBuildSettings.activeBuildTarget.ToString();
            string abTempOutput = Path.Combine(Application.dataPath, "../AssetBundleTemp", platformName);
            string streamingTargetPath = Path.Combine(Application.streamingAssetsPath, "AssetBundle", platformName);
            string backupPath = streamingTargetPath + "_backup";

            try
            {
                // ���� StreamingAssets Ŀ��Ŀ¼
                if (Directory.Exists(streamingTargetPath))
                {
                    if (Directory.Exists(backupPath))
                        Directory.Delete(backupPath, true);
                    Directory.Move(streamingTargetPath, backupPath);
                }

                Directory.CreateDirectory(streamingTargetPath);

                // ������ʱ AB
                if (!Directory.Exists(abTempOutput))
                    Directory.CreateDirectory(abTempOutput);

                BuildPipeline.BuildAssetBundles(abTempOutput, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
                Debug.Log($"��ʱ AB �����ɳɹ� => {abTempOutput}");

                // �����ļ�
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

                // ���� Res �ļ���
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
                Debug.Log($"AB ���Ѹ��Ƶ� StreamingAssets => {streamingTargetPath}");

                // ɾ�����ݣ������ɹ���
                if (Directory.Exists(backupPath))
                    Directory.Delete(backupPath, true);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("���� AB ������ʧ�� => " + ex.Message);

                // ��ԭ����
                if (Directory.Exists(backupPath))
                {
                    if (Directory.Exists(streamingTargetPath))
                        Directory.Delete(streamingTargetPath, true);
                    Directory.Move(backupPath, streamingTargetPath);
                    Debug.LogWarning("�ѻ�ԭ StreamingAssets �еľ� AB ��");
                }
            }
        }


    }
}
