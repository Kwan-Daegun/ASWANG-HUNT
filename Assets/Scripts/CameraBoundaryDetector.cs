using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBoundaryDetector : MonoBehaviour
{
    public GameObject player;
    public GameObject virtualCamera1;
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            virtualCamera1.gameObject.SetActive(true);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player) 
        {
            virtualCamera1.gameObject.SetActive(false);
        }
    }
}
