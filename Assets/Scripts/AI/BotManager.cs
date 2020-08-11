using Platformer.Mechanics;
using UnityEngine;
using UnityEngine.AI;
using Platformer.Gameplay;
using Platformer.Model;
using static Platformer.Core.Simulation;
using Platformer.Core;

public class BotManager : KinematicObject
{

    public Transform target;

    PlayerController playerController;

    float maxSpeed;

    JumpState jumpState = JumpState.Grounded;
    private bool stopJump;

    Vector2 move; 
    bool jump;
    float jumpTakeOffSpeed;
    SpriteRenderer spriteRenderer;
    internal Animator animator;
    
    readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

    public bool EnableControl;


    public void Awake()
    {

        playerController = GetComponent<PlayerController>();

        maxSpeed = playerController.maxSpeed;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        jumpTakeOffSpeed = playerController.jumpTakeOffSpeed;

    }

    protected override void Update()
    {
        if (EnableControl)
        {

            float direction = target.transform.position.x - transform.position.x;

            //if (Mathf.Abs(direction) < 1 && !jump)
            //{
            //    jump = true;
            //}
            //Debug.Log(" dir: " + this.name + " " + direction.ToString() + " " + jumpState.ToString());
            //{
            //    Vector3 pos = transform.position;
            //    pos.x += Mathf.Sign(direction) * maxSpeed * Time.deltaTime;
            //    move.x = pos.x > 0 ? 1 : -1;
            //    transform.position = pos;
            //}

            move.x = direction > 0 ? 1 : -1;

            if (jumpState == JumpState.Grounded && Mathf.Abs(direction) < 1)
                jumpState = JumpState.PrepareToJump;
            else if (jump)
            {
                stopJump = true;
                Schedule<PlayerStopJump>().player = playerController;
            }
        }
        else
        {
            move.x = 0;
        }

        UpdateJumpState();
        base.Update();
    }

    void UpdateJumpState()
    {
        //jump = false;
        switch (jumpState)
        {
            case JumpState.PrepareToJump:
                jumpState = JumpState.Jumping;
                jump = true;
                stopJump = false;
                break;
            case JumpState.Jumping:
                if (!IsGrounded)
                {
                    Schedule<PlayerJumped>().player = playerController;
                    jumpState = JumpState.InFlight;
                }
                break;
            case JumpState.InFlight:
                if (IsGrounded)
                {
                    Schedule<PlayerLanded>().player = playerController;
                    jumpState = JumpState.Landed;
                }
                break;
            case JumpState.Landed:
                jumpState = JumpState.Grounded;
                break;
        }
    }

    protected override void ComputeVelocity()
    {
        if (jump && IsGrounded)
        {
            velocity.y = jumpTakeOffSpeed * model.jumpModifier;
            jump = false;
        }
        else if (stopJump)
        {
            stopJump = false;
            if (velocity.y > 0)
            {
                velocity.y = velocity.y * model.jumpDeceleration;
            }
        }

        if (move.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (move.x < -0.01f)
            spriteRenderer.flipX = true;

        animator.SetBool("grounded", IsGrounded);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

        targetVelocity = move * maxSpeed;
    }

    public enum JumpState
    {
        Grounded,
        PrepareToJump,
        Jumping,
        InFlight,
        Landed
    }

}
