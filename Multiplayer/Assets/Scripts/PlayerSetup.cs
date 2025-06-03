using UnityEngine;
using Photon.Pun;


public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject[] LocalPlayerItems;
    public GameObject[] RemotePlayerItems;
    public FirstPersonController FirstPersonController;
    public GameObject Canvas;
    public GameObject CameraHolder;


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
            GetComponent<FirstPersonController>().enabled = true;
            Canvas.SetActive(true);
            CameraHolder.SetActive(true);
            
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
            GetComponent<FirstPersonController>().enabled = false;
            Canvas.SetActive(false);
            CameraHolder.SetActive(false);
            
        }
    }
}
