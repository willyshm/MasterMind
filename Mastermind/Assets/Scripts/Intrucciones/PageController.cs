using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;//libreria de animaciones
using UnityEngine.UI;

public class PageController : MonoBehaviour
{
    public RectTransform[] pages; // Array de las p�ginas
    public Image[] pageIndicators; // Array de los puntos indicadores
    public Color activeColor = Color.white; // Color del punto activo
    public Color inactiveColor = Color.gray; // Color de los puntos inactivos
    public float transitionTime = 0.5f; // Tiempo de la transici�n
    public float autoSwitchTime = 5f; // Tiempo de inactividad antes de cambiar de p�gina autom�ticamente
    public RectTransform instructionCanvas; // Referencia al Canvas de instrucciones
    public CanvasGroup canvasGroup; // Referencia al CanvasGroup
    private int currentPage = 0;
    private Coroutine autoSwitchCoroutine;

    private void Start()
    {
        UpdatePageIndicators(); // Inicializa los indicadores
        StartAutoSwitch(); // Inicia el temporizador autom�tico
    }

    // Funci�n para ir a la siguiente p�gina
    public void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            MovePage(currentPage, currentPage + 1); // Mover a la siguiente p�gina
            currentPage++;
        }
        else
        {
            // Volver a la primera p�gina
            MovePage(currentPage, 0);
            currentPage = 0;
        }

        UpdatePageIndicators();
        RestartAutoSwitch(); // Reinicia el temporizador cada vez que cambias de p�gina
    }

    // Funci�n para regresar a la p�gina anterior
    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            MovePage(currentPage, currentPage - 1); // Mover a la p�gina anterior
            currentPage--;
        }
        else
        {
            // Saltar a la �ltima p�gina
            MovePage(currentPage, pages.Length - 1);
            currentPage = pages.Length - 1;
        }

        UpdatePageIndicators();
        RestartAutoSwitch(); // Reinicia el temporizador cada vez que cambias de p�gina
    }

    // Funci�n para mover las p�ginas
    private void MovePage(int fromIndex, int toIndex)
    {
        pages[toIndex].anchoredPosition = new Vector2((toIndex > fromIndex ? Screen.width : -Screen.width), 0);
        pages[toIndex].DOAnchorPos(Vector2.zero, transitionTime);
        pages[fromIndex].DOAnchorPos(new Vector2((toIndex > fromIndex ? -Screen.width : Screen.width), 0), transitionTime);
    }

    // Funci�n para actualizar el estado de los puntos
    private void UpdatePageIndicators()
    {
        for (int i = 0; i < pageIndicators.Length; i++)
        {
            pageIndicators[i].color = (i == currentPage) ? activeColor : inactiveColor; 
        }
    }

    // Iniciar la corrutina para el cambio autom�tico de p�ginas
    private void StartAutoSwitch()
    {
        autoSwitchCoroutine = StartCoroutine(AutoSwitchPages());
    }

    // Reiniciar el temporizador autom�tico
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
            yield return new WaitForSeconds(autoSwitchTime); // Esperar el tiempo definido
            NextPage(); // Cambiar a la siguiente p�gina autom�ticamente
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
