using System.IO;
using UnityEngine;

namespace HFantasy.Script.Configs
{
    public static class ABPathConfig
    {
        public static string GetRuntimeABRootPath()
        {
#if UNITY_EDITOR
            return GetEditorABRootPath();  //�༭����ʹ���Զ���Ŀ¼
#elif UNITY_ANDROID
        return Application.persistentDataPath + "/AssetBundle"; //��׿���߿�дĿ¼
#elif UNITY_IOS
        return Application.persistentDataPath + "/AssetBundle"; //iOS
#else
        return Application.streamingAssetsPath; //����ƽ̨Ĭ���� StreamingAssets
#endif
        }

        public static string GetPlatformFolderName()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            return "StandaloneWindows64";
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        return "StandaloneOSX";
#elif UNITY_ANDROID
        return "Android";
#elif UNITY_IOS
        return "iOS";
#else
        return "UnknownPlatform";
#endif
        }

        public static string GetEditorABRootPath()
        {
            return Application.dataPath + "/../AssetBundle";  // �༭����ʹ���Զ���Ŀ¼
        }

        public static string GetBundleFullPath(string bundleName)
        {
            return Path.Combine(GetRuntimeABRootPath(), bundleName);
        }
    }

}