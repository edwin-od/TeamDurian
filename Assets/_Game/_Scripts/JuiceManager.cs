using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuiceManager : MonoBehaviour
{
    [SerializeField] private KeyCode PostProcessToggle = KeyCode.None;
    private bool ppOn = true;
    public GameObject postprocess;
    public bool IsPostProcessJuiceOn { get { return ppOn; } }

    [SerializeField] private KeyCode TimelineToggle = KeyCode.None;
    private bool timelineOn = true;
    public bool IsTimelineJuiceOn { get { return timelineOn; } }

    [SerializeField] private KeyCode PlayerToggle = KeyCode.None;
    private bool playerOn = true;
    public bool IsPlayerJuiceOn { get { return playerOn; } }

    [SerializeField] private KeyCode EnemyToggle = KeyCode.None;
    private bool enemyOn = true;
    public bool IsEnemyJuiceOn { get { return enemyOn; } }

    [SerializeField] private KeyCode ComboBarToggle = KeyCode.None;
    private bool comboOn = true;
    public bool IsComboBarJuiceOn { get { return comboOn; } }

    [SerializeField] private KeyCode GridToggle = KeyCode.None;
    private bool gridOn = true;
    public bool IsGridJuiceOn { get { return gridOn; } }

    [SerializeField] private KeyCode ShakesToggle = KeyCode.None;
    private bool shakeOn = true;
    public bool IsShakesJuiceOn { get { return shakeOn; } }

    [SerializeField] private KeyCode SyncToggle = KeyCode.None;
    private bool syncOn = true;
    public bool IsSyncJuiceOn { get { return syncOn; } }

    [SerializeField] private KeyCode MovementToggle = KeyCode.None;
    private bool movementOn = true;
    public bool IsMovementOn { get { return movementOn; } }

    [SerializeField] private KeyCode PrecisionToggle = KeyCode.None;
    private bool precisionOn = true;
    public bool PrecisionFeedback { get { return precisionOn; } }


    private static JuiceManager _instance;
    public static JuiceManager Instance { get { return _instance; } }
    void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this.gameObject);
    }

    private void Update()
    {
        if (PostProcessToggle != KeyCode.None && Input.GetKeyDown(PostProcessToggle)) { ppOn = !ppOn; postprocess.gameObject.SetActive(ppOn); }
        if (TimelineToggle != KeyCode.None && Input.GetKeyDown(TimelineToggle)) { timelineOn = !timelineOn; }
        if (PlayerToggle != KeyCode.None && Input.GetKeyDown(PlayerToggle)) { playerOn = !playerOn; }
        if (EnemyToggle != KeyCode.None && Input.GetKeyDown(EnemyToggle)) { enemyOn = !enemyOn; }
        if (ComboBarToggle != KeyCode.None && Input.GetKeyDown(ComboBarToggle)) { comboOn = !comboOn; }
        if (GridToggle != KeyCode.None && Input.GetKeyDown(GridToggle)) { gridOn = !gridOn; }
        if (ShakesToggle != KeyCode.None && Input.GetKeyDown(ShakesToggle)) { shakeOn = !shakeOn; }
        if (SyncToggle != KeyCode.None && Input.GetKeyDown(SyncToggle)) { syncOn = !syncOn; }
        if (MovementToggle != KeyCode.None && Input.GetKeyDown(MovementToggle)) { movementOn = !movementOn; }
        if (PrecisionToggle != KeyCode.None && Input.GetKeyDown(PrecisionToggle)) { precisionOn = !precisionOn; }
    }

}
