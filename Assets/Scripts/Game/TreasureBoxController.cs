using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class TreasureBoxController : MonoBehaviour
{
    public event Action OnFinished;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject[] _coins;
    [SerializeField] private float _delayBeforePushCoins;
    [SerializeField] private float _delayBtwMonets;
    private Vector3 _startCoinPosition;

    public void Start()
    {
        _startCoinPosition = _coins[0].transform.position;
        Game.Game.Instance.State.
            ObserveEveryValueChanged(v => v.Value).
            Subscribe(s =>
            {
                if(s== GameState.Fail)
                    Open();
            });
    }

    private void Open()
    {
        _animator.SetBool("Open", true);
        StartCoroutine(PushMonets());
    }

    private void Close()
    {
        _animator.SetBool("Open", false);
        foreach (var _coin in _coins)
        {
            _coin.transform.position = _startCoinPosition;
        }
    }

    private IEnumerator PushMonets()
    {
        yield return new WaitForSeconds(_delayBeforePushCoins);

        var maxMonets = Random.Range(_coins.Length - 5, _coins.Length - 1);

        for (int i = 0; i < maxMonets; i++)
        {
            _coins[i].SetActive(true);
            yield return new WaitForSeconds(_delayBtwMonets);

            if (i == maxMonets)
            {
                OnFinished?.Invoke();
                Close();
            }
        }
    }
}
