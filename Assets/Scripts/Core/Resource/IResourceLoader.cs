using System.Threading.Tasks;

namespace HFantasy.Script.Core.Resource
{
    public interface IResourceLoader
    {
        T LoadAsset<T>(string pathOrBundle, string assetName) where T : UnityEngine.Object;
        Task<T> LoadAssetAsync<T>(string pathOrBundle, string assetName) where T : UnityEngine.Object;
    }
}