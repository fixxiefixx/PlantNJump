using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    public float MaxWalkSpeed = 3f;
    public float GroundAcceleration = 35f;
    public float AirAcceleration = 5;
    public Transform GroundCheckPosTransform;
    public float JumpForce = 10f;
    public LayerMask DamageLayermask;
    public LayerMask GroundCheckMask;
    public LayerMask GrabbableCheckMask;
    public Transform Object3DTransform;
    public float GroundCheckRadius = 0.2f;
    public Transform TwineStartPosTrans;
    public float TwineThrowTime = 0.5f;
    public float TwineMaxDistance = 5;
    public float TwineMinDistance = 3;
    public float TwineHangForce = 20;
    public float MaxTwineHangForce = 50;
    public bool EasyMode = false;
    public float TwineClimbSpeed = 1f;
    public GameObject PlantExplosionPrefab;
    public GameObject VisibleObject;

    internal Rigidbody2D rigid;
    internal Animator anim;
    internal LineRenderer twineLineRenderer;

    private bool facingRight = true;
    private float rotlerp = 0;
    private bool doRotate = true;

    internal bool jumpPressed = false;
    internal bool mousePressed = false;

    internal bool onGround = false;
    internal Transform groundTransform = null;
    internal DXStateMachine machine = null;
    internal float horizontalMovement = 0;
    internal Transform twineHangTrans = null;


    // Moving platform support
    private Vector3 activeLocalPlatformPoint;
    private Vector3 activeGlobalPlatformPoint;
    private Vector3 lastPlatformVelocity;

    //States
    internal PlayerJumpRunState jumpRunState;
    internal PlayerThrowTwineState throwTwineState;
    internal PlayerHangTwineState hangTwineState;
    internal PlayerExplosionState explosionState;

    private void initStates()
    {
        jumpRunState = new PlayerJumpRunState(gameObject);
        throwTwineState = new PlayerThrowTwineState(gameObject);
        hangTwineState = new PlayerHangTwineState(gameObject);
        explosionState = new PlayerExplosionState(gameObject);
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
            {
                groundTransform= colliders[i].transform;
                return true;
            }
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

    private void preMovingPlatformSupport()
    {
        if (groundTransform != null)
        {
            Vector3 newGlobalPlatformPoint = groundTransform.TransformPoint(activeLocalPlatformPoint);
            Vector3 moveDistance = (newGlobalPlatformPoint - activeGlobalPlatformPoint);

            if (moveDistance != Vector3.zero)
                transform.position = transform.position + moveDistance;
            lastPlatformVelocity = (newGlobalPlatformPoint - activeGlobalPlatformPoint) / Time.deltaTime;
        }
        else
        {
            lastPlatformVelocity = Vector3.zero;
        }
        groundTransform = null;
    }

    private void postMovingPlatformSupport()
    {
        if (groundTransform != null)
        {
            activeGlobalPlatformPoint = transform.position;
            activeLocalPlatformPoint = groundTransform.InverseTransformPoint(transform.position);



        }
        else
        {
            lastPlatformVelocity = Vector3.zero;
        }
    }

    private void GlobalPreFixedUpdate()
    {
        if (machine.State == explosionState)
            return;

        preMovingPlatformSupport();
        horizontalMovement = Input.GetAxis("Horizontal") * MaxWalkSpeed;
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
        if (machine.State == explosionState)
            return;

        jumpPressed = false;
        mousePressed = false;

        //Runter gefallen?
        if (rigid.position.y < -20)
        {
            Camera cam = Camera.allCameras[0];
            transform.position = new Vector3(transform.position.x, cam.transform.position.y - cam.orthographicSize,transform.position.z-1.5f);
            //rigid.position =new Vector2(rigid.position.x, cam.transform.position.y - cam.orthographicSize);
            machine.State = explosionState;

        }

        postMovingPlatformSupport();
    }

    private void FixedUpdate()
    {
        GlobalPreFixedUpdate();
        machine.StateFixedUpdate();
        GlobalPostFixedUpdate();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (machine.State == explosionState)
            return;

        if((1<<collision.gameObject.layer & DamageLayermask.value) != 0)
        {
            machine.State = explosionState;
        }
    }
}
