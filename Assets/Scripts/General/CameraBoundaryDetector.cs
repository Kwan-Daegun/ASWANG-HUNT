using UnityEngine;

public class CameraBoundaryDetector : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject virtualCamera1;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!this || !collision || !player || !virtualCamera1) return;
        if (collision.gameObject != player) return;
        virtualCamera1.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!this || !collision || !player || !virtualCamera1) return;
        if (collision.gameObject != player) return;
        virtualCamera1.SetActive(false);
    }

    private void OnDisable()
    {
        if (virtualCamera1)
            virtualCamera1.SetActive(false);
    }
}
