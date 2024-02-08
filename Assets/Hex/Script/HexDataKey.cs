using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexDataKey
{
    public Vector3Int position;
    public bool cleared;
    public bool building;

    public HexDataKey(Vector3Int _position, bool _cleared = false, bool _building = false) {
        position = _position;
        cleared = _cleared;
        building = _building;
    }
}
