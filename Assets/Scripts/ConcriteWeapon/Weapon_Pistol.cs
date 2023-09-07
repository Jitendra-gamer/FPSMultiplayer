using Photon.Pun;
using UnityEngine;

namespace FPS
{
    public class Weapon_Pistol: Weapon
    {
        [SerializeField] Transform BulletSpawn;

        public override void Fire(ref bool isFiring)
        {
            base.Fire(ref isFiring);
            if (isFiring)
            {
                photonView.RPC("Fire", RpcTarget.AllViaServer, BulletSpawn.position, BulletSpawn.rotation);
            }
        }

        [PunRPC]
        public void Fire(Vector3 position, Quaternion rotation, PhotonMessageInfo info)
        {
            float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
            GameObject bullet;

            /** Use this if you want to fire one bullet at a time **/
            bullet = Instantiate(bulletPrefab) as GameObject;
            bullet.transform.localPosition = position;
            bullet.transform.localRotation = rotation;
            bullet.GetComponent<Bullet>().InitializeBullet(photonView.Owner, (rotation * Vector3.forward), Mathf.Abs(lag), velocitySpeed, DamagePerHit);
        }
    }
}
