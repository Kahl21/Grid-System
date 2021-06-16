using UnityEngine;
using UnityEngine.UI;

public enum UIInteractions
{
    NONE,
    FREE,
    ZOOMED,
    MENUOPEN,
    ABILITYOPEN,
    MOVESELECT,
    ATTACKSELECT,
    ABILITYSELECT,
    ABILITYUSE
}

public class BattleUI : MonoBehaviour
{
    UIHolder _uiRef;

    GameObject _selector;

    Ability _selectedAbility;
    Vector2 _abilityTarget;

    //upper UI
    Text _heightText;
    GameObject _timeline;
    BattleTimeLine _timeRef;
    public BattleTimeLine GetTimeline { get { return _timeRef; } }

    //short description
    GameObject _shortHolder;
    Image _shortPort;
    Text _shortHealth;
    Text _shortMana;

    //detailed description
    CharacterDescription _detailHolder;

    //ending message
    BattleEnding _endingHolder;

    //battle menu
    CharacterOptions _optionsRef;
    CameraFollow _battleCam;
    public Camera BattleCamera { get { return _battleCam.GetComponent<Camera>(); } }

    UIInteractions _interactState = UIInteractions.NONE;
    public UIInteractions GetInteractionState { get { return _interactState; } set { _interactState = value; } }

    bool _initialized;


    //initalize
    public void Init(CameraFollow camref)
    {
        if (!_initialized)
        {
            _uiRef = UIHolder.UIInstance;
            _battleCam = camref;
            _selector = Instantiate<GameObject>(Resources.Load<GameObject>("GridObjects/Selector"));
            _selector.SetActive(false);
            SetUI();
        }
        else
        {
            _timeRef.ContinueFight();
        }

        _battleCam.ChangeToBattleMode();
        _interactState = UIInteractions.FREE;
    }

    //grabs all relevent UI components on children
    void SetUI()
    {
        GameObject heightholder = transform.GetChild(0).gameObject;
        _heightText = heightholder.transform.GetChild(0).GetComponent<Text>();

        _timeline = transform.GetChild(1).gameObject;
        _timeRef = _timeline.GetComponent<BattleTimeLine>();
        _timeRef.Init(this);

        _shortHolder = transform.GetChild(2).gameObject;
        _shortPort = _shortHolder.transform.GetChild(0).GetComponent<Image>();
        _shortHealth = _shortHolder.transform.GetChild(1).GetComponent<Text>();
        _shortMana = _shortHolder.transform.GetChild(2).GetComponent<Text>();
        _shortHolder.SetActive(false);

        _optionsRef = transform.GetChild(3).GetComponent<CharacterOptions>();
        _optionsRef.Init();

        _detailHolder = transform.GetChild(4).GetComponent<CharacterDescription>();
        _detailHolder.Init();

        _endingHolder = transform.GetChild(5).GetComponent<BattleEnding>();
        _endingHolder.Init();

        _initialized = true;

        _timeRef.ContinueFight();
    }

    public void CheckNextTurn(Character currentchara)
    {
        _battleCam.FocusPosition(GridHandler.RetrieveToken(currentchara.CurrentPosition).gameObject, CameraModes.MOVING);

        if (_timeRef.GetCurrentTurnCharacter.Team == TeamType.PLAYER)
        {
            Debug.Log("Player turn");
            GameUpdate.PlayerSubscribe += PlayerInteract;
            _interactState = UIInteractions.MENUOPEN;
            currentchara.StartTurn();
            _optionsRef.ShowUI(currentchara);
        }
        else
        {
            Debug.Log("enemy turn");

            currentchara.StartTurn();
        }
    }

    //Sets the height number of height UI
    public void SetHeight(float height)
    {
        _heightText.text = "H " + height.ToString();
    }

    //Interactions with 3D space that changes depending on 
    void PlayerInteract()
    {
        Ray ray = _battleCam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        CheckingHeight(ray, hit);
        MovingSelector(ray, hit);
        CheckingCharacter(ray, hit); 
        UndoSelection();

        switch (_interactState)
        {
            case UIInteractions.NONE:
                break;
            case UIInteractions.FREE:
                LookingMode(ray, hit);
                break;
            case UIInteractions.ZOOMED:
                CheckForExitingClick();
                break;
            case UIInteractions.MENUOPEN:
                UndoSelection();
                break;
            case UIInteractions.MOVESELECT:
                MoveInteract(ray, hit);
                break;
            case UIInteractions.ATTACKSELECT:
                AttackInteract(ray, hit);
                break;
            case UIInteractions.ABILITYSELECT:
                AbilityInteract(ray, hit);
                break;
            case UIInteractions.ABILITYUSE:
                FinalizeAbility();
                break;
            default:
                break;
        }
    }

    void CheckingHeight(Ray mouse, RaycastHit point)
    {
        //Debug.DrawRay(ray.origin, ray.direction, Color.black);
        //Debug.Log("casting");
        if (Physics.Raycast(mouse, out point))
        {
            //if hits something

            //Debug.Log("hit something");
            Collider objCollider = point.collider;

            //set height UI
            SetHeight(objCollider.transform.position.y);
        }
        else
        {
            //Set height to nothing
            SetHeight(0f);
        }
    }
    private void MovingSelector(Ray mouse, RaycastHit point)
    {

        //Debug.DrawRay(ray.origin, ray.direction, Color.black);
        //Debug.Log("casting");
        if (Physics.Raycast(mouse, out point))
        {
            //if hits something

            //Debug.Log("hit something");
            Collider objCollider = point.collider;

            _selector.SetActive(true);

            if (_interactState == UIInteractions.FREE)
            {
                _selector.transform.position = objCollider.transform.position;
            }
            else
            {
                if (objCollider.GetComponent<Tile>())
                {
                    if (objCollider.GetComponent<Tile>().IsTargetable)
                    {
                        _selector.transform.position = objCollider.transform.position;
                    }
                }
                else if (objCollider.GetComponent<GridToken>())
                {
                    if (objCollider.GetComponent<GridToken>().GetTile.IsTargetable)
                    {
                        _selector.transform.position = objCollider.transform.position;
                    }
                }
            }
        }
        else
        {
            //Set height to nothing
            _selector.SetActive(false);
        }
    }

    void CheckingCharacter(Ray mouse, RaycastHit point)
    {

        if (Physics.Raycast(mouse, out point))
        {
            //if hits something

            //Debug.Log("hit something");
            Collider objCollider = point.collider;

            if (objCollider.GetComponent<Tile>())
            {
                //hover over tile
                //set short character UI

                //Debug.Log("tile hover");
                Tile tile = objCollider.GetComponent<Tile>();

                if (tile.PersonOnMe != null)
                {
                    HighlightCharacter(tile);
                }
                else
                {
                    UnHighlightCharacter();
                }
            }
            else if (objCollider.GetComponent<GridToken>())
            {
                //Debug.Log("character hover");
                GridToken gt = objCollider.GetComponent<GridToken>();

                HighlightCharacter(gt.GetTile);
            }
        }
        else
        {
            //turn off character detail
            UnHighlightCharacter();
        }
    }

    void LookingMode(Ray mouse, RaycastHit point)
    {
        if (_battleCam.GetCameraMode == CameraModes.FREE && !_optionsRef.IsMoving)
        {

                //Debug.DrawRay(ray.origin, ray.direction, Color.black);
                //Debug.Log("casting");
                if (Physics.Raycast(mouse, out point))
                {
                    //if hits something

                    //Debug.Log("hit something");
                    Collider objCollider = point.collider;

                    if (objCollider.GetComponent<Tile>())
                    {
                        //hover over tile
                        //set short character UI

                        //Debug.Log("tile hover");
                        Tile tile = objCollider.GetComponent<Tile>();
                        if (tile.PersonOnMe != null)
                        {
                            if (Input.GetMouseButtonDown(0))
                            {
                                //if left click
                                //move camera to focus on character

                                ShowActionsMenu(tile);
                            }
                            else if (Input.GetMouseButtonDown(1))
                            {
                                //if right click
                                //focus and zoom on character
                                //show long detail on right side

                                ShowDetail(tile);
                            }
                        }
                    }
                    else if (objCollider.GetComponent<GridToken>())
                    {
                        //Debug.Log("character hover");
                        GridToken gt = objCollider.GetComponent<GridToken>(); 
                        
                        if (Input.GetMouseButtonDown(0))
                        {
                            //if left click
                            //move camera to focus on character

                            ShowActionsMenu(gt.GetTile);
                        }
                        else if (Input.GetMouseButtonDown(1))
                        {
                            //if right click
                            //focus and zoom on character
                            //show long detail on right side

                            ShowDetail(gt.GetTile);
                        }
                    }
                }
            
            
        }
    }

    void MoveInteract(Ray mouse, RaycastHit point)
    {
        if (Physics.Raycast(mouse, out point))
        {
            //if hits something

            //Debug.Log("hit something");
            Collider objCollider = point.collider;

            if (objCollider.GetComponent<Tile>())
            {
                //hover over tile
                //set short character UI

                Tile tile = objCollider.GetComponent<Tile>();
                if (tile.IsTargetable && Input.GetMouseButtonDown(0))
                {
                    _battleCam.CameraFullStop();
                    GridHandler.DijkstraMove(_timeRef.GetCurrentTurnCharacter, tile);
                }
            }
        }
    }

    void AttackInteract(Ray mouse, RaycastHit point)
    {
        if (Physics.Raycast(mouse, out point))
        {
            //if hits something

            //Debug.Log("hit something");
            Collider objCollider = point.collider;

            if (objCollider.GetComponent<Tile>())
            {
                //hover over tile
                //set short character UI

                InitiateAttack(objCollider.GetComponent<Tile>());
                
            }
            else if (objCollider.GetComponent<GridToken>())
            {
                InitiateAttack(objCollider.GetComponent<GridToken>().GetTile);
            }
        }
    }

    void InitiateAttack(Tile tile)
    {
        if (tile.IsTargetable && Input.GetMouseButtonDown(0) && tile.PersonOnMe != null)
        {
            _battleCam.CameraFullStop();
            _timeRef.GetCurrentTurnCharacter.Attack(GridHandler.RetrieveCharacter(tile.GetXPosition, tile.GetYPosition));
        }
    }

    public void AbilitySelected(Ability ability)
    {
        _selectedAbility = ability;
        GridHandler.ShowReleventGrid(_timeRef.GetCurrentTurnCharacter.CurrentPosition, _selectedAbility.TargetRange, Color.red, Actions.ATTACK);
        _interactState = UIInteractions.ABILITYSELECT;
    }

    void AbilityInteract(Ray mouse, RaycastHit point)
    {
        if (Physics.Raycast(mouse, out point))
        {
            //if hits something

            //Debug.Log("hit something");
            Collider objCollider = point.collider;

            if (objCollider.GetComponent<Tile>())
            {
                Tile tile = objCollider.GetComponent<Tile>();
                if (tile.IsTargetable && Input.GetMouseButtonDown(0))
                {
                    _abilityTarget = new Vector2(tile.GetXPosition, tile.GetYPosition);
                    GridHandler.ShowReleventGrid(_abilityTarget, _selectedAbility.SplashRange, Color.red, Actions.ABILITY);
                    _interactState = UIInteractions.ABILITYUSE;
                    _battleCam.FocusPosition(tile.gameObject, CameraModes.ZOOMING);
                }

            }
            else if(objCollider.GetComponent<GridToken>())
            {
                GridToken gt = objCollider.GetComponent<GridToken>();
                if (gt.GetTile.IsTargetable && Input.GetMouseButtonDown(0))
                {
                    _abilityTarget = new Vector2(gt.GetTile.GetXPosition, gt.GetTile.GetYPosition);
                    GridHandler.ShowReleventGrid(_abilityTarget, _selectedAbility.SplashRange, Color.red, Actions.ABILITY);
                    _interactState = UIInteractions.ABILITYUSE;
                    _battleCam.FocusPosition(gt.gameObject, CameraModes.ZOOMING);
                }
            }
        }
    }

    void FinalizeAbility()
    {
        if (Input.GetMouseButtonDown(0) && GridHandler.CheckForEnemyWithinSpashZone(_abilityTarget, _selectedAbility.SplashRange))
        {
            _timeRef.GetCurrentTurnCharacter.Strategy.HasAttacked = true;
            _optionsRef.HideUI();
            _timeRef.GetCurrentTurnCharacter.UseSkill(_selectedAbility, GridHandler.RetrieveCharacter(_abilityTarget));
            _battleCam.FocusPosition(GridHandler.RetrieveTile(_abilityTarget).gameObject, CameraModes.MOVING); 
        }
    }

    public void CharacterDoneMoving(GridToken currChar)
    {
        if (_timeRef.GetCurrentTurnCharacter.Team == TeamType.PLAYER)
        {
            if (!_timeRef.GetCurrentTurnCharacter.CheckForMoreMove(currChar.GetTile.GetXPosition, currChar.GetTile.GetYPosition))
            {
                _interactState = UIInteractions.MOVESELECT;
            }
            else
            {
                _optionsRef.ShowUI();
                _interactState = UIInteractions.MENUOPEN;
            }
        }
        else
        {
            _timeRef.GetCurrentTurnCharacter.Strategy.ContinueTurn();
        }

        _battleCam.FocusPosition(currChar.gameObject, CameraModes.MOVING);
    }

    public void CharacterDoneAttacking()
    {
        if (_timeRef.GetCurrentTurnCharacter.Team == TeamType.PLAYER)
        {
            _timeRef.GetCurrentTurnCharacter.Strategy.HasAttacked = true;
            _optionsRef.ShowUI();
            _interactState = UIInteractions.MENUOPEN;
            _battleCam.FocusPosition(GridHandler.RetrieveToken(_timeRef.GetCurrentTurnCharacter.CurrentPosition).gameObject, CameraModes.MOVING);
        }
        else
        {
            _timeRef.GetCurrentTurnCharacter.Strategy.ContinueTurn();
         
        }
    }

    //if zoomed and click, unzoom and put details off screen
    void CheckForExitingClick()
    {
        if (!_battleCam.IsMoving)
        {
            if (Input.GetMouseButtonDown(0) || CancelButtonPressed())
            {
                _interactState = UIInteractions.NONE;
                UnShowDetail();
            }
        }
    }

    void UndoSelection()
    {
        if (!_optionsRef.IsMoving)
        {
            if (CancelButtonPressed())
            {
                switch (_interactState)
                {
                    case UIInteractions.MENUOPEN:
                        if (!_optionsRef.IsHidden)
                        {
                            HideActionsMenu();
                        }
                        else
                        {
                            _optionsRef.BackToMenu();
                            _optionsRef.GetAbilityPanel.ResetFading();
                        }
                        break;
                    case UIInteractions.MOVESELECT:
                        _battleCam.FocusPosition(GridHandler.RetrieveToken(_timeRef.GetCurrentTurnCharacter.CurrentPosition).gameObject, CameraModes.MOVING);
                        _interactState = UIInteractions.MENUOPEN;
                        _optionsRef.BackToMenu();
                        _optionsRef.GetAbilityPanel.ResetFading();
                        break;
                    case UIInteractions.ATTACKSELECT:
                        _battleCam.FocusPosition(GridHandler.RetrieveToken(_timeRef.GetCurrentTurnCharacter.CurrentPosition).gameObject, CameraModes.MOVING);
                        _interactState = UIInteractions.MENUOPEN;
                        _optionsRef.BackToMenu();
                        _optionsRef.GetAbilityPanel.ResetFading();
                        break;
                    case UIInteractions.ABILITYSELECT:
                        _battleCam.FocusPosition(GridHandler.RetrieveToken(_timeRef.GetCurrentTurnCharacter.CurrentPosition).gameObject, CameraModes.MOVING);
                        _interactState = UIInteractions.MENUOPEN;
                        _optionsRef.ShowOnlyAbilites();
                        break;
                    case UIInteractions.ABILITYUSE:
                        _battleCam.FocusPosition(GridHandler.RetrieveTile(_abilityTarget).gameObject, CameraModes.MOVING);
                        AbilitySelected(_selectedAbility);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    bool CancelButtonPressed()
    {
        //Debug.Log("Button canceling check");
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetMouseButtonDown(1))
        {
            //Debug.Log("back button pressed");
            return true;
        }
        else
        {
            //Debug.Log("back button not pressed");
            return false;
        }
    }

    //activates the short character detail UI
    void HighlightCharacter(Tile highlightedToken)
    {
        if (!_shortHolder.activeSelf)
        {
            //get character info from GridHandler by sending GridToken Position
            Character highlightedCharacter = GridHandler.RetrieveCharacter(highlightedToken.GetXPosition, highlightedToken.GetYPosition);
            //Debug.Log(highlightedCharacter.ClassName);

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
        if (_shortHolder.activeSelf)
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

        _detailHolder.ShowUI(clickedOnCharacter);
        _battleCam.FocusPosition(clickedOnToken.gameObject, CameraModes.ZOOMING);
    }

    //Turns off long character details
    void UnShowDetail()
    {
        //Debug.Log("detail unshown");
        _detailHolder.HideUI();
        _battleCam.ResetCamera();
    }
    
    //turn on character interaction UI
    void ShowActionsMenu(Tile chara)
    {
        _battleCam.FocusPosition(chara.gameObject, CameraModes.MOVING);
        
        Character gridChar = GridHandler.RetrieveCharacter(chara.GetXPosition, chara.GetYPosition);
        if(gridChar == _timeRef.GetCurrentTurnCharacter && !_optionsRef.IsMoving)
        {
            _interactState = UIInteractions.MENUOPEN;
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
        //Debug.Log("hide called");
        _optionsRef.ResetUIMovements();
        _interactState = UIInteractions.FREE;
        _selectedAbility = null;
    }

    public void EndCurrentTurn()
    {
        GameUpdate.ClearPlayerSubscriptions();
        HideActionsMenu();
        _timeRef.EndOfTurn();
    }

    public void EndBattle()
    {
        Debug.Log("ending called");
        HideActionsMenu();
        _optionsRef.GetAbilityPanel.ResetAbilityButtons();
        _timeRef.ResetTimeline();
        _endingHolder.StartEnding();
    }
}
