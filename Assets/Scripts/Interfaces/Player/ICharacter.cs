using UnityEngine;

public interface ICharacter
{
    int ID { get;}
    string Name { get;}
    void Move(Vector2 pos);
    void Jump();
}