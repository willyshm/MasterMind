using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSlot : MonoBehaviour
{
    public Image slotImage; // Para mostrar la carta en el slot
    private Image currentCard; // La carta actual en este slot

    public void SetCard(Image card)
    {
        currentCard = card;
        slotImage.sprite = card.sprite; // Actualiza la imagen del slot
    }

    public bool CheckMatch(Sprite[] colorSprites, int[] cardOrder)
    {
        if (currentCard != null)
        {
            // Compara la carta en el slot con la carta oculta correspondiente
            return currentCard.sprite == colorSprites[cardOrder[transform.GetSiblingIndex()]];
        }
        return false;
    }
}
