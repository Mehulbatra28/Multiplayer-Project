using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;


public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject[] LocalPlayerItems;
    public GameObject[] RemotePlayerItems;
    public SimpleFPSController FirstPersonController;
    public GameObject Canvas;
    public GameObject CameraHolder;
    private Animator animationController;
    private ShootScript shootScript;

    void Start()
    {
        animationController = GetComponent<Animator>();
        shootScript = GetComponent<ShootScript>();

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
            if(Input.GetButton("Fire1"))
            {
                shootScript.Fire();
            }
            CameraHolder.SetActive(true);
            animationController.SetBool("Soldier", true);

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
            animationController.SetBool("Soldier", false);

        }
        
    }
}
