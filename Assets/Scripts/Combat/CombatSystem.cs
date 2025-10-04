using UnityEngine;
using UnityEngine.Events;

public class CombatSystem : MonoBehaviour
{
    public UnityEvent OnCombatStart;
    public UnityEvent OnCombatEnd;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    bool playerTurn = true;
    public CharacterCombat player;
    public CharacterCombat enemy;
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
        else
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

    public void EndCombat()
    {
        Debug.Log("Combat Ended");
        OnCombatEnd?.Invoke();
    }
    public void PlayerEndTurn()
    {
        playerTurn = false;
    }
    public void EnemyEndTurn()
    {
        playerTurn = true;
    }
    public void Attack(CharacterCombat character, int attack)
    {
        if (character == player)
        {
            enemy.TakeDamage(attack);
            PlayerEndTurn();
        }
        else
        {
            player.TakeDamage(attack);
            EnemyEndTurn();
        }
    }
    public void Defend(CharacterCombat character)
    {
        character.Defend();
        EndTurn(character);
    }
    public void Heal(CharacterCombat character)
    {
        character.Heal();
        EndTurn(character);
    }

    public void EndTurn(CharacterCombat character)
    {
        if (character == player)
        {
            EnemyEndTurn();
        }
        else
        {
            PlayerEndTurn();
        }
    }
}
