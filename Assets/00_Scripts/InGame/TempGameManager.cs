using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempGameManager : MonoBehaviourPunCallbacks
{
    private static TempGameManager instance;
    public static TempGameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TempGameManager>();
            }
            return instance;
        }
    }
    [SerializeField] private GameObject prefab;
    [SerializeField] private List<Vector3> spawnTransform;

    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        var localPlayerIdx = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        var spawnPosition = spawnTransform[localPlayerIdx % spawnTransform.Count];

        PhotonNetwork.Instantiate(prefab.name, spawnPosition, Quaternion.identity);
    }

    public override void OnLeftRoom()
    {
        SceneLoader.Load(Scenes.MatchMakingScene);
    }
}
