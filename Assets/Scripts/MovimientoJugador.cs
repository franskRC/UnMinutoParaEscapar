using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MovimientoJugador : MonoBehaviour
{
    public float velocidad = 5f;
    public float fuerzaSalto = 8f;
    private float escalaInicial;

    [Header("Referencias de la Puerta y Victoria")]
    public SpriteRenderer spritePuerta;
    public GameObject textoVictoria;

    private SpriteRenderer srJugador;
    private Rigidbody2D rbJugador;
    private Animator animatorJugador;

    private bool tieneLlave = false;
    private bool nivelCompletado = false;
    private bool enSuelo = false;
    private bool estaMuerto = false;
    private bool enEscalera = false;
    private bool escalando = false;
    private float gravedadInicial;

    void Start()
    {
        srJugador = GetComponent<SpriteRenderer>();
        rbJugador = GetComponent<Rigidbody2D>();
        animatorJugador = GetComponent<Animator>();
        gravedadInicial = rbJugador.gravityScale;

        // Guarda el tamaño que configuraste en el Inspector (ej. 1.2)
        escalaInicial = Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(-escalaInicial, escalaInicial, 1f);

        if (spritePuerta != null)
        {
            spritePuerta.color = new Color(0f, 1f, 0f, 0f);
        }
    }

    void Update()
    {
        if (nivelCompletado || estaMuerto) return;

        // Salto con Espacio, W o Flecha Arriba cuando pisa el suelo y no está en una escalera
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && enSuelo && !enEscalera)
        {
            rbJugador.linearVelocity = new Vector2(rbJugador.linearVelocity.x, fuerzaSalto);
            enSuelo = false;
        }
    }

    void FixedUpdate()
    {
        if (nivelCompletado || estaMuerto)
        {
            rbJugador.linearVelocity = new Vector2(0f, rbJugador.linearVelocity.y);
            if (animatorJugador != null && !estaMuerto)
            {
                animatorJugador.SetFloat("Velocidad", 0f);
                animatorJugador.SetBool("Saltar", !enSuelo);
            }
            return;
        }

        float movX = 0f;

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            movX = velocidad;
            transform.localScale = new Vector3(-escalaInicial, escalaInicial, 1f);
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            movX = -velocidad;
            transform.localScale = new Vector3(escalaInicial, escalaInicial, 1f);
        }

        // Control vertical para subir o bajar la escalera
        float movY = 0f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) movY = 1f;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) movY = -1f;

        if (enEscalera && Mathf.Abs(movY) > 0f)
        {
            escalando = true;
        }

        if (escalando)
        {
            rbJugador.gravityScale = 0f;
            rbJugador.linearVelocity = new Vector2(movX, movY * 2f);

            if (animatorJugador != null)
            {
                animatorJugador.SetBool("Subiendo", true);
                animatorJugador.SetBool("Saltar", false);
                animatorJugador.speed = Mathf.Abs(movY) > 0f ? 1f : 0f;
            }
            return;
        }

        // Movimiento normal en el suelo o aire
        rbJugador.linearVelocity = new Vector2(movX, rbJugador.linearVelocity.y);

        if (animatorJugador != null)
        {
            animatorJugador.SetFloat("Velocidad", Mathf.Abs(movX));
            animatorJugador.SetBool("Saltar", !enSuelo);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            enSuelo = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Recoger la llave o tarjeta de seguridad (solo si no es el Nivel 2)
        if (collision.gameObject.name == "Llave" || collision.CompareTag("Llave"))
        {
            tieneLlave = true;
            Destroy(collision.gameObject);

            if (spritePuerta != null)
            {
                spritePuerta.color = new Color(0f, 1f, 0f, 0f);
            }
        }

        // 2. Intentar cruzar la puerta
        if (collision.gameObject.name == "Puerta" || collision.CompareTag("Puerta"))
        {
            // Si estamos en el Nivel 2, requiere que el mecanismo y el generador estén listos en PlayerPrefs
            if (SceneManager.GetActiveScene().name == "Nivel2_Mantenimiento")
            {
                bool reparado = PlayerPrefs.GetInt("mecanismoReparado", 0) == 1;
                bool activo = PlayerPrefs.GetInt("generadorActivo", 0) == 1;

                if (reparado && activo && !nivelCompletado)
                {
                    nivelCompletado = true;
                    StartCoroutine(EntrarPuerta(collision.transform.position));
                    FindAnyObjectByType<NivelManager>().CompletarNivel();
                }
            }
            else
            {
                // Para Nivel 1 y Nivel 3 requiere la tarjeta o llave física recogible
                if (tieneLlave && !nivelCompletado)
                {
                    nivelCompletado = true;
                    StartCoroutine(EntrarPuerta(collision.transform.position));
                    FindAnyObjectByType<NivelManager>().CompletarNivel();
                }
            }
        }

        // 3. Detectar entrada a la escalera
        if (collision.CompareTag("Escalera"))
        {
            enEscalera = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Escalera"))
        {
            enEscalera = false;
            escalando = false;
            rbJugador.gravityScale = gravedadInicial;

            if (animatorJugador != null)
            {
                animatorJugador.SetBool("Subiendo", false);
                animatorJugador.speed = 1f;
            }
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

    IEnumerator EntrarPuerta(Vector3 posicionPuerta)
    {
        if (animatorJugador != null)
        {
            animatorJugador.SetBool("Saltar", false);
            animatorJugador.SetBool("Entrando", true);
        }

        // Cambia la puerta a negro al instante (simula el interior oscuro abierto)
        if (spritePuerta != null)
        {
            spritePuerta.color = Color.black;
        }

        rbJugador.linearVelocity = Vector2.zero;
        rbJugador.gravityScale = 0f;

        float duracion = 0.7f;
        float tiempo = 0f;
        float posYInicial = transform.position.y;
        Color colorInicial = srJugador.color;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / duracion;

            float nuevoX = Mathf.MoveTowards(transform.position.x, posicionPuerta.x, 2f * Time.deltaTime);
            transform.position = new Vector3(nuevoX, posYInicial, transform.position.z);

            // El jugador se desvanece entrando hacia la oscuridad de la puerta
            srJugador.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, Mathf.Lerp(1f, 0f, progreso));

            yield return null;
        }
    }

    public void Morir()
    {
        estaMuerto = true;
        escalando = false;
        rbJugador.gravityScale = gravedadInicial;
        rbJugador.linearVelocity = new Vector2(0f, Mathf.Min(0f, rbJugador.linearVelocity.y));

        CapsuleCollider2D col = GetComponent<CapsuleCollider2D>();
        if (col != null)
        {
            col.size = new Vector2(0.75f, 0.35f);
            col.offset = new Vector2(0f, -0.1f);
        }

        if (animatorJugador != null)
        {
            animatorJugador.speed = 1f;
            animatorJugador.SetBool("Subiendo", false);
            animatorJugador.SetBool("Saltar", false);
            animatorJugador.SetBool("Muerto", true);
        }
    }
}