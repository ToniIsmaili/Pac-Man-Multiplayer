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
    public TMP_Text scoreText;
    private int score = 0;

    private void UpdateInventory()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("PacMan");
        if (player == null) return;
        inventory.sprite = (player.GetComponent<SyncPowerUp>().powerup == null)? null : player.GetComponent<PlayerController>().sprite;
    }

    private void UpdateSpeed()
    {
        if (speed != null) speed.text = player.GetComponent<MovementController>().speed.ToString();
    }

    public void ResetScore()
    {
        score = 0;
        scoreText.text = score.ToString();
    }

    public void UpdateScore()
    {
        score++;
        scoreText.text = score.ToString();
    }

    void Update()
    {
        // UpdateInventory();
        UpdateSpeed();
    }
}