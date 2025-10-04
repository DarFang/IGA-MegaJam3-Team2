using UnityEngine;
using UnityEngine.Events;
public class CharacterCombat : MonoBehaviour
{
    private int health;
    private int maxHealth;
    public bool isDead = false;
    public int defence = 0;
    public CombatSystem combatSystem;
    public UnityEvent<float> onHealthChange;
    public UnityEvent<int> onDefenceChange;
    [SerializeField] bool isPlayer = false;
    public bool IsPlayer => isPlayer;

    public void LoadCombat(int maxHealth, CombatSystem combatSystem)
    {
        Debug.Log(gameObject.name + " Loading Combat");
        this.maxHealth = maxHealth;
        health = maxHealth;
        isDead = false;
        this.combatSystem = combatSystem;
        onHealthChange.Invoke(GetHealthPercentage());
        onDefenceChange.Invoke(defence);
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
        onHealthChange.Invoke(GetHealthPercentage());
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
        onHealthChange.Invoke(GetHealthPercentage());
    }
    public void Defend(int amount)
    {
        defence += amount;  
        onDefenceChange.Invoke(defence);
    }

    public void Die()
    {
        isDead = true;
        Debug.Log(gameObject.name + " has died");
    }

    public void AttackAction()
    {
        Debug.Log(gameObject.name + " is attacking");
        combatSystem.Attack(this, 20);
    }
    public void DefendAction()
    {
        Debug.Log(gameObject.name + " is defending");
        combatSystem.Defend(this);
    }
    public void HealAction()
    {
        Debug.Log(gameObject.name + " is healing");
        combatSystem.Heal(this);
    }
    public void GainResourcesAction()
    {
        //Later
    }
    float GetHealthPercentage()
    {
        return (float)health / maxHealth;
    }

    public void RandomAction()
    {
        int randomAction = Random.Range(0, 3);
        switch (randomAction)
        {
            case 0:
                AttackAction();
                break;
            case 1:
                DefendAction();
                break;
            case 2:
                HealAction();
                break;
            default:
                Debug.Log("Invalid action");
                break;
        }
    }
}
