using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomScrollviewManager : MonoBehaviour
{
    [SerializeField] private ScrollRect roomScrollView;
    [SerializeField] private GameObject roomInfoObj;
    [SerializeField] private List<GameObject> currentRoomList;
    private void Start()
    {
        MatchMakingManager.Instance.RefreshAction += Refresh;
    }

    public void Refresh(List<RoomInfo> rooms)
    {
        foreach (var roomGO in currentRoomList)
            Destroy(roomGO);
        currentRoomList.Clear();

        foreach (var room in rooms)
        {
            var newRoom = Instantiate(roomInfoObj, roomScrollView.content);
            currentRoomList.Add(newRoom);
            newRoom.GetComponent<RoomInfoUI>().Refresh(room.Name, room.PlayerCount);
        }
    }

    private void OnDestroy()
    {
        MatchMakingManager.Instance.RefreshAction -= Refresh;
    }
}
