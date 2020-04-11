
using Cinemachine;
using Lightshift;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public static CameraFollow Instance;
    public float smoothSpeed = 0.04f;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    private Rigidbody2D _rigidbody;
    private GameObject _target;
    public void SetTarget(GameObject target)
    {
        _target = target;
        _rigidbody = _target.GetComponent<Rigidbody2D>();
    }
    public Vector3 Offset;
    public Vector3 zoomOffset;
    void Update()
    {
        if (_target)
        {
            Vector3 speedOffset = new Vector3(_rigidbody.velocity.x * smoothSpeed, _rigidbody.velocity.y * smoothSpeed, 0);
            if (Input.GetKey(Settings.Instance.ZoomOutKey) && !Settings.Instance.KeysLocked)
                transform.position = _target.transform.position + zoomOffset - speedOffset * Time.deltaTime;
            else
                transform.position = _target.transform.position + Offset - speedOffset * Time.deltaTime;
        }
    }
}