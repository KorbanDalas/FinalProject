using Platformer.Core;
using Platformer.Mechanics;
using static Platformer.Core.Simulation;


public class PackHealthEvent : Simulation.Event<PackHealthEvent>
{

    public int health;
    public Health playerhealth;

    public override void Execute()
    {

        if (playerhealth != null)
        {

            playerhealth.Increment(health);

        }

    }

}
