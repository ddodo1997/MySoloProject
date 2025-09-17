using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPCRelay : MonoBehaviourPun
{
    public static RPCRelay Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        gameObject.AddComponent<PhotonView>();
    }

    [PunRPC]
    public void RPC_LoadGameScene(Scenes scene)
    {
        SceneLoader.Load(scene);
    }
}
