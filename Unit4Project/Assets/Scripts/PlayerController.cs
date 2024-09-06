using System.Collections;
using System.Collections.Generic;
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
    GameObject tmpRocket;
    Coroutine powerupCountdown;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalpoint = GameObject.Find("Focal Point");
    }

    // Update is called once per frame
    void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalpoint.transform.forward * forwardInput * speed);
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.5f, 0);
        if (currentPowerUp == PowerUpType.Rockets && Input.GetKeyDown(KeyCode.F))
        {
            LaunchRockets(); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            hasPowerup = true;
            powerupIndicator.gameObject.SetActive(true);
            currentPowerUp = other.gameObject.GetComponent<PowerUpType>();
            Destroy(other.gameObject);
            StartCoroutine(PowerupCountdownRoutine());
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
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && currentPowerUp == PowerUpType.Pushback)
        {
            Debug.Log("Colliled with: " + collision.gameObject.name + "powerup set to " + currentPowerUp.ToString());
            Rigidbody enemyRIgidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = (collision.gameObject.transform.position - transform.position);
            enemyRIgidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
        }
    }

    void LaunchRockets()
    { 
        foreach (var enemy in FindObjectsOfType<Enemy>())
        { 
          tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up,Quaternion.identity);
          tmpRocket.GetComponent<RocketBehaviour>().Fire(enemy.transform);
        }
    }
}
