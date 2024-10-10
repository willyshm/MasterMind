using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition; // La posición original exacta de la carta
    private Transform parentToReturnTo = null;

    public Transform startParent; // El padre original al inicio
    public RectTransform startPosition; // La posición inicial exacta de la carta

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition; // Guarda la posición exacta al iniciar el arrastre
        canvasGroup.blocksRaycasts = false; // Permite arrastrar sobre otros elementos
        parentToReturnTo = this.transform.parent; // Guarda el padre actual antes de mover la carta
        this.transform.SetParent(this.transform.parent.parent); // Saca la carta del layout temporalmente
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / CanvasScalerFactor(); // Mueve la carta junto con el mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // Realiza un Raycast para detectar si la carta fue soltada sobre un CardSlot
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        bool placedInSlot = false;

        foreach (RaycastResult result in raycastResults)
        {
            CardSlot slot = result.gameObject.GetComponent<CardSlot>();
            if (slot != null)
            {
                // Verifica si el slot ya tiene una carta 
                if (slot.transform.childCount == 0)
                {
                    // Si el slot está vacío, coloca la carta en el slot
                    this.transform.SetParent(slot.transform); // Ajusta el padre al slot
                    rectTransform.anchoredPosition = Vector2.zero; // Coloca la carta en la posición correcta dentro del slot
                    placedInSlot = true;
                }
                else
                {
                    Debug.Log("Slot ya ocupado.");
                }
                break;
            }
        }

        if (!placedInSlot)
        {
            // Si no se soltó en un lugar válido o el slot está ocupado, regresa a la posición inicial
            rectTransform.anchoredPosition = startPosition.anchoredPosition;
            this.transform.SetParent(startParent); // Vuelve al padre original
        }
    }

    private float CanvasScalerFactor()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        return canvas.scaleFactor; // Para ajustar la escala de la UI
    }
}
