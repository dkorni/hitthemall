using System.Collections.Generic;
using Enemy;
using UnityEngine;

namespace Leveling
{
    public sealed class LevelContainer : MonoBehaviour
    {
        private static int _currentLevel;
        [SerializeField] private int m_debugStartLevel = 0;
        [SerializeField] private List<Level> m_progression;

        public EnemyContainer EnemyContainer => CurrentLevel.EnemyContainer;
        public Level CurrentLevel { get; private set; }

        private void Awake()
        {
            foreach (Level debugLevel in GetComponentsInChildren<Level>())
            {
                Destroy(debugLevel.gameObject);
            }
            
#if UNITY_EDITOR
            if (m_debugStartLevel > 0)
            {
                _currentLevel = m_debugStartLevel;
            }
#endif

            int levelIndex = _currentLevel;

            levelIndex %= m_progression.Count;
            if (levelIndex >= m_progression.Count)
            {
                _currentLevel = levelIndex = 0;
            }

            CurrentLevel = Instantiate(m_progression[levelIndex], transform);
            Debug.Log($"Level created {levelIndex}/{CurrentLevel.name}", CurrentLevel);
        }

        public void NextLevel()
        {
            _currentLevel += _currentLevel >= m_progression.Count - 1 ? -_currentLevel : 1;
        }
    }
}