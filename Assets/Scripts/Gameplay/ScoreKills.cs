using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class ScoreKills : MonoBehaviourPunCallbacks, IPunObservable
{

    public int currentKills;
    public ScoreManager scoreManager;
    
    // Start is called before the first frame update
    void Start()
    {
        currentKills = 0;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentKills);
        }
        else
        {
            currentKills = (int)stream.ReceiveNext();
        }

    }
    public void IncrementKill()
    {
        currentKills++;
        UpdateScore();
    }

    public void UpdateScore()
    {
        scoreManager.ScoreUpdate();
        if (scoreManager.CheckKills(currentKills)) 
            scoreManager.EndGame(this.name);
    }

}
