using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game;
using UniRx;
using UnityEngine;

public class WinController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _delayToShow;

    // Start is called before the first frame update
    void Start()
    {
        Game.Game.Instance.State.ObserveEveryValueChanged(v => v.Value).
            Subscribe(s =>
            {
                if (s == GameState.Win)
                    DOVirtual.DelayedCall(_delayToShow, () => Show());
            });
    }

    private void Show()
    {
        _animator.SetTrigger("Fail");
    }

    public void Next()
    {
        Game.Game.Instance.Reload(true);
    }
}
