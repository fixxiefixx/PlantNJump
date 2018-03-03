﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    public float WalkSpeed = 3f;
    public Transform GroundCheckPosTransform;
    public float JumpForce = 10f;
    public LayerMask GroundCheckMask;
    public LayerMask GrabbableCheckMask;
    public Transform Object3DTransform;
    public float GroundCheckRadius = 0.2f;
    public Transform TwineStartPosTrans;
    public float TwineThrowTime = 0.5f;
    public float TwineThrowDistance = 5;
    public float TwineHangDistance = 3;
    public float TwineHangForce = 20;
    public float MaxTwineHangForce = 50;
    public bool EasyMode = false;

    internal Rigidbody2D rigid;
    internal Animator anim;
    internal LineRenderer twineLineRenderer;

    private bool facingRight = true;
    private float rotlerp = 0;
    private bool doRotate = true;

    internal bool jumpPressed = false;
    internal bool mousePressed = false;

    internal bool onGround = false;
    internal DXStateMachine machine = null;
    internal float horizontalMovement = 0;
    internal Transform twineHangTrans = null;


    //States
    internal PlayerJumpRunState jumpRunState;
    internal PlayerThrowTwineState throwTwineState;
    internal PlayerHangTwineState hangTwineState;

    private void initStates()
    {
        jumpRunState = new PlayerJumpRunState(gameObject);
        throwTwineState = new PlayerThrowTwineState(gameObject);
        hangTwineState = new PlayerHangTwineState(gameObject);
    }

	// Use this for initialization
	void Start () {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        twineLineRenderer = GetComponent<LineRenderer>();
        machine = GetComponent<DXStateMachine>();

        initStates();
        machine.State = jumpRunState;
	}
	
    private bool checkGrounded()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheckPosTransform.position, GroundCheckRadius, GroundCheckMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
                return true;
        }
        return false;
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }
        if (Input.GetButtonDown("Fire1"))
        {
            mousePressed = true;
        }

        machine.StateUpdate();
    }

    private void GlobalPreFixedUpdate()
    {
        horizontalMovement = Input.GetAxis("Horizontal") * WalkSpeed;
        onGround = checkGrounded();
        anim.SetBool("grounded", onGround);

        if (horizontalMovement > 0.1f)
        {
            facingRight = true;
        }
        if (horizontalMovement < -0.1f)
        {
            facingRight = false;
        }

        if (doRotate)
        {
            if (facingRight)
            {
                Object3DTransform.rotation = Quaternion.RotateTowards(Object3DTransform.rotation, Quaternion.identity, 500 * Time.deltaTime);
            }
            else
            {
                Object3DTransform.rotation = Quaternion.RotateTowards(Object3DTransform.rotation, Quaternion.Euler(0, 179.9f, 0), 500 * Time.deltaTime);
            }
        }
    }

    private void GlobalPostFixedUpdate()
    {
        jumpPressed = false;
        mousePressed = false;

        //Runter gefallen?
        if (rigid.position.y < -20)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void FixedUpdate()
    {
        GlobalPreFixedUpdate();
        machine.StateFixedUpdate();
        GlobalPostFixedUpdate();
    }
}
