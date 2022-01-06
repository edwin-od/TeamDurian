using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : GridMoveable
{
    [SerializeField, Range(0.1f, 10f)] private float projectileLifetime = 5f;
    [SerializeField, Range(0.1f, 10f)] private float projectileSpeed = 1.15f;

    private bool skipBeat = false;

    private static PlayerController _instance;
    public static PlayerController Instance { get { return _instance; } }
    void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this.gameObject);
    }

    private void OnEnable()
    {
        Tempo.OnBeat += Beat;
        Tempo.OnIntervalBeatStart += BeatIntervalStart;
        Tempo.OnIntervalBeatEnd += BeatIntervalEnd;
    }

    private void OnDisable()
    {
        Tempo.OnBeat -= Beat;
        Tempo.OnIntervalBeatStart -= BeatIntervalStart;
        Tempo.OnIntervalBeatEnd -= BeatIntervalEnd;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Shoot();

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Z))
            MoveTile(DIRECTION.UP);
        if (Input.GetKeyDown(KeyCode.S))
            MoveTile(DIRECTION.DOWN);
        if (Input.GetKeyDown(KeyCode.D))
            MoveTile(DIRECTION.RIGHT);
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Q))
            MoveTile(DIRECTION.LEFT);
    }

    private void MoveTile(DIRECTION Direction)
    {
        if (!skipBeat)
        {
            if (Tempo.Instance)
            {
                if (Tempo.Instance.IsTempoPaused || !Tempo.Instance.IsTempoRunning)
                    return;
                if (Tempo.Instance.IsTempoActive && !Tempo.Instance.IsOnBeat)
                {
                    skipBeat = true;
                    return;
                }
            }

            Move(Direction);
            skipBeat = true;
        }
    }

    private void Shoot()
    {
        if (!skipBeat)
        {
            if (Tempo.Instance)
            {
                if (Tempo.Instance.IsTempoPaused || !Tempo.Instance.IsTempoRunning)
                    return;
                if (Tempo.Instance.IsTempoActive && !Tempo.Instance.IsOnBeat)
                {
                    skipBeat = true;
                    return;
                }
            }

            StartCoroutine(ShootProjectile());
            skipBeat = true;
        }
    }

    public override void Beat() { }

    private void BeatIntervalStart() { }

    private void BeatIntervalEnd() { skipBeat = false; }

    private IEnumerator ShootProjectile()
    {
        if (GridManager.Instance)
        {
            float tx = Time.realtimeSinceStartup;
            float tpause = 0;
            float elapsedTime = 0f;
            while (elapsedTime < MOVE_SPEED)
            {
                if (Tempo.Instance && !Tempo.Instance.IsTempoPaused)
                {
                    float deltaTime = 0f;

                    // Manage Pause Compensation
                    if (tpause != 0) { deltaTime = tpause - tx; tpause = 0; }
                    else { deltaTime = Time.realtimeSinceStartup - tx; }

                    tx = Time.realtimeSinceStartup;

                    // Shoot

                    elapsedTime += deltaTime;
                }
                else if (Tempo.Instance && Tempo.Instance.IsTempoPaused && tpause == 0)
                    tpause = Time.realtimeSinceStartup;

                yield return null;
            }
        }
    }
}
