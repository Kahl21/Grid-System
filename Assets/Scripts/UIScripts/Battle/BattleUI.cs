using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum UIInteractions
{
    NONE,
    FREE,
    ZOOMED,
    MOVESELECT,
    ATTACKSELECT,
    SPELLSELECT
}

public class BattleUI : MonoBehaviour
{
    UIHolder _uiRef;

    //upper UI
    Text _heightText;
    GameObject _timeline;

    //short description
    GameObject _shortHolder;
    Image _shortPort;
    Text _shortHealth;
    Text _shortMana;

    //detailed description
    GameObject _detailHolder;
    Image _detPort;
    Text _detCharInfo;
    Vector3 _detailStartPos, _detailEndPos;
    float _currTime, _startTime;
    float _speedDelta = .3f;

    //battle menu
    CharacterOptions _optionsRef;
    CameraFollow _battleCam;

    UIInteractions _interactState = UIInteractions.NONE;
    public UIInteractions GetInsteractionState { get { return _interactState; } set { _interactState = value; } }

    //initalize
    public void Init(UIHolder ui, CameraFollow camref)
    {
        _uiRef = ui;
        _battleCam = camref;
        _interactState = UIInteractions.FREE;

        SetUI();
    }

    //grabs all relevent UI components on children
    void SetUI()
    {
        GameObject heightholder = transform.GetChild(0).gameObject;
        _heightText = heightholder.transform.GetChild(0).GetComponent<Text>();

        GameObject timelineholder = transform.GetChild(1).gameObject;
        _timeline = timelineholder.transform.GetChild(0).gameObject;

        _shortHolder = transform.GetChild(2).gameObject;
        _shortPort = _shortHolder.transform.GetChild(0).GetComponent<Image>();
        _shortHealth = _shortHolder.transform.GetChild(1).GetComponent<Text>();
        _shortMana = _shortHolder.transform.GetChild(2).GetComponent<Text>();
        _shortHolder.SetActive(false);

        _optionsRef = transform.GetChild(3).GetComponent<CharacterOptions>();
        _optionsRef.Init(this);

        _detailHolder = transform.GetChild(4).gameObject;
        _detailStartPos = _detailHolder.GetComponent<RectTransform>().position;
        _detailEndPos = _detailStartPos;
        _detailEndPos.x = Screen.width;
        _detPort = _detailHolder.transform.GetChild(0).GetComponent<Image>();
        _detCharInfo = _detailHolder.transform.GetChild(1).GetComponent<Text>();
        _detailHolder.SetActive(false);

        GameUpdate.Subscribe += PlayerInteract;
    }

    //Sets the height number of height UI
    public void SetHeight(float height)
    {
        _heightText.text = "H " + height.ToString();
    }

    //Interactions with 3D space that changes depending on 
    void PlayerInteract()
    {
        switch (_interactState)
        {
            case UIInteractions.NONE:
                break;
            case UIInteractions.FREE:
                LookingMode();
                break;
            case UIInteractions.ZOOMED:
                CheckForExitingClick();
                break;
            case UIInteractions.MOVESELECT:
                UndoSelection();
                break;
            case UIInteractions.ATTACKSELECT:
                UndoSelection();
                break;
            case UIInteractions.SPELLSELECT:
                UndoSelection();
                break;
            default:
                break;
        }
    }

    void LookingMode()
    {
        if (_battleCam.GetCameraMode == CameraModes.FREE)
        {
            Ray ray = _battleCam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //Debug.DrawRay(ray.origin, ray.direction, Color.black);
            //Debug.Log("casting");
            if (Physics.Raycast(ray, out hit))
            {
                //if hits something

                //Debug.Log("hit something");
                Collider objCollider = hit.collider;

                //set height UI
                SetHeight(objCollider.transform.position.y);

                if (objCollider.GetComponent<Tile>())
                {
                    //hover over tile
                    //set short character UI

                    //Debug.Log("tile hover");
                    Tile tile = objCollider.GetComponent<Tile>();

                    if (tile.PersonOnMe != null)
                    {
                        ActivateCharacterDetail(tile);
                    }
                }
                else if (objCollider.GetComponent<GridToken>())
                {
                    //Debug.Log("character hover");
                    GridToken gt = objCollider.GetComponent<GridToken>();

                    HighlightCharacter(gt.GetTile);
                    ActivateCharacterDetail(gt.GetTile);
                }
            }
            else
            {
                //turn off character detail
                //Set height to nothing
                UnHighlightCharacter();
                SetHeight(0f);
            }
        }
    }

    //if zoomed and click, unzoom and put details off screen
    void CheckForExitingClick()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            _interactState = UIInteractions.NONE;
            UnShowDetail();
        }
    }

    void UndoSelection()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
        {
            _interactState = UIInteractions.FREE;
            _optionsRef.BackToMenu();
        }
    }

    //checks for a click on a tile or Grid token(character rep.)
    void ActivateCharacterDetail(Tile token)
    {
        if (Input.GetMouseButtonDown(0))
        {
            //if left click
            //move camera to focus on character

            ShowActionsMenu(token);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            //if right click
            //focus and zoom on character
            //show long detail on right side

            ShowDetail(token);
        }
    }

    //activates the short character detail UI
    void HighlightCharacter(Tile highlightedToken)
    {
        //get character info from GridHandler by sending GridToken Position
        Character highlightedCharacter = GridHandler.RetrieveCharacter(highlightedToken.GetXPosition, highlightedToken.GetYPosition); ;

        if (!_shortHolder.activeInHierarchy)
        {
            //turn on short detail if it isnt on 
            //plug character info into UI
            _shortPort.sprite = highlightedCharacter.GetSprite;
            _shortHealth.text = highlightedCharacter.CurrHealth.ToString() + "/" + highlightedCharacter.MaxHealth.ToString();
            _shortMana.text = highlightedCharacter.CurrMana.ToString() + "/" + highlightedCharacter.MaxMana.ToString();

            _shortHolder.SetActive(true);
        }
    }

    //Turn off short character detail if it is active
    void UnHighlightCharacter()
    {
        if (_shortHolder.activeInHierarchy)
        {
            _shortHolder.SetActive(false);
        }
    }

    //show the long character details
    void ShowDetail(Tile clickedOnToken)
    {
        _interactState = UIInteractions.NONE;
        //Debug.Log("detail shown");

        HideActionsMenu();

        //get character info from GridHandler by sending GridToken Position
        Character clickedOnCharacter = GridHandler.RetrieveCharacter(clickedOnToken.GetXPosition, clickedOnToken.GetYPosition);

        //Plug in all character details
        _detPort.sprite = clickedOnCharacter.GetSprite;

        string characterInfo = "Team " + clickedOnCharacter.Team + "\n";
        characterInfo += clickedOnCharacter.Name + "\n"
                        + "Weapon: " + clickedOnCharacter.HeldWeapon.Name + " --> " + clickedOnCharacter.HeldWeapon.WeaponElement + "\n\n"
                        + "Health: " + clickedOnCharacter.CurrHealth + "/" + clickedOnCharacter.MaxHealth + "\n"
                        + "Mana: " + clickedOnCharacter.CurrMana + "/" + clickedOnCharacter.MaxMana + "\n"
                        + "Strength: " + (clickedOnCharacter.Offense.Strength + clickedOnCharacter.HeldWeapon.StrengthMod) + " (" + clickedOnCharacter.HeldWeapon.StrengthMod + ")" + "\n"
                        + "Magic: " + (clickedOnCharacter.Offense.Strength + clickedOnCharacter.HeldWeapon.MagicMod) + "(" + clickedOnCharacter.HeldWeapon.MagicMod + ")" + "\n"
                        + "Defense: " + clickedOnCharacter.Defense.BaseDefense + "\n"
                        + "Accuracy: " + (clickedOnCharacter.Offense.Accuracy + clickedOnCharacter.HeldWeapon.Accuracy) + "%" + " (" + clickedOnCharacter.HeldWeapon.Accuracy + ")" + "\n"
                        + "Critical Chance: " + (clickedOnCharacter.Offense.CriticalChance + clickedOnCharacter.HeldWeapon.Crit) + "%" + " (" + clickedOnCharacter.HeldWeapon.Crit + ")" + "\n"
                        + "Speed: " + clickedOnCharacter.Speed + "\n"
                        + "Movement: " + clickedOnCharacter.Movement + "\n\n"
                        + "Resistances: Fire/" + clickedOnCharacter.Defense.FireRes + "\n"
                        + "             Ice/" + clickedOnCharacter.Defense.IceRes + "\n"
                        + "             Thunder/" + clickedOnCharacter.Defense.ThunderRes + "\n"
                        + "             Light/" + clickedOnCharacter.Defense.LightRes + "\n"
                        + "             Dark/" + clickedOnCharacter.Defense.DarkRes + "\n\n";

        _detCharInfo.text = characterInfo;


        //turn on the UI and move it into position
        _detailHolder.SetActive(true);
        _battleCam.FocusObject(clickedOnToken.gameObject, CameraModes.ZOOMING);
        _startTime = Time.time;
        GameUpdate.Subscribe += MoveDetailsOnScreen;
    }

    //Turns off long character details
    void UnShowDetail()
    {
        //Debug.Log("detail unshown");
        _battleCam.ResetCamera();
        _startTime = Time.time;
        GameUpdate.Subscribe += MoveDetailsOffScreen;
    }

    //moves long detail into position
    void MoveDetailsOnScreen() 
    {
        //math interpolate position
        _currTime = (Time.time - _startTime) / _speedDelta;
        //Debug.Log("MovingUI on");
        if (_currTime > 1)
        {
            _currTime = 1;
            //Debug.Log("details on");
            GameUpdate.Subscribe -= MoveDetailsOnScreen;
            _interactState = UIInteractions.ZOOMED;
        }


        //move UI on screen

        _detailHolder.GetComponent<RectTransform>().position = RandomThings.Interpolate(_currTime, _detailStartPos, _detailEndPos);
    }

    void MoveDetailsOffScreen()
    {
        //math interpolate position
        _currTime = (Time.time - _startTime) / _speedDelta;
        //Debug.Log("MovingUI off");
        
        if (_currTime > 1)
        {
            _currTime = 1;
            
            //Debug.Log("details off");
            _detailHolder.SetActive(false);
            GameUpdate.Subscribe -= MoveDetailsOffScreen;
            _interactState = UIInteractions.FREE;
        }

        //move UI off of screen
        _detailHolder.GetComponent<RectTransform>().position = RandomThings.Interpolate(_currTime, _detailEndPos, _detailStartPos);
    }

    //turn on character interaction UI
    void ShowActionsMenu(Tile chara)
    {
        _battleCam.FocusObject(chara.gameObject, CameraModes.MOVING);
        
        Character gridChar = GridHandler.RetrieveCharacter(chara.GetXPosition, chara.GetYPosition);
        if(gridChar.Team == TeamType.PLAYER)
        {
            _optionsRef.ShowUI(gridChar);
        }
        else
        {
            HideActionsMenu();
        }
    }
    
    //Turn off character Interaction UI
    public void HideActionsMenu()
    {
        Debug.Log("hide called");
        _optionsRef.ResetUIMovements();
    }
}
