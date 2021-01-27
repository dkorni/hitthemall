using System.Collections.Generic;
using Enemy;
using UnityEngine;

namespace Leveling
{
    public sealed class LevelContainer : MonoBehaviour
    {
        public static int CurrentLevelIndex { get; private set; }
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
                CurrentLevelIndex = m_debugStartLevel;
            }
#endif

            int levelIndex = CurrentLevelIndex;

            levelIndex %= m_progression.Count;
            if (levelIndex >= m_progression.Count)
            {
                CurrentLevelIndex = levelIndex = 0;
            }

            CurrentLevel = Instantiate(m_progression[levelIndex], transform);
            Debug.Log($"Level created {levelIndex}/{CurrentLevel.name}", CurrentLevel);
        }

        public void NextLevel()
        {
            CurrentLevelIndex += CurrentLevelIndex >= m_progression.Count - 1 ? -CurrentLevelIndex : 1;
        }
    }
}