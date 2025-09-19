using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonEventHandler : MonoBehaviour, IOnEventCallback
{
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        GameEvents gameEvent = (GameEvents)photonEvent.Code;

        switch (gameEvent)
        {
            case GameEvents.LoadGameScene:
                Scenes scene = (Scenes)photonEvent.CustomData;
                SceneLoader.Load(scene);
                break;
        }
    }
}
