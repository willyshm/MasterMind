using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
public class CargarPantalla : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("PantallaInicio"); 
    }
}
