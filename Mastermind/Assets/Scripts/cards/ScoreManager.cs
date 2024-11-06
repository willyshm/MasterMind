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

    // Inicialización
    private void Start()
    {
        opportunities = initialOpportunities; 
        UpdateScoreText(0);
        ShowOpportunities(); 
    }

    // Método para verificar el puntaje basado en los sprites de las cartas
    public int CheckScore()
    {
        int score = 0;

        for (int i = 0; i < hiddenCards.Length; i++)
        {
            // Obtener el sprite de la carta oculta
            Sprite hiddenSprite = hiddenCards[i].sprite;

            // Obtener el sprite de la carta que el jugador movió
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

    // Método para actualizar el puntaje en el canvas de juego
    private void UpdateScoreText(int score)
    {
        scoreText.text =  "" + score + "/" + hiddenCards.Length;
    }

    // Método que disminuye las oportunidades después de una verificación
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

    // Método para actualizar y mostrar cuántas oportunidades quedan
    private void ShowOpportunities()
    {
        Debug.Log("Oportunidades restantes: " + opportunities);
    }

    // Método para reiniciar el juego
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
