
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;

using Photon.Pun;
using Photon.Realtime;

namespace Platformer.Mechanics
{
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    public class PlayerController : KinematicObject, IPunObservable
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        /// Max horizontal speed of the player.
        public float maxSpeed = 7;

        /// Initial jump velocity at the start of a jump.
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        /*internal new*/ public Collider2D collider2d;
        /*internal new*/ public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public Bounds Bounds => collider2d.bounds;

        public bool isBot;

        float defSpeedBoost;
        float defJumpBoost;

        public GameObject activeBoost;

        public bool isClient;

        float _movex;
        
        //private PhotonView m_PhotonView;


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(spriteRenderer.flipX);
            }
            else
            {
                spriteRenderer.flipX = (bool)stream.ReceiveNext();
            }
        }

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

            isBot = GetComponent<BotManager>() != null;
            controlEnabled = !isBot;
            //m_PhotonView = GetComponent<PhotonView>();
        }

        protected override void Update()
        {

            if (!isClient)
                return;

            if (controlEnabled)
            {
                move.x = Input.GetAxis("Horizontal");
                if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
                    jumpState = JumpState.PrepareToJump;
                else if (Input.GetButtonUp("Jump"))
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }
            }
            else
            {
                move.x = 0;
            }
            UpdateJumpState();
            base.Update();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {

            //Debug.Log(name + " collided with " + collision.name + " -- " + collision.gameObject.active.ToString());
            if (!isClient) return;

            var itHealth = collision.GetComponent<PackHealth>();
            var itBoost = collision.GetComponent<PackBoost>();
            
            var activeGo = collision.gameObject.activeInHierarchy;

            if ((itHealth != null) && activeGo)
            {
                PackRespawner.instance.Add(itHealth.gameObject, itHealth.spawnTime);
                var ev = Schedule<PackHealthEvent>();
                ev.playerhealth = health;
                ev.health = itHealth.health;
                return;
            }
            if ((itBoost != null) && activeGo)
            {
                PackRespawner.instance.Add(itBoost.gameObject);
                var ev = Schedule<PackBoostEvent>();
                ev.playerController = this;
                ev.speedBoost = itBoost.speedBoost;
                ev.jumpBoost = itBoost.jumpBoost;
                ev.timeBoost = itBoost.timeBoost;
                return;
            }

            var enemyPlayer = collision.GetComponent<PlayerController>();
            if (enemyPlayer != null) 
            {

                //var attack = Mathf.Abs(Bounds.min.y) - Mathf.Abs(enemyPlayer.Bounds.max.y) > 0.01f;
                var attack = true;
                //Debug.Log("Trigger: " + name + " = ("+ isClient.ToString() + " Coll: " + enemyPlayer.name+ " attack: " + attack.ToString());
                if (attack && !enemyPlayer.health.isDead()) 
                {
                    //Debug.Log("Trigger: " + name + " = " + " Coll: " + enemyPlayer.name);
                    var ev = Schedule<PlayerBotCollision>();
                    ev.player = this;
                    ev.enemyPlayer = enemyPlayer;
                }
            }

            var enemy = collision.GetComponent<EnemyController>();
            //var player = collision.gameObject.GetComponent<PlayerController>();
            if (enemy != null)
            {
                var ev = Schedule<PlayerEnemyCollision>();
                ev.player = this;
                ev.enemy = enemy;
            }
        }

        void UpdateJumpState()
        {
            jump = false;
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
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
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


        public void boostSpeed(float speedBoost, float jumpBoost, float timeBoost)
        {

            defSpeedBoost = maxSpeed;
            defJumpBoost = jumpTakeOffSpeed;

            maxSpeed = maxSpeed + speedBoost;
            jumpTakeOffSpeed = jumpTakeOffSpeed + jumpBoost;

            //if (!isBot) activeBoost.SetActive(true);
            if (isClient) activeBoost.SetActive(true);

            Invoke("unBoostSpeed", timeBoost);
            
        }

        void unBoostSpeed()
        {

            maxSpeed = defSpeedBoost;
            jumpTakeOffSpeed = defJumpBoost;

            //if (!isBot) activeBoost.SetActive(false);
            if (isClient) activeBoost.SetActive(false);
            //Debug.Log(" прошло ");
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
}