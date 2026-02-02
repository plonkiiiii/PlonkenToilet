using UnityEngine;

public class Chaser : MonoBehaviour
{
    public static Chaser? Instance { get; private set; }

    public float distanceFromTarget = 1000f;
    public float stopDistance = 5f;
    public float acceleration = 2f;
    public float maxSpeed = 50f;
    public float foundRange = 100f;
    public GameObject miniGameCanvasPrefab;
    public AudioClip searchClip;
    public AudioClip foundClip;

    float speed;
    bool foundTriggered;
    bool minigameSpawned;
    AudioSource source;
    Transform target;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        target = MonoSingleton<NewMovement>.Instance.gameObject.transform;
        source = GetComponent<AudioSource>();

        Vector2 dir2D = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = target.position + new Vector3(dir2D.x, dir2D.y, 0f) * distanceFromTarget;
        transform.position = spawnPos;

        speed = 0;
        source.clip = searchClip;
        source.Play();
    }

    void Update()
    {
        Vector3 toTarget = target.position - transform.position;
        float distance = toTarget.magnitude;

        if (distance > stopDistance)
        {
            Vector3 dir = toTarget.normalized;
            transform.rotation = Quaternion.LookRotation(dir);

            speed = Mathf.MoveTowards(speed, maxSpeed, acceleration * Time.deltaTime);
            transform.position += dir * speed * Time.deltaTime;
        }
        else
        {
            speed = 0f;
        }

        if (!foundTriggered && distance <= foundRange)
        {
            foundTriggered = true;
            if (source.isPlaying) source.Stop();
            source.PlayOneShot(foundClip);
        }

        if (!minigameSpawned && distance <= stopDistance)
        {
            minigameSpawned = true;
            if (source.isPlaying) source.Stop();
            Instantiate(miniGameCanvasPrefab);
        }
    }
}
