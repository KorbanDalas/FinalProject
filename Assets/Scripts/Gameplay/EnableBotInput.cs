using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;

namespace Platformer.Gameplay
{
    /// <summary>
    /// This event is fired when user input should be enabled.
    /// </summary>
    public class EnableBotInput : Simulation.Event<EnablePlayerInput>
    {
        //PlatformerModel model = Simulation.GetModel<PlatformerModel>();
        public BotManager bot;

        public override void Execute()
        {
            //var player = model.player;
            bot.EnableControl = true;
        }
    }
}