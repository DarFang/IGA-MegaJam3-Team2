using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CombatUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Image playerHealthBar;
    public Image enemyHealthBar;
    public TextMeshProUGUI playerDefenceText;
    public TextMeshProUGUI enemyDefenceText;
    public CombatSystem combatSystem;

    void Awake() 
    {
       combatSystem.player.onHealthChange.AddListener(UpdatePlayerHealth);
       combatSystem.enemy.onHealthChange.AddListener(UpdateEnemyHealth);
       combatSystem.player.onDefenceChange.AddListener(UpdatePlayerDefence);
       combatSystem.enemy.onDefenceChange.AddListener(UpdateEnemyDefence);
    }
    /// <summary>
    /// Update the player health bar
    /// </summary>
    /// <param name="health">The health of the player</param>
    public void UpdatePlayerHealth(float health)
    {
        playerHealthBar.fillAmount = health;
    }   
    /// <summary>
    /// Update the enemy health bar
    /// </summary>
    /// <param name="health">The health of the enemy</param>
    public void UpdateEnemyHealth(float health)
    {
        enemyHealthBar.fillAmount = health;
    }
    /// <summary>
    /// Update the player defence text
    /// </summary>
    /// <param name="defence">The defence of the player</param>
    public void UpdatePlayerDefence(int defence)
    {
        playerDefenceText.text = "Defence: " + defence.ToString();
    }
    /// <summary>
    /// Update the enemy defence text
    /// </summary>
    /// <param name="defence">The defence of the enemy</param>
    public void UpdateEnemyDefence(int defence)
    {
        enemyDefenceText.text = "Defence: " + defence.ToString();
    }
}
