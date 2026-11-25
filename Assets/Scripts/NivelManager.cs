using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class NivelManager : MonoBehaviour
{
    public TextMeshProUGUI textoTiempo;
    public TextMeshProUGUI textoProgreso;
    public GameObject textoAviso;
    public GameObject textoVictoria;
    public GameObject fondoCartel;

    public float tiempoRestante = 60f;
    public string siguienteEscena = "Nivel2_Mantenimiento";
    public string escenaMenu = "Menu";

    private int ciclosJugados;
    private int mecanismoReparado;
    private int generadorActivo;
    private bool juegoTerminado = false;

    void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Nivel3_Salida")
        {
            PlayerPrefs.SetInt("tieneLlaveInglesa", 0);
            PlayerPrefs.SetInt("mecanismoReparado", 0);
            PlayerPrefs.SetInt("generadorActivo", 0);
            PlayerPrefs.Save();
        }
    }

    void Start()
    {
        Time.timeScale = 1f;

        // Si entramos a Nivel3_Salida, reinicia el estado del nivel desde el inicio
        if (SceneManager.GetActiveScene().name == "Nivel3_Salida")
        {
            PlayerPrefs.SetInt("tieneLlaveInglesa", 0);
            PlayerPrefs.SetInt("mecanismoReparado", 0);
            PlayerPrefs.SetInt("generadorActivo", 0);
            PlayerPrefs.Save();
        }

        ciclosJugados = PlayerPrefs.GetInt("ciclos", 1);
        mecanismoReparado = PlayerPrefs.GetInt("mecanismoReparado", 0);
        generadorActivo = PlayerPrefs.GetInt("generadorActivo", 0);

        if (textoAviso != null) textoAviso.SetActive(false);
        if (textoVictoria != null) textoVictoria.SetActive(false);
        if (fondoCartel != null) fondoCartel.SetActive(false);

        ActualizarTextoProgreso();
    }

    void Update()
    {
        if (juegoTerminado) return;

        if (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;
            if (textoTiempo != null)
            {
                textoTiempo.text = "Tiempo: " + Mathf.CeilToInt(tiempoRestante).ToString();

                if (tiempoRestante <= 5f)
                {
                    textoTiempo.color = Color.red;
                }
            }
        }
        else
        {
            StartCoroutine(ReiniciarCiclo("¡TIEMPO AGOTADO!"));
        }
    }

    public void ActualizarTextoProgreso()
    {
        if (textoProgreso != null)
        {
            textoProgreso.text = "Ciclo: " + ciclosJugados + " | Mecanismo: " + mecanismoReparado + "/1 | Energia: " + generadorActivo + "/1";
        }
    }

    public void GuardarAvance(string clave)
    {
        PlayerPrefs.SetInt(clave, 1);
        PlayerPrefs.Save();

        mecanismoReparado = PlayerPrefs.GetInt("mecanismoReparado", 0);
        generadorActivo = PlayerPrefs.GetInt("generadorActivo", 0);
        ActualizarTextoProgreso();
    }

    public void ReiniciarPorObstaculo()
    {
        if (!juegoTerminado)
        {
            StartCoroutine(ReiniciarCiclo("¡MORISTE! REINICIANDO..."));
        }
    }

    IEnumerator ReiniciarCiclo(string mensaje)
    {
        juegoTerminado = true;

        MovimientoJugador jugador = FindAnyObjectByType<MovimientoJugador>();
        if (jugador != null) jugador.Morir();
        tiempoRestante = 0;
        if (textoTiempo != null) textoTiempo.text = "Tiempo: 0";

        ciclosJugados += 1;
        PlayerPrefs.SetInt("ciclos", ciclosJugados);
        PlayerPrefs.SetInt("tieneLlaveInglesa", 0);
        PlayerPrefs.SetInt("mecanismoReparado", 0);
        PlayerPrefs.SetInt("generadorActivo", 0);
        PlayerPrefs.Save();

        mecanismoReparado = 0;
        generadorActivo = 0;
        ActualizarTextoProgreso();

        if (textoAviso != null)
        {
            TextMeshProUGUI txt = textoAviso.GetComponent<TextMeshProUGUI>();
            if (txt != null) txt.text = mensaje;
            if (fondoCartel != null) fondoCartel.SetActive(true);
            textoAviso.SetActive(true);
        }

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CompletarNivel()
    {
        if (!juegoTerminado)
        {
            StartCoroutine(VictoriaRutina());
        }
    }

    IEnumerator VictoriaRutina()
    {
        juegoTerminado = true;

        bool esUltimoNivel = SceneManager.GetActiveScene().name == "Nivel3_Salida";

        if (textoVictoria != null)
        {
            TextMeshProUGUI txtVictoria = textoVictoria.GetComponent<TextMeshProUGUI>();
            if (txtVictoria != null && esUltimoNivel)
            {
                txtVictoria.text = "¡ESCAPASTE DEL LABORATORIO!";
            }

            if (fondoCartel != null) fondoCartel.SetActive(true);
            textoVictoria.SetActive(true);
        }

        yield return new WaitForSeconds(2.5f);

        if (esUltimoNivel)
        {
            SceneManager.LoadScene(escenaMenu);
        }
        else if (!string.IsNullOrEmpty(siguienteEscena))
        {
            SceneManager.LoadScene(siguienteEscena);
        }
    }
}