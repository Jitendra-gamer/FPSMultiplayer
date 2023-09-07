using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Threading.Tasks;
using System;

namespace FPS
{
    /// <summary>
    /// Photon manager.
    /// Connects and watch Photon Status, Instantiate Player
    /// Deals with quiting the room and the game
    /// Deals with level loading (outside the in room synchronization)
    /// </summary>
    public class PhotonGameManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields

        static public PhotonGameManager Instance;

        #endregion

        #region Private Fields

        [Tooltip("The prefab to use for representing the player")]
        [SerializeField]
        private GameObject playerPrefab;

        [SerializeField]
        private Transform[]  PlayersSpawn;

        [SerializeField]
        private Weapon[] weapons;

        [SerializeField]
        private Transform[] weaponSpawn;

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        private void Start()
        {
            Instance = this;

            // in case we started this demo with the wrong scene being active, simply load the menu scene
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene("Lobby");
                return;
            }

            if (PhotonNetwork.InRoom && PlayerController.LocalPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Instantiate(this.playerPrefab.name, PlayersSpawn[0].position, Quaternion.identity, 0);
                    WeaponGenerator();
                }
                else {
                   
                    PhotonNetwork.Instantiate(this.playerPrefab.name, PlayersSpawn[1].position, Quaternion.identity, 0);
                }
            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }

        private void WeaponGenerator()
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                PhotonNetwork.InstantiateRoomObject(this.weapons[i].gameObject.name, weaponSpawn[i].position, Quaternion.identity, 0);
            }
        }

        public async void WeaponCollected(int weaponID)
        {
            Weapon weapon = null;
            int i = 0;
            for ( ; i < weapons.Length; i++)
            {
                if (weaponID == weapons[i].WeaponID)
                {
                    weapon = weapons[i];
                    break;
                }
            }
            Debug.Log("WeaponCollected "+ i);
            await Task.Delay(weapon.WeaponRegenerateDelay);
            Debug.Log("WeaponCollected waiting over");
            PhotonNetwork.InstantiateRoomObject(weapon.gameObject.name, weaponSpawn[i].position, Quaternion.identity, 0);
            Debug.Log("WeaponCollected done");
        }


        /// <summary>
        /// To Exit Game on Escape key
        /// </summary>
        private void Update()
        {
            // "back" button of phone equals "Escape". quit app if that's pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitApplication();
            }
        }

        #endregion

        #region Photon Callbacks

        public override void OnJoinedRoom()
        {
            // Note: it is possible that this monobehaviour is not created (or active) when OnJoinedRoom happens
            // due to that the Start() method also checks if the local player character was network instantiated!
            if (PlayerManager.LocalPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            }
        }

        #endregion

        #region Public Methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void QuitApplication()
        {
            Application.Quit();
        }

        #endregion
    }

}