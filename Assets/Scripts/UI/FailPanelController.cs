using System.Collections;
using System.Collections.Generic;
using Game;
using UniRx;
using UnityEngine;

public class FailPanelController : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        Game.Game.Instance.State.ObserveEveryValueChanged(v => v.Value).
            Subscribe(s =>
            {
                if(s == GameState.FailMenu)
                    Show();
            });
    }

    private void Show()
    {
        _animator.SetTrigger("Fail");
    }

    public void Retry()
    {
        Game.Game.Instance.Reload(false);
    }
}
