using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using SpriteToParticlesAsset;

public class EnemyController : GridMoveable
{
    private int beat, action;

    [SerializeField, Range(0f, 1f)] private float saturation = 0.5f;
    [SerializeField, Range(0f, 1f)] private float tint = 0.5f;

    [HideInInspector] public EnemyPattern movementPattern;
    public Vector3 scaleA = new Vector3(.5f, 1, .8f);
    public Vector3 scaleB = new Vector3(.8f, 1, .5f);

    public float radius = 8;
    public float strength = 1;
    public float strengthMax = 10;
    public float angle = 135;
    public float rotationAngle = 0;

    public GameObject desintegrateEnemyPrefab;

    private Animator animator;
    private SpriteRenderer sprite;

    private void Awake()
    {
        beat = 1;
        action = 0;
        LevelManager.Instance.RegisterEnemy(this);

        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (animator) { animator.SetBool("IsMoving", IsMoving); }
        if (sprite && sprite.flipX && !IsMoving) { sprite.flipX = false; }
    }

    public override void Beat()
    {
        if (!movementPattern || (Tempo.Instance && Tempo.Instance.IsFirstBeatEver))
            return;

        if (beat == (int)movementPattern.beatEvery)
        {
            Action();
            beat = 1;
        }
        else
            beat++;
    }

    void Action()
    {
        if (movementPattern.directions.Count == 0)
            return;

        Move(movementPattern.directions[action]);
        if (PlayerController.Instance && (int)loopTargetTile.y == 0) { PlayerController.Instance.PlayerHit(); }

        if (animator) 
        {
            if (movementPattern.directions[action] == DIRECTION.DOWN || movementPattern.directions[action] == DIRECTION.UP) { animator.SetTrigger("MoveStraight"); }
            if (movementPattern.directions[action] == DIRECTION.LEFT || movementPattern.directions[action] == DIRECTION.RIGHT) 
            {
                if (sprite && movementPattern.directions[action] == DIRECTION.RIGHT) { sprite.flipX = true; }
                animator.SetTrigger("MoveSide"); 
            }
        }

        //testing ....
        var timePerBeat = beatLength / 2;

        //transform.GetChild(0).DOScale(scaleA, timePerBeat).SetEase(Ease.OutExpo).OnComplete(() => transform.GetChild(0).DOScale(scaleB, timePerBeat).SetEase(Ease.InExpo));

        action++;
        if (action >= movementPattern.directions.Count)
            action = 0;
    }

    public void OnDeath(Vector3 projPos)
    {
        GameObject go = Instantiate(desintegrateEnemyPrefab, transform);
        go.transform.parent = gameObject.transform.parent;
        //transform.GetChild(0).transform.localScale = Vector3.one * 0.8f;
        //go.GetComponent<EffectorExplode>().ExplodeAt(projPos, radius, angle, rotationAngle, strength);
        go.GetComponent<EffectorExplode>().ExplodeTest();
        FindObjectOfType<CameraShake>().FireOnce(CameraShake.ShakeForce.Medium);
        //transform.GetChild(0).GetComponent<Collider>().enabled = false;
        Destroy(this.gameObject);
    }

    public void OnDeath()
    {
        GameObject go = Instantiate(desintegrateEnemyPrefab, transform);
        go.transform.parent = gameObject.transform.parent;

        Color newColor = Color.HSVToRGB(Random.Range(0f, 1f), saturation, tint);
        go.GetComponentInChildren<ParticleSystem>().GetComponent<ParticleSystemRenderer>().material = new Material(go.GetComponentInChildren<ParticleSystem>().GetComponent<ParticleSystemRenderer>().material);
        MaterialPropertyBlock color = new MaterialPropertyBlock();
        color.SetColor("_Color", newColor);
        color.SetColor("_EmissionColor", newColor * 4f);
        go.GetComponentInChildren<ParticleSystem>().GetComponent<ParticleSystemRenderer>().SetPropertyBlock(color);

        FindObjectOfType<CameraShake>().FireOnce(CameraShake.ShakeForce.Medium);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        LevelManager.Instance.UnregisterEnemy(this);
    }
}
