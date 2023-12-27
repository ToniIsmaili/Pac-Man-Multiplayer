using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
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

        text.text = player.GetComponent<Testingscript>().speed.ToString();
    }
}
