using UnityEngine;

public class ZonaCaida : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Detecta al jugador (por componente MovimientoJugador o Tag "Player")
        if (other.GetComponent<MovimientoJugador>() != null || other.CompareTag("Player"))
        {
            NivelManager manager = FindAnyObjectByType<NivelManager>();
            if (manager != null)
            {
                manager.ReiniciarPorObstaculo();
            }
        }
    }
}
