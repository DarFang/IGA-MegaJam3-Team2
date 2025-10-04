using UnityEngine;
using UnityEngine.Events;
using System.Collections;
public class CombatSystem : MonoBehaviour
{
    public UnityEvent OnCombatStart;
    public UnityEvent OnCombatEnd;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    bool playerTurn = true;
    public CharacterCombat player;
    public CharacterCombat enemy;
    bool isCombatStarted = false;
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

    public void StartCombat()
    {
        Debug.Log("Combat Started");
        player.LoadCombat(100, this);
        enemy.LoadCombat(100, this);
        OnCombatStart?.Invoke();
    }

    public void EndCombat(bool isPlayerWin)
    {
        Debug.Log("Combat Ended");
        OnCombatEnd?.Invoke();
        if (isPlayerWin)
        {
            Debug.Log("Player Won");
        }
        else
        {
            Debug.Log("Enemy Won");
        }
    }
    public void PlayerEndTurn()
    {
        playerTurn = false;
        StartCoroutine(IntervalTurn());
    }
    public void EnemyEndTurn()
    {
        playerTurn = true;
    }
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
    public void Defend(CharacterCombat character)
    {
        character.Defend(5);
        EndTurn(character);
    }
    public void Heal(CharacterCombat character)
    {
        character.Heal(10);
        EndTurn(character);
    }

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
        yield return new WaitForSeconds(1f);
        isCombatStarted = false;
    }
}
