﻿using System;
using Player;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Zenject;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        private Transform m_destination;
        [SerializeField] private NavMeshAgent m_navAgent;
        [SerializeField] private Animator m_animator;
        [SerializeField] private Rigidbody m_pelvisRigid;
        [SerializeField] private Collider m_collider;

        [HideInInspector] public ReactiveProperty<bool> IsAlive = new ReactiveProperty<bool>(true);

        private static readonly int IsDeadAnim = Animator.StringToHash("IsDead");
        private static readonly int SpeedAnim = Animator.StringToHash("Speed");

        private void Start()
        {
            m_destination = LockerController.Instance.transform;
        }

        public void Activate()
        {
            m_navAgent.enabled = true;
            m_navAgent.destination = m_destination.position;
        }

        public void Deactivate()
        {
            m_navAgent.enabled = false;
            m_navAgent.velocity = Vector3.zero;

            m_collider.enabled = false;
        }

        private void Update()
        {
            m_animator.SetFloat(SpeedAnim, m_navAgent.velocity.magnitude);
        }

        public void Kill(Vector3 force)
        {
            Deactivate();
            IsAlive.Value = false;

            m_animator.SetBool(IsDeadAnim, !IsAlive.Value);
            m_animator.enabled = false;

            m_pelvisRigid.AddForce(force, ForceMode.VelocityChange);
        }
    }
}