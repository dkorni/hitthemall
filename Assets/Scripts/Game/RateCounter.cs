using System.Collections;
using System.Collections.Generic;
using Leveling;
using UnityEngine;
using Zenject;

public class RateCounter : MonoBehaviour
{
    [SerializeField] private Vector2 _highRate;
    [SerializeField] private Vector2 _middleRate;
    [SerializeField] private Vector2 _lowRate;

    [SerializeField] private LevelContainer _levelContainer;

    [Inject] private DragGameobject _dragGameobject;

    private int _killedCount;
    private int _shootCount;

    // Start is called before the first frame update
    void Start()
    {
        var container = _levelContainer.CurrentLevel.EnemyContainer;
        container.OnStickmanDied += ()=>_killedCount++;
        _dragGameobject.OnShoot += () => _shootCount++;
    }

    public int CalculateRate()
    {
        int result = 0;

        if (_killedCount >= _highRate.x && _shootCount >= _highRate.y)
            result = 3;

        if (_killedCount >= _middleRate.x && _shootCount >= _middleRate.y)
            result = 2;

        if (_killedCount >= _lowRate.x && _shootCount >= _lowRate.y)
            result = 1;

        Debug.Log($"Rating: {result}");

        return result;
    }
}
