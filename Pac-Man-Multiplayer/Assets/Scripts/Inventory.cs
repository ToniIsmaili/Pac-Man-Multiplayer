using UnityEngine;

public class Inventory : MonoBehaviour
{
    public PowerUp powerUp = null;

    private void Update()
    {
        if (powerUp != null && Input.GetKeyDown(KeyCode.Space))
        {
            powerUp.Apply(gameObject);
            powerUp = null;
        }

        if (powerUp != null && Input.GetKeyDown(KeyCode.Q))
        {
            powerUp = null;
        }
    }

}
