using UnityEngine;
using UnityEngine.Tilemaps;

public enum TetrisFigures
{
    I, J, L, O, S, T, Z
}

[System.Serializable]
public struct TetrisFiguresData
{
    public Tile tile;
    public TetrisFigures figures;

    public Vector2Int[] cells { get; private set; }
    public Vector2Int[,] wallKicks { get; private set; }

    public void Initialize()
    {
        cells = Data.Cells[figures];
        wallKicks = Data.WallKicks[figures];
    }

}
