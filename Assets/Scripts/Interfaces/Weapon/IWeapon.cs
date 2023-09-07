
public interface IWeapon
{
	int WeaponID { get; } //Weapon ID
	int WeaponRegenerateDelay { get; } //GerenerateId
	string WeaponName { get; }// Name to show in GUI
	int RPM { get; } //Fire rounds per Minutes
	int TotalAmmo { get; } //Total Ammo of weapon hold
	float DamagePerHit { get; }
}