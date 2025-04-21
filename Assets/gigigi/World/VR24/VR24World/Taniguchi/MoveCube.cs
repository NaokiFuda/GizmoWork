using UnityEngine;

public class MoveCube : MonoBehaviour
{
    public Vector3 moveDirection = new Vector3(0f, 1f, 0f);
    public float moveDistance = 5f;
    public float moveSpeed = 2f;

   [SerializeField] private Vector3 startPosition;
   [SerializeField] private Vector3 endPosition;
   [SerializeField] private float _length = 1;
    private bool movingUp = true;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        endPosition = startPosition + moveDirection.normalized * moveDistance;
    }

    // Update is called once per frame
    void Update()
    {
        var value = Mathf.PingPong(Time.time, _length);
        transform.localPosition = new Vector3(0, value, 0);
        MovingCube();
    }

    void MovingCube()
    {
        if (movingUp)
        {
            transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;
            if (Vector3.Distance(transform.position, startPosition) > moveDistance)
            {
                movingUp = false;
            }
        }
    }
}
