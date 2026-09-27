using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MovimientoJugador : MonoBehaviour
{
    public float velocidad = 5f;

    [Header("Referencias de la Puerta y Victoria")]
    public SpriteRenderer spritePuerta;
    public GameObject textoVictoria;

    private SpriteRenderer srJugador;
    private Rigidbody2D rbJugador;
    private Animator animatorJugador;

    private bool tieneLlave = false;
    private bool nivelCompletado = false;

    void Start()
    {
        srJugador = GetComponent<SpriteRenderer>();
        rbJugador = GetComponent<Rigidbody2D>();
        animatorJugador = GetComponent<Animator>();

        if (spritePuerta != null)
        {
            spritePuerta.color = Color.white;
        }
    }

    void FixedUpdate()
    {
        if (nivelCompletado)
        {
            rbJugador.linearVelocity = Vector2.zero;
            if (animatorJugador != null)
            {
                animatorJugador.SetFloat("Velocidad", 0f);
            }
            return;
        }

        float movX = 0f;
        float movY = 0f;

        // Movimiento horizontal (flechas o A/D)
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            movX = velocidad;
            srJugador.flipX = false;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            movX = -velocidad;
            srJugador.flipX = true;
        }

        // Movimiento vertical (flechas o W/S)
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            movY = velocidad;
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            movY = -velocidad;
        }

        // Aplicar movimiento físico
        rbJugador.linearVelocity = new Vector2(movX, movY);

        // Control de animación con tu parámetro "Velocidad"
        if (animatorJugador != null)
        {
            // Calcula cuánto se mueve: si está quieto da 0, si se mueve da un valor mayor a 0
            float velocidadActual = Mathf.Abs(movX) + Mathf.Abs(movY);
            animatorJugador.SetFloat("Velocidad", velocidadActual);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Recoger la llave
        if (collision.gameObject.name == "Llave" || collision.CompareTag("Llave"))
        {
            tieneLlave = true;
            Destroy(collision.gameObject);

            if (spritePuerta != null)
            {
                spritePuerta.color = Color.green;
            }
        }

        // 2. Tocar la puerta con llave
        if ((collision.gameObject.name == "Puerta" || collision.CompareTag("Puerta")) && tieneLlave && !nivelCompletado)
        {
            StartCoroutine(VictoriaRutina());
        }
    }

    IEnumerator VictoriaRutina()
    {
        nivelCompletado = true;

        if (textoVictoria != null)
        {
            textoVictoria.SetActive(true);
        }

        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}