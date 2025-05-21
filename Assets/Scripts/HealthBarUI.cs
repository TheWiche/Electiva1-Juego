using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Image fillImage;

    public void SetHealth(float normalizedValue)
    {
        // Seguridad: evitar errores si fillImage no está asignado
        if (fillImage != null)
        {
            fillImage.fillAmount = Mathf.Clamp01(normalizedValue); // Nos aseguramos que siempre esté entre 0 y 1
        }
        else
        {
            Debug.LogWarning("fillImage no está asignado en HealthBarUI.");
        }
    }
}
