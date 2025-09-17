using Cysharp.Threading.Tasks;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader
{
    private static string nextScene;
    public static void Load(Scenes sceneName)
    {
        nextScene = sceneName.ToString();
        SceneManager.LoadScene(Scenes.LoadingScene.ToString());
    }

    public static async UniTask LoadNextSceneAsync(System.IProgress<float> progress = null)
    { 
        var op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            progress?.Report(op.progress);
            await UniTask.Yield();
        }

        progress?.Report(1f);
        op.allowSceneActivation = true;
    }
}
