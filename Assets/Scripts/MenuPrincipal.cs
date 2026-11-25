using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public GameObject panelComoJugar;

    public void Jugar()
    {
        // Reinicia los datos de partida antes de reproducir la intro
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene("Intro");
    }

    public void AbrirComoJugar()
    {
        if (panelComoJugar != null)
        {
            panelComoJugar.SetActive(true);
        }
    }

    public void CerrarComoJugar()
    {
        if (panelComoJugar != null)
        {
            panelComoJugar.SetActive(false);
        }
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
}