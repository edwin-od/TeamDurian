using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using SpriteToParticlesAsset;

public class EnemyController : GridMoveable
{
    private int beat, action;

    public EnemyPattern movementPattern;
    public Vector3 scaleA = new Vector3(.5f, 1, .8f);
    public Vector3 scaleB = new Vector3(.8f, 1, .5f);

    public float radius = 8;
    public float strength = 1;
    public float strengthMax = 10;
    public float angle = 135;
    public float rotationAngle = 0;

    public GameObject desintegrateEnemyPrefab;

    private void Awake()
    {
        beat = 1;
        action = 0;
        LevelManager.Instance.RegisterEnemy(this);
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

        beatLength = Tempo.Instance.TempoPeriod * .98f;

        //testing ....
        var timePerBeat = Tempo.Instance.TempoPeriod / 2;

        transform.GetChild(0).DOScale(scaleA, timePerBeat).SetEase(Ease.OutExpo).OnComplete(() => transform.GetChild(0).DOScale(scaleB, timePerBeat).SetEase(Ease.InExpo));

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

    private void OnDestroy()
    {
        LevelManager.Instance.UnregisterEnemy(this);
    }
}
