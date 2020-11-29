using Assets._Lightshift.Scripts.Data;
using Mirror;
using Newtonsoft.Json;
using SharedModels.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Animations;

namespace Assets._Lightshift.Scripts.Entity
{
    public class BaseEntity : NetworkBehaviour
    {
        private List<GameModifier> _modifiers = new List<GameModifier>();
        private List<GameProperty> _properties = new List<GameProperty>();

        public event Action<GameModifier> OnModifierUpdated;
        public event Action<GameProperty> OnPropertyUpdated;

        public event Action OnAwake;
        public event Action OnClientEstablished;
        public event Action OnInitialized;
        public override void OnStartClient()
        {
            base.OnStartClient();
            OnClientEstablished?.Invoke();

            CmdInit();
        }

        [Command]
        private void CmdInit() 
        {
            foreach (var property in _properties)
                TargetRpcUpdateProperty(connectionToClient, property.Key, property.Value);

            foreach (var modifier in _modifiers)
                TargetRpcUpdateModifier(connectionToClient, modifier.Type, modifier.Value);

            TargetRpcInitialized(connectionToClient);
        }

        [TargetRpc]
        private void TargetRpcInitialized(NetworkConnection target) 
        {
            OnInitialized?.Invoke();
        }

        public void UpdateAndBroadcastProperty(GameProperty property) 
        {
            UpdateProperty(property.Key, property.Value);
            RpcUpdateProperty(property.Key, property.Value);
        }

        [ClientRpc]
        private void RpcUpdateProperty(string key, string value) 
        {
            UpdateProperty(key, value);
        }

        [TargetRpc]
        private void TargetRpcUpdateProperty(NetworkConnection target, string key, string value)
        {
            UpdateProperty(key, value);
        }

        private void UpdateProperty(string key, string value) 
        {
            var property = GetProperty(key);

            if (property == null)
                property = new GameProperty { Key = key };
            else _properties.Remove(property);

            property.Value = JsonConvert.SerializeObject(value);

            _properties.Add(property);

            OnPropertyUpdated?.Invoke(property);
        }

        public GameProperty GetProperty(string key) => _properties.FirstOrDefault(p => p.Key.ToLower() == key.ToLower());

        public void UpdateAndBroadcastModifier(GameModifier modifier)
        {
            UpdateModifier(modifier.Type, modifier.Value);
            RpcUpdateModifier(modifier.Type, modifier.Value);
        }

        [ClientRpc]
        private void RpcUpdateModifier(Modifier type, float value)
        {
            UpdateModifier(type, value);
        }

        [TargetRpc]
        private void TargetRpcUpdateModifier(NetworkConnection target, Modifier type, float value)
        {
            UpdateModifier(type, value);
        }

        private void UpdateModifier(Modifier type, float value)
        {
            var modifier = GetModifier(type);

            if (modifier == null)
                modifier = new GameModifier {Type = type, Value = value };
            else _modifiers.Remove(modifier);

            modifier.Value = value;

            _modifiers.Add(modifier);

            OnModifierUpdated?.Invoke(modifier);
        }

        public GameModifier GetModifier(Modifier type) => _modifiers.FirstOrDefault(p => p.Type == type);
    }
}
