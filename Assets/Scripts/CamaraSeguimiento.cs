using UnityEngine;

public class CamaraSeguimiento : MonoBehaviour
{
    public Transform objetivo; // Arrastra aqui a Jugador
    public float limiteIzquierdo = 0f;
    public float limiteDerecho = 17.8f; // Ajusta segun el ancho de tu mapa

    void LateUpdate()
    {
        if (objetivo != null)
        {
            float posX = Mathf.Clamp(objetivo.position.x, limiteIzquierdo, limiteDerecho);
            transform.position = new Vector3(posX, transform.position.y, transform.position.z);
        }
    }
}