using UnityEngine;
using UnityEngine.AI;

public class NavMeshArea : MonoBehaviour
{
    public string nom = "Suelo";

    public GameObject agent;
    
   void Start()
    {
        float area = CalcularAreaDentro();

        float area_agente = CalcularAreaAgente();

        int total = Mathf.RoundToInt(area / area_agente);

        Debug.Log("Àrea dins: " + area + ". Àrea agent: " + area_agente + ". Màxim d'agents: " + total + ".");
    }

    float CalcularAreaDentro()
    {
        int area_index = NavMesh.GetAreaFromName("Suelo");
        NavMeshTriangulation navMeshinfo = NavMesh.CalculateTriangulation();
        Vector3[] vertex = navMeshinfo.vertices;
        int[] index = navMeshinfo.indices;

        float area = 0f;

        for (int i = 0; i < index.Length; i += 3)
        {
            Vector3 A = vertex[index[i]];
            Vector3 B = vertex[index[i+1]];
            Vector3 C = vertex[index[i+2]];

            if (navMeshinfo.areas[i / 3] == area_index)
            {
                Vector3 V = Vector3.Cross(A - B, A - C);

                area += V.magnitude * 0.5f;
            }

        }

        return area;
    }

    float CalcularAreaAgente()
    {
        BoxCollider boxCollider = agent.GetComponent<BoxCollider>();
        float area = boxCollider.size.x * boxCollider.size.z;
        return area;
    }

}
