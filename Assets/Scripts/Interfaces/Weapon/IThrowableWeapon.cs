public interface IThrowable : IWeapon
{
    float BlastDelay { get; }
    int Range {get; }
    bool IsPickable { get; }
    void Throw();
    void Blast();
}
