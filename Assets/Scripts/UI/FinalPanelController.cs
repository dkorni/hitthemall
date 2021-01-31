using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalPanelController : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    protected virtual void Show()
    {
        _audioSource.Play();
    }
}