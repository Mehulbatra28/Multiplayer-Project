using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField LoginInput;
    public TMP_InputField RoomName;
    public GameObject LoginPanel;
    public GameObject LobbyPanel;
    public GameObject RoomPanel;
    public GameObject ConnectingPanel;
    public GameObject RoomListPanel;
    public int MaxPlayers = 2;
    public GameObject RoomListPrefab;
    public GameObject RoomListParent;

    private Dictionary<string, RoomInfo> roomListData;
    private Dictionary<string, GameObject> roomListGameObject;
    private Dictionary<int, GameObject> PlayerListGameObject;

    [Header("InsideRoomPanel")]
    public GameObject InsideRoomPanel;
    public GameObject PlayerListItemPrefab;
    public GameObject PlayerListItemParent;
    public GameObject PlayButton;

    #region UnityMethods
    void Start()
    {
        ActivatePanel(LoginPanel.name);
        roomListData = new Dictionary<string, RoomInfo>();
        roomListGameObject = new Dictionary<string, GameObject>();
        PlayerListGameObject = new Dictionary<int, GameObject>();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Update()
    {
        Debug.Log("network state :" + PhotonNetwork.NetworkClientState);
    }
    #endregion

    #region UI Methods
    public void OnLoginClick()
    {
        string name = LoginInput.text;
        if (!string.IsNullOrEmpty(name))
        {
            PhotonNetwork.LocalPlayer.NickName = name;
            PhotonNetwork.ConnectUsingSettings();
            ActivatePanel(ConnectingPanel.name);
        }
        else
        {
            Debug.LogWarning("empty name");
        }
    }

    public void OnCreateRoomClick()
    {
        string roomName = RoomName.text;
        if (!string.IsNullOrEmpty(roomName))
        {
            roomName += Random.Range(0, 1000);
        }
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)MaxPlayers;
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void OnRoomListButtonClick()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        ActivatePanel(RoomListPanel.name);
    }
    public void OnBackFromRoomList()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        ActivatePanel(LobbyPanel.name);
    }
    public void BackFromPlayerList()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        ActivatePanel(LobbyPanel.name);
    }
    public void OnPlayMasterButtonClick()
    {
        if (PhotonNetwork.IsMasterClient)
        {
        PhotonNetwork.LoadLevel("Game");
        }
    }

    #endregion

    #region Photon Callbacks
    public override void OnConnected()
    {
        Debug.Log("Connected to the internet");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connection to photon established");
        ActivatePanel(LobbyPanel.name);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("RoomCreated: " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinedRoom()
    {
        ActivatePanel(InsideRoomPanel.name);
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined the room");

        if(PhotonNetwork.IsMasterClient)
        {
            PlayButton.SetActive(true);
        }
        else
        {
            PlayButton.SetActive(false);
        }
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                AddPlayerToList(p);
            }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerToList(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PlayerListGameObject.ContainsKey(otherPlayer.ActorNumber))
        {
            Destroy(PlayerListGameObject[otherPlayer.ActorNumber]);
            PlayerListGameObject.Remove(otherPlayer.ActorNumber);
        }
        if(PhotonNetwork.IsMasterClient)
        {
            PlayButton.SetActive(true);
        }
        else
        {
            PlayButton.SetActive(false);
        }
    }

    public override void OnLeftRoom()
    {
        ActivatePanel(LobbyPanel.name);
        foreach (var obj in PlayerListGameObject.Values)
        {
            Destroy(obj);
        }
        PlayerListGameObject.Clear();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomList();

        foreach (RoomInfo room in roomList)
        {
            if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                if (roomListData.ContainsKey(room.Name))
                {
                    roomListData.Remove(room.Name);
                }
            }
            else
            {
                if (roomListData.ContainsKey(room.Name))
                {
                    roomListData[room.Name] = room;
                }
                else
                {
                    roomListData.Add(room.Name, room);
                }
            }
        }

        foreach (RoomInfo roomItem in roomListData.Values)
        {
            GameObject roomListItemObject = Instantiate(RoomListPrefab);
            roomListItemObject.transform.SetParent(RoomListParent.transform, false);
            roomListItemObject.transform.localScale = Vector3.one;

            roomListItemObject.transform.GetChild(0).GetComponent<TMP_Text>().text = roomItem.Name;
            roomListItemObject.transform.GetChild(1).GetComponent<TMP_Text>().text = roomItem.PlayerCount + "/" + roomItem.MaxPlayers;
            roomListItemObject.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => RoomJoinFromList(roomItem.Name));

            roomListGameObject.Add(roomItem.Name, roomListItemObject);
        }
    }

    public override void OnLeftLobby()
    {
        ClearRoomList();
        roomListData.Clear();
    }
    #endregion

    #region Helper Methods
    public void ActivatePanel(string PanelName)
    {
        LoginPanel.SetActive(PanelName == LoginPanel.name);
        LobbyPanel.SetActive(PanelName == LobbyPanel.name);
        RoomPanel.SetActive(PanelName == RoomPanel.name);
        ConnectingPanel.SetActive(PanelName == ConnectingPanel.name);
        RoomListPanel.SetActive(PanelName == RoomListPanel.name);
        InsideRoomPanel.SetActive(PanelName == InsideRoomPanel.name);
    }

    public void RoomJoinFromList(string roomName)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        PhotonNetwork.JoinRoom(roomName);
    }

    public void ClearRoomList()
    {
        foreach (var v in roomListGameObject.Values)
        {
            Destroy(v);
        }
        roomListGameObject.Clear();
    }

    private void AddPlayerToList(Player player)
    {
        GameObject playerListItem = Instantiate(PlayerListItemPrefab);
        playerListItem.transform.SetParent(PlayerListItemParent.transform, false);
        playerListItem.transform.localScale = Vector3.one;

        playerListItem.transform.GetChild(0).GetComponent<TMP_Text>().text = player.NickName;
        playerListItem.transform.GetChild(1).gameObject.SetActive(player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);

        PlayerListGameObject.Add(player.ActorNumber, playerListItem);
    }
    #endregion
}
