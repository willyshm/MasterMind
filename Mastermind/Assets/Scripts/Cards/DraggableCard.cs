
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition; 
    private Transform parentToReturnTo = null;

    // Nueva referencia a la posici�n de inicio
    public Transform startParent; 
    public RectTransform startPosition; 

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition; 
        canvasGroup.blocksRaycasts = false; 
        parentToReturnTo = this.transform.parent; 
        this.transform.SetParent(this.transform.parent.parent); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / CanvasScalerFactor(); 
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
                    // Si el slot est� vac�o, coloca la carta en el slot
                    this.transform.SetParent(slot.transform); 
                    rectTransform.anchoredPosition = Vector2.zero; 
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
            // Si no se solt� en un lugar v�lido o el slot est� ocupado, regresa a la posici�n inicial
            rectTransform.anchoredPosition = startPosition.anchoredPosition;
            this.transform.SetParent(startParent);
        }
    }

    private float CanvasScalerFactor()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        return canvas.scaleFactor; 
    }
}
