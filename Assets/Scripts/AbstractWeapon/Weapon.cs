using Photon.Pun;
using UnityEngine;
using System.Threading.Tasks;

namespace FPS
{
    public abstract class Weapon : MonoBehaviour, IAttachableWeapon
    {
        #region SerializeField
        [SerializeField] Weapon_SO weaponData;
        
        #endregion

        [SerializeField] protected GameObject bulletPrefab;
        [SerializeField] protected float velocitySpeed = 100f;

        public int WeaponID => weaponData.regenerateDelay;
        public int WeaponRegenerateDelay => weaponData.regenerateDelay;
        public string WeaponName => weaponData.weaponName;
        public bool IsPrimaryWeapon => weaponData.isPrimaryWeapon;
        public int RPM => weaponData.rpm;
        public float ReloadTime => weaponData.reloadTime;
        public int MagzineSize => weaponData.magzineSize;

        public int AmmoInMagzine => ammoInMagzine;
        public int TotalAmmo => totalAmmo;
        public float DamagePerHit => weaponData.damage;

        private float lastFireTime, rpmPerSecond;
        private int ammoInMagzine, totalAmmo;

        protected PhotonView photonView;

        #region Unity Methods
        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
     
            ammoInMagzine = weaponData.ammoInMagzine;
            totalAmmo = weaponData.totalAmmo;
            rpmPerSecond = 1.0f / RPM;
        }
        #endregion

        #region public Methods
        public virtual void Fire(ref bool isFiring)
        {
            if (ammoInMagzine > 0 && (Time.time - lastFireTime) > rpmPerSecond)
            {
                isFiring = true;
                lastFireTime = Time.time;
                ammoInMagzine--;
            }
            else {
                isFiring = false;
            }
        }

        public async Task Reload()
        {
            Task reload = ReloadMag();
            await reload;
            Debug.Log("Reload completed");
        }

        /// <summary>
        /// Calcution bases on totalAmmo, remainingAmmo
        /// </summary>
        /// <returns></returns>
        private async Task ReloadMag()
        {
            Debug.Log("ReloadMag");
            //Can play animation for loading time
            int delayTime = (int)(ReloadTime * 1000);
            await Task.Delay(delayTime);
            Debug.Log("ReloadMag reloaded");
            if (totalAmmo > 0)
            {
                int remainingAmmo = totalAmmo - MagzineSize;
                if (remainingAmmo > 0)
                {
                    ammoInMagzine = MagzineSize;
                    totalAmmo = remainingAmmo;
                }
                else
                {
                    ammoInMagzine = totalAmmo;
                    totalAmmo = 0;
                }
            }
        }

        #endregion
    }
}
