using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class WorldGridHandler : MonoBehaviour
{
    GameObject _tileOBJ;
    GameObject _wallblickOBJ;
    GameObject _characterOBJ;
    Tile[,] _gridTiles;
    List<GameObject> _stackablefloor;
    List<Tile> _playerStartTiles;
    List<Tile> _tilesPlayerIsOn;
    List<GridToken> _charactersOnField;

    float _xpadding = 1.1f;
    float _zpadding = 1.1f;

    CameraFollow _myCamera;
    public CameraFollow GetGridCamera { get { return _myCamera; } }

    int _holesInTheFloor = 0;
    public int NumberOfHoles { get { return _holesInTheFloor; } }
    
    public void Init()
    {
        _tileOBJ = Resources.Load<GameObject>("GridObjects/Tile");
        _wallblickOBJ = Resources.Load<GameObject>("GridObjects/Wallblock");
        _characterOBJ = Resources.Load<GameObject>("GridObjects/CharacterPrefab");
        _charactersOnField = new List<GridToken>(); 
        _myCamera = transform.GetChild(0).GetComponent<CameraFollow>();
        _myCamera.GetComponent<AudioListener>().enabled = false;
        _myCamera.transform.SetParent(null);
        _myCamera.Init(gameObject);
        _myCamera.FocusObject(gameObject, CameraModes.SHOWCASING);
    }

    public void GenerateWorldGrid(TerrainSpace[,] terrainData)
    {
        _holesInTheFloor = 0;

        if (transform.childCount > 0)
        {
            DeleteBoard();
        }

        _gridTiles = new Tile[terrainData.GetLength(0), terrainData.GetLength(1)];

        for (int y = 0; y < _gridTiles.GetLength(1); y++)
        {
            for (int x = 0; x < _gridTiles.GetLength(0); x++)
            {
                _gridTiles[x, y] = GenerateTile(_tileOBJ, (x * _xpadding) - ((_gridTiles.GetLength(0) / 2) * _xpadding), terrainData[x, y].GetTerrainHeight, (y * _zpadding) - ((_gridTiles.GetLength(0) / 2) * _xpadding), terrainData[x, y].GetTerrainColor);
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

    Tile GenerateTile(GameObject tile, float xPos, float height, float zPos, Color tileColor) 
    {
        Vector3 spawnPos = new Vector3(xPos, height, zPos);


        GameObject newTileObj = Instantiate(tile, spawnPos, tile.transform.rotation, transform);
        Tile newTile = newTileObj.GetComponent<Tile>();

        newTile.Init(tileColor);

        return newTile;
    }

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

    public void SpawnCharacter(Character spawningCharacter, int xTilePos, int zTilePos)
    {
        GameObject newSpawn = Instantiate(_characterOBJ, _gridTiles[xTilePos, zTilePos].transform.position, Quaternion.identity, null);
        GridToken tokenref = newSpawn.GetComponent<GridToken>();
        tokenref.GetCharacter = spawningCharacter;
        _charactersOnField.Add(tokenref);
    }

    void DeleteCharacters()
    {
        for (int i = 0; i < _charactersOnField.Count; i = 0)
        {
            GridToken currentToken = _charactersOnField[i];
            _charactersOnField.Remove(currentToken);
            Destroy(currentToken.gameObject);
        }
    }
}
