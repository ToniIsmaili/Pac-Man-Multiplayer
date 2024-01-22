using UnityEngine;

public class SyncPowerUp : MonoBehaviour
{
    public PowerUp powerup;
    public PowerUp[] powerup_list;

    public void Sync(string name)
    {
        foreach (var powerup in powerup_list)
        {
            if (powerup.name == name)
            {
                this.powerup = powerup;
            }
        }
    }

}
