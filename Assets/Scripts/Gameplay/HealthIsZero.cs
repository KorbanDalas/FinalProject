using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using static Platformer.Core.Simulation;

using Photon.Pun;
using Photon.Realtime;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player health reaches 0. This usually would result in a 
    /// PlayerDeath event.
    /// </summary>
    /// <typeparam name="HealthIsZero"></typeparam>
    public class HealthIsZero : Simulation.Event<HealthIsZero>
    {
        public Health health;

        public bool isBot;

        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();
        PhotonView photonView;


        public void Awake()
        {
            //isBot = health.GetComponent<BotManager>() != null;
        }

        public override void Execute()
        {
            //if (isBot)
            //{
            //    Schedule<BotDeath>();
            //}
            //else
            //{
            Schedule<PlayerDeath>();
            //}

            photonView = PhotonView.Get(model.ScoreManager);
            photonView.RPC("UpdateScoreRPC", RpcTarget.All);

        }
    }
}