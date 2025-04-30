using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HFantasy.Script.Common
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        // ��̬ʵ��
        private static T _instance;

        // ʵ���ķ�������
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    // �����Ƿ�����ʵ��
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        // ���û�У��򴴽�һ���µ�GameObject�����ظ�ʵ��
                        GameObject singleton = new GameObject(typeof(T).Name);
                        _instance = singleton.AddComponent<T>();
                        DontDestroyOnLoad(singleton);  // ȷ���糡��������
                    }
                }
                return _instance;
            }
        }

        // ȷ��ֻ��һ��ʵ��
        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);  // ����Ѵ���ʵ���������ٵ�ǰʵ��
            }
            else
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);  // ��ֹ�ڳ����л�ʱ����
            }
        }
    }
}
