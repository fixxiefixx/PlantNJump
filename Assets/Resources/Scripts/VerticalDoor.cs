using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalDoor : MonoBehaviour {
    public Transform OpenTrans;
    [Range(0.01f, 100)]
    public float OpeningTime = 0.5f;

    [Range(0.01f, 100)]
    public float ClosingTime = 0.2f;

    public bool OpenAtStart = false;

    private Vector3 startPos = Vector3.zero;
    private float openState = 0;
    private bool moving = false;
    private bool opening = false;

	// Use this for initialization
	void Start () {
        startPos = transform.position;
        if(OpenAtStart)
        {
            transform.position = OpenTrans.position;
            openState = 1;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (moving)
        {
            if (opening)
            {
                float speed = 1 / OpeningTime;
                openState += speed * Time.deltaTime;
                if (openState >= 1)
                {
                    openState = 1;
                    moving = false;
                }
            }
            else
            {
                float speed = 1 / ClosingTime;
                openState -= speed * Time.deltaTime;
                if(openState<=0)
                {
                    openState = 0;
                    moving = false;
                }
            }
            transform.position = Vector3.Lerp(startPos, OpenTrans.position, openState);
        }
	}

    private void Open()
    {
        moving = true;
        opening = true;
    }

    private void Close()
    {
        moving = true;
        opening = false;
    }

    void OnPlayerHang()
    {
        if (OpenAtStart)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    void OnPlayerUnhang()
    {
        if (OpenAtStart)
        {
            Open();
        }
        else
        {
            Close();
        }
    }
}
