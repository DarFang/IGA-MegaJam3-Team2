using UnityEngine;
using UnityEngine.Events;
using System.Collections;
public class CombatSystem : MonoBehaviour
{
    public UnityEvent OnCombatStart;
    public UnityEvent<bool> OnCombatEnd;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    bool playerTurn = true;
    public CharacterCombat player;
    public CharacterCombat enemy;
    bool isCombatStarted = false;
    public UnityEvent OnPlayerEndTurn;
    public UnityEvent OnEnemyEndTurn;
    void Start()
    {
        StartCombat();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTurn)
        {
            //Await player action
            return;
        }
        else if (!isCombatStarted)
        {
            enemy.RandomAction();
        }
    }
    /// <summary>
    /// Start of combat, initialize player and enemy with their stats
    /// TODO: Add a way to load the stats from this function
    /// </summary>
    public void StartCombat()
    {
        Debug.Log("Combat Started");
        player.LoadCombat(100,0, this);
        enemy.LoadCombat(100,0, this);
        OnCombatStart?.Invoke();
    }
    /// <summary>
    /// End of combat, check if the player won or lost
    /// </summary>
    /// <param name="isPlayerWin">If the player won</param>
    public void EndCombat(bool isPlayerWin)
    {
        Debug.Log("Combat Ended");
        OnCombatEnd?.Invoke(isPlayerWin);
        if (isPlayerWin)
        {
            Debug.Log("Player Won");
        }
        else
        {
            Debug.Log("Enemy Won");
        }
    }
    /// <summary>
    /// End of player turn, start the interval turn
    /// </summary>
    public void PlayerEndTurn()
    {
        playerTurn = false;
        StartCoroutine(IntervalTurn());
    }
    /// <summary>
    /// End of enemy turn, start the interval turn
    /// </summary>
    public void EnemyEndTurn()
    {
        playerTurn = true;
    }
    /// <summary>
    /// Attack the character
    /// </summary>
    /// <param name="character">The character to attack</param>
    /// <param name="attack">The attack value</param>
    public void Attack(CharacterCombat character, int attack)
    {
        if (character.IsPlayer)
        {
            enemy.TakeDamage(attack);
            if (enemy.isDead)
            {
                EndCombat(true);
            }
            PlayerEndTurn();
        }
        else
        {
            player.TakeDamage(attack);
            if (player.isDead)
            {
                EndCombat(false);
            }
            EnemyEndTurn();
        }
    }
    /// <summary>
    /// Defend the character
    /// </summary>
    /// <param name="character">The character to defend</param>
    public void Defend(CharacterCombat character)
    {
        character.Defend(1);
        EndTurn(character);
    }
    /// <summary>
    /// Heal the character
    /// </summary>
    /// <param name="character">The character to heal</param>
    public void Heal(CharacterCombat character)
    {
        character.Heal(10);
        EndTurn(character);
    }

    /// <summary>
    /// End of turn, check if the character is the player or the enemy
    /// </summary>
    /// <param name="character">The character to end the turn</param>
    public void EndTurn(CharacterCombat character)
    {
        if (!character.IsPlayer)
        {
            EnemyEndTurn();
        }
        else
        {
            PlayerEndTurn();
        }
    }
    private IEnumerator IntervalTurn()
    {
        isCombatStarted = true;
        OnPlayerEndTurn?.Invoke();
        yield return new WaitForSeconds(1f);
        isCombatStarted = false;
        OnEnemyEndTurn?.Invoke();
    }
}
