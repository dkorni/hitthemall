using System;
using Enemy;
using UniRx;
using UnityEngine;

namespace Player
{
    public class LockerController : MonoBehaviour
    {
        public static LockerController Instance;

        [HideInInspector] public ReactiveProperty<bool> IsLockerSafe = new ReactiveProperty<bool>(true);

        private void Awake()
        {
            Instance = this;
        }

        private void OnCollisionEnter(Collision other)
        {
            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                IsLockerSafe.Value = false;
            }
        }
    }
}