using Leveling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class StartController : MonoBehaviour
{
    [Inject] private DragGameobject _dragGameobject;
    [SerializeField] private GameObject _prompt;

    // Start is called before the first frame update
    void Start()
    {
        if (LevelContainer.CurrentLevelIndex != 0)
        {
            gameObject.SetActive(false);
            return;
        }

        _dragGameobject.enabled = false;
        _prompt.SetActive(false);
    }

    public void StartGame()
    {
        _dragGameobject.enabled = true;
        _prompt.SetActive(true);
        gameObject.SetActive(false);
    }
}