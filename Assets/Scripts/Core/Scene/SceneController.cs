using HFantasy.Script.Common;
using HFantasy.Script.Common.Constant;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoSingleton<SceneController>
{
    private const string LOADING_SCENE_NAME = SceneConstant.Loading;
    
    //�������ؽ����¼���float1Ϊ��Դ���ؽ��ȣ�float2Ϊ�������ؽ���
    public event Action<float, float> OnLoadingProgressChanged;
    
    public void SwitchScene(string targetSceneName)
    {
        StartCoroutine(SwitchSceneRoutine(targetSceneName));
    }

   private IEnumerator SwitchSceneRoutine(string targetSceneName)
    {
        Scene currentScene = SceneManager.GetActiveScene();

        //���ع��ɳ���
        var loadingOperation = SceneManager.LoadSceneAsync(LOADING_SCENE_NAME, LoadSceneMode.Additive);
        while (!loadingOperation.isDone)
        {
            yield return null;
        }

        //ж�ص�ǰ����
        var unloadOperation = SceneManager.UnloadSceneAsync(currentScene);
        while (!unloadOperation.isDone)
        {
            yield return null;
        }

        //Ԥ������Դ
        yield return StartCoroutine(LoadSceneResources(targetSceneName));

        //����Ŀ�곡��
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

        //�����³���Ϊ�������ж�ؼ��س���
        Scene newScene = SceneManager.GetSceneByName(targetSceneName);
        SceneManager.SetActiveScene(newScene);
        SceneManager.UnloadSceneAsync(LOADING_SCENE_NAME);

        OnLoadingProgressChanged?.Invoke(1f, 1f);
    }

    private IEnumerator LoadSceneResources(string sceneName)
    {
        //���ݲ�ͬ�������ز�ͬ��Դ �Ժ��ټ�
        //��ʹ��ģ����ؽ���
        float progress = 0f;
        while (progress < 1f)
        {
            progress += Time.deltaTime;
            OnLoadingProgressChanged?.Invoke(progress, 0f);
            yield return null;
        }
    }
}