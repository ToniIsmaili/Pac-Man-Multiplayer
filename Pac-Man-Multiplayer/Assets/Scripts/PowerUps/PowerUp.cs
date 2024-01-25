using System.Collections;
using UnityEngine;

public abstract class PowerUp : ScriptableObject
{
    private MonoBehaviour MonoBehaviour = null;

    public GameObject tile = null;
    public bool tile_based = false;
    public bool teleport_based = false;

    public abstract void Apply(GameObject target);

    public virtual void StartNeutralize(GameObject gameObject, float duration)
    {
        MonoBehaviour = gameObject.GetComponent<MonoBehaviour>();
        MonoBehaviour.StartCoroutine(WaitNeutralize(gameObject, duration));
    }

    public void StartEffectDuration(GameObject target, float duration)
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

    private IEnumerator WaitNeutralize(GameObject target, float duration)
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(duration);

        // Reset back to its original value
        NeutralizeEffect(target);
    }

    public virtual void NeutralizeEffect(GameObject gameObject) { }

    public abstract void ResetEffect(GameObject target);

}
