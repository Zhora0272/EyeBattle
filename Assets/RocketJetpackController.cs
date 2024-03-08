using _Game.Scripts.Utility;

public class RocketJetpackController : GunController
{
    public override void Shoot()
    {
        this.WaitAndDoCycle(3, .5f, i =>
        {
            if (ammoController.GetAmmo(out var result))
            {
                print("attack number" + i + 1 + "started");
                //result.Attack();
            }
        });
    }
}