using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLimit : MonoBehaviour {

    public float mapX = 100f;
    public float mapY = 100f;

    private float minX;
     private float maxX;
     private float minY;
     private float maxY;
     
     void Start()
    {
        var vertExtent = Camera.main.GetComponent<Camera>().orthographicSize;
        var horzExtent = vertExtent * Screen.width / Screen.height;

        // Calculations assume map is position at the origin
        minX = horzExtent - mapX / 2f;
        maxX = mapX / 2f - horzExtent;
        minY = vertExtent - mapY / 2f;
        maxY = mapY / 2f - vertExtent;
    }

    void Update()
    {
        var v3 = transform.position;
        v3.x = Mathf.Clamp(v3.x, minX, maxX);
        v3.y = Mathf.Clamp(v3.y, minY, maxY);
        transform.position = v3;
    }
}
