using UnityEngine;

public class PlayerHealth : HealthSystem
{
    public void TakeDamage(int amount)
    {
        base.TakeDamage((float)amount); 
        Debug.Log("Player received " + amount + " DAMAGE from Enemy. Remaining Health: " + currentHealth);
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount); 
        Debug.Log("Player received " + amount + " FLOAT DAMAGE. Remaining Health: " + currentHealth);
    }

    public override void Heal(float amount)
    {
        base.Heal(amount); 
        Debug.Log("Player healed " + amount + ". Current Health: " + currentHealth);
    }

    protected override void Die()
    {
        base.Die(); 
        Debug.Log("Player has DIED! Game Over.");
        gameObject.SetActive(false); 
    }

    void Update()
    {
        if (!IsAlive) return;

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