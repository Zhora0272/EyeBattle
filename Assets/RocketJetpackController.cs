using _Game.Scripts.Utility;

namespace EyeGunSystem
{
    public class RocketJetpackController : GunController
    {
        public override void Shoot()
        {
            this.WaitAndDoCycle(3, .5f, i =>
            {
                if (ammoController.GetAmmo(out var result))
                {
                    var a = battleManager.Value.GetClosest(
                        battleParticipant.Value.EyeParameters,
                        out var enemy);
                    
                    if (enemy != null)
                    {
                        result.Attack(enemy.EyeTransform);
                    }
                }
            });
        }
    }
}