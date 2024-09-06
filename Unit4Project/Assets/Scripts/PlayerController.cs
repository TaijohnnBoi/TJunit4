using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private GameObject focalpoint;
    public float speed = 5.0f;
    public bool hasPowerup = false;
    public float powerupStrength = 15.0f;
    public GameObject powerupIndicator;

    public PowerUpType currentPowerUp = PowerUpType.None;
    public GameObject rocketPrefab;
    private GameObject tmpRocket;
    private Coroutine powerupCountdown;

    public float hangTime;
    public float smashSpeed, explosionForce, explosionRadius;
    private bool smashing = false;
    private float floorY;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalpoint = GameObject.Find("Focal Point");
    }

    void Update()
    {
        HandleMovement();
        HandlePowerUps();
    }

    private void HandleMovement()
    {
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalpoint.transform.forward * forwardInput * speed);
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.5f, 0);
    }

    private void HandlePowerUps()
    {
        if (currentPowerUp == PowerUpType.Rockets && Input.GetKeyDown(KeyCode.F))
        {
            LaunchRockets();
        }

        if (currentPowerUp == PowerUpType.Smash && Input.GetKeyDown(KeyCode.Space) && !smashing)
        {
            Debug.Log("Smash power-up is active. Starting Smash coroutine.");
            smashing = true;
            StartCoroutine(Smash());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            hasPowerup = true;
            powerupIndicator.gameObject.SetActive(true);

            PowerUp powerUpComponent = other.gameObject.GetComponent<PowerUp>();
            if (powerUpComponent != null)
            {
                currentPowerUp = powerUpComponent.powerUpType;
                Debug.Log("Acquired power-up: " + currentPowerUp);
            }
            else
            {
                Debug.LogWarning("PowerUp component not found on: " + other.gameObject.name);
            }

            Destroy(other.gameObject);
            if (powerupCountdown != null)
            {
                StopCoroutine(powerupCountdown);
            }
            powerupCountdown = StartCoroutine(PowerupCountdownRoutine());
        }
    }

    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(7);
        hasPowerup = false;
        currentPowerUp = PowerUpType.None;
        powerupIndicator.gameObject.SetActive(false);
        Debug.Log("Power-up expired. Resetting power-up state.");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && currentPowerUp == PowerUpType.Pushback)
        {
            Debug.Log("Collided with: " + collision.gameObject.name + " powerup set to " + currentPowerUp.ToString());
            Rigidbody enemyRigidBody = collision.gameObject.GetComponent<Rigidbody>();
            if (enemyRigidBody != null)
            {
                Vector3 awayFromPlayer = (collision.gameObject.transform.position - transform.position);
                enemyRigidBody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
            }
        }
    }

    void LaunchRockets()
    {
        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            if (enemy != null)
            {
                Debug.Log($"Launching rocket towards {enemy.name}");
                tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up, Quaternion.identity);
                var rocketBehaviour = tmpRocket.GetComponent<RocketBehaviour>();
                if (rocketBehaviour != null)
                {
                    rocketBehaviour.Fire(enemy.transform);
                }
                else
                {
                    Debug.LogWarning("RocketBehaviour component not found on rocketPrefab.");
                }
            }
        }
    }

    IEnumerator Smash()
    {
        Debug.Log("Smash coroutine started.");
        var enemies = FindObjectsOfType<Enemy>();

        floorY = transform.position.y;
        float jumpTime = Time.time + hangTime;

        // Move the player up
        while (Time.time < jumpTime)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, smashSpeed);
            yield return null;
        }

        // Move the player down
        while (transform.position.y > floorY)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, -smashSpeed * 2);
            yield return null;
        }

        // Apply explosion force to enemies
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
            }
        }

        smashing = false;
        Debug.Log("Smash coroutine completed.");
    }
}
