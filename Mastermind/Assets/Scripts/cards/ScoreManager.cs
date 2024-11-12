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

    // Probabilidades del primer intento
    [Header("Probabilidades del Primer Intento")]
    public float[] probabilidadesPrimerIntento = { 32.768f, 40.96f, 20.48f, 5.12f, 0.64f, 0.032f };

    // Probabilidades condicionales para el segundo intento
    [Header("Probabilidades del Segundo Intento")]
    public float[][] probabilidadesSegundoIntento = new float[][]
    {
        new float[] { 1.0f, 0.2f, 0.04f, 0.008f, 0.0016f, 0.0f },
        new float[] { 0.64f, 0.2f, 0.04f, 0.008f, 0.0016f, 0.0f },
        new float[] { 0.54f, 0.18f, 0.25f, 0.14f, 0.04f, 0.0f },
        new float[] { 0.21f, 0.073f, 0.149f, 0.35f, 0.28f, 0.0f },
        new float[] { 0.05f, 0.013f, 0.028f, 0.074f, 0.88f, 1.0f },
        new float[] { 0, 0, 0, 0, 0, 1.0f }
    };

    // Referencias a TextMeshPro para mostrar probabilidades en la UI
    [Header("Textos de Probabilidad en UI")]
    public TextMeshProUGUI[] textosProbabilidadPrimerIntento;
    public TextMeshProUGUI[] textosProbabilidadSegundoIntento;

    private void Start()
    {
        opportunities = initialOpportunities;
        UpdateScoreText(0);
        ShowOpportunities();

        // Actualizar probabilidades en la UI al inicio
        UpdateProbabilityTexts(0);
    }

    // Método para verificar el puntaje basado en los sprites de las cartas
    public int CheckScore()
    {
        int score = 0;

        for (int i = 0; i < hiddenCards.Length; i++)
        {
            Sprite hiddenSprite = hiddenCards[i].sprite;
            Transform playerCardTransform = playerSlots[i].transform.GetChild(0);
            Image playerCardImage = playerCardTransform.GetComponent<Image>();
            Sprite playerSprite = playerCardImage.sprite;

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

        // Actualizar los textos de probabilidad en la UI después de verificar
        UpdateProbabilityTexts(score);

        return score;
    }

    // Método para actualizar el puntaje en el canvas de juego
    private void UpdateScoreText(int score)
    {
        scoreText.text = "" + score + "/" + hiddenCards.Length;
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
                gameManager.OpenResultCanvas();
            }
        }
    }

    // Método para actualizar y mostrar cuántas oportunidades quedan
    private void ShowOpportunities()
    {
        Debug.Log("Oportunidades restantes: " + opportunities);
    }

    // Método para actualizar las probabilidades en la UI
    private void UpdateProbabilityTexts(int aciertosPrimerIntento)
    {
        // Actualizar probabilidades del primer intento
        for (int i = 0; i < probabilidadesPrimerIntento.Length; i++)
        {
            textosProbabilidadPrimerIntento[i].text = $" {probabilidadesPrimerIntento[i]}%";
        }

        // Actualizar probabilidades del segundo intento basadas en el número de aciertos del primer intento
        if (aciertosPrimerIntento >= 0 && aciertosPrimerIntento < probabilidadesSegundoIntento.Length)
        {
            for (int j = 0; j < probabilidadesSegundoIntento[aciertosPrimerIntento].Length; j++)
            {
                textosProbabilidadSegundoIntento[j].text = $"{probabilidadesSegundoIntento[aciertosPrimerIntento][j] * 100}%";
            }
        }
    }

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
