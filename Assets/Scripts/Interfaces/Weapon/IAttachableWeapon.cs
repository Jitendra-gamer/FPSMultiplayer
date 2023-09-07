using System.Threading.Tasks;

public interface IAttachableWeapon: IWeapon 
{
	bool IsPrimaryWeapon { get; }   // Primary & Secondary
	float ReloadTime { get; }   // Mag reload time
	int MagzineSize { get; }   // Ammo hold size 
	int AmmoInMagzine { get; } // Ammo in Magzin left 

	Task Reload();
	void Fire(ref bool isFiring);
}
