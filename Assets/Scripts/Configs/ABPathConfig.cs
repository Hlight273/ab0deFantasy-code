using System.IO;
using UnityEngine;

namespace HFantasy.Script.Configs
{
    public static class ABPathConfig
    {
        public static string GetRuntimeABRootPath()
        {
#if UNITY_EDITOR
            return GetEditorABRootPath();  //编辑器下使用自定义目录
#elif UNITY_ANDROID
        return Application.persistentDataPath + "/AssetBundle"; //安卓下走可写目录
#elif UNITY_IOS
        return Application.persistentDataPath + "/AssetBundle"; //iOS
#else
        return Application.streamingAssetsPath; //其它平台默认走 StreamingAssets
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
            return Application.dataPath + "/../AssetBundle";  // 编辑器下使用自定义目录
        }

        public static string GetBundleFullPath(string bundleName)
        {
            return Path.Combine(GetRuntimeABRootPath(), bundleName);
        }
    }

}