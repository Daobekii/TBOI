using System.Collections;
using UnityEngine;

public enum EnemyState
{
    Wander,
    Follow,
    Die,
};

public class EnemyController : MonoBehaviour
{
    GameObject player;
    public EnemyState currentState = EnemyState.Wander;

    public float detectionRange;
    public float moveSpeed;
    private float currentHP;
    public float maxHP = 22f;
    private bool chooseDirection = false;
    private bool isDead = false;
    private Vector3 randomDirection;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentHP = maxHP;
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Wander:
                Wander();
                break;
            case EnemyState.Follow:
                Follow();
                break;
            case EnemyState.Die:
                Die();
                break;
        }

        if (isPlayerInRange(detectionRange) && currentState != EnemyState.Die)
        {
            currentState = EnemyState.Follow;
        }
        else if (!isPlayerInRange(detectionRange) && currentState != EnemyState.Die)
        {
            currentState = EnemyState.Wander;
        }
    }

    private bool isPlayerInRange(float range)
    {
        return Vector2.Distance(transform.position, player.transform.position) <= range;
    }

    private IEnumerator ChooseDirection()
    {
        chooseDirection = true;
        yield return new WaitForSeconds(Random.Range(2f, 8f));
        randomDirection = new Vector3(0, 0, Random.Range(0, 360));
        Quaternion newRotation = Quaternion.Euler(randomDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Random.Range(0.5f, 2.5f));
        chooseDirection = false;
    }

    void Wander()
    {
        if (!chooseDirection)
        {
            StartCoroutine(ChooseDirection());
        }
        transform.position += -transform.right * moveSpeed * Time.deltaTime;
        if (isPlayerInRange(detectionRange))
        {
            currentState = EnemyState.Follow;
        }
    }

    void Follow()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
        if (!isPlayerInRange(detectionRange))
        {
            currentState = EnemyState.Wander;
        }
    }

    void Die()
    {
        isDead = true;
        // Implement death behavior (e.g., play animation, drop loot)
        Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentState = EnemyState.Die;
        }
    }
}
