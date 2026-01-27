using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EvilScary : MonoBehaviour
{
    public AudioClip bounceSfx;
    public AudioClip preparingRushSfx;
    public AudioClip jumpscareSfx;
    public GameObject jumpscareObject;
    public AudioSource sfxSource;
    public AudioSource spawnaudio;
    public float bounceStep = 15f;
    public float normalInterval = 2f;
    public float rushInterval = 0.5f;
    public int teleportsBeforePrepare = 6;
    public float prepareDelay = 2.5f;
    public float playerHpKillDelay = 2.25f;
    public float pitchMin = 0.5f;
    public float pitchMax = 1.25f;
    public float pitchDistanceRadius = 50f;

    SphereCollider sphereCollider;
    bool canKill;
    bool paused;

    void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        StartCoroutine(MainLoop());
        StartCoroutine(DisableAfterAudio(spawnaudio));
    }

    IEnumerator MainLoop()
    {
        int counter = 0;
        while (true)
        {
            if (paused)
            {
                yield return null;
                continue;
            }

            NOTHINGPERSONALKID();
            counter++;

            if (counter < teleportsBeforePrepare)
            {
                yield return new WaitForSeconds(normalInterval);
                continue;
            }

            if (preparingRushSfx != null)
                sfxSource.PlayOneShot(preparingRushSfx);

            yield return new WaitForSeconds(prepareDelay);

            yield return StartCoroutine(DoRushTeleports());

            counter = 0;
            yield return new WaitForSeconds(normalInterval);
        }
    }

    IEnumerator DoRushTeleports()
    {
        for (int i = 0; i < teleportsBeforePrepare; i++)
        {
            if (paused) yield break;
            NOTHINGPERSONALKID();
            yield return new WaitForSeconds(rushInterval);
        }
    }

    void NOTHINGPERSONALKID()
    {
        if (paused) return;

        var nm = MonoSingleton<NewMovement>.Instance;
        Vector3 targetPos = nm.transform.position;
        Vector3 dir = targetPos - transform.position;
        float dist = dir.magnitude;
        if (dist <= 0.001f) return;

        dir.Normalize();
        float move = Mathf.Min(bounceStep, dist);
        transform.position += dir * move;
        transform.LookAt(targetPos);

        float t = Mathf.Clamp01(dist / pitchDistanceRadius);
        sfxSource.pitch = Mathf.Lerp(pitchMax, pitchMin, t);
        sfxSource.PlayOneShot(bounceSfx);
        sfxSource.pitch = 1f;

        StartCoroutine(KillWindow());
    }

    IEnumerator KillWindow()
    {
        canKill = true;
        yield return new WaitForSeconds(0.25f);
        canKill = false;
    }

    IEnumerator uhohbigmistakev1()
    {
        jumpscareObject.SetActive(true);
        sfxSource.PlayOneShot(jumpscareSfx);
        MonoSingleton<NewMovement>.Instance.GetHurt(9999, false);
        paused = true;
        yield return new WaitForSeconds(playerHpKillDelay);

        jumpscareObject.SetActive(false);

        yield return new WaitForSeconds(30f);
        paused = false;

        enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!canKill || paused) return;

        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyIdentifier>().InstaKill();
            sfxSource.PlayOneShot(jumpscareSfx);
            return;
        }

        if (other.CompareTag("Player"))
        {
            StartCoroutine(uhohbigmistakev1());
        }
    }

    IEnumerator DisableAfterAudio(AudioSource audioSource)
    {
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        audioSource.gameObject.SetActive(false);
    }
}
