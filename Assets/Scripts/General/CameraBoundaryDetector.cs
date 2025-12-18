using UnityEngine;
using Cinemachine;

public class CameraBoundaryDetector : MonoBehaviour
{
    [SerializeField] private int activePriority = 20;
    [SerializeField] private int inactivePriority = 5;

    private CinemachineVirtualCamera vCam;

    private void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (vCam == null)
            return;

        vCam.Priority = inactivePriority;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (vCam == null)
            return;

        vCam.Priority = activePriority;
    }
}
