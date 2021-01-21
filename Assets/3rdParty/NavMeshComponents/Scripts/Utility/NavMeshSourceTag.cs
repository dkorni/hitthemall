using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Tagging component for use with the LocalNavMeshBuilder
// Supports mesh-filter and terrain - can be extended to physics and/or primitives
[DefaultExecutionOrder(-200)]
public class NavMeshSourceTag : MonoBehaviour
{
    // Global containers for all active mesh/terrain tags
    public static List<MeshFilter> m_Meshes = new List<MeshFilter>();

    private void OnEnable()
    {
        MeshFilter m = GetComponent<MeshFilter>();
        if (m != null)
        {
            m_Meshes.Add(m);
        }
    }

    private void OnDisable()
    {
        MeshFilter m = GetComponent<MeshFilter>();
        if (m != null)
        {
            m_Meshes.Remove(m);
        }
    }

    // Collect all the navmesh build sources for enabled objects tagged by this component
    public static void Collect(ref List<NavMeshBuildSource> sources)
    {
        sources.Clear();

        for (int i = 0; i < m_Meshes.Count; ++i)
        {
            MeshFilter mf = m_Meshes[i];
            if (mf == null)
            {
                continue;
            }

            Mesh m = mf.sharedMesh;
            if (m == null)
            {
                continue;
            }

            NavMeshBuildSource s = new NavMeshBuildSource();
            s.shape = NavMeshBuildSourceShape.Mesh;
            s.sourceObject = m;
            s.transform = mf.transform.localToWorldMatrix;
            s.area = 0;
            sources.Add(s);
        }
    }
}