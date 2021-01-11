using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragGameobject : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbody;

    private Vector3 mouseOffset;

    private float mZCoord;

    private void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        mouseOffset = transform.position - GetMousePosition();

        rigidbody.isKinematic = true;
    }

    private Vector3 GetMousePosition()
    {
        // pixel coordinates
        Vector3 mousePoint = Input.mousePosition;

        mousePoint.z = mZCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseDrag()
    {
        rigidbody.position = GetMousePosition() + mouseOffset;
    }

    private void OnMouseUp()
    {
        rigidbody.isKinematic = false;
    }
}
