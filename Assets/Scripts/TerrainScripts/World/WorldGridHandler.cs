using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.WSA;

public class WorldGridHandler : MonoBehaviour
{
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

    float _xpadding = 1.1f;
    float _zpadding = 1.1f;

    float _startTime, _currTime;
    float _moveSpeed = .5f;

    CameraFollow _myCamera;
    public CameraFollow GetGridCamera { get { return _myCamera; } }

    BattleUI _batRef;

    int _holesInTheFloor = 0;
    public int NumberOfHoles { get { return _holesInTheFloor; } }
    
    //Initialize
    //Loads in building blocks(planes, cubes) for generation
    public void Init(BattleUI uiref)
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
        _myCamera.Init(uiref, gameObject);
        _myCamera.FocusObject(gameObject, CameraModes.SHOWCASING);
        _batRef = uiref;
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
                _gridTiles[x, y] = GenerateTile(_tileOBJ, (x * _xpadding) - ((_gridTiles.GetLength(0) / 2) * _xpadding), terrainData[x, y].GetTerrainHeight, (y * _zpadding) - ((_gridTiles.GetLength(0) / 2) * _xpadding));
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

    //------------------CHARACTER SPAWNING---------------//

    //spawns in a single Gridtoken for a character
    public void SpawnCharacter(int xTilePos, int zTilePos)
    {
        GameObject newSpawn;
        if (GridHandler.RetrieveCharacter(xTilePos,zTilePos).Team == TeamType.PLAYER)
        {
            newSpawn = Instantiate(_warriorOBJ, _gridTiles[xTilePos, zTilePos].transform.position, Quaternion.identity, null);

        }
        else
        {
            newSpawn = Instantiate(_enemyOBJ, _gridTiles[xTilePos, zTilePos].transform.position, Quaternion.identity, null);

        }
        GridToken tokenref = newSpawn.GetComponent<GridToken>();
        tokenref.GetTile = _gridTiles[xTilePos, zTilePos];
        _gridTiles[xTilePos, zTilePos].PersonOnMe = tokenref;
        _charactersOnField.Add(tokenref);
    }

    //takes a single character off of the board
    void DeleteCharacters()
    {
        for (int i = 0; i < _charactersOnField.Count; i = 0)
        {
            GridToken currentToken = _charactersOnField[i];
            _charactersOnField.Remove(currentToken);
            Destroy(currentToken.gameObject);
        }
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

    public GridToken GetGridToken(Vector2 pos)
    {
        return _gridTiles[(int)pos.x, (int)pos.y].PersonOnMe;
    }

    public void SpawnDamageUI(Vector2 attackpos, string damage)
    {
        Tile spawn = _gridTiles[(int)attackpos.x, (int)attackpos.y];
        Vector3 spawnpos = _batRef.BattleCamera.WorldToScreenPoint(spawn.transform.position);
        GameObject newDamOBJ = Instantiate<GameObject>(_damageTextOBJ, spawnpos, _damageTextOBJ.transform.rotation, _batRef.transform);
        newDamOBJ.GetComponent<Text>().text = damage;
        newDamOBJ.GetComponent<DamageText>().Init();
    }
    
    public void LightUpBoard(List<Vector2> panelsToLightUp, Color col)
    {
        for (int i = 0; i < panelsToLightUp.Count; i++)
        {
            int xpos = (int)panelsToLightUp[i].x;
            int ypos = (int)panelsToLightUp[i].y;

            _gridTiles[xpos, ypos].SetColor(col);
        }
    }

    public void StartCharacterMove(List<Vector2> movePositions)
    {
        _currTileMoves = new List<Tile>();

        for (int i = 0; i < movePositions.Count; i++)
        {
            _currTileMoves.Add(_gridTiles[(int)movePositions[i].x, (int)movePositions[i].y]);
        }

        _currTileMoves[0].PersonOnMe.GetTile = _currTileMoves[_currTileMoves.Count - 1]; 
        
        _currTileMoves[_currTileMoves.Count - 1].PersonOnMe = _currTileMoves[0].PersonOnMe;
        _currTileMoves[0].PersonOnMe = null;

        _startTime = Time.time;
        //DebugBoard();
        GameUpdate.Subscribe += MoveGridToken;
    }

    public void MoveGridToken()
    {
        _currTime = (Time.time - _startTime) / _moveSpeed;

        if(_currTime > 1)
        {
            _currTime = 1;

            _currTileMoves[_currTileMoves.Count - 1].PersonOnMe.transform.position = _currTileMoves[1].transform.position;

            _currTileMoves.RemoveAt(0);
            _startTime = Time.time;

            if(_currTileMoves.Count <= 1)
            {
                ResetPanels();
                _batRef.CharacterDoneMoving(_currTileMoves[0].PersonOnMe);
                GameUpdate.Subscribe -= MoveGridToken;
                return;
            }
        }

        //Debug.Log(_currTileMoves[0].PersonOnMe);
       // Debug.Log(_currTileMoves[1].PersonOnMe);


        _currTileMoves[_currTileMoves.Count-1].PersonOnMe.transform.position = RandomThings.Interpolate(_currTime, _currTileMoves[0].transform.position, _currTileMoves[1].transform.position);
    }

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
