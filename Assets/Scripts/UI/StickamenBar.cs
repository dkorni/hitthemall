using System.Collections;
using System.Collections.Generic;
using Enemy;
using Game;
using Leveling;
using UniRx;
using UnityEngine;

public class StickamenBar : MonoBehaviour
{
    [SerializeField] private GameObject[] _diedStickmen;
    [SerializeField] private GameObject[] _aliveStickmen;
    [SerializeField] private LevelContainer _levelContainer;
    private int _count;

    // Start is called before the first frame update
    void Start()
    {
        Game.Game.Instance.State.ObserveEveryValueChanged(v => v.Value).Subscribe(Draw);
    }

    private void Draw(GameState state)
    {
        if (state == GameState.RoundPrepare)
        {
            var container = _levelContainer.CurrentLevel.EnemyContainer;
            _count = container.GetEnemiesCount();
            container.OnStickmanDied += DisableStickman;
            foreach (var o in _diedStickmen)
            {
                o.SetActive(false);
            }
            foreach (var o in _aliveStickmen)
            {
                o.SetActive(false);
            }

            for (int i = 0; i < _count; i++)
            {
                _diedStickmen[i].SetActive(true);
                _aliveStickmen[i].SetActive(true);
            }
        }
    }

    private void DisableStickman()
    {
        _aliveStickmen[_count-1].SetActive(false);
        _count -= 1;
    }
}
