using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._Lightshift.Scripts.Network
{
    public class Communication : NetworkBehaviour
    {

        public static Communication Instance;
        void Awake() 
        {
            Instance = this;
        }

        [Command(requiresAuthority = false)]
        private void CmdSendChatMessage(string message, NetworkConnectionToClient sender = null)
        {
            SendChatMessage(sender, message);
        }

        [ClientRpc]
        public void RpcSendChatMessage(string message)
        {
            ChatBox.Instance.AddMessage(message);
        }

        public static void SendChatMessage(NetworkConnectionToClient sender = null, string message = null) 
        {

            if (Instance.isServer && sender != null)
            {
                var player = Server.GetPlayer(sender);
                if (player == null)
                    return;

                message = $"{player.Username}: {message}";
                Instance.RpcSendChatMessage(message);
            }
            else Instance.CmdSendChatMessage(message);
        }


        public static void SetUserSpectating(NetworkConnection target, short entityId) 
        {
            Instance.TargetRpcSpectateTarget(target, entityId);
        }

        [TargetRpc]
        public void TargetRpcSpectateTarget(NetworkConnection target, short entityId)
        {
            var entity = EntityManager.GetEntity(entityId);
            if (entity != null)
                CameraFollow.Instance.SetTarget(entity.transform);
        }

        public enum AlertType
        {
            Popup,
            SystemMessage,
            ScreenDisplay
        }

        public static void ShowBroadcastAlert(string message, AlertType type)
        {
            Instance.RpcBroadcastAlert(message, (byte)type);
        }
        public static void ShowUserAlert(NetworkConnection target, string message, AlertType type)
        {
            Instance.TargetRpcAlert(target, message, (byte)type);
        }

        [ClientRpc]
        private void RpcBroadcastAlert(string message, byte typeId)
        {
            ShowAlert(message, typeId);
        }

        [TargetRpc]
        private void TargetRpcAlert(NetworkConnection target, string message, byte typeId)
        {
            ShowAlert(message, typeId);
        }

        private void ShowAlert(string message, byte typeId) 
        {
            var type = (AlertType)typeId;
            switch (type)
            {
                case AlertType.Popup:
                    DialogManager.ShowMessage(message);
                    break;
                case AlertType.ScreenDisplay:
                    GameUIManager.Instance.ShowAnnouncementText($"{message}");
                    break;
                case AlertType.SystemMessage:
                    ChatBox.Instance.AddMessage($"SYSTEM: {message}");
                    break;
            }
        }

        public static int GetPing()
        {
            return (int)((NetworkTime.rtt / 2) * 1000);
        }
    }
}

