using Platformer.Core;
using Platformer.Mechanics;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Gameplay
{

    /// <summary>
    /// Fired when a Player collides with an Enemy.
    /// </summary>
    /// <typeparam name="EnemyCollision"></typeparam>
    public class PlayerEnemyCollision : Simulation.Event<PlayerEnemyCollision>
    {
        public EnemyController enemy;
        public PlayerController player;

        public override void Execute()
        {

            // var willHurtEnemy = player.Bounds.center.y >= enemy.Bounds.max.y; // Orig

            var willHurtEnemy = player.Bounds.center.y >= enemy.Bounds.max.y;

            if (willHurtEnemy)
            {
                //var enemyHealth = enemy.GetComponent<Health>();
                //if (enemyHealth != null)
                //{
                //    //enemyHealth.Decrement();
                //    if (!enemyHealth.IsAlive)
                //    {
                //        //Schedule<EnemyDeath>().enemy = enemy;
                //        Debug.Log(" damage 1");
                //        player.Bounce(2);
                //    }
                //    else
                //    {
                //        player.Bounce(4);
                //    }
                //}
                //else
                //{
                    //Schedule<EnemyDeath>().enemy = enemy;
                    //Debug.Log(" damage 2");
                    player.Bounce(5);
                //}
            }
            else
            {
                
                if (enemy.isDamage) 
                { 
                
                    var playerHealth = player.GetComponent<Health>();
                    playerHealth.Decrement();

                    enemy.StopEnemy();

                }

            }

        }


    }
}