using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum PlayerClasses
{
    WARRIOR,
    MAGE
}


public class UIHolder : MonoBehaviour
{
    //singleton
    static UIHolder _uiInstance;
    public static UIHolder UIInstance { get { return _uiInstance; }}

    //character Select UI
    CharacterSelectUI _csUI;

    //Grid Setup UI
    GridSetupUI _gridUI;

    //FightSetUp Vars
    List<DraggableCharacter> _playerCharacters;
    public List<DraggableCharacter> GetPlayerTeam { get { return _playerCharacters; } }

    //Fight UI
    BattleUI _battleUI;
    public BattleUI GetBattleUI { get { return _battleUI; } }
    CameraFollow _gridCamera;

    //Init is the first thing that happens
    private void Awake()
    {
        if (_uiInstance != null && _uiInstance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _uiInstance = this;
        }

        SpellBook.LoadSpellBook();
        LoadUIRefs();
        ShowCharacterSelect();
    }

    private void Update()
    {
        GameUpdate.CheckUpdate();
    }

    public void ShowCharacterSelect()
    {
        _csUI.gameObject.SetActive(true);
        _gridUI.gameObject.SetActive(false);
        _battleUI.gameObject.SetActive(false);
        _csUI.Init();
    }
    //grabs all relevent UI scripts from children
    void LoadUIRefs()
    {
        _csUI = transform.GetChild(0).GetComponent<CharacterSelectUI>();
        _gridUI = transform.GetChild(1).GetComponent<GridSetupUI>();
        _battleUI = transform.GetChild(2).GetComponent<BattleUI>();
    }

    //called when the player is done with the character select screen
    //sets up grid setup menu
    public void CharacterSelectDone(List<DraggableCharacter> playerTeam)
    {
        _playerCharacters = new List<DraggableCharacter>();
        _playerCharacters = playerTeam;

        _csUI.gameObject.SetActive(false);

        _gridUI.gameObject.SetActive(true);
        _gridUI.Init();
    }

    //called when grid setup is complete, start battle on grid
    public void StartFight(int enemies, CameraFollow battleCam)
    {
        _gridCamera = battleCam;
        //Camera.main.enabled = false;
        _gridUI.gameObject.SetActive(false);

        //_gridCamera.Init();
        _gridCamera.HardResetCamera();
        
        FightHandler.Init(enemies, _playerCharacters);
        _battleUI.gameObject.SetActive(true);
        _battleUI.Init(_gridCamera);
        WorldGridHandler.WorldInstance.ResetPanels();
        //Debug.Log("starting Player Control");
    }

    public void ResetToTeamSelect()
    {
        GameUpdate.ClearPlayerSubscriptions();
        GameUpdate.ClearWorldSubscriptions();
        ShowCharacterSelect();
        _gridCamera.FocusPosition(Vector3.zero, CameraModes.MOVING);
        //Camera.main.enabled = true;
    }

    public void DebugUpdate()
    {
        Debug.Log("PlayerUpdate: " + GameUpdate.PlayerSubscribe);
        Debug.Log("UIUpdate: " + GameUpdate.UISubscribe);
        Debug.Log("WorldUpdate: " + GameUpdate.ObjectSubscribe);
    }
}
