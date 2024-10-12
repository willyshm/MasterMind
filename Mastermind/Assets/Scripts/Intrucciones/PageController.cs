using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PageController : MonoBehaviour
{
    public RectTransform[] pages;
    public Image[] pageIndicators; 
    public Color activeColor = Color.white; 
    public Color inactiveColor = Color.gray; 
    public float transitionTime = 0.5f; 
    public float autoSwitchTime = 5f; 
    public RectTransform instructionCanvas;
    public CanvasGroup canvasGroup; 
    private int currentPage = 0;
    private Coroutine autoSwitchCoroutine;

    private void Start()
    {
        UpdatePageIndicators();
        StartAutoSwitch(); 
    }

    // Funci�n para ir a la siguiente p�gina
    public void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            MovePage(currentPage, currentPage + 1); 
            currentPage++;
        }
        else
        {
            // Volver a la primera p�gina
            MovePage(currentPage, 0);
            currentPage = 0;
        }

        UpdatePageIndicators();
        RestartAutoSwitch(); 
    }

    // Funci�n para regresar a la p�gina anterior
    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            MovePage(currentPage, currentPage - 1); 
            currentPage--;
        }
        else
        {
            // Saltar a la �ltima p�gina
            MovePage(currentPage, pages.Length - 1);
            currentPage = pages.Length - 1;
        }

        UpdatePageIndicators();
        RestartAutoSwitch(); 
    }

    // Funci�n para mover las p�ginas
    private void MovePage(int fromIndex, int toIndex)
    {
       
        float targetX = (toIndex > fromIndex) ? Screen.width : -Screen.width;
        pages[toIndex].anchoredPosition = new Vector2(targetX, 0);

        pages[toIndex].DOAnchorPos(Vector2.zero, transitionTime).SetEase(Ease.OutQuad);

        float moveOutX = (toIndex > fromIndex) ? -Screen.width : Screen.width;
        pages[fromIndex].DOAnchorPos(new Vector2(moveOutX, 0), transitionTime).SetEase(Ease.OutQuad);
    }

    // Funci�n para actualizar el estado de los puntos
    private void UpdatePageIndicators()
    {
        for (int i = 0; i < pageIndicators.Length; i++)
        {
            pageIndicators[i].color = (i == currentPage) ? activeColor : inactiveColor; 
        }
    }

    private void StartAutoSwitch()
    {
        autoSwitchCoroutine = StartCoroutine(AutoSwitchPages());
    }

    private void RestartAutoSwitch()
    {
        StopCoroutine(autoSwitchCoroutine);
        StartAutoSwitch();
    }

    // Corrutina que cambia de p�gina autom�ticamente despu�s de un tiempo
    private IEnumerator AutoSwitchPages()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSwitchTime); 
            NextPage(); 
        }
    }

    // Funci�n para cerrar la ventana de instrucciones
    public void CloseInstructions()
    {
        // Desvanecer el Canvas y escalarlo a cero
        canvasGroup.DOFade(0, transitionTime).OnUpdate(() =>
        {
            instructionCanvas.DOScale(Vector3.zero, transitionTime);
        }).OnComplete(() =>
        {
            instructionCanvas.gameObject.SetActive(false); 
        });
    }
}
