using UnityEngine;

public class Weapon_HandGrenade : MonoBehaviour, IThrowable
{
    #region SerializeField

    [SerializeField] private int weaponId;
    [SerializeField] private string weaponName;
    [SerializeField] private bool isPickable;
    [SerializeField] private float damage, blastDelay;
    [SerializeField] private int rpm, totalAmmo, range;
    #endregion

    private float lastFireTime, rpmPerSecond;
    private Coroutine reloadMag_Coroutine;

    public int WeaponID => weaponId;
    public string WeaponName => weaponName;
    public int RPM => rpm;
    public float DamagePerHit => damage;
    public int TotalAmmo => totalAmmo;
    public float BlastDelay => blastDelay;
    public int Range => range;
    public bool IsPickable => isPickable;

    public int WeaponRegenerateDelay => throw new System.NotImplementedException();

    #region Unity Methods
    void Awake()
    {
        rpmPerSecond = 1.0f / rpm;
    }
    #endregion

    #region public Methods
    public void Throw()
    {
        if (totalAmmo > 0 && (Time.time - lastFireTime) > rpmPerSecond)
        {
            lastFireTime = Time.time;
            isPickable = false;
            Invoke("Blast", blastDelay);
        }
    }
    public void Blast()
    {
        //Blast effect and damage will give to near by user based on range
    }
    #endregion
}
