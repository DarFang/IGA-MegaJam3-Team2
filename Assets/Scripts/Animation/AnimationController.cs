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