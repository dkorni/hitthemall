using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    public float Spring = 100;

    private SpringJoint[] joints;

    public void Start()
    {
        joints = GetComponentsInChildren<SpringJoint>().Where(j=>!j.connectedBody.CompareTag("Column")).ToArray();

        foreach (var joint in joints)
        {
            joint.spring = Spring;
        }
    }
}
