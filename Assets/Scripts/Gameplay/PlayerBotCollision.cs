using Platformer.Core;
using Platformer.Mechanics;

using static Platformer.Core.Simulation;

using Photon.Pun;
using Photon.Realtime;
using System.Diagnostics;

namespace Platformer.Gameplay
{

    /// <summary>
    /// Fired when a Player collides with an Enemy.
    /// </summary>
    /// <typeparam name="EnemyCollision"></typeparam>
    public class PlayerBotCollision : Simulation.Event<PlayerBotCollision>
    {
        
        public PlayerController player;
        public PlayerController enemyPlayer;
        
        public override void Execute()
        {
            //var willHurtEnemy = player.Bounds.min.y == enemy.Bounds.max.y;
            //var willHurtEnemy = Mathf.Abs(player.Bounds.min.y) - Mathf.Abs(bot.Bounds.max.y) > 0.001f;
            //var willHurtEnemy = Mathf.Abs(player.Bounds.max.y) - Mathf.Abs(enemyPlayer.Bounds.min.y) > 0.005f;

            //if (willHurtEnemy)
            //{
                var health = enemyPlayer.GetComponent<Health>();

                if (health != null)
                {
                    health.Decrement();
                var pview = PhotonView.Get(enemyPlayer.transform);

                if (pview != null)
                {
                    pview.RPC("GetDamage", pview.Owner, pview.ViewID);

                }

                //Debug.Log(" damage bot 1");
                if (health.isDead())
                    {
                    //    if (enemyPlayer.isBot)
                    //    {
                    //        Schedule<BotDeath>().bot = enemyPlayer;
                    //    }
                    //    else
                    //    {
                    //        Schedule<PlayerDeath>();
                    //    }
    
                    player.GetComponent<ScoreKills>().IncrementKill();

                    }

                    player.Bounce(4);

                }
            //}
        }
    }
}