using HFantasy.Script.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HFantasy.Script.Core.Scene
{
    public class SceneManager : MonoSingleton<SceneManager>
    {
        // �첽���س���
        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        // ж�س���
        public void UnloadScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
        }
    }
}
