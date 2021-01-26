using System.Collections;
using System.Collections.Generic;
using Game;
using UniRx;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] private GameObject[] _fires;

    // Start is called before the first frame update
    void Start()
    {
        Game.Game.Instance.State.ObserveEveryValueChanged(v => v.Value).Subscribe(
            s =>
            {
                if (s == GameState.Win)
                    Activate();
            });
    }

    private void Activate()
    {
        foreach (var fire in _fires)
        {
            fire.SetActive(true);
        }
    }
}
