using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShotSettings", menuName = "Slingshot/ShotSettings", order = 1)]
public class ShotSettings : ScriptableObject
{
    public float MaxDistance = 10;

    [Range(0.05f, 1.0f)]
    public float PullSpeed = 0.1f;
}
