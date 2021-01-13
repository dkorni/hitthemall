using System;
using UnityEngine;
using UnityEngine.AI;

namespace Character
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private Transform m_destination;
        [SerializeField] private NavMeshAgent m_navAgent;
        [SerializeField] private Animator m_animator;

        private bool m_isAlive;
        private static readonly int IsDeadAnim = Animator.StringToHash("IsDead");
        private static readonly int SpeedAnim = Animator.StringToHash("Speed");

        private void Start()
        {
            Activate();
        }

        private void Activate()
        {
            m_navAgent.enabled = true;
            m_navAgent.destination = m_destination.position;
            m_isAlive = true;
        }

        private void Update()
        {
            m_animator.SetFloat(SpeedAnim, m_navAgent.velocity.magnitude);
        }

        public void Kill()
        {
            m_navAgent.enabled = false;
            m_navAgent.velocity = Vector3.zero;
            m_isAlive = false;
            m_animator.SetBool(IsDeadAnim, !m_isAlive);
        }
    }
}