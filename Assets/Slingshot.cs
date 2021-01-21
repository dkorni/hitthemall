using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [SerializeField]
    private float _spring = 100;

    [SerializeField]
    private float _dumper = 100;

    private HingeJoint[] joints;

    public void Start()
    {
        joints = GetComponentsInChildren<HingeJoint>();

        JointSpring sping = new JointSpring(){ damper = _dumper, spring = _spring};

        foreach (var joint in joints)
        {
            joint.useSpring = true;
            joint.spring = sping;
        }
    }
}
