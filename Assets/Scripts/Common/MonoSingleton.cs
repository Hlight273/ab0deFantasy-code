using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HFantasy.Script.Common
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        // 静态实例
        private static T _instance;

        // 实例的访问属性
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    // 查找是否已有实例
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        // 如果没有，则创建一个新的GameObject并挂载该实例
                        GameObject singleton = new GameObject(typeof(T).Name);
                        _instance = singleton.AddComponent<T>();
                        DontDestroyOnLoad(singleton);  // 确保跨场景不销毁
                    }
                }
                return _instance;
            }
        }

        // 确保只有一个实例
        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);  // 如果已存在实例，则销毁当前实例
            }
            else
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);  // 防止在场景切换时销毁
            }
        }
    }
}
