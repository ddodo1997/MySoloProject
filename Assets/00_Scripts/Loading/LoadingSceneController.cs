using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    [SerializeField] private Slider progressBar;

    private async void Start()
    {
        var progress = new Progress<float>(p => progressBar.value = p);
        await SceneLoader.LoadNextSceneAsync(progress);
    }
}
