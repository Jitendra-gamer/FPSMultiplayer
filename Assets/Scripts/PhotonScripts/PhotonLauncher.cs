using UnityEngine;
using TMPro;

using Photon.Realtime;
using Photon.Pun;

namespace FPS
{
    /// <summary>
    /// Launch manager. Connect, join a random room or create one if none or all full.
    /// </summary>
    public class PhotonLauncher : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields

        // Message for user
        [SerializeField] private TMP_Text logText;

        //UI GameObject 
        [SerializeField] private GameObject connectUI; 

        //The maximum number of players per room
        [SerializeField] private byte maxPlayersPerRoom = 4;

        #endregion

        #region Private Fields
        /// <summary>
        /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon, 
        /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
        /// Typically this is used for the OnConnectedToMaster() callback.
        /// </summary>
        bool isConnecting;

        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string gameVersion = "1";

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start the connection process. 
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect()
        {
            connectUI.SetActive(false);
            
            logText.text = "";

            // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
            isConnecting = true;

            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                LogOnScreen("Joining Room...");
                //we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                LogOnScreen("Connecting...");
                //we must first and foremost connect to Photon Online Server.

                PhotonNetwork.LocalPlayer.NickName = "Player_"+Random.Range(100, 10000);
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = this.gameVersion;
            }
        }

        #endregion

        #region MonoBehaviourPunCallbacks CallBacks
        // you can find PUN's callbacks in the class MonoBehaviourPunCallbacks

        /// <summary>
        /// Called after the connection to the master is established and authenticated
        /// </summary>
        public override void OnConnectedToMaster()
        {
            // we don't want to do anything if we are not attempting to join a room. 
            // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
            // we don't want to do anything.
            if (isConnecting)
            {
                LogOnScreen("OnConnectedToMaster: Next -> try to Join Random Room");
                Debug.Log("PhotonLauncher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");

                // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
                PhotonNetwork.JoinRandomRoom();
            }
        }

        /// <summary>
        /// Called when entering a room (by creating or joining it). Called on all clients (including the Master Client).
        /// </summary>
        /// <remarks>
        /// This method is commonly used to instantiate player characters.
        /// If a match has to be started "actively", you can call an [PunRPC](@ref PhotonView.RPC) triggered by a user's button-press or a timer.
        ///
        /// When this is called, you can usually already access the existing players in the room via PhotonNetwork.PlayerList.
        /// Also, all custom properties should be already available as Room.customProperties. Check Room..PlayerCount to find out if
        /// enough players are in the room to start playing.
        /// </remarks>
        public override void OnJoinedRoom()
        {
            LogOnScreen("<color=green>OnJoinedRoom</color> with " + PhotonNetwork.CurrentRoom.PlayerCount + " Player(s)");
            Debug.Log("PhotonLauncher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running.");

            // We only load if we are the first player, else we rely on  PhotonNetwork.AutomaticallySyncScene to sync our instance scene.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                // Load the Room Level. 
                  PhotonNetwork.LoadLevel("GamePlay");
            }
        }

        /// <summary>
        /// Called when a JoinRandom() call failed. The parameter provides ErrorCode and message.
        /// </summary>
        /// <remarks>
        /// Most likely all rooms are full or no rooms are available. <br/>
        /// </remarks>
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            LogOnScreen("<color=red>OnJoinRandomFailed</color>: Next -> Create a new Room");
            Debug.Log("PhotonLauncher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            //we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom });
        }

        /// <summary>
        /// Called after disconnecting from the Photon server.
        /// </summary>
        public override void OnDisconnected(DisconnectCause cause)
        {
            LogOnScreen("<color=red>OnDisconnected</color> " + cause);
            Debug.LogError("PhotonLauncher:Disconnected");
            connectUI.SetActive(true);
            isConnecting = false;
        }

        #endregion

        private void LogOnScreen(string message)
        {
            if (logText == null)
            {
                return;
            }

            logText.text += System.Environment.NewLine + message;
        }
    }
}