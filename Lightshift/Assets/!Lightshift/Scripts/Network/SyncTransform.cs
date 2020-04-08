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
    public int SendRate = 10;
    public bool LocalAuthority;

    private MotionGenerator _motionGenerator;
    private Rigidbody2D _rigidbody;

    private Vector2 _lastSentPosition;
    private Vector2 _lastSentVelocity;
    private float _timeSinceLastSync;
    private float _lastSentRotation;
    private float _lastSentAngularVelocity;

    private void Start()
    {
        _motionGenerator = new MotionGenerator();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (LocalAuthority && isLocalPlayer || isServer && !LocalAuthority)
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
        if (_timeSinceLastSync >= (SendRate / 100))
        {
            if ((Vector2)transform.position != _lastSentPosition || _rigidbody.velocity != _lastSentVelocity || transform.rotation.eulerAngles.z != _lastSentRotation || _rigidbody.angularVelocity != _lastSentAngularVelocity)
            {
                if (LocalAuthority)
                    SendSyncMessageToServer();
                else SendSyncMessageToClients();
            }
        }
    }

    private void SendSyncMessageToClients()
    {
        RpcSync(_timeSinceLastSync, transform.position, Quaternion.Euler(0,0, transform.rotation.z), _rigidbody.velocity, _rigidbody.angularVelocity);
        _lastSentPosition = transform.position;
        _lastSentVelocity = _rigidbody.velocity;
        _lastSentRotation = transform.rotation.eulerAngles.z;
        _timeSinceLastSync = 0f;
    }

    private void SendSyncMessageToServer()
    {
        CmdSync(_timeSinceLastSync, transform.position, Quaternion.Euler(0, 0, transform.rotation.z), _rigidbody.velocity, _rigidbody.angularVelocity);
        _lastSentPosition = transform.position;
        _lastSentVelocity = _rigidbody.velocity;
        _lastSentRotation = transform.rotation.eulerAngles.z;
        _timeSinceLastSync = 0f;
    }

    [Command]
    private void CmdSync(float interpolationTime, Vector3 position, Quaternion rotation, Vector3 velocity, float angularVelocity)
    {
        // Add keyframe to buffer.
        _motionGenerator.AddKeyframe(interpolationTime, position, velocity, rotation, new Vector3(0, 0, angularVelocity));
    }

    [ClientRpc]
    private void RpcSync(float interpolationTime, Vector3 position, Quaternion rotation, Vector3 velocity, float angularVelocity)
    {
        // Prevent receiving keyframes on owner client and host.
        if (isServer)
            return;

        // Add keyframe to buffer.
        _motionGenerator.AddKeyframe(interpolationTime, position, velocity, rotation, new Vector3(0,0, angularVelocity));
    }
}

