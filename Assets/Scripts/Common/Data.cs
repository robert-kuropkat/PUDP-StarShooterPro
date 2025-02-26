using UnityEngine;


//
// Enumerations
//

    
public enum SpawnableTypes
{
     Enemy
   , PowerUp
}

public enum SpwanableNames
{
     Vertical
   , HorizontalRight
   , HorizontalLeft
   , HorizontalZigZagRight
   , HorizontalZagZagLeft
   , Ammo
   , Speed
   , Shield
   , TripleShot
   , Health
   , Spiral
   , NegativeAmmo
}


//
// Data Structures
//


[System.Serializable]
public struct WaveManager
{
    [Tooltip("High and Low value in seconds to spawn next game object.  Array size should be 2.  Element 0 is the lower range, Element 1 the upper.  Values are floats.")]
    public float[]     _spawnRateRange;
    [Tooltip("Total number of enemy ships to spawn, regardless of type.")]
    public int         _numberOfEnemyToSpawn;
    [Range(0,100), Tooltip("Perecentage of enemy to have protective shield.")]
    public int         _shieldedEnemyPercentage;
    [Range(0,100), Tooltip("Perecentage of enemy to have aggressive (ramming) behavior.")]
    public int         _aggressiveEnemyPercentage;
    [Tooltip("Create a new element for each object type to spawn in this wave.  This includes both enemy and powerups.")]
    public Spawnable[] _spawnableObjects;
    public WaveManager( int         numberOfEnemyToSpawn
                      , int         shieldedEnemyPercentage
                      , int         aggressiveEnemyPercentage
                      , float[]     spawnRateRange
                      , Spawnable[] spawnableObjects
                      )
    {
        _numberOfEnemyToSpawn      = numberOfEnemyToSpawn;
        _shieldedEnemyPercentage   = shieldedEnemyPercentage;
        _aggressiveEnemyPercentage = aggressiveEnemyPercentage;
        _spawnRateRange            = spawnRateRange;
        _spawnableObjects          = spawnableObjects;
    }
    public float[]     SpawnRateRange            { get { return _spawnRateRange;            } set { _spawnRateRange            = value; } }
    public int         NumberOfEnemyToSpawn      { get { return _numberOfEnemyToSpawn;      } set { _numberOfEnemyToSpawn      = value; } }
    public int         ShieldedEnemyPercentage   { get { return _shieldedEnemyPercentage;   } set { _shieldedEnemyPercentage   = value; } }
    public int         AggressiveEnemyPercentage { get { return _aggressiveEnemyPercentage; } set { _aggressiveEnemyPercentage = value; } }
    public Spawnable[] SpawnableObjects          { get { return _spawnableObjects;          } set { _spawnableObjects          = value; } }
}

//
//  ToDo: Implement movementSpeed
//

[System.Serializable]
public struct Spawnable
{
    public GameObject     _prefab;
    public int            _weight;
    public float          _movementSpeed;
    public SpwanableNames _name;
    public SpawnableTypes _type;
    public Spawnable( GameObject     prefab
                    , int            weight
                    , SpwanableNames name
                    , SpawnableTypes type
                    , float          movementSpeed
                    )
    {
        _prefab        = prefab;
        _weight        = weight;
        _name          = name;
        _type          = type;
        _movementSpeed = movementSpeed;
    }
    public GameObject     Prefab        { get { return _prefab; }        set { _prefab        = value; } }
    public SpwanableNames Name          { get { return _name; }          set { _name          = value; } }
    public SpawnableTypes Type          { get { return _type; }          set { _type          = value; } }
    public float          MovementSpeed { get { return _movementSpeed; } set { _movementSpeed = value; } }
    public int            Weight        { get { return _weight; }        set { _weight        = value; } }
}

//
// Note:  Upper point is based on the nose of the player, not center.    (-4,6)
//        right/left is the right/left of the player, also not center.   (9.5,-9.5)
//        Maybe need three sets of boundaries.  Screen, object and spawn...
//
//  May need to give all four values, especially for the spawn boundaries
//

[System.Serializable]
public struct Boundary
{
    public float _x, _y;
    public Boundary(float x, float y) { _x = x; _y = y; }
    public float X { get { return _x; } }
    public float Y { get { return _y; } }
}
