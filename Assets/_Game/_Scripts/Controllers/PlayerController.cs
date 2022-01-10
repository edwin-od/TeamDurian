using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : GridMoveable
{
    [SerializeField, Range(0.1f, 10f)] private float projectileLifetime = 5f;
    [SerializeField, Range(0.1f, 10f)] private float projectileSpeed = 2f;
    [SerializeField, Range(0.1f, 10f)] private float projectileHitRadius = 0.1f;
    [SerializeField, Range(1f, 10000)] private int projectileHitScore = 1;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject projectileSpawn;

    [SerializeField] private bool ignoreBeatRestriction = false;

    private int score = 0;

    private DIRECTION currentDirection = DIRECTION.UP;

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
        /*if (Input.GetKeyDown(KeyCode.UpArrow))
            Shoot(DIRECTION.UP);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            Shoot(DIRECTION.DOWN);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Shoot(DIRECTION.LEFT);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            Shoot(DIRECTION.RIGHT);*/

        if(Input.GetKeyDown(KeyCode.Space))
            Shoot(DIRECTION.UP);

        /*if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Z))
            MoveTile(DIRECTION.UP);
        if (Input.GetKeyDown(KeyCode.S))
            MoveTile(DIRECTION.DOWN);*/
        if (Input.GetKeyDown(KeyCode.D))
            MoveTile(DIRECTION.RIGHT);
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Q))
            MoveTile(DIRECTION.LEFT);
    }

    public int Score { get { return score; } }

    private void MoveTile(DIRECTION Direction)
    {
        if (ignoreBeatRestriction) { Move(Direction); return; }

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

    private void Shoot(DIRECTION direction)
    {
        if(ignoreBeatRestriction) { StartCoroutine(ShootProjectile(direction)); return; }

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

            StartCoroutine(ShootProjectile(direction));
            skipBeat = true;
        }
    }

    private void KilledEnemy(EnemyController enemy)
    {
        Destroy(enemy.gameObject);
        score += projectileHitScore;
    }

    public override void Beat() { }

    private void BeatIntervalStart() { }

    private void BeatIntervalEnd() { skipBeat = false; }

    private IEnumerator ShootProjectile(DIRECTION shootDirection)
    {
        if (GridManager.Instance && Tempo.Instance)
        {
            RotatePlayer(shootDirection);
            GameObject projectile = Instantiate(projectilePrefab);
            projectile.transform.parent = null;
            projectile.transform.position = projectileSpawn.transform.position;
            projectile.transform.rotation = projectileSpawn.transform.rotation;

            float tx = Time.realtimeSinceStartup;
            float tpause = 0;
            float elapsedTime = 0f;
            Vector2 direction = shootDirection == DIRECTION.UP ? UP : shootDirection == DIRECTION.DOWN ? DOWN : shootDirection == DIRECTION.RIGHT ? RIGHT : shootDirection == DIRECTION.LEFT ? LEFT : Vector2.zero;
            while (elapsedTime < projectileLifetime)
            {
                if (!Tempo.Instance.IsTempoPaused)
                {
                    float deltaTime = 0f;

                    // Manage Pause Compensation
                    if (tpause != 0) { deltaTime = tpause - tx; tpause = 0; }
                    else { deltaTime = Time.realtimeSinceStartup - tx; }

                    tx = Time.realtimeSinceStartup;

                    projectile.transform.position += new Vector3(direction.x, 0f, direction.y) * deltaTime * projectileSpeed;
                    Collider[] hits = Physics.OverlapSphere(projectile.transform.position, projectileHitRadius);
                    foreach (Collider hit in hits) 
                    {
                        EnemyController enemy = hit.gameObject.GetComponentInParent<EnemyController>();
                        if (enemy)
                        {
                            KilledEnemy(enemy);
                            elapsedTime = projectileLifetime;
                        }
                    }

                    elapsedTime += deltaTime;
                }
                else if (Tempo.Instance.IsTempoPaused && tpause == 0)
                    tpause = Time.realtimeSinceStartup;

                yield return null;
            }

            Destroy(projectile);
        }
    }

    private void RotatePlayer(DIRECTION direction)
    {
        if (currentDirection == direction)
            return;

        currentDirection = direction;
        float newAngle = direction == DIRECTION.UP ? 0 : direction == DIRECTION.DOWN ? 180 : direction == DIRECTION.RIGHT ? 90 : direction == DIRECTION.LEFT ? -90 : 0;

        transform.GetChild(0).rotation = Quaternion.Euler(0f, newAngle, 0f);
    }
}
