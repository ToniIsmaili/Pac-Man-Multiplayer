using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp/Invincible")]
public class Invincible : PowerUp
{
    public float invincibility_duration = 5f;

    public override void onPickUp(GameObject gameObject)
    {
        MonoBehaviour.Destroy(gameObject);
    }

    public override void Apply(GameObject target)
    {
        Testingscript player_controller = target.GetComponent<Testingscript>();

        if (player_controller != null )
        {
            player_controller.isInvincible = true;

            StartEffectDuration(target, invincibility_duration);
        }

    }

    public override void ResetEffect(GameObject target)
    {
        target.GetComponent<Testingscript>().isInvincible = false;
    }
}
