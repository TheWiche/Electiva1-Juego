using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject loadingPanel;
    public GameObject mainMenuPanel;
    public Image loadingBarFill;
    public Text tipsText;

    [Header("Configuración")]
    public float fakeLoadingTime = 5f;

    [TextArea]
    public string[] consejos = {
        "Usa W, A, S, D para moverte.",
        "Presiona ESPACIO para saltar.",
        "Haz clic izquierdo para atacar.",
        "Haz clic derecho para bloquear.",
        "Presiona ESC para pausar el juego.",
        "Explora todos los rincones para encontrar secretos."
    };

    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        loadingPanel.SetActive(true);
        StartCoroutine(LoadGameWithTips());
    }

    IEnumerator LoadGameWithTips()
    {
        float timer = 0f;
        float tipChangeInterval = 2f;
        float nextTipTime = 0f;

        tipsText.text = consejos[Random.Range(0, consejos.Length)];

        while (timer < fakeLoadingTime)
        {
            timer += Time.deltaTime;
            loadingBarFill.fillAmount = timer / fakeLoadingTime;

            if (timer >= nextTipTime)
            {
                nextTipTime += tipChangeInterval;
                tipsText.text = consejos[Random.Range(0, consejos.Length)];
            }

            yield return null;
        }

        SceneManager.LoadScene("Nivel1");
    }
}
