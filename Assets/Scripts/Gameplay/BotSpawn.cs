using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player is spawned after dying.
    /// </summary>
    public class BotSpawn : Simulation.Event<BotSpawn>
    {
        //PlatformerModel model = Simulation.GetModel<PlatformerModel>();
        public PlayerController bot;
        public Transform spawnPoint;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {

            bot.collider2d.enabled = true;
            bot.controlEnabled = false;
            if (bot.audioSource && bot.respawnAudio)
                bot.audioSource.PlayOneShot(bot.respawnAudio);
            //player.health.Increment();
            bot.health.reSpawn();
            
            if (spawnPoint == null)
            {
                var spp = model.spawnBotPoints.GetComponent<returnSpawnPoint>();
                var spawnNewPoint = spp.ReturnPoint();
                if (spawnNewPoint == null) spawnNewPoint = model.spawnPoint;
                bot.Teleport(spawnNewPoint.transform.position);
            }
            else
            {
                bot.Teleport(spawnPoint.transform.position);
            }
            
            bot.jumpState = PlayerController.JumpState.Grounded;
            bot.animator.SetBool("dead", false);
            //model.virtualCamera.m_Follow = bot.transform;
            //model.virtualCamera.m_LookAt = bot.transform;
            Simulation.Schedule<EnableBotInput>(2f).bot = bot.GetComponent<BotManager>();
        }
    }
}