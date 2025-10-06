using System;
using UnityEngine;

public class BossAnimationController : AnimationController
{
    [Tooltip("The encounter assigned to this boss")]
    [SerializeField] private Encounter _encounter;

    /// <summary>
    /// The referenced enemy when combat starts
    /// </summary>
    private Enemy _enemyReference;

    protected override Animator Animator { get; set; }
    private void Awake()
    {
        Animator = GetComponent<Animator>();
        if (_encounter == null)
            Debug.LogError("Encounter not assigned in inspector", this);
    }

    #region Event Subscription Methods
    private void Start()
    {
        if(_encounter == null)
        {
            Debug.LogError("Encounter not assigned in inspector" , this);
            return;
        }

        _encounter.OnEnemyCreated += Encounter_OnEnemyCreated;
    }

    private void OnDisable()
    {
        if(_encounter != null)
            _encounter.OnEnemyCreated -= Encounter_OnEnemyCreated;

        if (_enemyReference != null)
            StopListeningToEnemyEvents();
    }
    private void StartListeningToEnemyEvents()
    {
        _enemyReference.OnActionPerformed += PlayActionPerformed;
        _enemyReference.OnDamageTaken += PlayDamageTaken;
        _enemyReference.OnDeath += PlayDead;
    }
    private void StopListeningToEnemyEvents()
    {
        _enemyReference.OnActionPerformed -= PlayActionPerformed;
        _enemyReference.OnDamageTaken -= PlayDamageTaken;
        _enemyReference.OnDeath -= PlayDead;
    }
    #endregion

    // Calling SetTrigger in base class to play the corresponding animation
    #region Event Handling Methods

    private void Encounter_OnEnemyCreated(Enemy enemy)
    {
        _enemyReference = enemy;
        StartListeningToEnemyEvents();
    }

    override protected void PlayDead(Entity entity)
    {
        base.PlayDead(entity);
        StopListeningToEnemyEvents();
    }

    #endregion
}
