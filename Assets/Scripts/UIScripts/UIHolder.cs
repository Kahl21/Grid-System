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
    //character Select UI
    CharacterSelectUI _csUI;

    //Grid Setup UI
    GridSetupUI _gridUI;

    //FightSetUp Vars
    List<DraggableCharacter> _playerCharacters;
    public List<DraggableCharacter> GetPlayerTeam { get { return _playerCharacters; } }

    //Fight UI
    BattleUI _battleUI;
    CameraFollow _gridCamera;

    bool _castingForClicks = false, _inBattle = false;

    RenderTexture _textureforgridcam;

    //Init is the first thing that happens
    private void Awake()
    {
        SpellBook.LoadSpellBook();
        LoadUIRefs();
        _csUI.gameObject.SetActive(true);
        _gridUI.gameObject.SetActive(false);
        _battleUI.gameObject.SetActive(false);
       
    }

    //for resetting game 
    public void Reawake()
    {
        _csUI.gameObject.SetActive(true);
        _gridUI.gameObject.SetActive(false);
        _battleUI.gameObject.SetActive(false);
    }

    private void Update()
    {
        GameUpdate.CheckUpdate();
    }

    //grabs all relevent UI scripts from children
    void LoadUIRefs()
    {
        _csUI = transform.GetChild(0).GetComponent<CharacterSelectUI>();
        _csUI.Init(this);
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

        _gridUI.Init(this, _battleUI);
        _gridUI.gameObject.SetActive(true);
    }

    //called when grid setup is complete, start battle on grid
    public void StartFight(int enemies, CameraFollow battleCam)
    {
        _gridCamera = battleCam;
        _textureforgridcam = _gridCamera.GetComponent<Camera>().targetTexture;
        _gridCamera.GetComponent<Camera>().targetTexture = null;
        _gridCamera.GetComponent<Camera>().enabled = true;
        Camera.main.enabled = false;
        _gridUI.gameObject.SetActive(false);

        //_gridCamera.Init();
        _gridCamera.HardResetCamera();
        
        //HistoryHandler.Init(this);
        FightHandler.Init(enemies, _playerCharacters);
        _battleUI.gameObject.SetActive(true);
        _battleUI.Init(this, _gridCamera);
        _castingForClicks = false;
        GridHandler.StopSelection();
        //Debug.Log("starting Player Control");
        _inBattle = true;
    }

   
}
