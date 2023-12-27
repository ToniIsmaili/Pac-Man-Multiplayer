using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int dots_remaining;

    void Start()
    {
        dots_remaining = GameObject.FindGameObjectsWithTag("PacDot").Length;
    }

}
