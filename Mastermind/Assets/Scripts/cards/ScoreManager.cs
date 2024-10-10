using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Image[] hiddenCards; 
    public Image[] playerSlots;  
    public Text scoreText;       

    // Método para verificar el puntaje basado en los sprites de las cartas
    public void CheckScore()
    {
        int score = 0; 

        // Recorre los slots de las cartas ocultas y del jugador
        for (int i = 0; i < hiddenCards.Length; i++)
        {
            // Obtener el sprite de la carta oculta
            Sprite hiddenSprite = hiddenCards[i].sprite;

            // Obtener el sprite de la carta que se mueve
            Transform playerCardTransform = playerSlots[i].transform.GetChild(0); 
            Image playerCardImage = playerCardTransform.GetComponent<Image>();
            Sprite playerSprite = playerCardImage.sprite;

            // Verificar que los sprites no sean nulos
            if (hiddenSprite != null && playerSprite != null)
            {
                
                Debug.Log($"Comparando: Oculta - {hiddenSprite.name}, Jugador - {playerSprite.name}");

                // Comparar los sprites de las cartas ocultas con las que están en los slots del jugador
                if (hiddenSprite == playerSprite)
                {
                    score++; // Si los sprites coinciden suma 1 al puntaje
                }
            }
            else
            {
                Debug.LogWarning($"Sprite en la posición {i} es nulo. Oculta: {hiddenSprite}, Jugador: {playerSprite}");
            }
        }

        // Mostrar el puntaje en la consola
        Debug.Log("Puntaje obtenido: " + score);

        // Actualizar el texto del puntaje en la UI
        scoreText.text = "Puntaje: " + score + "/" + hiddenCards.Length;
    }
}
