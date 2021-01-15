using System.Linq;
using Lean.Touch;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public float Spring = 100;

        private SpringJoint[] joints;

        public bool IsInputEnabled { get; private set; }

        [SerializeField] private LeanDragTranslate m_leanDrag;

        public void Start()
        {
            joints = GetComponentsInChildren<SpringJoint>().Where(j => !j.connectedBody.CompareTag("Column")).ToArray();

            foreach (var joint in joints)
            {
                joint.spring = Spring;
            }
        }

        public void ToggleInput(bool enable)
        {
            m_leanDrag.enabled = enable;
            IsInputEnabled = enable;
        }
    }
}