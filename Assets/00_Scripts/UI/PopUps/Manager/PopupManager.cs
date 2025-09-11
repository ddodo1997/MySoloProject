using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }
    public GameObject prefab;
    public Canvas canvas;
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += FindCanvas;
    }

    public void FindCanvas(Scene scene, LoadSceneMode mode)
    {
        canvas = FindAnyObjectByType<Canvas>();
    }

    public void ShowPopup(string message)
    {
        var popup = Instantiate(prefab, canvas.transform).GetComponent<Popup>();
        popup.ChangeMassage(message);
    }
}
