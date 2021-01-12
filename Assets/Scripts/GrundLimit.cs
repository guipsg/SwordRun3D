using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrundLimit : MonoBehaviour
{
    public MeshCollider col;
    public Transform player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = col.ClosestPointOnBounds(player.position);
        
    }
}
