
using Lightshift;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public static CameraFollow Instance;
    public Vector3 Offset;
    public Vector3 zoomOffset;
    public float smoothTime = 0.125f;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    //private MotionGenerator _rigidbody;
    //private GameObject _target;
    //public GameObject Target 
    //{
    //    get => _target;
    //    set 
    //    {
    //        _target = value;
    //        _rigidbody = value.GetComponent<Entity>().synchronizer.motionGenerator;
    //    }
    //}
    //public Vector3 Offset;
    //public Vector3 zoomOffset;
    //void Update()
    //{
    //    if (Target)
    //    {
    //        Vector3 speedOffset = new Vector3(_rigidbody.Velocity.x * 0.05f, _rigidbody.Velocity.y * 0.05f, 0);
    //        if (Input.GetKey(Settings.Instance.ZoomOutKey) && !Settings.Instance.KeysLocked)
    //            transform.position = Target.transform.position + zoomOffset - speedOffset * Time.deltaTime;
    //        else
    //            transform.position = Target.transform.position + Offset - speedOffset * Time.deltaTime;
    //    }
    //}

    public GameObject Target;
    public void LateUpdate()
    {
        if (Target == null)
            return;

        Vector3 desiredPosition;

        if (Input.GetKey(Settings.Instance.ZoomOutKey) && !Settings.Instance.KeysLocked)
            desiredPosition = Target.transform.position + zoomOffset;
        else desiredPosition = Target.transform.position + Offset;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothTime);
        transform.position = smoothedPosition;
    }
}