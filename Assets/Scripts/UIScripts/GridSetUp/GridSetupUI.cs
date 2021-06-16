using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSetupUI : MonoBehaviour
{
    //UI vars
    InputField _numberOfEnemiesText;
    Dropdown _gridSizeDD;
    Dropdown _gridTraitsDD;
    Slider _ElevationSlide;

    //Changeables
    [SerializeField]
    int _numberOfEnemies = 1;
    [SerializeField]
    GridSize _gsChoice;
    [SerializeField]
    GridTraits _gtChoice;
    [SerializeField]
    float _elevation;

    WorldGridHandler _battlefield;

    UIHolder _uiRef;
    bool _initialized = false;

    //initialize
    //loads empty that holds the entire field (all tiles and characters)
    public void Init()
    {
        if (!_initialized)
        {
            _uiRef = UIHolder.UIInstance;

            GameObject field = Resources.Load<GameObject>("GridObjects/BattleFieldHolder");

            GameObject newfield = Instantiate(field, Vector3.zero, Quaternion.identity, null);

            _battlefield = newfield.GetComponent<WorldGridHandler>();
            _battlefield.Init();
            GridHandler.Init();

            _initialized = true;
            SetUI();
        }
        else
        {
            _battlefield.Init();
            GridHandler.Init();
        }

        ChangeGrid();
    }

    //sets all relevent UI
    void SetUI()
    {
        GameObject enemiesholder = transform.GetChild(1).gameObject;
        GameObject enemytextempty = enemiesholder.transform.GetChild(1).gameObject;
        _numberOfEnemiesText = enemytextempty.transform.GetChild(0).GetComponent<InputField>();
        SetEnemiesText();

        GameObject ddHolder = transform.GetChild(2).gameObject;
        _gridSizeDD = ddHolder.transform.GetChild(1).GetComponent<Dropdown>();
        GameObject terrainOptionHolder = ddHolder.transform.GetChild(2).gameObject;
        _gridTraitsDD = terrainOptionHolder.transform.GetChild(2).GetComponent<Dropdown>();
        _ElevationSlide = terrainOptionHolder.transform.GetChild(4).GetComponent<Slider>();
        _gsChoice = GridSize.Small;
        _gtChoice = GridTraits.NONE;
        _elevation = _ElevationSlide.value;
    }

    //sends player into battle
    //
    public void StartFight()
    {
        _uiRef.StartFight(_numberOfEnemies, _battlefield.GetGridCamera);
    }

    //called whenever the player is messing around with the Grid setup UI
    public void ChangeGrid()
    {
        _gsChoice = (GridSize)_gridSizeDD.value;
        _gtChoice = (GridTraits)_gridTraitsDD.value;

        GridHandler.CreateNewGrid(_gsChoice, _gtChoice, _elevation);
        CheckGridPopulationMath();
    }

    //truncates all Y values for the grid spaces to .5 (0, .5, 1, 1.5, etc)
    public void CheckElevationValue()
    {
        float elevationmath = (float)System.Math.Round(_ElevationSlide.value * 2f, System.MidpointRounding.AwayFromZero) / 2;

        if(elevationmath > _elevation || elevationmath < _elevation)
        {
            _elevation = elevationmath;
            ChangeGrid();
        }
    }

    //sets the number of enemies that player wants
    //as a string to the UI component
    void SetEnemiesText()
    {
        _numberOfEnemiesText.text = _numberOfEnemies.ToString();
    }

    //checks math anytime the player changes something
    //makes sure that there cannot be an amount of players that is more than
    //size of the board - max number of player characters
    public void CheckGridPopulationMath()
    {
        int temp = Mathf.Abs(System.Int32.Parse(_numberOfEnemiesText.text));
        //Debug.Log(temp);
        int dimension = GridHandler.GetGridLength;
        dimension *= dimension;
        //Debug.Log(dimension);
        int playerTeamCount = _uiRef.GetPlayerTeam.Count;

        try
        {
            if (playerTeamCount + temp > dimension)
            {
                _numberOfEnemies = dimension - playerTeamCount;
            }
            else
            {
                _numberOfEnemies = temp;
            }
        }
        catch
        {
            _numberOfEnemies = 1;
        }

        SetEnemiesText();
    }
}
