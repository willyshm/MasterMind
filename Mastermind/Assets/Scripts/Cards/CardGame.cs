using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardGame : MonoBehaviour
{
    public Image[] cardSpaces; 
    public Sprite[] colorSprites; 
    public Sprite unknownSprite; 
    private int[] cardOrder; 

    public Transform[] shufflePositions; 
    public Transform[] finalSlots; 
    public Button verifyButton; 

    void Start()
    {
        // Inicializa las cartas en incógnito
        InitializeCards();
        verifyButton.onClick.AddListener(() => VerifyCards(FindObjectOfType<ScoreManager>())); 
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

        // Comenzar la animación de mezcla de las cartas
        ShuffleCards();
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

    // Animar la mezcla de cartas con rotación
    void ShuffleCards()
    {
        for (int i = 0; i < cardSpaces.Length; i++)
        {
            int randomIndex = Random.Range(0, shufflePositions.Length);

            // Genera una rotación aleatoria para cada carta durante la mezcla
            float randomRotation = Random.Range(-30f, 30f); 

            // Mover y rotar cada carta a una posición de mezcla aleatoria
            cardSpaces[i].transform.DOMove(shufflePositions[randomIndex].position, 1f)
                .SetEase(Ease.InOutQuad);

            cardSpaces[i].transform.DORotate(new Vector3(0, 0, randomRotation), 1f)
                .SetEase(Ease.InOutQuad); 

            // Cuando todas las cartas terminen de moverse, colocarlas en los slots
            if (i == cardSpaces.Length - 1)
            {
                cardSpaces[i].transform.DOMove(shufflePositions[randomIndex].position, 1f).OnComplete(() =>
                {
                    DealCardsToFinalSlots();
                });
            }
        }
    }

    // Colocar las cartas en los slots finales
    void DealCardsToFinalSlots()
    {
        for (int i = 0; i < cardSpaces.Length; i++)
        {
            // Mueve cada carta a su slot final
            cardSpaces[i].transform.DOMove(finalSlots[i].position, 1f).SetEase(Ease.InOutQuad);
            cardSpaces[i].transform.DORotate(Vector3.zero, 1f).SetEase(Ease.InOutQuad); 
        }
    }

    public void VerifyCards(ScoreManager scoreManager)
    {
        StartCoroutine(FlipCardsOneByOne(scoreManager)); // Inicia la corutina para voltear las cartas
    }

    private IEnumerator FlipCardsOneByOne(ScoreManager scoreManager)
    {
        for (int i = 0; i < cardSpaces.Length; i++)
        {
            int index = i; 

            // Crear la animación de volteo en el eje X
            cardSpaces[i].transform.DOScaleX(0, 0.5f).OnComplete(() =>
            {
                // Cambiar el sprite cuando la carta esté en el punto medio del volteo
                cardSpaces[index].sprite = colorSprites[cardOrder[index]];
                Debug.Log($"Asignando sprite: {cardSpaces[index].sprite.name} a la carta visible en {index}");

                // Asigna el sprite a las cartas ocultas después de que se ha cambiado el sprite visible
                scoreManager.hiddenCards[index].sprite = cardSpaces[index].sprite;
                Debug.Log($"Asignando sprite oculto: {scoreManager.hiddenCards[index].sprite.name} a hiddenCards en {index}");

                // Completar el giro
                cardSpaces[index].transform.DOScaleX(1, 0.5f);
            });

            yield return new WaitForSeconds(1f); 
        }

        // Llama a la función para verificar puntuaciones al final de voltear las cartas
        scoreManager.CheckScore(); 
    }
}
