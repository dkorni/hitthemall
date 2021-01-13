using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private Transform m_destination;
        [SerializeField] private NavMeshAgent m_navAgent;
        [SerializeField] private Animator m_animator;
        [SerializeField] private Rigidbody m_pelvisRigid;
        [SerializeField] private Collider m_collider;

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

        public void Kill(Vector3 force)
        {
            m_navAgent.enabled = false;
            m_navAgent.velocity = Vector3.zero;

            m_isAlive = false;

            m_animator.SetBool(IsDeadAnim, !m_isAlive);
            m_animator.enabled = false;

            m_pelvisRigid.AddForce(force, ForceMode.VelocityChange);
            m_collider.enabled = false;
        }
    }
}