using UnityEditor;
using UnityEngine;

namespace HFantasy.Editor.Collider
{
    public class ColliderTool
    {
        [MenuItem("���ι���/�����Ӷ����MeshCollider")]
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

            Debug.Log($"����� {cnt} �� MeshCollider");
        }

        [MenuItem("���ι���/�����Ӷ����Ƴ�MeshCollider")]
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
            Debug.Log($"�Ƴ��� {cnt} �� MeshCollider��֧�ֳ�����");
        }

    }
}

