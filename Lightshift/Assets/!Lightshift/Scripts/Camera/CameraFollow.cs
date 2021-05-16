using Cinemachine;
using Lightshift;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public static CameraFollow Instance;

	public Transform target;

	public float smoothSpeed = 0.125f;
	public Vector3 offset;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else Destroy(gameObject);
	}
	public void SetTarget(Transform target)
	{
		this.target = target;
	}
	void LateUpdate()
	{
		if (target == null)
			return;

		if (!Settings.KeysLocked)
		{
			var zoom = Input.GetAxis("Mouse ScrollWheel") * 20;

			if (offset.z > -5)
			{
				zoom = 0;
				offset = new Vector3(offset.x, offset.y, -5f);
			}
			else if (offset.z < -100)
			{
				zoom = 0;
				offset = new Vector3(offset.x, offset.y, -100f);
			}
			else
			{
				offset = new Vector3(offset.x, offset.y, offset.z += zoom);
			}
		}



		Vector3 desiredPosition = target.position + offset;

		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
		transform.position = smoothedPosition;

		transform.LookAt(target);
	}

}