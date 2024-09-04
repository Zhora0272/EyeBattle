using _Game.Scripts.Utility;

namespace EyeGunSystem
{
    public class RocketJetpackController : GunController
    {
        public override void Shoot()
        {
            ammoController.TryReloadAmmo();
                
            this.WaitAndDoCycle(3, .5f, i =>
            {
                if (ammoController.GetAmmo(out var result))
                {
                    battleManager.Value.GetClosest(battleParticipant.Value.battleParticipantParameters,
                        out var enemy);

                    if (enemy != null)
                    {
                        result.Attack(enemy.BotTransform);
                    }
                }
            });
        }
    }
}