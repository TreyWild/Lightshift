//using UnityEngine;
//using UnityEngine.Networking;

//namespace AnySync.Examples
//{
//    /// <summary>
//    /// Expert example using advanced techniques and showing off most of the features.
//    /// For people who are already comfortable with the asset and want the full potential.
//    /// </summary>
//    public class UNetRigidbodySync : NetworkBehaviour
//    {
//        // Up to 10 messages per second.
//        private const float MinimumSendInterval = 0.1f;
//        private const float ExpectedFrameTime = 1f / 60f;
//        private const float MovementAcceleration = 15f;

//        // Adding an ExpectedFrameTime to interpolationLatency because we're always sending the keyframes late (after a MinimumSendInterval delay).
//        public readonly MotionGenerator MotionGenerator = new MotionGenerator(MinimumSendInterval + ExpectedFrameTime);

//        private float _timeSinceLastSync;
//        private Vector3 _lastSentVelocity;
//        private Vector3 _lastSentPosition;
//        private Quaternion _lastSentRotation;
//        private Vector3 _lastSentAngularVelocity;

//        private Rigidbody _rigidbody;
//        private void Awake()
//        {
//            _rigidbody = GetComponent<Rigidbody>();
//        }

//        private void Update()
//        {
//            if (hasAuthority)
//            {
//                _timeSinceLastSync += Time.deltaTime;

//                // Teleportation.
//                if (Input.GetKeyDown(KeyCode.Space))
//                {
//                    // Send a keyframe to interpolate to before teleporting.
//                    SendSyncMessage();

//                    var newPosition = new Vector3(Random.Range(-6f, 6f), 2f, Random.Range(-6f, 6f));
//                    transform.position = newPosition;
//                    transform.rotation = Quaternion.identity;
//                    _rigidbody.velocity = Vector3.zero;
//                    _rigidbody.angularVelocity = Vector3.zero;

//                    // And then immediately send a teleport keyframe with 0 interpolation time.
//                    SendSyncMessage();
//                    return;
//                }

//                // Local movement.
//                var movementInput = Vector2.ClampMagnitude(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), 1f);
//                _rigidbody.AddForce(new Vector3(-movementInput.x, 0f, -movementInput.y) * MovementAcceleration * Time.deltaTime, ForceMode.VelocityChange);

//                // Sync rigidbody on change.
//                if (_timeSinceLastSync >= MinimumSendInterval)
//                {
//                    if (transform.position != _lastSentPosition || _rigidbody.velocity != _lastSentVelocity || transform.rotation != _lastSentRotation || _rigidbody.angularVelocity != _lastSentAngularVelocity)
//                        SendSyncMessage();
//                }
//            }
//            else
//            {
//                // Proxy movement.
//                if (MotionGenerator.HasKeyframes)
//                {
//                    MotionGenerator.UpdatePlayback(Time.deltaTime);
//                    transform.position = MotionGenerator.Position;
//                    transform.rotation = MotionGenerator.Rotation;
//                }
//            }
//        }

//        private void SendSyncMessage()
//        {
//            CmdSync(_timeSinceLastSync, transform.position, transform.rotation, _rigidbody.velocity, _rigidbody.angularVelocity);
//            _lastSentPosition = transform.position;
//            _lastSentVelocity = _rigidbody.velocity;
//            _lastSentRotation = transform.rotation;
//            _lastSentAngularVelocity = _rigidbody.angularVelocity;
//            _timeSinceLastSync = 0f;
//        }

//        [Command]
//        private void CmdSync(float interpolationTime, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
//        {
//            // Add keyframe to buffer.
//            MotionGenerator.AddKeyframe(interpolationTime, position, velocity, rotation, angularVelocity * Mathf.Rad2Deg);
//            // Send it to other clients.
//            RpcSync(interpolationTime, position, rotation, velocity, angularVelocity);
//        }

//        [ClientRpc]
//        private void RpcSync(float interpolationTime, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
//        {
//            // Prevent receiving keyframes on owner client and host.
//            if (hasAuthority || isServer)
//                return;

//            // Add keyframe to buffer.
//            MotionGenerator.AddKeyframe(interpolationTime, position, velocity, rotation, angularVelocity * Mathf.Rad2Deg);
//        }
//    }
//}
