using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

using Photon.Pun;
using Photon.Realtime;

namespace Platformer.Mechanics
{
    /// <summary>
    /// Represebts the current vital statistics of some game entity.
    /// </summary>
    public class Health : MonoBehaviourPunCallbacks, IPunObservable
    {
        /// <summary>
        /// The maximum hit points for the entity.
        /// </summary>
        public int maxHP;

        /// <summary>
        /// Indicates if the entity should be considered 'alive'.
        /// </summary>
        public bool IsAlive; // => currentHP > 0;

        int currentHP;

        public GameObject[] massHealth;

        public ParticleSystem bloodEffect;

        int viewId;


        void Awake()
        {
            currentHP = maxHP;
            viewId = PhotonView.Get(this).ViewID;
        }
        public void updateHealthBar()
        {

            for (int i = 0; i <= currentHP - 1; i++)
            {
                massHealth[i].SetActive(true);
            }
            for (int i = currentHP; i <= massHealth.Length - 1; i++)
            {
                massHealth[i].SetActive(false);
            }

        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(currentHP);
            }
            else
            {
                currentHP = (int)stream.ReceiveNext();
            }

            updateHealthBar();

        }

        public bool isDead()
        {
            return currentHP == 0;
        }

        public void reSpawn()
        {
            currentHP = maxHP;
            updateHealthBar();
        }

        public void Increment(int healthPlus)
        {
            //currentHP = Mathf.Clamp(currentHP + 1, 0, maxHP);
            currentHP = Mathf.Min(currentHP + healthPlus, maxHP);
            updateHealthBar();
        }

        public void Decrement()
        {
            currentHP = Mathf.Clamp(currentHP - 1, 0, maxHP);
            if (currentHP == 0)
            {
                var ev = Schedule<HealthIsZero>();
                ev.health = this;

            }

            PlayBloodEffect();
            updateHealthBar();

        }

        public void PlayBloodEffect()
        {
            bloodEffect.Play();
        }

        public void Die()
        {
            while (currentHP > 0) Decrement();
        }

        [PunRPC]
        private void GetDamage(int viewId)
        {
            if (this.viewId == viewId)
            {
                Decrement();
                //Debug.Log(this.name + " - " + currentHP.ToString());
            }
                

        }

    }
}
