using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Zenject;

public class Prompt : MonoBehaviour
{
    [Inject] private DragGameobject _dragGameobject;

    // Start is called before the first frame update
    void Start()
    {
        _dragGameobject.OnStartShoot += () =>
        {
            gameObject.SetActive(false);

            // start game
            Game.Game.Instance.State.Value = GameState.Round;
        };
    }
}
