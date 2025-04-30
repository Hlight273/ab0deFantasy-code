using System.Threading.Tasks;

namespace HFantasy.Script.Core.Resource {
    public static class ResourceLoader
    {
        private static IResourceLoader loader;

        static ResourceLoader()
        {
#if UNITY_EDITOR
            loader = EditorResourceLoader.Instance;
#else
        loader = new AssetBundleManager.Instance;
#endif
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