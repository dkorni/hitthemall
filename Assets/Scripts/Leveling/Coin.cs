using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField]
    private int _amount = 1;

    [SerializeField]
    private float _jumpVelocity = 5;

    [SerializeField]
    private float _timeToDestroy = 3.5f;

    [SerializeField] private Rigidbody _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        Game.Game.Instance.AddMoney(_amount);
        _rigidbody.AddForce(Vector3.up * _jumpVelocity, ForceMode.Impulse);
        DOVirtual.DelayedCall(_timeToDestroy, () => gameObject.SetActive(false));
    }
}
