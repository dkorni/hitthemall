using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField]
    private float _jumpVelocity = 5;

    [SerializeField]
    private float _timeToDestroy = 3.5f;

    [SerializeField] private Rigidbody _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        Game.Game.Instance.AddMoney(1);
        _rigidbody.AddForce(Vector3.up * _jumpVelocity, ForceMode.Impulse);   
        Destroy(gameObject, _timeToDestroy);
    }
}
