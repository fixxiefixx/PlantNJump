using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    public Transform targetpos;
    public float Speed = 2f;
    public bool TriggerOnPlayerHang = false;

    private Vector3 pos1;
    private Vector3 pos2;
    private Vector2 target;
    private bool back = false;
    private bool playerHanging = false;
    private bool moving = false;

	// Use this for initialization
	void Start () {
        pos1 = transform.position;
        pos2 = targetpos.transform.position;
        target = pos2;
        moving = !TriggerOnPlayerHang;
	}
	

	void Update () {

        if (!moving)
            return;

         transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * Speed);
        if ((new Vector2( transform.position.x,transform.position.y) - target).sqrMagnitude < 0.01f)
        {
            if (TriggerOnPlayerHang)
            {
                moving = false;
            }
            else
            {
                if (back)
                {
                    target = pos1;
                    back = false;
                }
                else
                {
                    target = pos2;
                    back = true;
                }
            }
        }
        
        
	}

    void OnPlayerHang()
    {
        if (TriggerOnPlayerHang)
        {
            playerHanging = true;
            target = pos2;
            moving = true;
        }
    }

    void OnPlayerUnhang()
    {
        if (TriggerOnPlayerHang)
        {
            playerHanging = false;
            target = pos1;
            moving = true;
        }
    }
}
