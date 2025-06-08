using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

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
        "Usa W, A, S, D para moverte o apuntar.",
        "Presiona ESPACIO para saltar.",
        "Haz clic izquierdo para atacar.",
        "Haz clic derecho para bloquear.",
        "Presiona ESC para pausar el juego."
    };

    private AnimationCurve loadingCurve;
    private List<string> tipsPool; // Consejos únicos para esta carga

    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        loadingPanel.SetActive(true);
        GenerateSmoothRandomCurve();

        // Crear nueva copia de consejos para esta carga
        tipsPool = new List<string>(consejos);

        StartCoroutine(LoadGameWithTips());
    }

    IEnumerator LoadGameWithTips()
    {
        float timer = 0f;
        float tipChangeInterval = 2f;
        float nextTipTime = 0f;
        float lastProgress = 0f;

        tipsText.text = GetNextUniqueTip();

        while (timer < fakeLoadingTime)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / fakeLoadingTime);
            float visualProgress = loadingCurve.Evaluate(t);

            // Evitar retroceso de la barra
            if (visualProgress > lastProgress)
            {
                loadingBarFill.fillAmount = visualProgress;
                lastProgress = visualProgress;
            }

            if (timer >= nextTipTime && tipsPool.Count > 0)
            {
                nextTipTime += tipChangeInterval;
                tipsText.text = GetNextUniqueTip();
            }

            yield return null;
        }

        SceneManager.LoadScene("Nivel1");
    }

    string GetNextUniqueTip()
    {
        if (tipsPool.Count == 0) return "";

        int index = Random.Range(0, tipsPool.Count);
        string selectedTip = tipsPool[index];
        tipsPool.RemoveAt(index); // Eliminar para no repetir

        return selectedTip;
    }

    void GenerateSmoothRandomCurve()
    {
        loadingCurve = new AnimationCurve();

        loadingCurve.AddKey(new Keyframe(0f, 0f, 0f, Random.Range(1f, 2f)));
        loadingCurve.AddKey(new Keyframe(Random.Range(0.2f, 0.4f), Random.Range(0.3f, 0.6f), Random.Range(0.5f, 1.5f), Random.Range(0.5f, 1.5f)));
        loadingCurve.AddKey(new Keyframe(Random.Range(0.6f, 0.8f), Random.Range(0.7f, 0.9f), Random.Range(0.5f, 1.5f), Random.Range(0.5f, 1.5f)));
        loadingCurve.AddKey(new Keyframe(1f, 1f, Random.Range(1f, 2f), 0f));
    }
}
