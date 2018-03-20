using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    public Transform targetpos;
    public float Speed = 2f;
    public bool TriggerOnPlayerHang = false;
    public bool AlwaysMoving = true;

    private Vector3 pos1;
    private Vector3 pos2;
    private Vector2 target;
    private bool back = false;
    private bool moving = false;

	// Use this for initialization
	void Start () {
        pos1 = transform.position;
        pos2 = targetpos.transform.position;
        target = pos2;
        moving = AlwaysMoving;
	}
	

	void Update () {

        if (!moving)
            return;

         transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * Speed);
        if ((new Vector2( transform.position.x,transform.position.y) - target).sqrMagnitude < 0.01f)
        {
            if (!AlwaysMoving)
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
            OnTrigger();
        }
    }

    void OnPlayerUnhang()
    {
        if (TriggerOnPlayerHang)
        {
            OnUnTrigger();
        }
    }

    private void OnTrigger()
    {
        target = pos2;
        moving = true;
    }

    private void OnUnTrigger()
    {
        target = pos1;
        moving = true;
    }
}
