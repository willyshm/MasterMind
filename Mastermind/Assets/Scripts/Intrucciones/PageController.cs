using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;//libreria de animaciones
using UnityEngine.UI;

public class PageController : MonoBehaviour
{
    public RectTransform[] pages; // Array de las páginas
    public Image[] pageIndicators; // Array de los puntos indicadores
    public Color activeColor = Color.white; // Color del punto activo
    public Color inactiveColor = Color.gray; // Color de los puntos inactivos
    public float transitionTime = 0.5f; // Tiempo de la transición
    public float autoSwitchTime = 5f; // Tiempo de inactividad antes de cambiar de página automáticamente
    public RectTransform instructionCanvas; // Referencia al Canvas de instrucciones
    public CanvasGroup canvasGroup; // Referencia al CanvasGroup
    private int currentPage = 0;
    private Coroutine autoSwitchCoroutine;

    private void Start()
    {
        UpdatePageIndicators(); // Inicializa los indicadores
        StartAutoSwitch(); // Inicia el temporizador automático
    }

    // Función para ir a la siguiente página
    public void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            MovePage(currentPage, currentPage + 1); // Mover a la siguiente página
            currentPage++;
        }
        else
        {
            // Volver a la primera página
            MovePage(currentPage, 0);
            currentPage = 0;
        }

        UpdatePageIndicators();
        RestartAutoSwitch(); // Reinicia el temporizador cada vez que cambias de página
    }

    // Función para regresar a la página anterior
    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            MovePage(currentPage, currentPage - 1); // Mover a la página anterior
            currentPage--;
        }
        else
        {
            // Saltar a la última página
            MovePage(currentPage, pages.Length - 1);
            currentPage = pages.Length - 1;
        }

        UpdatePageIndicators();
        RestartAutoSwitch(); // Reinicia el temporizador cada vez que cambias de página
    }

    // Función para mover las páginas
    private void MovePage(int fromIndex, int toIndex)
    {
        pages[toIndex].anchoredPosition = new Vector2((toIndex > fromIndex ? Screen.width : -Screen.width), 0);
        pages[toIndex].DOAnchorPos(Vector2.zero, transitionTime);
        pages[fromIndex].DOAnchorPos(new Vector2((toIndex > fromIndex ? -Screen.width : Screen.width), 0), transitionTime);
    }

    // Función para actualizar el estado de los puntos
    private void UpdatePageIndicators()
    {
        for (int i = 0; i < pageIndicators.Length; i++)
        {
            pageIndicators[i].color = (i == currentPage) ? activeColor : inactiveColor; 
        }
    }

    // Iniciar la corrutina para el cambio automático de páginas
    private void StartAutoSwitch()
    {
        autoSwitchCoroutine = StartCoroutine(AutoSwitchPages());
    }

    // Reiniciar el temporizador automático
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
            yield return new WaitForSeconds(autoSwitchTime); // Esperar el tiempo definido
            NextPage(); // Cambiar a la siguiente página automáticamente
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
