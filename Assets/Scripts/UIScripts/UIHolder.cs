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

    //Init
    private void Awake()
    {
        SpellBook.LoadSpellBook();
        LoadUIRefs();
        _csUI.gameObject.SetActive(true);
        _gridUI.gameObject.SetActive(false);
        _battleUI.gameObject.SetActive(false);
       
    }

    public void Reawake()
    {
        _csUI.gameObject.SetActive(true);
        _gridUI.gameObject.SetActive(false);
        _battleUI.gameObject.SetActive(false);
    }

    void LoadUIRefs()
    {
        _csUI = transform.GetChild(0).GetComponent<CharacterSelectUI>();
        _csUI.Init(this);
        _gridUI = transform.GetChild(1).GetComponent<GridSetupUI>();
        _battleUI = transform.GetChild(2).GetComponent<BattleUI>();
    }

    public void CharacterSelectDone(List<DraggableCharacter> playerTeam)
    {
        _playerCharacters = new List<DraggableCharacter>();
        _playerCharacters = playerTeam;

        _csUI.gameObject.SetActive(false);

        _gridUI.Init(this);
        _gridUI.gameObject.SetActive(true);
    }

    public void StartFight(int enemies, CameraFollow battleCam)
    {
        _gridCamera = battleCam;
        _textureforgridcam = _gridCamera.GetComponent<Camera>().targetTexture;
        _gridCamera.GetComponent<Camera>().targetTexture = null;
        _gridCamera.GetComponent<Camera>().enabled = true;
        Camera.main.enabled = false;
        _gridUI.gameObject.SetActive(false);

        //_gridCamera.Init();
        _gridCamera.ResetCamera();
        
        //HistoryHandler.Init(this);
        FightHandler.Init(enemies, _playerCharacters);
        _battleUI.gameObject.SetActive(true);
        _battleUI.Init(this, _gridCamera);
        _castingForClicks = false;
        _inBattle = true;
    }

    private void Update()
    {
        if (_inBattle)
        {
            if (_castingForClicks)
            {
                CheckForExitingClick();
            }
            else
            {
                PlayerInteract();
            }
        }
    }

    void PlayerInteract()
    {
        Ray ray = _gridCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        //Debug.DrawRay(ray.origin, ray.direction, Color.black);
        //Debug.Log("casting");
        if (Physics.Raycast(ray, out hit))
        {

            //Debug.Log("hit something");
            Collider objCollider = hit.collider;

            _battleUI.SetHeight(objCollider.transform.position.y);

            if (objCollider.GetComponent<Tile>())
            {
                //Debug.Log("tile hover");
                Tile tile = objCollider.GetComponent<Tile>();

                if(tile.PersonOnMe != null)
                {
                    ActivateCharacterDetail(tile.PersonOnMe);
                }
            }
            else if (objCollider.GetComponent<GridToken>())
            {
                //Debug.Log("character hover");
                GridToken gt = objCollider.GetComponent<GridToken>();

                _battleUI.HighlightCharacter(gt);
                ActivateCharacterDetail(gt);
            }
        }
        else
        {
            _battleUI.UnHighlightCharacter();
            _battleUI.SetHeight(0f);
        }
    }

    public void CheckForExitingClick()
    {
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            _battleUI.UnShowDetail();
            _castingForClicks = false;
        }
    }

    public void ActivateCharacterDetail(GridToken token)
    {
        if (Input.GetMouseButtonDown(0))
        {
            _battleUI.ShowActionsMenu(token);
        }
        else if(Input.GetMouseButtonDown(1))
        {
            _battleUI.ShowDetail(token);
            _castingForClicks = true;
        }
    }
}
