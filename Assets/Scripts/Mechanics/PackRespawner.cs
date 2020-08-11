using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class PackRespawner : MonoBehaviour
{
    class Pending
    {
        public GameObject go;
        public float timeLeft;
    }

    List<Pending> pending = new List<Pending>();

    public float respawnTime;

    public static PackRespawner instance { get; private set; }

    void Awake()
    {
        instance = this;
    }

    void SendActive(bool active, GameObject go)
    {
        var pview = PhotonView.Get(go);
        if (pview != null)
        {
            pview.RPC("SetActiveGO", pview.Owner, active);
        }
    }

    public void Add(GameObject go, float spawnTime =0)
    {
        go.SetActive(false);
        SendActive(false, go);
        var pendingObj = new Pending();
        pendingObj.go = go;
        pendingObj.timeLeft = spawnTime == 0f ? respawnTime : spawnTime;
        pending.Add(pendingObj);

    }

    void Update()
    {
        int n = pending.Count;
        while (n-- > 0)
        {
            pending[n].timeLeft -= Time.deltaTime;
            if (pending[n].timeLeft <= 0.0f)
            {
                pending[n].go.SetActive(true);
                SendActive(true, pending[n].go);
                pending.RemoveAt(n);
            }
        }
    }

}
