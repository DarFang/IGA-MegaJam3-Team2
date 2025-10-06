using UnityEngine;
using System.Collections;
using UnityEngine.Events;
    public class ManaManager: MonoBehaviour 
    {
        public int NeutralMana;
        public Entity Player;
        public Entity Enemy;
        public int MaxMana = 100;
        public UnityEvent<int> OnMaxManaChanged;

        public void Initialize(Entity player, Entity enemy) {
            Player = player;
            Enemy = enemy;
            MaxMana = 100;
            NeutralMana = MaxMana - (int)player.Stats.Mana.CurrentValue - (int)enemy.Stats.Mana.CurrentValue;
            Debug.Log("NeutralMana at start: " + NeutralMana);
            OnMaxManaChanged?.Invoke(NeutralMana);
        }

        public void ConsumeMana(Entity entity, int amountConsumed) {
            NeutralMana += amountConsumed;
            OnMaxManaChanged?.Invoke(NeutralMana);
            if (NeutralMana > MaxMana) NeutralMana = MaxMana;
            if(entity is Player)
            {
                Player.ConsumeMana(-amountConsumed);
            }
            else if(entity is Enemy)
            {
                Enemy.ConsumeMana(-amountConsumed);
            }
        }
        public void GainMana(Entity entity) {
            float amountGained = (int)entity.amountManaGained();
            NeutralMana -= (int)amountGained;
            float extraManaRequired = 0;
            Entity currentEntity = null;
            Entity otherEntity = null;
            if (NeutralMana < 0)
            {
                extraManaRequired = -NeutralMana;
                NeutralMana = 0;
            } 
            OnMaxManaChanged?.Invoke(NeutralMana);
            if(entity is Player)
            {
                currentEntity = Player;
                otherEntity = Enemy;
            }
            else if(entity is Enemy)
            {
                currentEntity = Enemy;
                otherEntity = Player;
            }
            currentEntity.ApplyManaBuff(amountGained);
            Debug.Log(extraManaRequired + " extra mana required for " + otherEntity.name);
            if (extraManaRequired > 0)
            {
                otherEntity.ConsumeMana(extraManaRequired);
            }
        }
    }