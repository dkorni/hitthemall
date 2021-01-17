using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragGameobject : MonoBehaviour
{
    [SerializeField] private float maxDistance = 10;

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
       var result= GetMousePosition() + mouseOffset;

       var distance = Vector3.Distance(result, Vector3.zero);

       // calculate distance
        if (distance < maxDistance)
        {
            rigidbody.position = result;
            Debug.Log("A");
        }
        else
        {

            // find angle 
            var direction = (Vector3.zero - result).normalized;
            var angle = Mathf.Atan2(direction.z, direction.x);

            Debug.Log(angle);

            // new correct position
            var newCorrectX = Mathf.Cos(angle) * maxDistance *-1;
            var newCorrectZ = Mathf.Sin(angle) * maxDistance *-1;

            var correctPosition = new Vector3(newCorrectX,result.y,newCorrectZ);
            rigidbody.position = correctPosition;
        }
    }

    private void OnMouseUp()
    {
        rigidbody.isKinematic = false;
    }
}
