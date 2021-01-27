using Leveling;
using UnityEngine;
using Zenject;

public class RateCounter : MonoBehaviour
{
    private int _maxEnemies;

    private const int _highShootRate = 5;
    private const int _middleShootRate = 7;
    private const int _lowShootRate = 9;

    [SerializeField] private LevelContainer _levelContainer;

    [Inject] private DragGameobject _dragGameobject;

    private int _killedCount;
    private int _shootCount;

    // Start is called before the first frame update
    void Start()
    {
        var container = _levelContainer.CurrentLevel.EnemyContainer;
        container.OnStickmanDied += () => _killedCount++;
        _dragGameobject.OnShoot += () => _shootCount++;
        _maxEnemies = container.GetEnemiesCount();
    }

    public int CalculateRate()
    {
        int result = 0;

        if (_killedCount >= _maxEnemies && _shootCount == _maxEnemies)
        {
            result = 3;
            return result;
        }

        _shootCount = _shootCount < _middleShootRate ? _shootCount + (_middleShootRate - _shootCount) : _shootCount;

        if (_killedCount >= _maxEnemies && _shootCount <= _middleShootRate)
        {
            result = 2;
            return result;
        }

        _shootCount = _shootCount < _lowShootRate ? _shootCount + (_lowShootRate - _shootCount) : _shootCount;

        if (_killedCount >= _maxEnemies && _shootCount >= _lowShootRate)
        {
            result = 1;
            return result;
        }

        return 0;
    }
}
