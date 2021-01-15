using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Build and update a localized navmesh from the sources marked by NavMeshSourceTag
[DefaultExecutionOrder(-102)]
public class LocalNavMeshBuilder : MonoBehaviour
{
    // The center of the build
    public Transform m_Tracked;

    // The size of the build bounds
    public Vector3 m_Size = new Vector3(80.0f, 20.0f, 80.0f);
    private NavMeshDataInstance m_Instance;

    private NavMeshData m_NavMesh;
    private AsyncOperation m_Operation;
    private List<NavMeshBuildSource> m_Sources = new List<NavMeshBuildSource>();

    private IEnumerator Start()
    {
        while (true)
        {
            UpdateNavMesh(true);
            yield return m_Operation;
        }
    }

    private void OnEnable()
    {
        // Construct and add navmesh
        m_NavMesh = new NavMeshData();
        m_Instance = NavMesh.AddNavMeshData(m_NavMesh);
        if (m_Tracked == null)
        {
            m_Tracked = transform;
        }

        UpdateNavMesh();
    }

    private void OnDisable()
    {
        // Unload navmesh and clear handle
        m_Instance.Remove();
    }

    private void OnDrawGizmosSelected()
    {
        if (m_NavMesh)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(m_NavMesh.sourceBounds.center, m_NavMesh.sourceBounds.size);
        }

        Gizmos.color = Color.yellow;
        Bounds bounds = QuantizedBounds();
        Gizmos.DrawWireCube(bounds.center, bounds.size);

        Gizmos.color = Color.green;
        Vector3 center = m_Tracked ? m_Tracked.position : transform.position;
        Gizmos.DrawWireCube(center, m_Size);
    }

    private void UpdateNavMesh(bool asyncUpdate = false)
    {
        NavMeshSourceTag.Collect(ref m_Sources);
        NavMeshBuildSettings defaultBuildSettings = NavMesh.GetSettingsByID(0);
        Bounds bounds = QuantizedBounds();

        if (asyncUpdate)
        {
            m_Operation = NavMeshBuilder.UpdateNavMeshDataAsync(m_NavMesh, defaultBuildSettings, m_Sources, bounds);
        }
        else
        {
            NavMeshBuilder.UpdateNavMeshData(m_NavMesh, defaultBuildSettings, m_Sources, bounds);
        }
    }

    private static Vector3 Quantize(Vector3 v, Vector3 quant)
    {
        float x = quant.x * Mathf.Floor(v.x / quant.x);
        float y = quant.y * Mathf.Floor(v.y / quant.y);
        float z = quant.z * Mathf.Floor(v.z / quant.z);
        return new Vector3(x, y, z);
    }

    private Bounds QuantizedBounds()
    {
        // Quantize the bounds to update only when theres a 10% change in size
        Vector3 center = m_Tracked ? m_Tracked.position : transform.position;
        return new Bounds(Quantize(center, 0.1f * m_Size), m_Size);
    }
}