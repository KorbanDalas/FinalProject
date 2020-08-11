using Platformer.Core;
using Platformer.Model;
using UnityEngine;
using UnityEngine.SceneManagement;

//using UnityEngine.UIElements;
using UnityEngine.UI;


using Photon.Pun;
using Photon.Realtime;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;

public class ScoreManager : MonoBehaviourPunCallbacks, IPunObservable
{

    //public ScoreKills player1;
    //public ScoreKills player2;
    //public ScoreKills player3;
    //public ScoreKills player4;

    //public Text text1;
    //public Text text2;
    //public Text text3;
    //public Text text4;

    public ScoreKills[] players;
    public Text[] texts;

    readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

    public Text endGameText;
    public Button reloadButton;

    //string SceneName = "MainScene";
    string SceneName = "InitialScene";

    public GameObject goEndGame;

    public bool gameEnd;

    void Start()
    {

        ScoreUpdate();
        reloadButton.onClick.AddListener(ReloadGame);

    }

    
    void FixedUpdate()
    {
        int count = 0;
        for (int i = 0; i <= 3; i++)
        {
            if (players[i] != null) count++;
        }

        if (count != PhotonNetwork.CountOfPlayers) UpdatePlayers();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {


    }

    // Update is called once per frame
    public void ScoreUpdate()
    {
        
        //text1.text = player1 != null ? player1.name + ": " + player1.currentKills.ToString() : "";
        //text2.text = player2 != null ? player2.name + ": " + player2.currentKills.ToString() : "";
        //text3.text = player3 != null ? player3.name + ": " + player3.currentKills.ToString() : "";
        //text4.text = player4 != null ? player4.name + ": " + player4.currentKills.ToString() : "";

        for (int i=0; i<=3; i++)
        {
            texts[i].text = players[i] != null ? players[i].name + ": " + players[i].currentKills.ToString() : "";
        }

    }

    public void setPlayers(ScoreKills current)
    {
        for (int i = 0; i <= 3; i++)
        {
            
            if (players[i] == null)
            {
                players[i] = current;
                ScoreUpdate();
                break;
            }
        }
    }

    [PunRPC]
    private void SetPlayersRPC()
    {
        UpdatePlayers();
    }

    [PunRPC]
    public void UpdateScoreRPC()
    {
        ScoreUpdate();
        //if (scoreManager.CheckKills(currentKills)) scoreManager.EndGame(this.name);
    }

    [PunRPC]
    private void StopPlayers(string playerName)
    {
        model.player.controlEnabled = false;
        goEndGame.SetActive(true);
        gameEnd = true;
        endGameText.text = playerName + " - win!";

    }

    void UpdatePlayers()
    {
        for (int i = 0; i <= 3; i++)
        {
            players[i] = null;
        }

        var PlPhoton = PhotonNetwork.PlayerList;

        GameObject[] massGO = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in massGO)
        {
            var pvPlayer = PhotonView.Get(go);
            if (pvPlayer != null)
            {
                foreach (Photon.Realtime.Player player in PlPhoton)
                {
                    if (pvPlayer.OwnerActorNr == player.ActorNumber)
                    {
                        var sk = go.GetComponent<ScoreKills>();
                        if (go.name != player.NickName) go.name = player.NickName;
                        setPlayers(sk);
                    }
                }
            }
        }

    }

    public bool CheckKills(int currentKill)
    {
        
        return (currentKill == model.scoreToWin);
            
    }
    public void EndGame(string playerName)
    {
        //Time.timeScale = 0;
        Debug.Log(playerName + " - win!");
        goEndGame.SetActive(true);
        endGameText.text = playerName + " - win!";

        var pview = PhotonView.Get(this);
        pview.RPC("StopPlayers", RpcTarget.All, playerName);

        var ev = Schedule<PlayerEnteredVictoryZone>();        

    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneName);
    }

}
