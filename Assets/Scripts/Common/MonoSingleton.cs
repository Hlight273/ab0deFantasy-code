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
        //�����Ƿ��ڳ����л�ʱ����ʵ��
        protected static bool isPersistent = true;
        static MonoSingleton()
        {
            //��̬���캯���м�����ԣ��Ժ����Ҫ�ǳ־û����������������ϼ��� NonPersistentSingletonAttribute ���ԡ���Ȼ��û�����Щ��������ͳһ�������ö��̳�MonoSingleton������ֻ��һ������Ҫά��
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
            Debug.Log($"[MonoSingleton] {typeof(T).Name} ����������");
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                Debug.LogWarning($"[MonoSingleton] {typeof(T).Name} ʵ���Ѵ��ڣ������µ�ʵ����");
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

        //���ó־û��ķ���
        protected static void SetPersistent(bool persistent)
        {
            isPersistent = persistent;
        }
    }
}
