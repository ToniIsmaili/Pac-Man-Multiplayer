using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(menuName = "PowerUp/Speed decrease")]
public class SlowDown : PowerUp
{
    public float effect_duration = 10f;
    public float speed_decrease = 50f;

    public override void onPickUp(GameObject gameObject)
    {
        MonoBehaviour.Destroy(gameObject);
    }

    public override void StartNeutralize(GameObject gameObject, float duration)
    {
        gameObject.GetComponent<Testingscript>().speed += speed_decrease;
    
        base.StartNeutralize(gameObject, effect_duration);
    }

    public override void NeutralizeEffect(GameObject gameObject)
    {
        gameObject.GetComponent<Testingscript>().speed -= speed_decrease;
    }

    public override void Apply(GameObject target)
    {
        Testingscript player_controller = target.GetComponent<Testingscript>();

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
        target.GetComponent<Testingscript>().speed += speed_decrease;
    }
}
