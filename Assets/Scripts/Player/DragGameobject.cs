using System.Collections;
using Player;
using UniRx;
using UnityEngine;

public class DragGameobject : MonoBehaviour
{
    /// <summary>
    /// Pulling returns value in percents. Max distance is 100%. 
    /// </summary>
    [HideInInspector] public ReactiveProperty<float> CurrentPulling;

    [SerializeField] private Rigidbody _slingshot;

    [SerializeField] private LayerMask layerMask;

    [SerializeField] private Rigidbody rigidbody;

    [SerializeField] private ShotSettings shotSettings;

    [SerializeField] private EnemyDestroyer _enemyDestroyer;

    [SerializeField] private float _offsetBtwObst = 0.3f;

    [SerializeField] private LineRenderer _aimLine;

    [SerializeField] private HingeJoint _hingeJoint;

    private Vector3 _startPosition;

    private Vector3 mouseOffset;

    private int _ignoreLayerMask;

    private float prevDist;

    private void Start()
    {
        _startPosition = rigidbody.position;
        _enemyDestroyer.CanDestroy = false;
        _ignoreLayerMask = LayerMask.GetMask("Obstacle");
    }

    #region Control

    private void OnMouseDown()
    {
        mouseOffset = transform.position - GetMousePosition();

        rigidbody.isKinematic = true;
        _aimLine.enabled = true;
    }

    private void OnMouseDrag()
    {
        var result = GetMousePosition() + mouseOffset;

        var distance = Vector3.Distance(result, Vector3.zero);

        var maxDistance = shotSettings.MaxDistance;

        // calculate distance
        if (distance < maxDistance)
        {
            rigidbody.position = result;
        }
        else
        {
            // clamp position of shot

            var correctPosition = CalculateNewPosition(result, maxDistance);
            rigidbody.position = correctPosition;
            distance = maxDistance;
            result = correctPosition;
        }

        var dirToResult = result - _slingshot.position;

        // check obstacles
        var obstacleRay = new Ray(_slingshot.position, dirToResult);
        if (Physics.SphereCast(obstacleRay, 1, out var hit, distance, _ignoreLayerMask))
        {
            // new correct position
            distance = Vector3.Distance(hit.point, _slingshot.position) - _offsetBtwObst;

            var correctPosition = CalculateNewPosition(result, distance);
            rigidbody.position = correctPosition;
            result = correctPosition;
        }

        // pulling calculate
        CalculatePulling(distance);

        // rotate slingshot to direction of shooting
        var forward = (result - _slingshot.transform.position).normalized;
        forward.x *= -1;
        forward.z *= -1;

        // it's temp sln, 10.71641f should calculated 
        // todo magic integers make as constant
        var aimDist = distance * 10.71641f / 16;

        _aimLine.SetPosition(0, result);
       _aimLine.SetPosition(1, new Vector3(forward.x * aimDist, transform.position.y,forward.z * aimDist));
       Debug.Log(distance);

        var dirRot = Quaternion.LookRotation(forward, Vector3.up).eulerAngles;
        dirRot.x = 0;
        dirRot.z = 0;

        _slingshot.rotation = Quaternion.Euler(dirRot);
    }

    private void OnMouseUp()
    {
        rigidbody.isKinematic = false;
        _enemyDestroyer.CanDestroy = true;
        var shootPosition = transform.TransformPoint(Vector3.zero);
        var shootDistance = Vector3.Distance(_slingshot.position, shootPosition);
        Debug.Log("Shoot!!!: " + shootDistance);
        StartCoroutine(FlyInShoot(shootPosition));
        CurrentPulling.Value = 0;
        _aimLine.enabled = false;
    }

    #endregion

    private Vector3 CalculateNewPosition(Vector3 targetPosition, float distance)
    {

        // find angle 
        var direction = (Vector3.zero - targetPosition).normalized;
        var angle = Mathf.Atan2(direction.z, direction.x);

        // new correct position
        var newCorrectX = Mathf.Cos(angle) * distance * -1;
        var newCorrectZ = Mathf.Sin(angle) * distance * -1;

        return new Vector3(newCorrectX, targetPosition.y, newCorrectZ);
    }

    private void CalculatePulling(float distance)
    {
        var pulling =  distance * 100 / shotSettings.MaxDistance;
        CurrentPulling.Value = pulling;
    }

    private Vector3 GetMousePosition()
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

    private IEnumerator FlyInShoot(Vector3 shootPosition)
    {
        yield return new WaitForFixedUpdate();

        var position = transform.TransformPoint(Vector3.zero);
        var startDirection = (position - shootPosition).normalized;
        var stopCalculatingPulling = false;
        while (true)
        {
            position = transform.TransformPoint(Vector3.zero);
            var direction = (position - shootPosition).normalized;

            // whether different completely opposite directions 
            if (Vector3.Dot(startDirection, direction)<0)
            {
                Debug.Log("Comes back at "+transform.position);
                _enemyDestroyer.CanDestroy = false;
                break;
            }

            shootPosition = position;

            yield return null;
        }

        // return shot to start state
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
                rigidbody.position = Vector3.Lerp(rigidbody.position, _startPosition, shotSettings.PullSpeed);
            }

            else
            {
                break;
            }

            yield return new WaitForFixedUpdate();
        }
    }
}