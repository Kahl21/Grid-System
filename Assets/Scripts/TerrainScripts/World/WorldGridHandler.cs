using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldGridHandler : MonoBehaviour
{
    //singleton
    static WorldGridHandler _worldInstance;
    public static WorldGridHandler WorldInstance { get { return _worldInstance; } }

    GameObject _tileOBJ;
    GameObject _wallblickOBJ;
    GameObject _enemyOBJ;
    GameObject _warriorOBJ;
    GameObject _damageTextOBJ;
    Tile[,] _gridTiles;
    List<GameObject> _stackablefloor;
    List<Tile> _playerStartTiles;
    List<GridToken> _charactersOnField;
    List<Tile> _currTileMoves;
    List<DamageText> _damageOnScreen;
    GridToken _currMoveToken;

    float _xpadding = 1f;
    float _zpadding = 1f;

    float _startTime, _currTime;
    float _moveSpeed = .5f;

    CameraFollow _myCamera;
    public CameraFollow GetGridCamera { get { return _myCamera; } }

    BattleUI _batRef;

    int _holesInTheFloor = 0;
    public int NumberOfHoles { get { return _holesInTheFloor; } }

    bool _initialized = false;
    
    //Initialize
    //Loads in building blocks(planes, cubes) for generation
    public void Init()
    {
        if (_worldInstance != null && _worldInstance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _worldInstance = this;
        }

        if (!_initialized)
        {
            _tileOBJ = Resources.Load<GameObject>("GridObjects/Tile");
            _wallblickOBJ = Resources.Load<GameObject>("GridObjects/Wallblock");
            _enemyOBJ = Resources.Load<GameObject>("GridObjects/EnemyPrefab");
            _warriorOBJ = Resources.Load<GameObject>("GridObjects/WarriorPrefab");
            _damageTextOBJ = Resources.Load<GameObject>("GridObjects/DamageTextPrefab");
            _charactersOnField = new List<GridToken>();
            _myCamera = transform.GetChild(0).GetComponent<CameraFollow>();
            _myCamera.GetComponent<AudioListener>().enabled = false;
            _myCamera.transform.SetParent(null);
            _myCamera.Init(gameObject);
            _batRef = UIHolder.UIInstance.GetBattleUI;
            _initialized = true;
        }
        else
        {
            _myCamera.Init(gameObject);
        }

        _damageOnScreen = new List<DamageText>();
        _myCamera.FocusPosition(gameObject, CameraModes.SHOWCASING);
    }

    //Uses GridHandler data
    //creates a grid in 3D space using building blocks in the RESOURCES folder
    public void GenerateWorldGrid(TerrainSpace[,] terrainData)
    {
        _holesInTheFloor = 0;

        //if there is alrady a board, delete it
        if (transform.childCount > 0)
        {
            DeleteBoard();
        }

        //creates all the tiles in 3D depending on Terrain Data in GridHandler
        _gridTiles = new Tile[terrainData.GetLength(0), terrainData.GetLength(1)];

        for (int y = 0; y < _gridTiles.GetLength(1); y++)
        {
            for (int x = 0; x < _gridTiles.GetLength(0); x++)
            {
                _gridTiles[x, y] = GenerateTile(_tileOBJ, (x * _xpadding) - ((_gridTiles.GetLength(0) / 2) * _xpadding) + (_xpadding/2), terrainData[x, y].GetTerrainHeight, (y * _zpadding) - ((_gridTiles.GetLength(0) / 2) * _zpadding) + (_zpadding / 2));
                _gridTiles[x, y].Init(terrainData[x, y].GetTerrainColor, x, y);
                if (terrainData[x, y].GetTerrainType == 'O')
                {
                    _gridTiles[x, y].gameObject.SetActive(false);
                    _holesInTheFloor++;
                }
            }
        }

        SetPlayerbase();
        GenerateWalls();    
    }

    //Creates a single tile in World space
    Tile GenerateTile(GameObject tile, float xPos, float height, float zPos) 
    {
        Vector3 spawnPos = new Vector3(xPos, height, zPos);

        GameObject newTileObj = Instantiate(tile, spawnPos, tile.transform.rotation, transform);
        Tile newTile = newTileObj.GetComponent<Tile>();

        return newTile;
    }

    //Generates a stack of blocks to create walls
    //blocks are .5 units in height
    //checks tile Y position and spawn the necessary amount
    void GenerateWalls()
    {
        _stackablefloor = new List<GameObject>();

        for (int y = 0; y < _gridTiles.GetLength(1); y++)
        {
            for (int x = 0; x < _gridTiles.GetLength(0); x++)
            {
                GenerateWall(_gridTiles[x,y]);
            }
        }
    }

    //creates a single block in World space
    void GenerateWall(Tile currbuildingTile)
    {
        Vector3 tilePos = currbuildingTile.transform.position;

        if (tilePos.y > 0 && currbuildingTile.gameObject.activeInHierarchy)
        { 
            for (float i = 0; i < tilePos.y - .1f; i+=.5f)
            {
                Vector3 spawnPos = new Vector3(tilePos.x, i, tilePos.z);
                GameObject newWallPiece = Instantiate(_wallblickOBJ, spawnPos, _wallblickOBJ.transform.rotation, transform);

                _stackablefloor.Add(newWallPiece);
            }
        }
    }

    //Takes 8 (max amount of player characters) spaces in a rectangle
    //makes them the "player base" AKA places the player characters can spawn
    void SetPlayerbase()
    {
        _playerStartTiles = new List<Tile>();

        int playerNum = 8;

        if(playerNum%2 != 0)
        {
            playerNum++;
        }

        int midpoint = _gridTiles.GetLength(0) / 2;
        int offset = playerNum / 4;

        for (int i = 0; i < 2; i++)
        {
            for (int v = midpoint - offset; v < midpoint + offset; v++)
            {
                _gridTiles[v, i].gameObject.SetActive(true);
                _gridTiles[v, i].SetColor(Color.cyan);
                _playerStartTiles.Add(_gridTiles[v,i]);
                
            }
        }
    }

    

    //------------------CHARACTER SPAWNING---------------//

    //spawns in a single Gridtoken for a character
    public void SpawnEnemy(int xTilePos, int zTilePos)
    {
        GameObject newSpawn;
        
        newSpawn = Instantiate(_enemyOBJ, _gridTiles[xTilePos, zTilePos].transform.position, Quaternion.identity, null);

        GridToken tokenref = newSpawn.GetComponent<GridToken>();
        tokenref.GetTile = _gridTiles[xTilePos, zTilePos];
        _gridTiles[xTilePos, zTilePos].PersonOnMe = tokenref;
        _charactersOnField.Add(tokenref);
    }

    //spawns in player characters
    //spawns as Gridtokens
    public void SpawnPlayer(Character newPlayer)
    {
        GameObject newSpawn;
        
        if (newPlayer.GetType() == typeof(Warrior))
        {
            newSpawn = Instantiate(_warriorOBJ, _gridTiles[_playerStartTiles[0].GetXPosition, _playerStartTiles[0].GetYPosition].transform.position, Quaternion.identity, null);
        }
        else
        {
            Debug.Log("didnt work");
            newSpawn = Instantiate(_warriorOBJ, _gridTiles[_playerStartTiles[0].GetXPosition, _playerStartTiles[0].GetYPosition].transform.position, Quaternion.identity, null);
        }

        GridToken tokenref = newSpawn.GetComponent<GridToken>();
        tokenref.GetTile = _gridTiles[_playerStartTiles[0].GetXPosition, _playerStartTiles[0].GetYPosition];
        _gridTiles[_playerStartTiles[0].GetXPosition, _playerStartTiles[0].GetYPosition].PersonOnMe = tokenref;
        newPlayer.CurrentPosition = new Vector2(_playerStartTiles[0].GetXPosition, _playerStartTiles[0].GetYPosition);
        _playerStartTiles.RemoveAt(0);
        _charactersOnField.Add(tokenref);
    }

    

    //------------------BOARD INTERACTIONS---------------//

    void DebugBoard()
    {
        string board = "";
        for (int y = 0; y < _gridTiles.GetLength(1); y++)
        {
            for (int x = 0; x < _gridTiles.GetLength(0); x++)
            {
                string _currentTile;
                if (_gridTiles[x, y].PersonOnMe == null)
                {
                    _currentTile = "[ ] ";
                }
                else
                {
                    _currentTile = "[O] ";
                }
                board += _currentTile;
            }
            board += "\n";
        }

        Debug.Log("World Grid: \n" + board);
    }

    //takes in a position in the grid and returns a gridtoken
    public GridToken GetGridToken(Vector2 pos)
    {
        return _gridTiles[(int)pos.x, (int)pos.y].PersonOnMe;
    }

    //take in a position in the Grid and return one of the tiles in the scene
    public Tile GetTile(Vector2 pos)
    {
        return _gridTiles[(int)pos.x, (int)pos.y];
    }

    //spawn a single UI onto the Canvas to reflect damage
    public void SpawnDamageUI(Vector2 attackpos, string damage)
    {
        Tile spawn = _gridTiles[(int)attackpos.x, (int)attackpos.y];
        Vector3 spawnpos = _batRef.BattleCamera.WorldToScreenPoint(spawn.transform.position);
        GameObject newDamOBJ = Instantiate<GameObject>(_damageTextOBJ, spawnpos, _damageTextOBJ.transform.rotation, _batRef.transform);
        newDamOBJ.GetComponent<Text>().text = damage;
        newDamOBJ.GetComponent<DamageText>().Init(spawn.transform.position);
        ResetPanels();
        _batRef.CharacterDoneAttacking();
    }
    
    //highlight tiles colors to reflect what player is trying to target
    public void LightUpBoard(List<Vector2> panelsToLightUp, Color col)
    {
        for (int i = 0; i < panelsToLightUp.Count; i++)
        {
            int xpos = (int)panelsToLightUp[i].x;
            int ypos = (int)panelsToLightUp[i].y;

            _gridTiles[xpos, ypos].SetColor(col);
        }
    }

    //takes in a list of positions
    //finds tiles int the same postions and adds those to a list 
    //this list is used to go through positions to move Gridtokens one position at a time
    public void StartCharacterMove(List<Vector2> movePositions)
    {

        ResetPanels();
        _currTileMoves = new List<Tile>();

        for (int i = 0; i < movePositions.Count; i++)
        {
            _currTileMoves.Add(_gridTiles[(int)movePositions[i].x, (int)movePositions[i].y]);
            //Debug.Log(_currTileMoves[i]);
        }

        //Debug.Log(_currTileMoves[0].PersonOnMe.name);
        _currMoveToken = _currTileMoves[0].PersonOnMe;

        int lasttile = _currTileMoves.Count - 1;
        _currMoveToken.GetTile = _currTileMoves[lasttile]; 
        
        _currTileMoves[lasttile].PersonOnMe = _currMoveToken;
        _currTileMoves[0].PersonOnMe = null;

        //Debug.Log("before move, " + _currTileMoves.Count);
        _startTime = Time.time;
        //DebugBoard();
        GameUpdate.ObjectSubscribe += MoveGridToken;
    }

    //method that moves a single gridtoken around the scene
    //moves characters one tile at a time, going through a list of positions 
    public void MoveGridToken()
    {
        _currTime = (Time.time - _startTime) / _moveSpeed;

        if(_currTime > 1)
        {
            _currTime = 1;

            //Debug.Log("current move, " + _currTileMoves.Count);
            _currMoveToken.transform.position = _currTileMoves[1].transform.position;

            _currTileMoves.RemoveAt(0);
            _startTime = Time.time;

            //Debug.Log("after move, " + _currTileMoves.Count);
            if (_currTileMoves.Count <= 1)
            {
                GameUpdate.ObjectSubscribe -= MoveGridToken;
                Debug.Log("ending movement");
                _batRef.CharacterDoneMoving(_currTileMoves[0].PersonOnMe);
                return;
            }

            _currTime = 0;
        }

        //Debug.Log(_currTileMoves[0].PersonOnMe);
       // Debug.Log(_currTileMoves[1].PersonOnMe);

        if(_currTileMoves.Count > 1)
        {
            _currMoveToken.transform.position = RandomThings.Interpolate(_currTime, _currTileMoves[0].transform.position, _currTileMoves[1].transform.position);
        }
    }

    //a single gridtoken and its gameobject from the scene
    public void RemoveGridPiece(Vector2 posToRemove)
    {
        if(_gridTiles[(int)posToRemove.x, (int)posToRemove.y].PersonOnMe != null)
        {
            GameObject piece = _gridTiles[(int)posToRemove.x, (int)posToRemove.y].PersonOnMe.gameObject;
            _charactersOnField.Remove(_gridTiles[(int)posToRemove.x, (int)posToRemove.y].PersonOnMe);
            _gridTiles[(int)posToRemove.x, (int)posToRemove.y].PersonOnMe = null;
            Destroy(piece);
        }
    }
    
    //takes and deletes all gridtokens off of the board
    void DeleteCharacters()
    {
        for (int i = 0; i < _charactersOnField.Count; i = 0)
        {
            GridToken currentToken = _charactersOnField[i];
            Destroy(currentToken.gameObject);
            _charactersOnField.RemoveAt(0);
        }

        _charactersOnField = new List<GridToken>();
    }
    
    //Deletes the board so a new one can be built
    //deletes all building blocks and empties all Lists
    void DeleteBoard()
    {
        for (int y = 0; y < _gridTiles.GetLength(1); y++)
        {
            for (int x = 0; x < _gridTiles.GetLength(0); x++)
            {
                GameObject objDeletion = _gridTiles[x, y].gameObject;
                _gridTiles[x, y] = null;
                Destroy(objDeletion);
            }
        }

        for (int i = 0; i < _stackablefloor.Count; i = 0)
        {
            GameObject currentfloorpiece = _stackablefloor[i];
            _stackablefloor.Remove(currentfloorpiece);
            Destroy(currentfloorpiece);
        }

        if (_charactersOnField.Count > 0)
        {
            DeleteCharacters();
        }
    }

    //resets all panels to their base colors
    public void ResetPanels()
    {
        for (int x = 0; x < _gridTiles.GetLength(0); x++)
        {
            for (int y = 0; y < _gridTiles.GetLength(1); y++)
            {
                _gridTiles[x, y].ResetColor();
            }
        }
    }
}
