using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField, Min(1)] private int maxHealth = 3;

    public int CurrentHealth { get; private set; }

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage()
    {
        if (CurrentHealth <= 0)
            return;

        CurrentHealth--;
        Debug.Log($"{name} took damage. Health: {CurrentHealth}/{maxHealth}", this);

        if (CurrentHealth == 0)
            gameObject.SetActive(false);
    }
}
