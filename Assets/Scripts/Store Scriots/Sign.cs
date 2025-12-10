using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour
{
    [Header("UI Settings")]
    [Tooltip("Drag the Canvas, Panel, or Text object you want to show here.")]
    public GameObject messageUI;

    // This runs once when the game starts
    void Start()
    {
        // Ensure the text/UI is hidden when the game begins
        if (messageUI != null)
        {
            messageUI.SetActive(false);
        }
    }

    // Triggered when another collider enters this object's trigger zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object colliding is the Player
        if (other.CompareTag("Player"))
        {
            if (messageUI != null)
            {
                messageUI.SetActive(true); // Show the message
            }
        }
    }

    // Triggered when the collider exits the trigger zone
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (messageUI != null)
            {
                messageUI.SetActive(false); // Hide the message
            }
        }
    }
}
