using Photon.Pun;
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
    private int startingDots = 0;

    private void UpdateInventory()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("PacMan");

        if (player == null) return;
        if (player.GetComponent<SyncPowerUp>().powerup == null)
            inventory.sprite = null;
        else inventory.sprite = player.GetComponent<PlayerController>().sprite;
    }

    private void UpdateSpeed()
    {
        if (speed != null) speed.text = player.GetComponent<MovementController>().speed.ToString();
    }

    private void UpdateScore()
    {
        score.text = (startingDots - SyncPacDots.GetDotsRemaining()).ToString();
    }

    void Update()
    {
        // UpdateInventory();
        UpdateSpeed();
        if (startingDots == 0)
            startingDots = SyncPacDots.GetDotsRemaining();
        if (gameManager != null)
            UpdateScore();
    }
}