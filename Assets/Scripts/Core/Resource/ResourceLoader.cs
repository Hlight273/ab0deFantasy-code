using System.Threading.Tasks;

namespace HFantasy.Script.Core.Resource {
    public static class ResourceLoader
    {
        private static IResourceLoader loader;

        private static bool isEditorMode = false;

        static ResourceLoader()
        {
#if UNITY_EDITOR
            loader = EditorResourceLoader.Instance;
#else
            loader = AssetBundleResourceLoader.Instance;
#endif
            if(!isEditorMode)
                loader = AssetBundleResourceLoader.Instance;
        }

        public static T LoadAsset<T>(string pathOrBundle, string assetName) where T : UnityEngine.Object
        {
            return loader.LoadAsset<T>(pathOrBundle, assetName);
        }

        public static Task<T> LoadAssetAsync<T>(string pathOrBundle, string assetName) where T : UnityEngine.Object
        {
            return loader.LoadAssetAsync<T>(pathOrBundle, assetName);
        }
    }
}