using Photon.Realtime;
using UnityEngine;

namespace FPS
{
    public class Bullet : MonoBehaviour
    {
        public Player Owner { get; private set; }
        public float Damage { get; private set; }

        public void Start()
        {
            Destroy(gameObject, 3.0f);
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("Bullet hit to player");
                collision.gameObject.GetComponent<PlayerController>().RecieveDamage(Damage);
            }
            Destroy(gameObject);
        }

        public void InitializeBullet(Player owner, Vector3 originalDirection, float lag, float velocitySpeed, float damage)
        {
            Owner = owner;
            Damage = damage;

            transform.forward = originalDirection;

            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.velocity = originalDirection * velocitySpeed;
            rigidbody.position += rigidbody.velocity * lag;
        }
    }
}