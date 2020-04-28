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
        Vector3 targetPos = new Vector3();
        if (targetEntity != null)
            targetPos = targetEntity.transform.position;

        if (targetPos != transform.position)
        {
            Vector3 lookPos = targetPos - transform.position;
            float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), rotationSpeed * Time.deltaTime);
        }

        transform.position += transform.up * Time.deltaTime * moveSpeed;
    }
}