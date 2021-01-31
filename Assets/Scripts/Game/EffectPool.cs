using System.Collections;
using System.Collections.Generic;
using Game;
using Leveling;
using UniRx;
using UnityEngine;

public class EffectPool : MonoBehaviour
{
    public static EffectPool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EffectPool>();
            }

            return _instance;
        }
    }


    private static EffectPool _instance;

    [SerializeField] private GameObject _particlePrefab;
    [SerializeField] private LevelContainer _levelContainer;

    private Stack<GameObject> _effectObjects;

    // Start is called before the first frame update
    void Start()
    {
        Game.Game.Instance.State.
            ObserveEveryValueChanged(v => v.Value).Subscribe((v) =>
            {
                if(v == GameState.RoundPrepare)
                    Initialize();
            });
    }

    private void Initialize()
    {
        var container = _levelContainer.CurrentLevel.EnemyContainer;
        var count = container.GetEnemiesCount();
        _effectObjects = new Stack<GameObject>(count);

        for (int i = 0; i < count; i++)
        {
            var particle = Instantiate(_particlePrefab, transform.position, Quaternion.identity, transform);
            particle.SetActive(false);
            _effectObjects.Push(particle);
        }
    }

    public GameObject Take()
    {
        var particle = _effectObjects.Pop();
        particle.SetActive(true);
        return particle;
    }

    public void Put(GameObject particle)
    {
        _effectObjects.Push(particle);
        particle.SetActive(false);
    }
}