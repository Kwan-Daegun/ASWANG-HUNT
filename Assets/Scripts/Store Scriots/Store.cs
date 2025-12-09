using UnityEngine;

public class Store : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRadius = 2f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Player Reference")]
    public Transform player;

    [Header("UI")]
    public GameObject storeUI;       // Assign your store panel here
    public GameObject interactPrompt; // "Press E" text (optional)

    private bool isOpen = false;

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        // Inside Radius
        if (distance <= interactRadius)
        {
            if (!isOpen && interactPrompt != null)
                interactPrompt.SetActive(true);

            if (Input.GetKeyDown(interactKey))
                ToggleStore();
        }
        else
        {
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
    }

    void ToggleStore()
    {
        isOpen = !isOpen;
        storeUI.SetActive(isOpen);

        if (interactPrompt != null)
            interactPrompt.SetActive(!isOpen);

        Time.timeScale = isOpen ? 0f : 1f; // Pause game when opened
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }

    public void back()
    {
        Time.timeScale = 1f;
    }
}
