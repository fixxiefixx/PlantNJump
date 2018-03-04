using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraLimit : MonoBehaviour {

    public Collider2D CameraBoundsCollider;
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
        /*minX = horzExtent - mapX / 2f;
        maxX = mapX / 2f - horzExtent;
        minY = vertExtent - mapY / 2f;
        maxY = mapY / 2f - vertExtent;*/

        /*Tilemap map = FindObjectOfType<Tilemap>();
        BoundsInt bounds = map.cellBounds;

        minX = horzExtent + bounds.xMin;
        maxX = bounds.xMax - horzExtent;
        minY = vertExtent + bounds.yMin;
        maxY = bounds.yMax - vertExtent;*/

        Bounds bounds = CameraBoundsCollider.bounds;

        minX = horzExtent + bounds.min.x;
        maxX = bounds.max.x - horzExtent;
        minY = vertExtent + bounds.min.y;
        maxY = bounds.max.y - vertExtent;


    }

    void Update()
    {
        var v3 = transform.position;
        v3.x = Mathf.Clamp(v3.x, minX, maxX);
        v3.y = Mathf.Clamp(v3.y, minY, maxY);
        transform.position = v3;
    }
}
