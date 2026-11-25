using UnityEngine;

public class Enemigo : MonoBehaviour
{
    [Header("Configuracion de Patrulla")]
    public float velocidad = 2.5f;
    public float limiteIzquierdo = 12f;
    public float limiteDerecho = 22f;

    private int direccion = 1; // 1 = derecha, -1 = izquierda
    private SpriteRenderer srEnemigo;

    void Start()
    {
        srEnemigo = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Mueve al robot horizontalmente
        transform.Translate(Vector2.right * direccion * velocidad * Time.deltaTime);

        // Si llega al tope derecho, gira a la izquierda
        if (transform.position.x >= limiteDerecho)
        {
            direccion = -1;
            if (srEnemigo != null) srEnemigo.flipX = true;
        }
        // Si llega al tope izquierdo, gira a la derecha
        else if (transform.position.x <= limiteIzquierdo)
        {
            direccion = 1;
            if (srEnemigo != null) srEnemigo.flipX = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FindAnyObjectByType<NivelManager>().ReiniciarPorObstaculo();
        }
    }
}
