using UnityEngine;
using System.Collections;

public class PlayerThrowTwineState : PlayerState
{
    private const float TWINE_ROTATION_SPEED = 6f;

    private float direction = 0;
    private float throwTimer = 0;
    private Camera cam;

    public PlayerThrowTwineState(GameObject go) : base(go, "throwtwine")
    {
        cam = GameObject.FindObjectOfType<Camera>();
    }

    private Transform FindNearestGrabbable()
    {
        Vector3 pos= cam.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] colliders= Physics2D.OverlapCircleAll(pos, player.TwineThrowDistance,player.GrabbableCheckMask);
        if (colliders.Length == 0)
            return null;
        Transform nearest = colliders[0].transform;
        float nearestDist = (nearest.position - pos).sqrMagnitude;
        for(int i = 1; i < colliders.Length; i++)
        {
            Transform testTrans = colliders[i].transform;
            float dist= (testTrans.position - pos).sqrMagnitude;
            if (dist < nearestDist)
            {
                nearest = testTrans;
                nearestDist = dist;
            }
        }
        return nearest;
    }

    public override void EnterState()
    {
        for(int i = 0; i < player.twineLineRenderer.positionCount; i++)
        {
            player.twineLineRenderer.SetPosition(i, player.TwineStartPosTrans.position);
        }

        if (player.EasyMode)
        {
            player.twineHangTrans = FindNearestGrabbable();
        }

        direction = getThrowDirection();
        player.twineLineRenderer.enabled = true;
        
        throwTimer = 0;

        player.anim.Play("opening");
    }

    private float getThrowDirection()
    {
        Vector3 campos;
        if (player.EasyMode && player.twineHangTrans!=null)
        {
            campos = player.twineHangTrans.position;
        }
        else
        {
            campos = cam.ScreenToWorldPoint(Input.mousePosition);
        }
        
        Vector3 startPos = player.TwineStartPosTrans.position;
        Vector2 dir=new Vector2(campos.x - startPos.x, campos.y - startPos.y);
        return Mathf.Atan2(dir.x, dir.y);
    }

    private float throwParable(float x)
    {
        if (x <= 0 || x>=1)
            return 0;

        //f(x) = -((x-0.5)*2)^2+1
        float a = (x - 0.5f) * 2;
        return -(a * a) + 1;
    }

    public override void ExitState()
    {
        
    }

    public override void FixedUpdate()
    {
        player.jumpRunState.DoMovementUpdate();
    }

    public override void Update()
    {
        Vector3 startPos = player.TwineStartPosTrans.position;

        float sollDir = getThrowDirection();
        direction = Mathf.MoveTowardsAngle(direction*Mathf.Rad2Deg, sollDir*Mathf.Rad2Deg, Time.deltaTime * TWINE_ROTATION_SPEED*Mathf.Rad2Deg)*Mathf.Deg2Rad;

        float distance = throwParable(throwTimer / player.TwineThrowTime) * player.TwineThrowDistance;

        Vector3 dirVec = new Vector3(Mathf.Sin(direction)*distance, Mathf.Cos(direction)*distance);


        player.twineLineRenderer.SetPosition(0, startPos);
        player.twineLineRenderer.SetPosition(1, startPos + dirVec);

        RaycastHit2D[] hits = Physics2D.RaycastAll(startPos, dirVec, distance, player.GrabbableCheckMask);
        foreach(RaycastHit2D hit in hits)
        {
            player.twineHangTrans= hit.transform;
            player.machine.State = player.hangTwineState;
        }

        if (player.jumpPressed && player.onGround)
        {
            player.anim.Play("jump");
            player.rigid.velocity = new Vector2(player.rigid.velocity.x, player.JumpForce);
        }

        throwTimer += Time.deltaTime;
        if(throwTimer>player.TwineThrowTime)
        {
            player.twineLineRenderer.enabled = false;
            player.machine.State = player.jumpRunState;
            player.anim.Play("closing");
        }
    }
}
