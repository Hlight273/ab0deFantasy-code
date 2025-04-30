using UnityEditor;
using UnityEngine;

namespace HFantasy.Editor.Collider
{
    public class ColliderTool
    {
        [MenuItem("地形工具/所有子对象加MeshCollider")]
        static void AddColliders()
        {
            int cnt = 0;
            foreach (GameObject go in Selection.gameObjects)
            {
                var renderers = go.GetComponentsInChildren<MeshRenderer>();
                foreach (var renderer in renderers)
                {
                    var mf = renderer.GetComponent<MeshFilter>();
                    if (mf != null && renderer.GetComponent<MeshCollider>() == null)
                    {
                        Undo.AddComponent<MeshCollider>(renderer.gameObject);
                        cnt++;
                    }
                }
            }

            Debug.Log($"添加了 {cnt} 个 MeshCollider");
        }

        [MenuItem("地形工具/所有子对象移除MeshCollider")]
        static void RemoveColliders()
        {
            int cnt = 0;
            foreach (GameObject go in Selection.gameObjects)
            {
                var colliders = go.GetComponentsInChildren<MeshCollider>(true);
                foreach (var col in colliders)
                {
                    Undo.DestroyObjectImmediate(col);
                    cnt++;
                }
            }
            Debug.Log($"移除了 {cnt} 个 MeshCollider（支持撤销）");
        }

    }
}

