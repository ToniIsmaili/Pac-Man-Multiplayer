using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp/Speed Increase")]
public class SpeedUp : PowerUp
{
    public float speed_increase = 100f;
    public float speed_duration = 5f;

    public override void onPickUp(GameObject gameObject)
    {
        MonoBehaviour.Destroy(gameObject);
    }

    public override void Apply(GameObject target)
    {
        Testingscript player_controller = target.GetComponent<Testingscript>();

        if (player_controller != null)
        {
            // Increase speed
            player_controller.speed += speed_increase;

            // Start a coroutine to reset speed after duration
            StartEffectDuration(target, speed_duration);
        }
        else
        {
            Debug.LogWarning("Testingscript component not found on the target GameObject.");
        }
    }

    public override void ResetEffect(GameObject target)
    {
        target.GetComponent<Testingscript>().speed -= speed_increase;
    }

}
