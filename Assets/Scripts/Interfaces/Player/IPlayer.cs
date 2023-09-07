
public interface IPlayer: ICharacter
{
    IAttachableWeapon PrimaryWeaponFirst { get; }
    IAttachableWeapon PrimaryWeaponSecond { get; }
    IAttachableWeapon SecondaryWeaponFirst { get; }
    IAttachableWeapon EquippedWeapon { get; }

    void PickWeapon(IAttachableWeapon weapon);
    void SelectWeapon(IAttachableWeapon weapon);
    void Fire();
    void Reload();
    void RecieveDamage(float damage);
}