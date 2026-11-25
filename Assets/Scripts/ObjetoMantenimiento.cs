using UnityEngine;

public class ObjetoMantenimiento : MonoBehaviour
{
    public string tipoObjeto; // "LlaveInglesa", "Mecanismo" o "Generador"
    public SpriteRenderer spriteVisual;
    public GameObject contornoRojo; // Opcional: asigna aquí el contorno rojo si tiene

    private bool jugadorCerca = false;
    private Color colorOriginal;

    private void Awake()
    {
        if (spriteVisual != null)
        {
            colorOriginal = spriteVisual.color;
        }
    }

    private void Start()
    {
        if (tipoObjeto == "Mecanismo")
        {
            bool reparado = PlayerPrefs.GetInt("mecanismoReparado", 0) == 1;
            if (spriteVisual != null)
            {
                spriteVisual.color = reparado ? Color.green : colorOriginal;
            }
        }

        if (tipoObjeto == "Generador")
        {
            bool activo = PlayerPrefs.GetInt("generadorActivo", 0) == 1;
            if (spriteVisual != null)
            {
                spriteVisual.color = activo ? Color.green : colorOriginal;
            }
        }
    }

    private void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            NivelManager manager = FindAnyObjectByType<NivelManager>();

            if (tipoObjeto == "LlaveInglesa")
            {
                PlayerPrefs.SetInt("tieneLlaveInglesa", 1);
                PlayerPrefs.Save();
                Destroy(gameObject);
            }
            else if (tipoObjeto == "Mecanismo")
            {
                if (PlayerPrefs.GetInt("tieneLlaveInglesa", 0) == 1)
                {
                    manager.GuardarAvance("mecanismoReparado");
                    if (spriteVisual != null) spriteVisual.color = Color.green;
                    if (contornoRojo != null) contornoRojo.SetActive(false);
                    Debug.Log("Mecanismo reparado.");
                }
                else
                {
                    Debug.Log("Necesitas la llave inglesa para reparar este mecanismo.");
                }
            }
            else if (tipoObjeto == "Generador")
            {
                if (PlayerPrefs.GetInt("mecanismoReparado", 0) == 1)
                {
                    manager.GuardarAvance("generadorActivo");
                    if (spriteVisual != null) spriteVisual.color = Color.green;
                    if (contornoRojo != null) contornoRojo.SetActive(false);
                    Debug.Log("Generador encendido.");
                }
                else
                {
                    Debug.Log("Debes reparar el mecanismo antes de encender el generador.");
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }
}