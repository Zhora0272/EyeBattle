namespace EyeGunSystem
{
    public class HeadGunController : GunController
    {
        public override void Shoot()
        {
            Reload();
            if (ammoController.GetAmmo(out var result))
            {
                battleManager.Value.GetClosest(battleParticipant.Value.battleParticipantParameters,
                    out var enemy);

                if (enemy != null)
                {
                    result.Attack(enemy.BotTransform);
                }
            }
        }
    }
}