using UnityEngine;

namespace FPS
{
    [CreateAssetMenu(menuName = "WeaponData")]
    public class Weapon_SO : ScriptableObject
    {
        public enum WeaponType
        {
            Pistol,
            Bow,
            RocketLauncher
        }

        public int weaponId;
        public WeaponType weaponType;
        public string weaponName;
        public int regenerateDelay; //in mm
        public bool isPrimaryWeapon;
        public int rpm;
        public float reloadTime;
        public int magzineSize;
        public int ammoInMagzine;
        public int totalAmmo;
        public float damage;
    } 
}