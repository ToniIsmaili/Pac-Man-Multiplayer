using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp/Speed decrease")]
public class SlowDown : PowerUp
{
    public float effect_duration = 10f;
    public float speed_decrease = 50f;

    public override void StartNeutralize(GameObject gameObject, float duration)
    {
        gameObject.GetComponent<MovementController>().speed += speed_decrease;
    
        base.StartNeutralize(gameObject, effect_duration);
    }

    public override void NeutralizeEffect(GameObject gameObject)
    {
        gameObject.GetComponent<MovementController>().speed -= speed_decrease;
    }

    public override void Apply(GameObject target)
    {
        MovementController player_controller = target.GetComponent<MovementController>();

        if (player_controller != null)
        {
            // Increase speed
            player_controller.speed -= speed_decrease;

            // Start a coroutine to reset speed after duration
            StartEffectDuration(target, effect_duration);
        }
        else
        {
            Debug.LogWarning("Testingscript component not found on the target GameObject.");
        }
    }

    public override void ResetEffect(GameObject target)
    {
        target.GetComponent<MovementController>().speed += speed_decrease;
    }
}
