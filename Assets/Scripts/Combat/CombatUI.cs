using UnityEngine;
using UnityEngine.UI;
public class CombatUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Image playerHealthBar;
    public Image enemyHealthBar;
    public void UpdatePlayerHealth(float health)
    {
        playerHealthBar.fillAmount = health;
    }
    public void UpdateEnemyHealth(float health)
    {
        enemyHealthBar.fillAmount = health;
    }
}
