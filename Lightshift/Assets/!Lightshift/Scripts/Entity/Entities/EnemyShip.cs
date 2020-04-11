using UnityEngine;

public class EnemyShip : Entity 
{
    public float moveSpeed = 5; //move speed
    public float rotationSpeed = 5; //speed of turning

    private void Update()
    {
        FollowTarget();
    }

    public void FollowTarget()
    {
        if (targetEntity == null)
            return;

        if (targetEntity.transform.position != transform.position)
        {
            Vector3 lookPos = targetEntity.transform.position - transform.position;
            float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), rotationSpeed * Time.deltaTime);
        }

        transform.position += transform.up * Time.deltaTime * moveSpeed;
    }
}