using System;
using Character;
using UnityEngine;

namespace Player
{
    public class EnemyDestroyer : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            EnemyController enemy = other.gameObject.GetComponentInParent<EnemyController>();
            if (enemy != null)
            {
                Debug.Log("Kill!");
                enemy.Kill();
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            EnemyController enemy = other.gameObject.GetComponentInParent<EnemyController>();
            if (enemy != null)
            {
                Debug.Log("Kill!");
                enemy.Kill();
            }
        }
    }
}