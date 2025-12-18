using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIhoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Settings")]
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    public Vector3 hoverOffset = new Vector3(0, 10f, 0); // Move up by 10 units
    public float speed = 10f;

    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Vector3 targetScale;
    private Vector3 targetPosition;

    void Start()
    {
        // Store the starting values
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;

        // Set initial targets to original values
        targetScale = originalScale;
        targetPosition = originalPosition;
    }

    void Update()
    {
        // Smoothly transition to the target values
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * speed);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * speed);
    }

    // Triggered when mouse enters the button area
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = hoverScale;
        targetPosition = originalPosition + hoverOffset;
    }

    // Triggered when mouse leaves the button area
    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
        targetPosition = originalPosition;
    }
}
