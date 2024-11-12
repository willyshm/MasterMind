using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasToggleButton : MonoBehaviour
{
    public Canvas targetCanvas;  // Asigna el Canvas que quieres activar o desactivar


    private void Start()
    {
        if (targetCanvas != null)
        {
            targetCanvas.enabled = false; // Desactiva el canvas al inicio
        }
    }


    // Este m�todo ser� llamado cuando se presione el bot�n
    public void ToggleCanvas()
    {
        if (targetCanvas != null)
        {
            // Alterna entre activar y desactivar el Canvas
            targetCanvas.enabled = !targetCanvas.enabled;
        }
    }
}
