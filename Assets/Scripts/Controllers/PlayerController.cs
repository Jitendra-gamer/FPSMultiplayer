using System;
using Photon.Pun;
using UnityEngine;
using System.Threading.Tasks;

namespace FPS
{
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable, IPlayer
    {
        #region Public Fields

        public static GameObject LocalPlayerInstance;
        public Renderer remotePlayerRenderer;

        public int ID => id;
        public string Name => name;
        public float Health { get => health; }
        public IAttachableWeapon PrimaryWeaponFirst => primaryWeaponFirst;
        public IAttachableWeapon PrimaryWeaponSecond => primaryWeaponSecond;
        public IAttachableWeapon SecondaryWeaponFirst => secondaryWeaponFirst;
        public IAttachableWeapon EquippedWeapon => selectedWeapon;

        #endregion

        #region Private Fields
        [SerializeField] private Transform gunPoint;
        [SerializeField] private GameObject playerUiPrefab;

        private PlayerMovement playerMovement;
        //True, when the user is firing
        bool IsFiring;
        private int id;
        private string name;

        private IAttachableWeapon primaryWeaponFirst,
            primaryWeaponSecond,
            secondaryWeaponFirst,
            selectedWeapon;

        private float health = 100f;

        #endregion
        //UI Update
        public static event Action UpdateUIEvent;

        #region Unity Methods

        private void Awake()
        {
            playerMovement = GetComponent<PlayerMovement>();

            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
            }
            else
            {
                // Set different color for Remote user
                remotePlayerRenderer.material.SetColor("_Color", Color.red);
            }

            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            CameraFollow _cameraFollow = gameObject.GetComponent<CameraFollow>();
            
            if (_cameraFollow != null)
            {
                if (photonView.IsMine)
                {
                    FindObjectOfType<UIManager>().SetPlayer(this);
                    _cameraFollow.OnStartFollowing();
                }
            }

            // Create the UI
            if (this.playerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(this.playerUiPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
        }

        private void OnDestroy()
        {
            CleanUpActivity();
        }

        #region Interface implementation 
        public void Move(Vector2 input)
        {
            //Prevent control is connected to Photon and represent the localPlayer
            if (!photonView.IsMine)
            {
                return;
            }

            playerMovement.ProcessMove(input);
        }

        public void Jump()
        {
            // Prevent control is connected to Photon and represent the localPlayer
            if (!photonView.IsMine)
            {
                return;
            }

            playerMovement.Jump();
        }

        public void Fire()
        {
            // Prevent control is connected to Photon and represent the localPlayer
            if (!photonView.IsMine)
            {
                return;
            }

            IsFiring = false;
            if (selectedWeapon != null)
            {
                selectedWeapon.Fire(ref IsFiring);
                Debug.Log("IsFiring: " + IsFiring);
                
                UpdateUIEvent?.Invoke();
            }
        }

        public async void Reload()
        {
            Debug.Log("Reload");
            // Prevent control is connected to Photon and represent the localPlayer
            if (!photonView.IsMine)
            {
                return;
            }
            if (selectedWeapon != null)
            {
                await selectedWeapon.Reload();
                Debug.Log("Invoke");
                UpdateUIEvent?.Invoke();
            }
        }
        #endregion

        public void PickWeapon(IAttachableWeapon weapon)
        {
            if (weapon.IsPrimaryWeapon)
            {
                if (primaryWeaponFirst == null)
                    primaryWeaponFirst = weapon;
                else if (primaryWeaponSecond == null)
                    primaryWeaponSecond = weapon;
            }
            else
            {
                secondaryWeaponFirst = weapon;
            }

            //if (selectedWeapon == null) for this prototy, we are selecting whenever any weapon picked up
            {
                SelectWeapon(weapon);
            }
        }

        public void RecieveDamage(float damage)
        {
            if (health >= 0)
                health -= damage;
            else
                PlayerDeath();
        }

        public void SelectWeapon(IAttachableWeapon weapon)
        {
            Debug.Log("SelectWeapon");
            selectedWeapon = weapon;
            if (photonView.IsMine)
            {
                UpdateUIEvent?.Invoke();
            }
        }

        #endregion

        #region private Methods

        private void PlayerDeath()
        {
            //Death stuff here
            CleanUpActivity();
            ReStartPlayer();
        }

        private void CleanUpActivity()
        {
            primaryWeaponFirst = null;
            primaryWeaponSecond = null;
            secondaryWeaponFirst = null;
            selectedWeapon = null;
            if(olderWeapon)
            {
                Destroy(olderWeapon);
            }
 
            this.transform.gameObject.SetActive(false);
            if (photonView.IsMine)
            {
                UpdateUIEvent?.Invoke();
            }
        }

        private async void ReStartPlayer()
        {
            health = 100;
            transform.position = LocalPlayerInstance.transform.position;
            transform.rotation = LocalPlayerInstance.transform.rotation;
            await Task.Delay(3000);
            this.transform.gameObject.SetActive(true);
        }

        #endregion

        #region Pickup and Damage

        GameObject olderWeapon;
        int i = 0;
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.name + " "+ i++);

            if (other.CompareTag("IAttachableWeapon"))
            {
                //Delete older picked weapon
                if (olderWeapon)
                {
                    Destroy(olderWeapon);
                }
                olderWeapon = other.gameObject;

                olderWeapon.transform.SetParent(this.transform);
                olderWeapon.transform.position = gunPoint.position;
                olderWeapon.gameObject.transform.rotation = gunPoint.rotation;
                Weapon weapon = olderWeapon.GetComponent<Weapon>();
                PickWeapon(weapon);
                PhotonGameManager.Instance.WeaponCollected(weapon.WeaponID);
            }
        }

        #endregion

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(this.IsFiring);
                stream.SendNext(this.Health);
            }
            else
            {
                // Network player, receive data
                this.IsFiring = (bool)stream.ReceiveNext();
                this.health = (float)stream.ReceiveNext();
            }
        }

        #endregion
    }
}