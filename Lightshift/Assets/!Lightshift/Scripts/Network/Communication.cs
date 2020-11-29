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
        [Command(ignoreAuthority = true)]
        private void CmdSendChatMessage(string message, NetworkConnectionToClient sender = null)
        {
            SendChatMessage(message, sender);
        }

        [ClientRpc]
        private void RpcSendChatMessage(string message)
        {
            ChatBox.Instance.AddMessage(message);
        }

        public void SendChatMessage(string message, NetworkConnectionToClient sender) 
        {
            if (isServer)
            {
                var player = Server.GetPlayer(sender);
                if (player == null)
                    return;

                message = $"{player.GetProfile().Username}{message}";
                RpcSendChatMessage(message);
            }
            else CmdSendChatMessage(message);
        }

        //public void ShowPopup(NetworkConnectionToClient connection, string message)
        //{
        //    if (isServer)
        //        TargetRpcShowPopup(message, connection);
        //}

        //[TargetRpc]
        //private void TargetRpcShowPopup(string message, NetworkConnectionToClient connection) 
        //{
        //    DialogManager.ShowMessage(message);
        //    return;
        //}
    }
}
