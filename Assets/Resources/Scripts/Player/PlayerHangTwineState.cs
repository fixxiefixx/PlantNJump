using UnityEngine;
using System.Collections;

public class PlayerHangTwineState : PlayerState
{
    PID pidHang;

    public PlayerHangTwineState(GameObject go) : base(go, "hangtwine")
    {
        float p = 30f;
        float i = 20;
        float d = 5f;
        pidHang = new PID(p, i, d);
    }

    public override void EnterState()
    {
        pidHang.Reset();
    }

    public override void ExitState()
    {
        //player.twineLineRenderer.enabled = false;
        if(player.twineHangTrans!=null)
            player.twineHangTrans.SendMessageUpwards("OnPlayerUnhang",SendMessageOptions.DontRequireReceiver);
    }

    public override void FixedUpdate()
    {
        if (player.twineHangTrans == null || player.jumpPressed || player.mousePressed)
        {
            player.anim.Play("closing");
            player.machine.State = player.jumpRunState;
            return;
        }
        

        

        Vector2 diff = player.rigid.position - new Vector2(player.twineHangTrans.position.x, player.twineHangTrans.position.y);

        Vector2 moveVec = Vector2Extension.Rotate(diff, 90);
        moveVec.Normalize();

        float horMove = diff.y > 0 ? -player.horizontalMovement : player.horizontalMovement;

        player.rigid.velocity = player.rigid.velocity + moveVec * horMove*3f * Time.deltaTime;


        float dist = diff.magnitude;
        /*dist = Mathf.MoveTowards(dist, player.TwineHangDistance, Time.deltaTime * 10f);
        diff = diff.normalized * dist;
        player.rigid.position = player.twineHangTrans.position + new Vector3(diff.x,diff.y,0);*/



        /*float minVelo = -5;
        if(player.rigid.velocity.y<minVelo)
        {
            player.rigid.velocity = new Vector2(player.rigid.velocity.x, minVelo);
        }*/

        //if (dist > player.TwineHangDistance)
        {
            //float a = (player.TwineHangDistance - dist);
            float twineForce = pidHang.Update(player.TwineHangDistance, dist, Time.deltaTime);

            if (twineForce > player.MaxTwineHangForce)
                twineForce = player.MaxTwineHangForce;
            if (dist > player.TwineThrowDistance+3)
            {
                player.rigid.velocity = new Vector2(0, 0);
            }
            player.rigid.velocity = player.rigid.velocity + diff.normalized * twineForce * Time.deltaTime;
        }

    }

    public override void Update()
    {
        Vector3 startPos = player.TwineStartPosTrans.position;
        player.twineLineRenderer.SetPosition(0, startPos);
        player.twineLineRenderer.SetPosition(1, player.twineHangTrans.position);
    }
}
