using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public void DeSpawn(MoveableBattleParticipantBaseController obj)
    {
        obj.OnDespawned();
    }
}