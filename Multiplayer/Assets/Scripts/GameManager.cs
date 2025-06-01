using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{

    public GameObject PlayerPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            Vector3 SpawnLocation = new Vector3(-16.7f,2.177f,-18.82f);
            Quaternion SpawnRotation = Quaternion.identity;
            PhotonNetwork.Instantiate(PlayerPrefab.name,SpawnLocation, SpawnRotation);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
