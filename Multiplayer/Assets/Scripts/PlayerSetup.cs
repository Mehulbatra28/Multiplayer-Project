using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject[] LocalPlayerItems;
    public GameObject[] RemotePlayerItems;

    void Start()
    {
        if (photonView.IsMine)
        {
            foreach (GameObject g in LocalPlayerItems)
            {
                g.SetActive(true);
            }
            foreach (GameObject g in RemotePlayerItems)
            {
                g.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject g in LocalPlayerItems)
            {
                g.SetActive(false);
            }
            foreach (GameObject g in RemotePlayerItems)
            {
                g.SetActive(true);
            }
        }
    }
}
