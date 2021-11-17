using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



public class Centipede : MonoBehaviour
{
    [SerializeField] private GameObject _headPrefab;
    [SerializeField] private GameObject _bodyPrefab;
    [SerializeField] private GameObject _tailPrefab;
    [SerializeField] private Transform _parent;
    public int BodyLength;
    public float SpacingOffset = 0.1f;

    private Rigidbody2D _head;
    private List<Rigidbody2D> _segments;
    private Rigidbody2D _tail;

    private enum SegmentType 
    {
        Head,
        Body,
        Tail
    }

    private void Start()
    {
        AddBodySegment(SegmentType.Head);

        for (int i = 0; i < BodyLength; i++)
            AddBodySegment(SegmentType.Body);

        AddBodySegment(SegmentType.Tail);
    }

    private void AddBodySegment(SegmentType type) 
    {
        if (_segments == null)
            _segments = new List<Rigidbody2D>();

        var leaderSegment = _segments.LastOrDefault();
        if (leaderSegment == null)
            leaderSegment = _head;
        if (leaderSegment == null)
            leaderSegment = GetComponent<Rigidbody2D>();

        var prefab = _headPrefab;
        switch (type)
        {
            case SegmentType.Body:
                prefab = _bodyPrefab;
                break;
            case SegmentType.Tail:
                prefab = _tailPrefab;
                break;
            case SegmentType.Head:
                prefab = _headPrefab;
                break;
        }

        var offset = new Vector3(0, -SpacingOffset, 0);
        var segment = Instantiate(prefab, leaderSegment.transform.position + offset, leaderSegment.transform.rotation);

        var body = segment.AddComponent<Rigidbody2D>();
        body = SetupRigidBody(body, type == SegmentType.Head);

        var joint = leaderSegment.gameObject.AddComponent<HingeJoint2D>();
        joint.connectedBody = body;
        joint.enableCollision = true;

        body.gameObject.AddComponent<CircleCollider2D>();

        if (type == SegmentType.Body)
            _segments.Add(body);
        else 
        {
            _tail = body;
                
            if (_parent != null)
            body.transform.parent = _parent;
        }
    }


    private Rigidbody2D SetupRigidBody(Rigidbody2D body, bool head = false) 
    {
        body.gravityScale = 0;
        if (head)
            body.mass = 3;
        else body.mass = 0.3f;

        body.angularDrag = 1;
        body.bodyType = RigidbodyType2D.Dynamic;
        
        return body;
    }
}
