using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PackBoost : MonoBehaviour, IPunObservable
{
    public float spawnTime;

    public float speedBoost;
    public float jumpBoost;

    public float timeBoost;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //if (stream.IsWriting)
        //{
        //    stream.SendNext(gameObject.activeInHierarchy);
        //    Debug.Log(" send: " + gameObject.activeInHierarchy.ToString());
        //}
        //else
        //{
        //    var act = (bool)stream.ReceiveNext();
        //    gameObject.SetActive(act);
        //    Debug.Log(" Receive: " + act.ToString());
        //}
    }

    [PunRPC]
    private void SetActiveGO(bool active)
    {
        gameObject.SetActive(active);
    }


}
