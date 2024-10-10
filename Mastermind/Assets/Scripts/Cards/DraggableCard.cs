using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform parentToReturnTo = null;

    // Nueva referencia a la posición de inicio
    public RectTransform startPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false; // Evita que interfiera con otros elementos mientras se arrastra
        parentToReturnTo = this.transform.parent; // Guarda el padre original
        this.transform.SetParent(this.transform.parent.parent); // Saca la carta del layout temporalmente para arrastrarla
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / CanvasScalerFactor(); // Mueve la carta con el puntero
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
                // Cambia el padre al slot y ajusta la posición
                this.transform.SetParent(slot.transform); // Ajusta el padre al slot
                rectTransform.anchoredPosition = Vector2.zero; // Coloca la carta en la posición correcta dentro del slot
                placedInSlot = true;
                break;
            }
        }

        if (!placedInSlot)
        {
            // Si no se soltó en un lugar válido, regresa a la posición inicial referenciada
            rectTransform.anchoredPosition = startPosition.anchoredPosition;
            this.transform.SetParent(startPosition); // Regresa al padre original del inicio
        }
    }

    private float CanvasScalerFactor()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        return canvas.scaleFactor; // Para que funcione correctamente con diferentes resoluciones y escalas de UI
    }
}
