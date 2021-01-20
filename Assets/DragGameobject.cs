using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragGameobject : MonoBehaviour
{
    [SerializeField] private Rigidbody _slingshot;

    [SerializeField] private LayerMask layerMask;

    [SerializeField] private bool lockY;

    [SerializeField] private float maxDistance = 10;

    [SerializeField] private Rigidbody rigidbody;

    [SerializeField]
    [Range(0.05f, 1.0f)] 
    private float _pullSpeed = 0.1f;

    private Vector3 _startPosition;

    private Vector3 mouseOffset;

    private float mZCoord;

    private void Start()
    {
        _startPosition = rigidbody.position;
    }

    private void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        mouseOffset = transform.position - GetMousePosition();

        rigidbody.isKinematic = true;
    }

    private Vector3 GetMousePosition()
    {
        if (lockY)
        {
            Vector3 tempPos = transform.position;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                tempPos = hit.point;
            }
            return new Vector3(tempPos.x, transform.position.y, tempPos.z);
        }

        // pixel coordinates
        Vector3 mousePoint = Input.mousePosition;

        mousePoint.z = mZCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);

    }

    private void OnMouseDrag()
    {
        var result = GetMousePosition() + mouseOffset;

        var distance = Vector3.Distance(result, Vector3.zero);

       // calculate distance
        if (distance < maxDistance)
        {
            rigidbody.position = result;
        }
        else
        {

            // find angle 
            var direction = (Vector3.zero - result).normalized;
            var angle = Mathf.Atan2(direction.z, direction.x);

            // new correct position
            var newCorrectX = Mathf.Cos(angle) * maxDistance *-1;
            var newCorrectZ = Mathf.Sin(angle) * maxDistance *-1;

            var correctPosition = new Vector3(newCorrectX,result.y,newCorrectZ);
            rigidbody.position = correctPosition;
        }

        // rotate slingshot to direction of shooting
        var forward = result - _slingshot.transform.position;
        forward.x *= -1;
        forward.z *= -1;

        var dirRot = Quaternion.LookRotation(forward, Vector3.up).eulerAngles;
        dirRot.x = 0;
        dirRot.z = 0;

        _slingshot.rotation = Quaternion.Euler(dirRot);
    }

    private void OnMouseUp()
    {
        rigidbody.isKinematic = false;
        var shootPosition = transform.TransformPoint(Vector3.zero);
        var shootDistance = Vector3.Distance(_slingshot.position, shootPosition);
        Debug.Log("Shoot!!!: "+shootDistance);
        StopAllCoroutines();
        StartCoroutine(FlyInShoot(shootPosition));
    }

    // todo refactor
    private IEnumerator FlyInShoot(Vector3 shootPosition)
    {
        var previousDistance = 0f;

        yield return new WaitForFixedUpdate();

        var position = transform.TransformPoint(Vector3.zero);
        var startDirection = (position - shootPosition).normalized;

        Debug.Log(Math.Sign(startDirection.z));

        // wait until reached center
        while (true)
        {
            position = transform.TransformPoint(Vector3.zero);
            var direction = (position - shootPosition).normalized;

            // Debug.Log(Math.Sign(direction.z));

            // todo probably optimize
            if (Mathf.Sign(direction.z) != Mathf.Sign(startDirection.z))
            {
                Debug.Log("Comes back at "+transform.position);
                break;
            }

            shootPosition = position;
            Debug.Log(direction);

            yield return null;
        }

        yield return PullOnSling();
    }

    private IEnumerator PullOnSling()
    {
        rigidbody.isKinematic = true;
        while (true)
        {
            var shootDistance = Vector3.Distance(_startPosition,rigidbody.position);

            if (shootDistance > 0.1)
            {
                rigidbody.position = Vector3.Lerp(rigidbody.position, _startPosition, _pullSpeed);
            }

            else
            {
                break;
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
