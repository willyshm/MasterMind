using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardGame : MonoBehaviour
{
    public Image[] cardSpaces; // Espacios donde se mostrarán las cartas
    public Sprite[] colorSprites; // Sprites de colores para las cartas
    public Sprite unknownSprite; // Sprite para la carta en incógnito
    private int[] cardOrder; // Orden aleatorio de las cartas

    void Start()
    {
        // Inicializa las cartas en incógnito
        InitializeCards();
    }

    void InitializeCards()
    {
        cardOrder = new int[cardSpaces.Length];

        // Generar un orden aleatorio para las cartas
        for (int i = 0; i < cardSpaces.Length; i++)
        {
            cardOrder[i] = i; // Asigna un índice para cada carta
        }

        ShuffleArray(cardOrder); // Mezcla el orden

        // Establecer las cartas en incógnito
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
        // Animar la rotación de las cartas
        for (int i = 0; i < cardSpaces.Length; i++)
        {
            int index = i; // Necesario para capturar correctamente el índice en la lambda

            // Crear la animación de volteo en el eje X
            cardSpaces[i].transform.DOScaleX(0, 0.5f).OnComplete(() =>
            {
                // Cambiar el sprite cuando la carta esté en el punto medio del volteo
                cardSpaces[index].sprite = colorSprites[cardOrder[index]];

                // Completar el giro
                cardSpaces[index].transform.DOScaleX(1, 0.5f);
            });
        }
    }
}
