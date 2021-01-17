using System.Linq;
using Lean.Touch;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public float Spring = 100;

        private HingeJoint[] joints;

        public bool IsInputEnabled { get; private set; }

        [SerializeField] private LeanDragTranslate m_leanDrag;

        public void Start()
        {
            joints = GetComponentsInChildren<HingeJoint>();

            var spring = new JointSpring()
            {
                spring = Spring
            };


            foreach (var joint in joints)
            {
                joint.spring = spring;
            }
        }

        public void ToggleInput(bool enable)
        {
            m_leanDrag.enabled = enable;
            IsInputEnabled = enable;
        }
    }
}