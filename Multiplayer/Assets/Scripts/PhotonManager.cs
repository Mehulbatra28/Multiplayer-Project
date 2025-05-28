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

    [Header("InsideRoomPanel")]
    public GameObject InsideRoomPanel;
    #region UnityMethods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ActivatePanel(LoginPanel.name);
        roomListData = new Dictionary<string, RoomInfo>();
        roomListGameObject = new Dictionary<string, GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("network state :" + PhotonNetwork.NetworkClientState);
    }
    #endregion

    #region UIMethods
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
            roomName = roomName + Random.Range(0, 1000);
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
    

    #endregion

        #region Photon_callbacks

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
        
        Debug.Log("RoomCreated" + PhotonNetwork.CurrentRoom.Name);
    }
    public override void OnJoinedRoom()
    {
        ActivatePanel(InsideRoomPanel.name);
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + "Roomjoined");
       
    }
    public override void OnRoomListUpdate(List<RoomInfo>roomList)
    {
        //clear List
        ClearRoomList();
        foreach (RoomInfo rooms in roomList)
        {
            Debug.Log("Room name"+rooms.Name);
            if (!rooms.IsOpen || !rooms.IsVisible || rooms.RemovedFromList)
            {
                if (roomListData.ContainsKey(rooms.Name))

                {
                    roomListData.Remove(rooms.Name); 
                }
            }
            else
            {
                if (roomListData.ContainsKey(rooms.Name))
                {
                    //update list
                    roomListData[rooms.Name] = rooms;
                }
                else
                {
                    roomListData.Add(rooms.Name, rooms);
                }
            }
                
        }
        foreach(RoomInfo roomItem in roomListData.Values)
        {
            GameObject roomListItemObject = Instantiate(RoomListPrefab);
            roomListItemObject.transform.SetParent(RoomListParent.transform);
            roomListItemObject.transform.localScale=Vector3.one;
            //room name player number button room join
            roomListItemObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text=roomItem.Name;
            roomListItemObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text=roomItem.PlayerCount+"/"+roomItem.MaxPlayers;
            roomListItemObject.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(()=>RoomJoinFromList(roomItem.Name));
            roomListGameObject.Add(roomItem.Name,roomListItemObject);
        }
    }
    public override void OnLeftLobby()
    {
        ClearRoomList();
        roomListData.Clear();
    }
    #endregion

    #region Public_Methods
    public void ActivatePanel(string PanelName)
    {
        LoginPanel.SetActive(PanelName.Equals(LoginPanel.name));
        LobbyPanel.SetActive(PanelName.Equals(LobbyPanel.name));
        RoomPanel.SetActive(PanelName.Equals(RoomPanel.name));
        ConnectingPanel.SetActive(PanelName.Equals(ConnectingPanel.name));
        RoomListPanel.SetActive(PanelName.Equals(RoomListPanel.name));
        InsideRoomPanel.SetActive(PanelName.Equals(InsideRoomPanel.name));
        
    }
    public void RoomJoinFromList(string roomname)
    {
        if(PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        PhotonNetwork.JoinRoom(roomname);
    }
    public void ClearRoomList()
    {
        if (roomListGameObject.Count > 0)
        {
            foreach (var v in roomListGameObject.Values)
            {
                Destroy(v);
            }
            roomListGameObject.Clear();
        }
    }

    #endregion

}
