using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCollider : MonoBehaviour
{
    public Color gizmoColor;
    public enum ColliderType { BOX, SPHERE/*, MESH*/}

    public ColliderType colliderType;

    BoxCollider boxColl;
    SphereCollider sphereColl;
    //MeshCollider meshColl;

    void Start()
    {
        switch (colliderType)
        {
            case ColliderType.BOX:
                boxColl = GetComponent<BoxCollider>();
                break;
            case ColliderType.SPHERE:
                sphereColl = GetComponent<SphereCollider>();
                break;
            //case ColliderType.MESH:
            //    meshColl = GetComponent<MeshCollider>();
            //    break;
            default:
                Debug.LogWarning("ColliderType not rcognized");
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        switch (colliderType)
        {
            case ColliderType.BOX:
                Gizmos.DrawWireCube(boxColl.transform.position + boxColl.center, boxColl.size);
                break;
            case ColliderType.SPHERE:
                Gizmos.DrawWireSphere(sphereColl.transform.position + sphereColl.center, sphereColl.radius);
                break;
            //case ColliderType.MESH:
            //    Gizmos.DrawWireMesh();
            //    break;
            default:
                Debug.LogWarning("ColliderType not rcognized");
                break;
        }
    }
}
