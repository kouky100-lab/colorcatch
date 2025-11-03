using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3f;
    Transform player;
    bool hasCaughtPlayer = false; //  Prevent multiple triggers

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null || hasCaughtPlayer) return;

        // Move toward the player
        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            speed * Time.deltaTime
        );
    }

    void OnCollisionEnter(Collision collision)
    {
        // When the enemy hits the player
        if (collision.gameObject.CompareTag("Player") && !hasCaughtPlayer)
        {
            hasCaughtPlayer = true; //  Prevent multiple game over triggers
            Debug.Log("Enemy caught the player!");

            // Trigger Game Over from GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerCaught();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //  Support "Is Trigger" colliders too
        if (other.CompareTag("Player") && !hasCaughtPlayer)
        {
            hasCaughtPlayer = true;
            Debug.Log("Enemy caught the player! (Trigger)");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerCaught();
            }
        }
    }
}
