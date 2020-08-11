using Platformer.Core;
using Platformer.Model;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using Cinemachine;


namespace Platformer.Mechanics
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }

        //This model field is public and can be therefore be modified in the 
        //inspector.
        //The reference actually comes from the InstanceRegister, and is shared
        //through the simulation and events. Unity will deserialize over this
        //shared reference when the scene loads, allowing the model to be
        //conveniently configured inside the inspector.
        public PlatformerModel model = Simulation.GetModel<PlatformerModel>();


        public GameObject Prefab;
        public GameObject vcam;
        public ScoreManager score;
        public GameObject spawnPoints;
        public GameObject activeBoost;

        PhotonView photonView;

        void OnEnable()
        {
            Instance = this;
        }

        void OnDisable()
        {
            if (Instance == this) Instance = null;
        }

        void Update()
        {
            if (Instance == this) Simulation.Tick();
        }

        private void Start()
        {

            var spawnPoint = spawnPoints.GetComponent<returnSpawnPoint>();
            var point = spawnPoint.ReturnPoint();

            GameObject prefab = PhotonNetwork.Instantiate(Prefab.name, point.position, Quaternion.identity);
            //var vcam = GetComponent<Cinema>();
            prefab.GetComponent<ScoreKills>().scoreManager = score;
            var pc = prefab.GetComponent<PlayerController>();
            pc.isClient = true;
            pc.activeBoost = activeBoost;
            prefab.name = PhotonNetwork.LocalPlayer.NickName;
            
            var virtCam = vcam.GetComponent<Cinemachine.CinemachineVirtualCamera>();
            virtCam.Follow = prefab.transform;
            virtCam.LookAt = prefab.transform;

            model.player = pc;
            
            photonView = PhotonView.Get(model.ScoreManager); 
            photonView.RPC("SetPlayersRPC", RpcTarget.All);
            
        }


    }
}