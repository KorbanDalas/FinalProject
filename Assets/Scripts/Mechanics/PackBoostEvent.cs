using Platformer.Core;
using Platformer.Mechanics;
using static Platformer.Core.Simulation;

public class PackBoostEvent : Simulation.Event<PackBoostEvent>
{
    public float speedBoost;
    public float jumpBoost;

    public float timeBoost;

    public PlayerController playerController;

    public override void Execute()
    {

        if (playerController != null)
        {
            
            playerController.boostSpeed(speedBoost, jumpBoost, timeBoost);

        }

    }


}
