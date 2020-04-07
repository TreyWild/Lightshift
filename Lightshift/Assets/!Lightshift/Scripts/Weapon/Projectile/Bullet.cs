using UnityEngine;


public class Bullet : MonoBehaviour
{
    private Rigidbody2D _rigidBody;
    private TrailRenderer _trailRenderer;
    private Entity _entity;
    
    private float _currentRange;
    private bool _canTrackTarget;
    private BulletData _bulletData;
    public int uniqueId;
    public void AddRigidBody(Rigidbody2D body) 
    {
        _rigidBody = body;
    }
    public void Initialize(Entity entity, BulletData bulletData, float rotation, Vector2 velocity) 
    {
        _currentRange = 0;
        _entity = entity;
        _rigidBody.rotation = rotation;
        _rigidBody.velocity = velocity * 3;
        _rigidBody.position = entity.transform.position;
    }

    void Update()
    {
        _currentRange += 1.0f * Time.deltaTime;

        if (_bulletData.trackingDelay > _currentRange)
            _canTrackTarget = true;


        if (_currentRange > _bulletData.range)
            Distruct(withSpark: false);

        //if (_canTrackTarget)
        //    TrackTarget();

        //HandleMovement();
    }

    //public void TrackTarget() 
    //{
    //    var target = _entity.targetNeutral;
    //    if (target == null)
    //        return;

    //    var currentAngle = transform.rotation.eulerAngles.z + 90;
    //    var targetAngle = Mathf.Atan2(target.transform.position.y, target.transform.position.x) * 57.29578f;
    //    var angleDiff = currentAngle - targetAngle;

    //    for (int i = 0; i < 2; i++)
    //    {
    //        if (angleDiff > 180)
    //            angleDiff -= 360;
    //        else if (angleDiff < -180)
    //            angleDiff += 360;
    //    }

    //    if (angleDiff > 5)
    //        _inputObject.HorizontalInput = 1;
    //    else if (angleDiff < -5)
    //        _inputObject.HorizontalInput = -1;
    //    else
    //        _inputObject.HorizontalInput = 0;
    //}

    //public void HandleMovement()
    //{
    //    _rigidBody.rotation += -_inputObject.HorizontalInput * _bulletData.guidance * Time.deltaTime;

    //    /* Handle Movement */
    //    if (_inputObject.VerticalInput > 0)
    //    {
    //        if (_rigidBody.velocity.magnitude < _bulletData.speed * 1000.0f)
    //            _rigidBody.AddForce(transform.forward * _inputObject.VerticalInput * 1000.0f * _bulletData.acceleration * Time.deltaTime);
    //    }

    //    transform.position = _rigidBody.position;
    //    transform.rotation = Quaternion.Euler(0, 0, _rigidBody.rotation);
    //}

    void OnEnable()
    {
        if (_trailRenderer == null)
            _trailRenderer = GetComponent<TrailRenderer>();

        if (_bulletData.hasTrail)
        {
            _trailRenderer.emitting = true;
            _trailRenderer.material.SetColor("_TintColor", _bulletData.trailColor);
            _trailRenderer.widthMultiplier = _bulletData.trailSize;
            _trailRenderer.time = _bulletData.trailLength;
            _trailRenderer.sortingOrder = SortingOrders.BULLET_TRAIL;
        }
        else 
        {
            _trailRenderer.Clear();
            _trailRenderer.emitting = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var entity = collision.gameObject.GetComponent<Entity>();

        if (entity.teamId == _entity.teamId || entity.IsInSafezone)
            return;

        if (entity == null)
            gameObject.SetActive(false);

        transform.position = entity.transform.position;
        Distruct();
    }

    public void Distruct(bool withSpark = true)
    {
        if (withSpark)
        {
            var spark = LSObjectPoolManager.Instance.GetUsableEffect(_bulletData.hitEffect);

            if (spark != null)
            {
                spark.transform.position = transform.position;
                spark.SetActive(true);
            }
        }

        _trailRenderer.Clear();
        _trailRenderer.emitting = false;
        gameObject.SetActive(false);
    }
}

