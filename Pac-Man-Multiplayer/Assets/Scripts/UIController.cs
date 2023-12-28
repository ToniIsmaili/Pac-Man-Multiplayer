using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject player;
    public TMP_Text inventory;
    public TMP_Text speed;

    private void UpdateInventory()
    {
        if (player.GetComponent<PlayerController>().powerUp == null)
            inventory.text = "Empty";
        else inventory.text = player.GetComponent<PlayerController>().powerUp.name;
    }

    private void UpdateSpeed()
    {
        speed.text = player.GetComponent<MovementController>().speed.ToString();
    }

    void Update()
    {
        if (player == null)
            return;

        UpdateInventory();
        UpdateSpeed();
    }
}
