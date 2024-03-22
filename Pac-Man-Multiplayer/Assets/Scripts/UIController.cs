using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject gameManager;
    private GameObject player = null;
    public Image inventory;
    public TMP_Text speed;
    public TMP_Text score;

    private void UpdateInventory()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("PacMan");
        if (player.GetComponent<PlayerController>().powerUp == null)
            inventory.sprite = null;
        else inventory.sprite = player.GetComponent<PlayerController>().sprite;
    }

    private void UpdateSpeed()
    {
        if (speed != null) speed.text = player.GetComponent<MovementController>().speed.ToString();
    }

    private void UpdateScore()
    {
        score.text = gameManager.GetComponent<GameManager>().dots_remaining.ToString();
    }

    void Update()
    {
        UpdateInventory();
        UpdateSpeed();

        if (gameManager != null)
        {
            UpdateScore();
        }

    }
}
