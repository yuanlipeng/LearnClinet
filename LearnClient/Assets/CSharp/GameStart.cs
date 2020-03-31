using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ViewMgr.Instance.Init();

        ViewMgr.Instance.Open(ViewNames.Login);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
