using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeoController : MonoBehaviour
{ public int counter = 0;
    string playerName = "Hello World";

    // Start is called before the first frame update
    public void Start()
    { 
        Debug.Log("hasdasd");
    
    }

    // Update is called once per frame
    private void Update()
    { 
        Debug.Log(counter);
    counter++;

     if (Input.GetkeyDown(KeyCode.W));
    trasnform.position += new Vector(0, 1, 0);

    }
}