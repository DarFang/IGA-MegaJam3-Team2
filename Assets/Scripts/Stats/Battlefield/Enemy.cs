using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
// ==================== ENEMY ====================
public class Enemy : Entity {
    [SerializeField] private float decisionDelay = 1f;

    public override async UniTask DoActionAsync(BattleContext context, CancellationToken cancellationToken = default) {
        if (IsDead) return;

        // Затримка для імітації "думання"
        await UniTask.Delay(TimeSpan.FromSeconds(decisionDelay));

        PerformRandomAction(context);
    }
}
