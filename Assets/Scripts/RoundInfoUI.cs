using UnityEngine;
using UnityEngine.UI;

public class RoundInfoUI : MonoBehaviour
{
    public Text roundText;
    public Text enemiesText;

    public void UpdateRound(int current, int total)
    {
        roundText.text = $"Ronda: {current} / {total}";
    }

    public void UpdateEnemies(int remaining, int total)
    {
        enemiesText.text = $"Enemigos restantes: {remaining} / {total}";
    }
}
