using UnityEngine;

public abstract class Damageable : MonoBehaviour
{
    [SerializeField] private int startHealth;
    public int StartHealth => startHealth;
    private int health;
    public int Health => health;
    private float defensePercentage;
    public float DefensePercentage => defensePercentage;
    public virtual void OnDamaged(int damage)
    {
        health -= Mathf.RoundToInt(damage * (1.0f - defensePercentage));

        if(health < 0)
        {
            Death();
            return;
        }

        ResetDefenses();
    }

    public virtual void OnHealed(int healAmount)
    {
        health += healAmount;
        Mathf.Clamp(health, 0, startHealth);
    }

    public virtual void OnDefend(float defendPercentage)
    {
        defensePercentage = defendPercentage;
    }

    public virtual void ResetDefenses()
    {
        defensePercentage = 0;
    }

    public virtual void Death()
    {

    }
}
