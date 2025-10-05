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
    public SoundList soundList;
    /// <summary>
    /// Load the combat stats for the character
    /// </summary>
    /// <param name="maxHealth">The maximum health of the character</param>
    /// <param name="combatSystem">The combat system</param>
    /// <param name="defence">The defence of the character</param>
    public void LoadCombat(int maxHealth, int defence, CombatSystem combatSystem)
    {
        Debug.Log(gameObject.name + " Loading Combat");
        this.maxHealth = maxHealth;
        health = maxHealth;
        isDead = false;
        this.defence = defence;
        this.combatSystem = combatSystem;
        onHealthChange.Invoke(GetHealthPercentage());
        onDefenceChange.Invoke(defence);
    }

    /// <summary>
    /// Take damage from the character
    /// </summary>
    /// <param name="damage">The damage to take</param>
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
        SoundManager.Instance.CreateSound().AutoDuckMusic().Play(soundList.GetSound("TakeDamage"));
        Debug.Log(gameObject.name + " has taken " + damage + " damage");
        Debug.Log(gameObject.name + " has " + health + " health");
    }
    /// <summary>
    /// Heal the character
    /// </summary>
    /// <param name="amount">The amount to heal</param>
    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        onHealthChange.Invoke(GetHealthPercentage());
    }
    /// <summary>
    /// Defend the character
    /// </summary>
    /// <param name="amount">The amount to defend</param>
    public void Defend(int amount)
    {
        defence += amount;  
        onDefenceChange.Invoke(defence);
    }

    /// <summary>
    /// Kill the character
    /// </summary>
    public void Die()
    {
        isDead = true;
        Debug.Log(gameObject.name + " has died");
    }

    /// <summary>
    /// Attack the character, this is called by Unity Inspector
    /// </summary>
    public void AttackAction()
    {
        Debug.Log(gameObject.name + " is attacking");
        combatSystem.Attack(this, 20);
        SoundManager.Instance.CreateSound().AutoDuckMusic().Play(soundList.GetSound("Attack"));
    }
    /// <summary>
    /// Defend the character, this is called by Unity Inspector
    /// </summary>
    public void DefendAction()
    {
        Debug.Log(gameObject.name + " is defending");
        combatSystem.Defend(this);
        SoundManager.Instance.CreateSound().AutoDuckMusic().Play(soundList.GetSound("Defend"));
    }
    /// <summary>
    /// Heal the character, this is called by Unity Inspector
    /// </summary>
    public void HealAction()
    {
        Debug.Log(gameObject.name + " is healing");
        combatSystem.Heal(this);
        SoundManager.Instance.CreateSound().AutoDuckMusic().Play(soundList.GetSound("Heal"));
    }
    /// <summary>
    /// Gain resources the character, this is called by Unity Inspector
    /// </summary>
    public void GainResourcesAction()
    {
        //Later
    }
    /// <summary>
    /// Get the health percentage of the character
    /// </summary>
    /// <returns>The health percentage of the character</returns>
    float GetHealthPercentage()
    {
        return (float)health / maxHealth;
    }

    /// <summary>
    /// Random action the character, this is called by Unity Inspector
    /// </summary>
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
