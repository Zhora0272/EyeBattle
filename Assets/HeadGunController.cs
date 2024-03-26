namespace EyeGunSystem
{
    public class HeadGunController : GunController
    {
        
        public override void Shoot()
        {
            if (ammoController.GetAmmo(out var result))
            {
                battleManager.Value.GetClosest(battleParticipant.Value.EyeParameters,
                    out var enemy);

                if (enemy != null)
                {
                    result.Attack(enemy.EyeTransform);
                }
            }
        }
    }
}