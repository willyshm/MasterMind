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

    // Función para ir a la siguiente página
    public void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            MovePage(currentPage, currentPage + 1); 
            currentPage++;
        }
        else
        {
            // Volver a la primera página
            MovePage(currentPage, 0);
            currentPage = 0;
        }

        UpdatePageIndicators();
        RestartAutoSwitch(); 
    }

    // Función para regresar a la página anterior
    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            MovePage(currentPage, currentPage - 1); 
            currentPage--;
        }
        else
        {
            // Saltar a la última página
            MovePage(currentPage, pages.Length - 1);
            currentPage = pages.Length - 1;
        }

        UpdatePageIndicators();
        RestartAutoSwitch(); 
    }

    // Función para mover las páginas
    private void MovePage(int fromIndex, int toIndex)
    {
       
        float targetX = (toIndex > fromIndex) ? Screen.width : -Screen.width;
        pages[toIndex].anchoredPosition = new Vector2(targetX, 0);

        pages[toIndex].DOAnchorPos(Vector2.zero, transitionTime).SetEase(Ease.OutQuad);

        float moveOutX = (toIndex > fromIndex) ? -Screen.width : Screen.width;
        pages[fromIndex].DOAnchorPos(new Vector2(moveOutX, 0), transitionTime).SetEase(Ease.OutQuad);
    }

    // Función para actualizar el estado de los puntos
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

    // Corrutina que cambia de página automáticamente después de un tiempo
    private IEnumerator AutoSwitchPages()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSwitchTime); 
            NextPage(); 
        }
    }

    // Función para cerrar la ventana de instrucciones
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
