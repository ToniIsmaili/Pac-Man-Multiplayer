using System.Collections;
using UnityEngine;

public abstract class PowerUp : ScriptableObject
{

    private MonoBehaviour MonoBehaviour = null;

    public abstract void Apply(GameObject target);

    public void StartEffectDuration(GameObject target,float duration)
    {
        MonoBehaviour = target.GetComponent<MonoBehaviour>();
        MonoBehaviour.StartCoroutine(Wait(target, duration));
    }

    private IEnumerator Wait(GameObject target, float duration)
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(duration);

        // Reset back to its original value
        ResetEffect(target);
    }

    public abstract void ResetEffect(GameObject target);

}
