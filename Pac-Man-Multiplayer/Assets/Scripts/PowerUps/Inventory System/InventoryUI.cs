using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject player;
    private TMP_Text text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            return;

        if (player.GetComponent<Inventory>().powerUp == null)
            text.text = "Empty";
        else text.text = player.GetComponent<Inventory>().powerUp.name;
    }
}
