using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class RocketMove : MonoBehaviour
{
    [SerializeField] GameObject[] bullets;
    [SerializeField] GameObject aiming ;
    [SerializeField] GameObject player;
    [SerializeField] float _bulletSpeed= 0.1f;
    Rigidbody rb;
    private bool readyBullet=false;
    private bool launched=false;
    [SerializeField] bool isEnemy=false;
    Vector3 lastVelocity;

    GameObject bullet;
    GameObject aimed;
    private float targetDistace;
    private float bulletDistace;
    private float targetHalfDistance;
    private float simulatedBulletHeight;
    private float totalBulletDistance = 0.0f;
    private Vector3 _bulletBallistic;

    private float _bulletTime = 0.0f;

    private int i = 0;
    
    void Start()
    {
    }

    void Update()
    {
        _bulletTime += Time.deltaTime;
        if (isEnemy)
        {
            if(_bulletTime>2.0f && !readyBullet)
            {
                Destroy(bullet);
                i = 0;
                if (Input.GetKey(KeyCode.O))
                { i = 0; }
                else if (Input.GetKey(KeyCode.P))
                { i = 1; }
                //transform.GetChild(0).transform.position = transform.position;
                //transform.GetChild(0).transform.rotation = transform.rotation;
                aimed = Instantiate(aiming, transform.position, Quaternion.identity);
                aimed.SetActive(true);

                bullet = Instantiate(bullets[i], transform.position, transform.rotation);
                bullet.transform.parent = transform.Find("Head");
                bullet.SetActive(true);

                //bullet.transform.LookAt(aimed.transform.position + new Vector3(0, (aimed.transform.position - transform.position).magnitude / 2, 0));
                //rb = bullet.GetComponent<Rigidbody>();
                readyBullet = true;
            }
            if (readyBullet && !launched)
            {
                aimed.transform.position = player.transform.position;
                transform.transform.Find("Head").LookAt(aimed.transform.position);
            }
            if (_bulletTime > 5.0f && !launched)
            {
                bullet.transform.parent = null;
                launched = true;
                transform.transform.Find("Head").rotation = new Quaternion(0, 0, 0, 0);
                targetDistace = Vector3.Distance(aimed.transform.position, transform.position); 
                //targetDistace = (aimed.transform.position-transform.position).x * (aimed.transform.position - transform.position).x;
                //Debug.Log(targetDistace);
            }
            if (launched)
            {
                // bulletDistace = Vector3.Distance(bullet.transform.position, transform.position);

                //Debug.Log(bulletDistace);
            }
        }
    }
    private void FixedUpdate()
    {
        //ëSéËìÆíeìπåvéZver

        //ìÒéüã»ê¸ver

        if (launched)
        {
            bulletDistace = (bullet.transform.GetChild(0).position - transform.position).x * (bullet.transform.position - transform.position).x;
            if (totalBulletDistance <= targetDistace)
            {
                totalBulletDistance += _bulletSpeed;
                bullet.transform.GetChild(0).localPosition = new Vector3(0.0f, (-1.0f * totalBulletDistance * totalBulletDistance + targetDistace * totalBulletDistance)*0.05f, totalBulletDistance);
                bullet.transform.GetChild(0).localRotation = new Quaternion(Mathf.Cos(Mathf.Atan(-0.1f * totalBulletDistance + 0.05f * targetDistace) / 2), 0, 0, Mathf.Sin(Mathf.Atan(-0.1f * totalBulletDistance + 0.05f * targetDistace) / 2)) * new Quaternion(1, 0, 0, 0);
                bullet.transform.GetChild(0).Rotate(0, 0, 1000* totalBulletDistance/ targetDistace);
            }
            else
            {
                Destroy(aimed);
                launched = false;
                _bulletTime = 0.0f;
                readyBullet = false;
                totalBulletDistance = 0.0f;
            }
        }
        
        //ìÒíiäKåvéZver
        /*
        if (launched)
        {
            if (targetDistace / 4 <= bulletDistace)
            {
                transform.GetChild(0).transform.LookAt(aimed.transform.position);
                bullet.transform.localPosition += new Vector3(0.0f, 0.0f, _bulletSpeed);
                bullet.transform.rotation *= new Quaternion(0.005f, 0.01f, 0.05f, 0.95f);


                if (targetDistace <= bulletDistace)
                {
                    Destroy(aimed);
                    launched = false;
                    _bulletTime = 0.0f;
                    readyBullet = false;
                }
            }
            else
            {
                transform.GetChild(0).transform.localPosition += new Vector3(0.0f, 0.0f, _bulletSpeed);
            }
            lastVelocity = rb.velocity;

        }
        */
    }

}
