using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public Image[] hiddenCards;    
    public Image[] playerSlots;   
    public TextMeshProUGUI scoreText; 
    public Canvas gameCanvas;      
    public Canvas resultCanvas;    
    public GameManager gameManager; 

    private int initialOpportunities = 2;
    private int opportunities;

    // Inicializaci�n
    private void Start()
    {
        opportunities = initialOpportunities; 
        UpdateScoreText(0);
        ShowOpportunities(); 
    }

    // M�todo para verificar el puntaje basado en los sprites de las cartas
    public int CheckScore()
    {
        int score = 0;

        for (int i = 0; i < hiddenCards.Length; i++)
        {
            // Obtener el sprite de la carta oculta
            Sprite hiddenSprite = hiddenCards[i].sprite;

            // Obtener el sprite de la carta que el jugador movi�
            Transform playerCardTransform = playerSlots[i].transform.GetChild(0);
            Image playerCardImage = playerCardTransform.GetComponent<Image>();
            Sprite playerSprite = playerCardImage.sprite;

            // Verificar que los sprites no sean nulos
            if (hiddenSprite != null && playerSprite != null)
            {
                if (hiddenSprite == playerSprite)
                {
                    score++; 
                }
            }
        }

     

        UpdateScoreText(score);

        DecreaseOpportunities();

        return score;
    }

    // M�todo para actualizar el puntaje en el canvas de juego
    private void UpdateScoreText(int score)
    {
        scoreText.text =  "" + score + "/" + hiddenCards.Length;
    }

    // M�todo que disminuye las oportunidades despu�s de una verificaci�n
    private void DecreaseOpportunities()
    {
        if (opportunities > 0)
        {
            opportunities--;
            ShowOpportunities();

            if (opportunities == 0)
            {
                // Si se terminan las oportunidades, se abre el canvas de resultados
                gameManager.OpenResultCanvas();
            }
        }
    }

    // M�todo para actualizar y mostrar cu�ntas oportunidades quedan
    private void ShowOpportunities()
    {
        Debug.Log("Oportunidades restantes: " + opportunities);
    }

    // M�todo para reiniciar el juego
    public void ResetGame()
    {
        Debug.Log("Reiniciando el juego...");

        UpdateScoreText(0);

        opportunities = initialOpportunities;
        ShowOpportunities();

        ShuffleCards();

        gameCanvas.enabled = true;

        resultCanvas.enabled = false;

        Debug.Log("El juego ha sido reiniciado.");
    }

    private void ShuffleCards()
    {
        foreach (Image card in hiddenCards)
        {
            
            Debug.Log("Carta mezclada: " + card.name);
        }
    }
}
