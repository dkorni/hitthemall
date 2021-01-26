using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using UniRx;
using Zenject;

namespace Enemy
{
    public class EnemyContainer : MonoBehaviour
    {
        public event Action OnStickmanDied;

        private List<EnemyController> m_enemies;

        [HideInInspector] public ReactiveProperty<bool> IsAllEnemiesDestroyed = new ReactiveProperty<bool>(false);

        private void Start()
        {
            m_enemies = new List<EnemyController>(GetComponentsInChildren<EnemyController>());
            m_enemies.ForEach(e =>
            {
                e.IsAlive.Subscribe(RemoveDeadEnemy).AddTo(Game.Game.Instance.Disposable);
                e.IsAlive.Subscribe(b=>OnStickmanDied?.Invoke()).AddTo(Game.Game.Instance.Disposable);
            });
        }

        private void RemoveDeadEnemy(bool isAlive)
        {
            if (isAlive)
                return;

            IsAllEnemiesDestroyed.Value = m_enemies.TrueForAll(e => !e.IsAlive.Value);
        }

        public void ActivateAll()
        {
            m_enemies.ForEach(e => e.Activate());
        }

        public void DeactivateAll()
        {
            m_enemies.ForEach(e => e.Deactivate());
        }

        public int GetEnemiesCount()
        {
            return m_enemies.Count;
        }
    }
}