using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class ControlIntro : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private bool cargandoNivel = false;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += AlTerminarVideo;
        }
    }

    void Update()
    {
        if (cargandoNivel) return;

        // Cualquier tecla inicia Nivel1_Acceso (evita saltar en el primer milisegundo si venías de un clic)
        if (Input.anyKeyDown && Time.timeSinceLevelLoad > 0.25f)
        {
            IniciarNivel1();
        }
    }

    void AlTerminarVideo(VideoPlayer vp)
    {
        IniciarNivel1();
    }

    void IniciarNivel1()
    {
        if (cargandoNivel) return;
        cargandoNivel = true;
        SceneManager.LoadScene("Nivel1_Acceso");
    }
}