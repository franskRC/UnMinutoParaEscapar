using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class Temporizador : MonoBehaviour
{
    public float tiempoRestante = 60f;
    public TextMeshProUGUI textoTiempo;
    public GameObject textoAviso;

    private bool juegoTerminado = false;

    void Update()
    {
        if (juegoTerminado) return;

        if (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;
            textoTiempo.text = "Tiempo: " + Mathf.CeilToInt(tiempoRestante).ToString();

            // Cambia a rojo en los ultimos 5 segundos
            if (tiempoRestante <= 5f)
            {
                textoTiempo.color = Color.red;
            }
        }
        else
        {
            StartCoroutine(TerminarTiempo());
        }
    }

    IEnumerator TerminarTiempo()
    {
        juegoTerminado = true;
        tiempoRestante = 0;
        textoTiempo.text = "Tiempo: 0";

        // Muestra el letrero de ¡TIEMPO AGOTADO!
        if (textoAviso != null)
        {
            textoAviso.SetActive(true);
        }

        // Pausa 2 segundos para que el mensaje sea visible
        yield return new WaitForSeconds(2f);

        // Reinicia la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}