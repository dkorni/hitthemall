using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ForceBarController : MonoBehaviour
{
    [Inject]
    private DragGameobject _dragGameobject;

    [SerializeField] private Image _forceBar;

    // Start is called before the first frame update
    void Start()
    {
        _dragGameobject.CurrentPulling.ObserveEveryValueChanged(v => v.Value).Subscribe(UpdateForceBar);
    }

    void UpdateForceBar(float pulling)
    {
        _forceBar.fillAmount = pulling / 100;
    }
}
