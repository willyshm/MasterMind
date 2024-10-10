using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform parentToReturnTo = null;

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
                // Ajusta la carta al slot si se detecta uno
                rectTransform.anchoredPosition = slot.GetComponent<RectTransform>().anchoredPosition;
                this.transform.SetParent(slot.transform); // Ajusta el padre al slot
                placedInSlot = true;
                break;
            }
        }

        if (!placedInSlot)
        {
            // Si no se soltó en un lugar válido, regresa a su posición original
            rectTransform.anchoredPosition = originalPosition;
            this.transform.SetParent(parentToReturnTo);
        }
    }

    private float CanvasScalerFactor()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        return canvas.scaleFactor; // Para que funcione correctamente con diferentes resoluciones y escalas de UI
    }
}
