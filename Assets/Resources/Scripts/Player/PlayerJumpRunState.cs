using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpRunState : PlayerState {



    public PlayerJumpRunState(GameObject go) : base(go, "jumprun")
    {
        
    }

    public override void Update()
    {
        if (player.mousePressed)
        {
            player.machine.State = player.throwTwineState;
            return;
        }

        if (player.twineLineRenderer.enabled)
        {
            //Ranke einziehen.
            Vector3 startPos = player.TwineStartPosTrans.position;
            Vector3 twinePos = player.twineLineRenderer.GetPosition(1);
            twinePos = Vector3.MoveTowards(twinePos, startPos, Time.deltaTime * 30f);
            player.twineLineRenderer.SetPosition(0, startPos);
            player.twineLineRenderer.SetPosition(1, twinePos);
            if((twinePos-startPos).sqrMagnitude<0.01f)
            {
                player.twineLineRenderer.enabled = false;
            }
        }
    }



    public void DoMovementUpdate()
    {
        player.rigid.velocity = new Vector2(Mathf.MoveTowards(player.rigid.velocity.x, player.horizontalMovement, Time.deltaTime *(player.onGround? 35f:5f)), player.rigid.velocity.y);
    }

    public override void FixedUpdate()
    {
        DoMovementUpdate();
        player.anim.SetFloat("speed", Mathf.Abs(player.horizontalMovement));

        if (player.jumpPressed && player.onGround)
        {
            player.anim.Play("jump");
            player.rigid.velocity = new Vector2(player.rigid.velocity.x, player.JumpForce);
        }
    }
}
