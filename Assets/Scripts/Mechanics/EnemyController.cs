using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

using Photon.Pun;
using Photon.Realtime;

namespace Platformer.Mechanics
{
    /// <summary>
    /// A simple controller for enemies. Provides movement control over a patrol path.
    /// </summary>
    [RequireComponent(typeof(AnimationController), typeof(Collider2D))]
    public class EnemyController : MonoBehaviourPunCallbacks, IPunObservable
    {
        public PatrolPath path;
        public AudioClip ouch;

        internal PatrolPath.Mover mover;
        internal AnimationController control;
        internal Collider2D _collider;
        internal AudioSource _audio;
        SpriteRenderer spriteRenderer;

        public Bounds Bounds => _collider.bounds;

        float defSpeed;
        
        public bool isDamage;
        public float timeDamageStop;

        void Awake()
        {
            control = GetComponent<AnimationController>();
            _collider = GetComponent<Collider2D>();
            _audio = GetComponent<AudioSource>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            isDamage = true;

        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

        }


        void OnCollisionEnter2D(Collision2D collision)
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                var ev = Schedule<PlayerEnemyCollision>();
                ev.player = player;
                ev.enemy = this;
            }
        }

        void Update()
        {
            if (path != null)
            {
                if (mover == null) mover = path.CreateMover(control.maxSpeed * 0.5f);
                control.move.x = Mathf.Clamp(mover.Position.x - transform.position.x, -1, 1);
            }
        }

        public void StopEnemy()
        {
            isDamage = false;
            defSpeed = control.maxSpeed;
            control.maxSpeed = 0;
            Invoke("RunEnemy", timeDamageStop);
        }

        void RunEnemy()
        {
            control.maxSpeed = defSpeed;
            isDamage = true;
        }

    }
}