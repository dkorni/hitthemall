using System.Collections;
using System.Collections.Generic;
using Game;
using UniRx;
using UnityEngine;
using Zenject;

public class RatePanelController : MonoBehaviour
{
    [Inject] private RateCounter _rateCounter;

    [SerializeField] private GameObject[] _stars;

    // Start is called before the first frame update
    void Start()
    {
        Game.Game.Instance.State.ObserveEveryValueChanged(v => v.Value).
            Subscribe(s =>
            {
                if (s == GameState.Win)
                    Draw();
            });
    }

    void Draw()
    {
        var rating = _rateCounter.CalculateRate();

        for (int i = 0; i < rating; i++)
        {
            _stars[i].SetActive(true);
        }
    }
}
