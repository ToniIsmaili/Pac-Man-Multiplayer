using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{

    public InputField usernameInput;
    public Text buttonText;

    public void OnClickConnect()
    {
        if (usernameInput.text.Length > 0)
        {
            PhotonNetwork.NickName = usernameInput.text;
            buttonText.text = "Connecting...";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby");
    }

}
