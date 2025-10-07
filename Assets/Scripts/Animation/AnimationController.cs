using UnityEngine;

public abstract class AnimationController : MonoBehaviour
{
    protected abstract Animator Animator { get; set; }

    protected void SetTrigger(ActionAnimationType type)
    {
        if (Animator == null)
        {
            Debug.LogWarning("Animator is not set.", this);
            return;
        }

        Animator.SetTrigger(type.ToString());
    }

    protected void PlayActionPerformed(BattleAction action)
    {
        switch(action.ActionType)
        {
            case BattleActionType.Attack:
                SetTrigger(ActionAnimationType.Attack);
                break;
            case BattleActionType.Heal:
                SetTrigger(ActionAnimationType.Heal);
                break;
            case BattleActionType.Defense:
                SetTrigger(ActionAnimationType.Defend);
                break;
            case BattleActionType.WillGain:
                SetTrigger(ActionAnimationType.Mana);
                break;
        }
    }
    protected void PlayDamageTaken(float _) => SetTrigger(ActionAnimationType.GetHit);
    protected virtual void PlayDead(Entity _) => SetTrigger(ActionAnimationType.Dead);

    protected enum ActionAnimationType
    {
        Attack,
        Defend,
        Mana,
        Heal,
        GetHit,
        Dead
    }
}