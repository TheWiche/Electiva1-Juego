using UnityEngine;

public class AmbientMusic : MonoBehaviour
{
    private static AmbientMusic instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject); // Evita duplicados al cambiar de escena
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Hace que este objeto persista entre escenas
    }
}
