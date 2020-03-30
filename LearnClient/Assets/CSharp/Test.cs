using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    public float Speed = 0.2f;
    public Vector3 DestPos;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 curPos = transform.position;
        Vector3 dir = DestPos - curPos;
        Vector3 diff = Vector3.Normalize(dir) * Speed;
        Vector3 destPos = curPos + diff;

        if (diff.sqrMagnitude >= dir.sqrMagnitude)
        {
            transform.position = DestPos;
        }
        else
        {
            transform.position = destPos;
        }
    }
}
