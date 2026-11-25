using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    public GameObject panelPausa;
    public string nombreEscenaMenu = "Menu";

    private bool estaPausado = false;

    void Start()
    {
        Time.timeScale = 1f;
        if (panelPausa != null) panelPausa.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (estaPausado)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Pausar()
    {
        estaPausado = true;
        if (panelPausa != null) panelPausa.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Reanudar()
    {
        estaPausado = false;
        if (panelPausa != null) panelPausa.SetActive(false);
        Time.timeScale = 1f;
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}