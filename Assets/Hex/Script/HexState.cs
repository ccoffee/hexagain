using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexState
{

    public HexDataKey key;
    
    public Vector2Int position {
        get {
            return (Vector2Int) key.position;
        }
    }

    public Vector3Int position3 {
        get {
            return key.position;
        }
    }

    public int level {
        get {
            return key.position.z;
        }
    }

    public HexState(HexDataKey _key) {
        key = _key;
    }
}
