using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    public Canvas gameCanvas;      // Referencia al canvas del juego
    public Canvas resultCanvas;    // Referencia al canvas de resultados

    // Método para abrir el canvas de resultados
    public void OpenResultCanvas()
    {
        // Desactiva el canvas del juego
        gameCanvas.enabled = false;

        // Activa el canvas de resultados
        resultCanvas.enabled = true;

        Debug.Log("Canvas de resultados abierto.");
    }

    // Método para reiniciar la escena
    public void RestartGame()
    {
        // Recarga la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Reiniciando el juego...");
    }
}
