using Enemy;
using UnityEngine;

namespace Leveling
{
    public class Level : MonoBehaviour
    {
        [SerializeField] private Transform m_playerStart;
        [SerializeField] private EnemyContainer m_enemyContainer;

        public EnemyContainer EnemyContainer => m_enemyContainer;

        public void SetToPlayerStart(Transform obj)
        {
            obj.position = m_playerStart.position;
        }
    }
}