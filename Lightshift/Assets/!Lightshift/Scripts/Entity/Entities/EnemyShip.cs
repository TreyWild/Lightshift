//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//public class EnemyShip : Ship
//{
//    public float trackingDistance;
//    public float shootingDistance;

//    private Ship _target;
//    private void Start()
//    {
//        InitShip();
//    }
//    private void Update()
//    {
//        HandleAI();
//        HandleMovement();
//        HandleLightLance();
//        HandlePowerRegen();
//        HandleShieldRegen();
//    }

//    public void HandleAI()
//    {
//        if (TryFindTarget())
//        {
//            if (!IsTargetInFront())
//                TryRotateTowardsTarget();
//            TryMoveTowardsTarget();
//        }
//    }

//    public bool TryFindTarget()
//    {
//        if (_target != null)
//        {
//            if (IsTargetInRange(_target.gameObject))
//                return true;
//            else
//            {
//                _target = null;
//                return false;
//            }
//        }


//        var hitColliders = Physics2D.OverlapCircleAll(transform.position, trackingDistance);

//        float closestShipDistance = int.MaxValue;
//        Ship targetShip = null;

//        for (int i = 0; i < hitColliders.Length; i++)
//        {
//            var ship = hitColliders[i].gameObject.GetComponent<Ship>();

//            if (ship == null)
//                continue;

//            if (ship.teamId == teamId)
//                continue;

//            var distance = Vector2.Distance(ship.transform.position, transform.position);
//            if (distance < closestShipDistance)
//            {
//                closestShipDistance = distance;
//                targetShip = ship;
//            }
//        }

//        if (targetShip != null && IsTargetInRange(targetShip.gameObject))
//        {
//            _target = targetShip;
//            return true;
//        }
//        return false;
//    }

//    public bool IsTargetInRange(GameObject target)
//    {
//        if (target == null)
//            return false;
//        return (Vector2.Distance(target.transform.position, transform.position) < trackingDistance);
//    }

//    public void TryRotateTowardsTarget()
//    {
//        if (_target == null)
//            return;

//        var currentAngle = transform.rotation.eulerAngles.z;
//        var targetAngle = -Math.Atan2(_target.transform.position.x - transform.position.x, _target.transform.position.y - transform.position.y) * Mathf.Rad2Deg;
//        var angleDiff = currentAngle - targetAngle;

//        if (angleDiff > 180)
//            angleDiff -= 360;
//        else if (angleDiff < -180)
//            angleDiff += 360;

//        if (angleDiff < -10)
//            inputFrame.Left = true;
//        else if (angleDiff > 10)
//            inputFrame.Right = true;
//        else if (rigidBody2D.angularVelocity > 0)
//            inputFrame.Right = true;
//        else if (rigidBody2D.angularVelocity < 0)
//            inputFrame.Left = true;
//        else inputFrame.HorizontalInput = 0;
//    }

//    public void TryMoveTowardsTarget()
//    {
//        if (_target == null)
//            return;

//        if (IsTargetInFront())
//            inputFrame.Forwards = true;
//        else inputFrame.Backwards = true;

//    }
//    public bool IsTargetInFront()
//    {
//        if (_target != null)
//        {
//            var currentAngle = transform.eulerAngles.z + 90;
//            var targetAngle = Math.Atan2(_target.transform.position.y - transform.position.y, _target.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
//            var angleDiff = currentAngle - targetAngle;

//            if (angleDiff > 180)
//                angleDiff -= 360;
//            else if (angleDiff < -180)
//                angleDiff += 360;

//            if (angleDiff > -50 && angleDiff < 50 && Vector2.Distance(_target.transform.position, transform.position) < trackingDistance)
//                return true;
//        }
//        return false;
//    }
//}

