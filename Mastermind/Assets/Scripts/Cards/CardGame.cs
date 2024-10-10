using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardGame : MonoBehaviour
{
    public Image[] cardSpaces; // Espacios donde se mostrar�n las cartas
    public Sprite[] colorSprites; // Sprites de colores para las cartas
    public Sprite unknownSprite; // Sprite para la carta en inc�gnito
    private int[] cardOrder; // Orden aleatorio de las cartas

    void Start()
    {
        // Inicializa las cartas en inc�gnito
        InitializeCards();
    }

    void InitializeCards()
    {
        cardOrder = new int[cardSpaces.Length];

        // Generar un orden aleatorio para las cartas
        for (int i = 0; i < cardSpaces.Length; i++)
        {
            cardOrder[i] = i; // Asigna un �ndice para cada carta
        }

        ShuffleArray(cardOrder); // Mezcla el orden

        // Establecer las cartas en inc�gnito
        foreach (var space in cardSpaces)
        {
            space.sprite = unknownSprite;
        }
    }

    void ShuffleArray(int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int temp = array[i];
            int randomIndex = Random.Range(0, array.Length);
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    public void VerifyCards()
    {
        // Animar la rotaci�n de las cartas
        for (int i = 0; i < cardSpaces.Length; i++)
        {
            int index = i; // Necesario para capturar correctamente el �ndice en la lambda

            // Crear la animaci�n de volteo en el eje X
            cardSpaces[i].transform.DOScaleX(0, 0.5f).OnComplete(() =>
            {
                // Cambiar el sprite cuando la carta est� en el punto medio del volteo
                cardSpaces[index].sprite = colorSprites[cardOrder[index]];

                // Completar el giro
                cardSpaces[index].transform.DOScaleX(1, 0.5f);
            });
        }
    }
}
