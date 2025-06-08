using UnityEngine;

public class ShootScript : MonoBehaviour
{
    public Camera FPS_Camera;
    public GameObject bulletEffectPrefab;

    void Update()
    {
        if (Input.GetButton("Fire1")) // Corrected input
        {
            Fire();
        }
    }

    public void Fire()
    {
        Ray ray = FPS_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Corrected ViewportPoint spelling and Vector3 case

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            GameObject effect=Instantiate(bulletEffectPrefab, hit.point, Quaternion.identity);
            Destroy(effect, 1f);
        }
    }
}
