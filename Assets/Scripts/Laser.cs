using UnityEngine;

public class Laser : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FindAnyObjectByType<NivelManager>().ReiniciarPorObstaculo();
        }
    }
}