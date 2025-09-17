using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomScrollviewManager : MonoBehaviour
{
    [SerializeField] private ScrollRect roomScrollView;
    [SerializeField] private GameObject roomInfoObj;
    [SerializeField] private Dictionary<string, GameObject> currentRoomUIDict = new();
    private void Start()
    {
        PhotonManager.Instance.RefreshAction += Refresh;
    }

    public void Refresh(List<RoomInfo> removedRooms, List<RoomInfo> addedRooms)
    {
        foreach (var room in removedRooms)
        {
            if (currentRoomUIDict.TryGetValue(room.Name, out var roomGO))
            { 
                Destroy(roomGO); 
                currentRoomUIDict.Remove(room.Name);
            }
        }
        foreach(var room in addedRooms)
        {
            if (currentRoomUIDict.TryGetValue(room.Name, out var roomGO))
            {
                roomGO.GetComponent<RoomInfoUI>()?.Refresh(room.Name, room.PlayerCount);
            }
            else
            { 
                var newGO = Instantiate(roomInfoObj, roomScrollView.content);
                newGO.GetComponent<RoomInfoUI>()?.Refresh(room.Name, room.PlayerCount);
                currentRoomUIDict[room.Name] = newGO;
            }
        }
    }

    private void OnDestroy()
    {
        PhotonManager.Instance.RefreshAction -= Refresh;
    }
}
