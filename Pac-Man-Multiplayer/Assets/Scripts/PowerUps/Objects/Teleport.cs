using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "PowerUp/Teleport")]
public class Teleport : PowerUp
{

    public override void Apply(GameObject target)
    {
        
    }

    public override void onPickUp(GameObject gameObject)
    {
        MonoBehaviour.Destroy(gameObject);
    }

    public override void ResetEffect(GameObject target) { }

}
