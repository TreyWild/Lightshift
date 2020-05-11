using Cinemachine;
using Lightshift;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public static CameraFollow Instance;

	public Transform target;
	public CinemachineVirtualCamera _virtualCamera;

	public float smoothSpeed = 0.125f;
	public Vector3 offset;
	public Vector3 zoomOffset = new Vector3(0,0, -60);
	private bool _immersive;
	public void SetCameraMode(bool immersive)
	{
		_immersive = immersive;
		GetComponent<CinemachineBrain>().enabled = immersive;
		_virtualCamera.enabled = immersive;
	}

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else Destroy(gameObject);
	}

	private void Start()
	{
		Settings.Instance.RefreshCameraMode();
	}
	public void SetTarget(Transform target)
	{
		this.target = target;
		_virtualCamera.Follow = target;
		_virtualCamera.LookAt = target;
	}
	void LateUpdate()
	{
		if (target == null || _immersive)
			return;

		Vector3 desiredPosition = target.position + offset;
		if (Input.GetKey(Settings.Instance.ZoomOutKey))
			desiredPosition = target.position + zoomOffset;
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
		transform.position = smoothedPosition;

		transform.LookAt(target);
	}

}