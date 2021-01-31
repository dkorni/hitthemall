using System.Collections;
using System.Collections.Generic;
using Game;
using UniRx;
using UnityEngine;

public class Effect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Game.Game.Instance.State.ObserveEveryValueChanged(v => v.Value).Subscribe(v =>
        {
            if (v == GameState.RoundPrepare)
            {
                EffectPool.Instance.Put(gameObject);
            }
        });
    }
}
