using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ShootScript : MonoBehaviourPunCallbacks
{
    public Camera FPS_Camera;
    public GameObject bulletEffectPrefab;

    [Header("Health")]
    public float StartHealth = 100f;
    public float health;
    public Image healthSlider;

    private Animator animator;

    private void Start()
    {
        health = StartHealth;
        healthSlider.fillAmount = health / StartHealth;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            Fire();
        }
    }

    public void Fire()
    {
        Ray ray = FPS_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);

            // Call effect across all clients
            photonView.RPC("CreateEffect", RpcTarget.All, hit.point);

            if (hit.collider.CompareTag("Player"))
            {
                PhotonView targetPhotonView = hit.collider.GetComponent<PhotonView>();
                if (targetPhotonView != null && !targetPhotonView.IsMine)
                {
                    targetPhotonView.RPC("DamageHealth", RpcTarget.AllBuffered, 10f, PhotonNetwork.LocalPlayer);
                }
            }
        }
    }

    [PunRPC]
    public void CreateEffect(Vector3 position)
    {
        GameObject effect = Instantiate(bulletEffectPrefab, position, Quaternion.identity);
        Destroy(effect, 1f);
    }

    [PunRPC]
    public void DamageHealth(float damage, Photon.Realtime.Player sender)
    {
        health -= damage;
        healthSlider.fillAmount = health / StartHealth;

        if (health <= 0)
        {
            Debug.Log(sender.NickName + " killed " + photonView.Owner.NickName);
            animator.SetBool("IsDead", true);
            Die();
        }
    }

    public void Die()
    {
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        GameObject respText = GameObject.Find("RespawnText");
        float RespawnTime = 8f;

        GetComponent<SimpleFPSController>().enabled = false;

        while (RespawnTime > 0f)
        {
            yield return new WaitForSeconds(1f);
            RespawnTime -= 1f;

            if (respText != null)
            {
                respText.GetComponent<TMP_Text>().text = "Player Respawning in: " + RespawnTime.ToString("0");
            }
        }

        if (respText != null)
        {
            respText.GetComponent<TMP_Text>().text = "";
        }

        transform.position = new Vector3(-16.7f, 2f, -18.82f);
        animator.SetBool("IsDead", false);
        GetComponent<SimpleFPSController>().enabled = true;

        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RegainHealth()
    {
        health = StartHealth;
        healthSlider.fillAmount = health / StartHealth;
    }
}
