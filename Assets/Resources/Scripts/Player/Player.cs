using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    public float WalkSpeed = 3f;
    public Transform GroundCheckPosTransform;
    public float JumpForce = 10f;
    public LayerMask GroundCheckMask;
    public Transform Object3DTransform;

    private Rigidbody2D rigid;
    private Animator anim;

    private Quaternion flippedRot;
    private bool facingRight = true;
    private float rotlerp = 0;
    private bool doRotate = true;

    private bool jumpPressed = false;
    private bool onGround = false;

	// Use this for initialization
	void Start () {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        flippedRot = Quaternion.Euler(0, 180, 0);
	}
	
    private bool checkGrounded()
    {
        if (rigid.velocity.y > 0.1f)
            return false;

        Vector2 direction = new Vector2( GroundCheckPosTransform.position.x,GroundCheckPosTransform.position.y) - rigid.position;

        return Physics2D.Raycast(rigid.position, direction, direction.magnitude,GroundCheckMask);
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }
	}

    private void FixedUpdate()
    {
        float horizontalMovement = Input.GetAxis("Horizontal")*WalkSpeed;
        onGround = checkGrounded();
        anim.SetBool("grounded",onGround);

        if (horizontalMovement > 0.1f)
        {
            facingRight = true;
        }
        if(horizontalMovement < -0.1f)
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

        rigid.velocity =new Vector2(  horizontalMovement,rigid.velocity.y);
        anim.SetFloat("speed", Mathf.Abs(horizontalMovement));

        if (jumpPressed && onGround)
        {
            anim.Play("jump");
            rigid.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);
        }

        jumpPressed = false;

        //Runter gefallen?
        if (rigid.position.y < -20)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
