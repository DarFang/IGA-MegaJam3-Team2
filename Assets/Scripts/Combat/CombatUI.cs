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
    public void UpdatePlayerHealth(float health)
    {
        playerHealthBar.fillAmount = health;
    }
    public void UpdateEnemyHealth(float health)
    {
        enemyHealthBar.fillAmount = health;
    }
    public void UpdatePlayerDefence(int defence)
    {
        playerDefenceText.text = "Defence: " + defence.ToString();
    }
    public void UpdateEnemyDefence(int defence)
    {
        enemyDefenceText.text = "Defence: " + defence.ToString();
    }
}
