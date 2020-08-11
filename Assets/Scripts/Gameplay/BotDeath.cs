using Platformer.Core;
using Platformer.Mechanics;

namespace Platformer.Gameplay
{

    public class BotDeath : Simulation.Event<BotDeath>
    {
        //PlatformerModel model = Simulation.GetModel<PlatformerModel>();
        public PlayerController bot;
        //BotManager botManager;

        public override void Execute()
        {
            //var bot = model.player;
            //if (player.health.IsAlive)
            if (bot.health.isDead())
            {
                //player.health.Die();

                bot.GetComponent<BotManager>().EnableControl = false;

                if (bot.audioSource && bot.ouchAudio)
                    bot.audioSource.PlayOneShot(bot.ouchAudio);
                bot.animator.SetTrigger("hurt");
                bot.animator.SetBool("dead", true);
                var ev = Simulation.Schedule<BotSpawn>(2);
                ev.bot = bot;
                //ev.spawnPoint = returnSpawnPoint
            }
        }
    }
}