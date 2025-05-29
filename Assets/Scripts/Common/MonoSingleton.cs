using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HFantasy.Script.Common
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NonPersistentSingletonAttribute : Attribute { }

    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        //控制是否在场景切换时保持实例
        protected static bool isPersistent = true;
        static MonoSingleton()
        {
            //静态构造函数中检查特性，以后如果要非持久化单例，可以在类上加上 NonPersistentSingletonAttribute 特性。当然最好还是这些单例可以统一管理，不用都继承MonoSingleton，这样只有一个类需要维护
            isPersistent = !typeof(T).IsDefined(typeof(NonPersistentSingletonAttribute), false);
        }

        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject(typeof(T).Name);
                        _instance = singleton.AddComponent<T>();
                        if (isPersistent)
                        {
                            DontDestroyOnLoad(singleton);
                        }
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            Debug.Log($"[MonoSingleton] {typeof(T).Name} 单例已生成");
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                Debug.LogWarning($"[MonoSingleton] {typeof(T).Name} 实例已存在，销毁新的实例。");
            }
            else
            {
                _instance = this as T;
                if (isPersistent)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
        }

        //设置持久化的方法
        protected static void SetPersistent(bool persistent)
        {
            isPersistent = persistent;
        }
    }
}
