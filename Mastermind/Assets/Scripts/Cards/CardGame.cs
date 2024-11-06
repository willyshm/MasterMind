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
    public CanvasGroup congratulationsCanvasGroup;
    public TextMeshProUGUI finalScoreText;

    public TextMeshProUGUI opportunitiesText;
    public TextMeshProUGUI currentScoreText;

    private int remainingAttempts = 2;
    private int finalScore = 0;
    private bool gameStarted = false;

    public AudioSource shuffleSound;
    public AudioSource flipSound;

    public GameObject verificationMessagePrefab;
    private GameObject currentMessageInstance;

    void Start()
    {
        verifyButton.onClick.AddListener(() => VerifyCards(FindObjectOfType<ScoreManager>()));
        UpdateGameCanvas();
    }

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

    void ShuffleCards()
    {
        shuffleSound.Play();

        for (int i = 0; i < cardSpaces.Length; i++)
        {
            int randomIndex = Random.Range(0, shufflePositions.Length);
            float randomRotation = Random.Range(-30f, 30f);

            cardSpaces[i].transform.DOMove(shufflePositions[randomIndex].position, 1f)
                .SetEase(Ease.InOutQuad);

            cardSpaces[i].transform.DORotate(new Vector3(0, 0, randomRotation), 1f)
                .SetEase(Ease.InOutQuad);

            if (i == cardSpaces.Length - 1)
            {
                cardSpaces[i].transform.DOMove(shufflePositions[randomIndex].position, 1f).OnComplete(() =>
                {
                    DealCardsToFinalSlots();
                    shuffleSound.Stop();
                });
            }
        }
    }

    void DealCardsToFinalSlots()
    {
        for (int i = 0; i < cardSpaces.Length; i++)
        {
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

        finalScore = scoreManager.CheckScore();

        if (remainingAttempts == 2)
        {
            UpdateVerificationMessage();
        }

        remainingAttempts--;

        UpdateGameCanvas();

        if (finalScore == cardSpaces.Length && remainingAttempts == 1)
        {
            ShowCongratulationsCanvas();
        }
        else if (remainingAttempts <= 0)
        {
            ShowResultCanvas();
        }
    }

    private void UpdateGameCanvas()
    {
        opportunitiesText.text = "Oportunidades restantes: " + remainingAttempts;
        currentScoreText.text = "Puntaje actual: " + finalScore;
    }

    private void ShowResultCanvas()
    {
        gameCanvasGroup.DOFade(0, 0.5f).OnComplete(() =>
        {
            gameCanvasGroup.gameObject.SetActive(false);
            resultCanvasGroup.gameObject.SetActive(true);
            resultCanvasGroup.DOFade(1, 0.5f);
        });

        finalScoreText.text = "" + finalScore + "/" + cardSpaces.Length;
    }

    private void ShowCongratulationsCanvas()
    {
        gameCanvasGroup.DOFade(0, 0.5f).OnComplete(() =>
        {
            gameCanvasGroup.gameObject.SetActive(false);
            congratulationsCanvasGroup.gameObject.SetActive(true);
            congratulationsCanvasGroup.DOFade(1, 0.5f);
        });
    }

    private void UpdateVerificationMessage()
    {
        if (currentMessageInstance != null)
        {
            Destroy(currentMessageInstance);
        }

        currentMessageInstance = Instantiate(verificationMessagePrefab, gameCanvasGroup.transform);
        TextMeshProUGUI messageText = currentMessageInstance.GetComponentInChildren<TextMeshProUGUI>();
        messageText.text = $"Acertaste {finalScore} carta(s), te queda 1 intento";
        currentMessageInstance.transform.DOScale(1.2f, 0.2f).From(1f).SetLoops(2, LoopType.Yoyo);

        StartCoroutine(HideVerificationMessageAfterDelay(3f));
    }

    private IEnumerator HideVerificationMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (currentMessageInstance != null)
        {
            Destroy(currentMessageInstance);
        }
    }
} 
    
