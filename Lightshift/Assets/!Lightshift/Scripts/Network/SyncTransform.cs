using AnySync;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SyncTransform : NetworkBehaviour
{
    [Range(1f, 60f), Tooltip("Packets per second.")]
    public int SendRate;
    public bool ServerControlledObject;

    private MotionGenerator _motionGenerator;
    private Rigidbody2D _rigidbody;

    private Vector2 _lastSentPosition;
    private Vector2 _lastSentVelocity;
    private float _timeSinceLastSync;
    private float _lastSentRotation;
    private float _lastSentAngularVelocity;
    private bool _serverAuthority => ServerControlledObject && isServer;
    private void Awake()
    {
        _motionGenerator = new MotionGenerator((SendRate + 2)/100);
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (hasAuthority && !ServerControlledObject || _serverAuthority)
            HandleSync();

        else if (_motionGenerator.HasKeyframes)
        {
            _motionGenerator.UpdatePlayback(Time.deltaTime);
            transform.position = _motionGenerator.Position;
            transform.rotation = _motionGenerator.Rotation;
        }
    }

    public void HandleSync() 
    {
        _timeSinceLastSync += Time.deltaTime;
        if (_timeSinceLastSync >= (SendRate / 100))
        {
            if ((Vector2)transform.position != _lastSentPosition || _rigidbody.velocity != _lastSentVelocity || transform.rotation.eulerAngles.z != _lastSentRotation || _rigidbody.angularVelocity != _lastSentAngularVelocity)
            {
                if (isLocalPlayer)
                    SendSyncMessageToServer();
                else if (isServer)
                    SendSyncMessageToClients();
            }
        }
    }

    private void SendSyncMessageToClients()
    {
        RpcSync(_timeSinceLastSync, transform.position, transform.rotation.eulerAngles.z, _rigidbody.velocity, _rigidbody.angularVelocity);
        _lastSentPosition = transform.position;
        _lastSentVelocity = _rigidbody.velocity;
        _lastSentRotation = transform.rotation.eulerAngles.z;
        _timeSinceLastSync = 0f;
    }

    private void SendSyncMessageToServer()
    {
        CmdSync(_timeSinceLastSync, transform.position, transform.rotation.eulerAngles.z, _rigidbody.velocity, _rigidbody.angularVelocity);
        _lastSentPosition = transform.position;
        _lastSentVelocity = _rigidbody.velocity;
        _lastSentRotation = transform.rotation.eulerAngles.z;
        _timeSinceLastSync = 0f;
    }

    [Command(channel = 3)]
    private void CmdSync(float interpolationTime, Vector3 position, float rotation, Vector3 velocity, float angularVelocity)
    {
        // Add keyframe to buffer.
        _motionGenerator.AddKeyframe(interpolationTime, position, velocity, Quaternion.Euler(0,0, rotation), new Vector3(0, 0, angularVelocity));

        RpcSync(interpolationTime, position, rotation, velocity, angularVelocity);
    }

    [ClientRpc(channel = 3)]
    private void RpcSync(float interpolationTime, Vector3 position, float rotation, Vector3 velocity, float angularVelocity)
    {
        if (hasAuthority)
            return;

        // Add keyframe to buffer.
        _motionGenerator.AddKeyframe(interpolationTime, position, velocity, Quaternion.Euler(0, 0, rotation), new Vector3(0, 0, angularVelocity));
    }
}

