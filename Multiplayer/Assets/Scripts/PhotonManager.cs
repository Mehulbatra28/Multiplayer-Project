using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField LoginInput;
    public GameObject LoginPanel;
    public GameObject LobbyPanel;
    public GameObject RoomPanel;
    public GameObject ConnectingPanel;
    #region UnityMethods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ActivatePanel(LoginPanel.name);
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

    #endregion

    #region Public_Methods
    public void ActivatePanel(string PanelName)
    {
        LoginPanel.SetActive(PanelName.Equals(LoginPanel.name));
        LobbyPanel.SetActive(PanelName.Equals(LobbyPanel.name));
        RoomPanel.SetActive(PanelName.Equals(RoomPanel.name));
        ConnectingPanel.SetActive(PanelName.Equals(ConnectingPanel.name));
    }

    #endregion

}
