using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class MoneyTextController : MonoBehaviour
{

    [SerializeField] private Text _text;

    // Start is called before the first frame update
    void Start()
    {
        Game.Game.Instance.CoinsCount.ObserveEveryValueChanged(x=>x.Value).
            Subscribe(UpdateText);
    }

    private void UpdateText(int amount)
    {
        _text.text = $"{amount}";
    }
}
