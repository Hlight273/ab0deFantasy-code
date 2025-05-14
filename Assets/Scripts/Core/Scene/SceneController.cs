using HFantasy.Script.Common;
using HFantasy.Script.Common.Constant;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoSingleton<SceneController>
{
    private const string LOADING_SCENE_NAME = SceneConstant.Loading;
    
    //场景加载进度事件，float1为资源加载进度，float2为场景加载进度
    public event Action<float, float> OnLoadingProgressChanged;
    
    public void SwitchScene(string targetSceneName)
    {
        StartCoroutine(SwitchSceneRoutine(targetSceneName));
    }

   private IEnumerator SwitchSceneRoutine(string targetSceneName)
    {
        Scene currentScene = SceneManager.GetActiveScene();

        //加载过渡场景
        var loadingOperation = SceneManager.LoadSceneAsync(LOADING_SCENE_NAME, LoadSceneMode.Additive);
        while (!loadingOperation.isDone)
        {
            yield return null;
        }

        //卸载当前场景
        var unloadOperation = SceneManager.UnloadSceneAsync(currentScene);
        while (!unloadOperation.isDone)
        {
            yield return null;
        }

        //预加载资源
        yield return StartCoroutine(LoadSceneResources(targetSceneName));

        //加载目标场景
        var targetOperation = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
        targetOperation.allowSceneActivation = false;

        while (targetOperation.progress < 0.9f)
        {
            OnLoadingProgressChanged?.Invoke(1f, targetOperation.progress);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        targetOperation.allowSceneActivation = true;

        while (!targetOperation.isDone)
        {
            yield return null;
        }

        //设置新场景为活动场景并卸载加载场景
        Scene newScene = SceneManager.GetSceneByName(targetSceneName);
        SceneManager.SetActiveScene(newScene);
        SceneManager.UnloadSceneAsync(LOADING_SCENE_NAME);

        OnLoadingProgressChanged?.Invoke(1f, 1f);
    }

    private IEnumerator LoadSceneResources(string sceneName)
    {
        //根据不同场景加载不同资源 以后再加
        //先使用模拟加载进度
        float progress = 0f;
        while (progress < 1f)
        {
            progress += Time.deltaTime;
            OnLoadingProgressChanged?.Invoke(progress, 0f);
            yield return null;
        }
    }
}