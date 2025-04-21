using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyManeger : MonoBehaviour
{
    [SerializeField] GameObject _player;
    [SerializeField] float enemySpeed = 0.1f;

    Animator animator;

    public enum EnemyDirection
    {
        right,
        left,
    }
    public EnemyDirection enemyDirection;

    void Start()
    {
        animator = GetComponent<Animator>();
        enemyDirection = EnemyDirection.right;
    }

    void Update()
    {
        if (enemyDirection == EnemyDirection.right && transform.position.x >= 1)
        {
            animator.SetBool("rightWalk", false);
            animator.SetBool("leftWalk", true);
            enemyDirection = EnemyDirection.left;
            enemySpeed *= -1.0f;
        }
        if (enemyDirection == EnemyDirection.left && transform.position.x <= -1)
        {
            animator.SetBool("leftWalk", false);
            animator.SetBool("rightWalk", true);
            enemyDirection = EnemyDirection.right;
            enemySpeed *= -1.0f;
        }
        if (_player.GetComponent<InnerPlayerControler>().BulletHit == true)
        {
            transform.position = new Vector3(0, 2, 0);
        }
        else 
        {
            
        }
        
    }
    private void FixedUpdate()
    {
        transform.position += new Vector3(enemySpeed, 0, 0);
    }
}
