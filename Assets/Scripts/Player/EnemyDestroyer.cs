using System;
using Enemy;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Player
{
    public class EnemyDestroyer : MonoBehaviour
    {
        public bool CanDestroy = false;

        [Inject] private EffectPool effectPool;
        [SerializeField] private float m_pushForceMult = 5;

        private void OnCollisionEnter(Collision other)
        {
            if(!CanDestroy)
                return;

            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                Debug.Log("Kill!");
                Vector3 vel = GetComponent<Rigidbody>().velocity;
                vel.y += 5;

                enemy.Kill(vel.normalized * m_pushForceMult);
                var hitEffect =  effectPool.Take();
                hitEffect.transform.position = enemy.m_pelvisRigid.position;
                hitEffect.GetComponent<ParticleSystem>().Play();
            }
        }
    }
}