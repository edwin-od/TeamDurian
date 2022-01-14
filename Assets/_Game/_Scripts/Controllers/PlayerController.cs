using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : GridMoveable
{
    [SerializeField, Range(1, 10)] private int totalLifes = 1;
    [SerializeField, Range(0.1f, 10f)] private float projectileLifetime = 5f;
    [SerializeField, Range(0.1f, 10f)] private float projectileSpeed = 4f;
    [SerializeField, Range(0.1f, 10f)] private float projectileHitRadius = 0.1f;
    [SerializeField, Range(1, 10000)] private int killEnemyScore = 1;
    [SerializeField, Range(1, 10000)] private int normalScore = 1;
    [SerializeField, Range(1, 10000)] private int greatScore = 2;
    [SerializeField, Range(1, 10000)] private int perfectScore = 3;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject projectileSpawn;
    [SerializeField] private GameObject walkParticleSpawn;

    //public Vector3 scaleA = new Vector3(0.5f, 1, 0.8f);
    //public Vector3 scaleB = new Vector3(0.8f, 1, 0.5f);

    [SerializeField] private bool ignoreBeatRestriction = false;

    private Animator animator;
    private SpriteRenderer sprite;

    [SerializeField, Range(1f, 5f)] private float comboMultiplier = 1f;
    [SerializeField, Range(1, 100)] private int maxCombo = 15;
    private int lifes = 0;
    private int score = 0;
    private int consecutiveCombos = 0;
    private bool actionOnBeat = false;

    private DIRECTION currentDirection = DIRECTION.UP;

    public delegate void ComboLost();
    public static event ComboLost OnComboLost;

    public delegate void ComboAdd();
    public static event ComboAdd OnComboAdd;

    public delegate void Die();
    public static event Die OnDie;

    public delegate void Miss();
    public static event Miss OnMiss;

    public delegate void NormalBeat();
    public static event NormalBeat OnNormalBeat;

    [SerializeField, Range(0f, 1f)] private float greatThreshold = 0.5f;
    public float GreatThreshold { get { return greatThreshold; } }
    public delegate void GreatBeat();
    public static event GreatBeat OnGreatBeat;

    [SerializeField, Range(0f, 1f)] private float perfectThreshold = 0.1f;
    public float PerfectThreshold { get { return perfectThreshold; } }
    public delegate void PerfectBeat();
    public static event PerfectBeat OnPerfectBeat;

    private bool skipBeat = false;

    private bool isBeatInStart = false, isBeatInEnd = false;

    private static PlayerController _instance;
    public static PlayerController Instance { get { return _instance; } }
    void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this.gameObject);

        lifes = totalLifes;
        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        if (sprite) { sprite.flipX = false; }
        if (walkParticleSpawn) { walkParticleSpawn.SetActive(true); }
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

        if (animator) { animator.SetBool("IsMoving", IsMoving); }
        if (sprite && sprite.flipX && !IsMoving) { sprite.flipX = false; }
        if (walkParticleSpawn && IsMoving && !walkParticleSpawn.activeSelf)
        {
            walkParticleSpawn.SetActive(true);
            ParticleSystem walk = walkParticleSpawn.GetComponentInChildren<ParticleSystem>();
            if (walk.isPlaying) { walk.Stop(); }
            if (!walk.isPlaying) { walk.Play(); }
        }
        else if (walkParticleSpawn && !IsMoving && walkParticleSpawn.activeSelf) { walkParticleSpawn.SetActive(false); }
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
                    OnMiss?.Invoke();
                    return;
                }
            }

            if (Direction == DIRECTION.LEFT) { if (sprite) { sprite.flipX = true; } }

            Move(Direction);
            DeathNoteController.instance.FireOnce();
            consecutiveCombos = Mathf.Clamp(consecutiveCombos + 1, 0, maxCombo);
            actionOnBeat = true;
            OnComboAdd?.Invoke();

            EvaluateBeat();

            var timePerBeat = beatLength / 2;

            //transform.GetChild(0).DOScale(scaleA, timePerBeat).SetEase(Ease.OutExpo).OnComplete(() => transform.GetChild(0).DOScale(scaleB, timePerBeat).SetEase(Ease.InExpo));
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
                    OnMiss?.Invoke();
                    return;
                }
            }

            StartCoroutine(ShootProjectile(direction));
            actionOnBeat = true;

            if (animator) { animator.SetTrigger("Shoot"); }

            EvaluateBeat();

            skipBeat = true;
        }
    }

    private void KilledEnemy(EnemyController enemy)
    {
        enemy.OnDeath();
        scoreComboMultiplier(killEnemyScore);
    }

    public void PlayerHit()
    {
        lifes--;
        if (lifes == 0) { OnDie?.Invoke(); if (animator) { animator.SetTrigger("Dead"); } if (LevelManager.Instance) { LevelManager.Instance.StopLevel(); } }
    }

    public override void Beat() { if (Tempo.Instance && Tempo.Instance.IsFirstBeatEver) { actionOnBeat = true; } isBeatInStart = false; isBeatInEnd = true; }

    private void BeatIntervalStart() { isBeatInStart = true; isBeatInEnd = false; actionOnBeat = false; }

    private void BeatIntervalEnd() 
    {

        isBeatInStart = false; isBeatInEnd = false;

        skipBeat = false; 
        if (!actionOnBeat) { if (consecutiveCombos > 0) { consecutiveCombos = 0; OnComboLost?.Invoke(); } } 
        actionOnBeat = false; 
    }

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

    private void scoreComboMultiplier(int valueToAdd)
    {
        score += valueToAdd * (1 + Mathf.FloorToInt(consecutiveCombos * comboMultiplier));
        if (UI_Texts.Instance) { UI_Texts.Instance.SetScore(score); }
    }

    public int COMBO { get { return consecutiveCombos; } }
    public int MAX_COMBP { get { return maxCombo; } }

    private void EvaluateBeat()
    {
        if (Tempo.Instance)
        {
            float acceptableInterval = Tempo.Instance.BeatAcceptablePercentage;
            float curPercentage = isBeatInStart ? Tempo.Instance.PercentageToBeat : isBeatInEnd ? (1 - Tempo.Instance.PercentageToBeat) : 0f;
            if (curPercentage <= acceptableInterval && curPercentage > acceptableInterval * greatThreshold) { OnNormalBeat?.Invoke(); scoreComboMultiplier(normalScore); }
            else if (curPercentage <= acceptableInterval * greatThreshold && curPercentage > acceptableInterval * perfectThreshold) { OnGreatBeat?.Invoke(); scoreComboMultiplier(greatScore); }
            else if (curPercentage <= perfectThreshold * greatThreshold) { OnPerfectBeat?.Invoke(); scoreComboMultiplier(perfectScore); }
        }
    }

    public void RestartPlayer()
    {
        score = 0;
        consecutiveCombos = 0;
        lifes = totalLifes;
        if (sprite) { sprite.flipX = false; }
    }
}
