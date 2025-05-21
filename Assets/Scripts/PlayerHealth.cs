using UnityEngine;

public class PlayerHealth : HealthSystem
{
    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10f);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Heal(5f);
        }
    }
}
