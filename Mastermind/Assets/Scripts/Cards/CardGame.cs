using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class CardGame : MonoBehaviour
{
    public Image[] cardSpaces;
    public Sprite[] colorSprites;
    public Sprite unknownSprite;
    private int[] cardOrder;

    public Transform[] shufflePositions;
    public Transform[] finalSlots;
    public Button verifyButton;

    public CanvasGroup gameCanvasGroup;
    public CanvasGroup resultCanvasGroup;
    public TextMeshProUGUI finalScoreText;

    public TextMeshProUGUI opportunitiesText;
    public TextMeshProUGUI currentScoreText;

    private int remainingAttempts = 2;
    private int finalScore = 0;        
    private bool gameStarted = false;  

    public AudioSource shuffleSound;
    public AudioSource flipSound;

    void Start()
    {
        
        verifyButton.onClick.AddListener(() => VerifyCards(FindObjectOfType<ScoreManager>()));
        UpdateGameCanvas();
    }

    // Método que se llama para iniciar el juego
    public void StartGame()
    {
        if (!gameStarted) 
        {
            gameStarted = true; 
            InitializeCards(); 
        }
    }

    void InitializeCards()
    {
        cardOrder = new int[cardSpaces.Length];

        
        for (int i = 0; i < cardSpaces.Length; i++)
        {
            cardOrder[i] = i; 
        }

        ShuffleArray(cardOrder); 

        
        foreach (var space in cardSpaces)
        {
            space.sprite = unknownSprite;
        }

        
        InitializeHiddenCards();

       
        ShuffleCards();
    }

    void InitializeHiddenCards()
    {
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            Debug.Log("ScoreManager encontrado.");
            if (scoreManager.hiddenCards != null && scoreManager.hiddenCards.Length == colorSprites.Length)
            {
                for (int i = 0; i < scoreManager.hiddenCards.Length; i++)
                {
                    if (colorSprites[cardOrder[i]] != null)
                    {
                        Debug.Log($"Asignando sprite a carta oculta {i}: {colorSprites[cardOrder[i]].name}");
                        scoreManager.hiddenCards[i].sprite = colorSprites[cardOrder[i]]; 
                    }
                    else
                    {
                        Debug.LogWarning($"El sprite en colorSprites[{cardOrder[i]}] es nulo.");
                    }
                }
            }
            else
            {
                Debug.LogWarning("El array hiddenCards es nulo o tiene un tamaño incorrecto.");
            }
        }
        else
        {
            Debug.LogWarning("ScoreManager no encontrado.");
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

    // Animar la mezcla de cartas con rotación
    void ShuffleCards()
    {
        // Reproduce el sonido de mezclar
        shuffleSound.Play();

        for (int i = 0; i < cardSpaces.Length; i++)
        {
            int randomIndex = Random.Range(0, shufflePositions.Length);
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
                    // Detiene el sonido después de que se complete la animación
                    shuffleSound.Stop();
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
        StartCoroutine(FlipMatchingCards(scoreManager)); 
    }

    private IEnumerator FlipMatchingCards(ScoreManager scoreManager)
    {
        for (int i = 0; i < scoreManager.hiddenCards.Length; i++)
        {
            Sprite hiddenSprite = scoreManager.hiddenCards[i].sprite;
            Transform playerCardTransform = scoreManager.playerSlots[i].transform.GetChild(0);
            Image playerCardImage = playerCardTransform.GetComponent<Image>();
            Sprite playerSprite = playerCardImage.sprite;

            if (hiddenSprite != null && playerSprite != null)
            {
                Debug.Log($"Comparando: Oculta - {hiddenSprite.name}, Jugador - {playerSprite.name}");

                if (hiddenSprite == playerSprite)
                {
                    int index = i;
                    cardSpaces[index].transform.DOScaleX(0, 0.5f).OnComplete(() =>
                    {
                        flipSound.Play();
                        cardSpaces[index].sprite = colorSprites[cardOrder[index]];
                        cardSpaces[index].transform.DOScaleX(1, 0.5f);
                    });

                    yield return new WaitForSeconds(1f);
                }
            }
            else
            {
                Debug.LogWarning($"Sprite en la posición {i} es nulo. Oculta: {hiddenSprite}, Jugador: {playerSprite}");
            }
        }

        // Llama a la función para verificar puntuaciones al final de voltear las cartas
        finalScore = scoreManager.CheckScore(); 

        // Restar una oportunidad después de verificar
        remainingAttempts--;

        // Actualizar el marcador en el canvas del juego
        UpdateGameCanvas();

        // Verificar si se han terminado las oportunidades
        if (remainingAttempts <= 0)
        {
            ShowResultCanvas(); 
        }
    }

    private void UpdateGameCanvas()
    {
        // Actualizar la UI del canvas del juego para mostrar las oportunidades restantes y puntaje actual
        opportunitiesText.text = "Oportunidades restantes: " + remainingAttempts;
        currentScoreText.text = "Puntaje actual: " + finalScore;
    }

    private void ShowResultCanvas()
    {
        // Cambiar entre canvas con una animación suave
        gameCanvasGroup.DOFade(0, 0.5f).OnComplete(() =>
        {
            gameCanvasGroup.gameObject.SetActive(false);
            resultCanvasGroup.gameObject.SetActive(true);
            resultCanvasGroup.DOFade(1, 0.5f);
        });

        // Mostrar el puntaje final en el canvas de resultados
        finalScoreText.text = "Puntaje final: " + finalScore + "/" + cardSpaces.Length;
    }
} 
    
