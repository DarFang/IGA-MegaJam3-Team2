using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    private int health;
    private int maxHealth;
    public bool isDead = false;
    public int defence = 0;
    public CombatSystem combatSystem;


    public void LoadCombat(int maxHealth, CombatSystem combatSystem)
    {
        Debug.Log(gameObject.name + " Loading Combat");
        this.maxHealth = maxHealth;
        health = maxHealth;
        isDead = false;
        this.combatSystem = combatSystem;
    }

    public void TakeDamage(int damage)
    {
        if (damage - defence <= 0)
        {
            damage = 1;
        }
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        Debug.Log(gameObject.name + " has taken " + damage + " damage");
        Debug.Log(gameObject.name + " has " + health + " health");
    }
    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
    public void Defend(int amount)
    {
        defence += amount;
    }

    public void Die()
    {
        isDead = true;
        Debug.Log(gameObject.name + " has died");
    }

    public void Attack()
    {
        Debug.Log(gameObject.name + " is attacking");
        combatSystem.Attack(this, 10);
    }
    public void Defend()
    {
        Debug.Log(gameObject.name + " is defending");
        combatSystem.Defend(this);
    }
    public void Heal()
    {
        Debug.Log(gameObject.name + " is healing");
        combatSystem.Heal(this);
    }
    public void GainResources()
    {
        //Later
    }

    public void RandomAction()
    {
        int randomAction = Random.Range(0, 3);
        switch (randomAction)
        {
            case 0:
                Attack();
                break;
            case 1:
                Defend();
                break;
            case 2:
                Heal();
                break;
        }
    }
}
