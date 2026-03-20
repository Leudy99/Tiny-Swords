using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class MapDecorationPainter : MonoBehaviour
{
    public enum PlacementZone
    {
        Anywhere,
        NearEdge,
        Center
    }

    [System.Serializable]
    public class DecorationRule
    {
        public string name = "Decoration";
        public TileBase tile;
        [Min(1)] public int count = 10;
        public PlacementZone zone = PlacementZone.Anywhere;
        [Min(0)] public int minDistanceFromSameType = 2;
        [Min(0)] public int minDistanceFromAnyDecoration = 1;
    }

    [Header("Required Tilemaps")]
    public Tilemap groundTilemap;
    public Tilemap decorationsTilemap;

    [Header("Blocked Tilemaps")]
    public List<Tilemap> blockedTilemaps = new List<Tilemap>();

    [Header("Generation Settings")]
    public int seed = 12345;
    [Min(0)] public int mapBorderPadding = 1;
    [Min(0)] public int blockedTilePadding = 0;
    [Min(1)] public int nearEdgeDistance = 4;
    [Min(1)] public int centerDistanceFromEdge = 6;

    [Header("Decoration Rules")]
    public List<DecorationRule> rules = new List<DecorationRule>();

    private readonly List<Vector3Int> placedCells = new List<Vector3Int>();

    public void GenerateDecorations()
    {
        if (groundTilemap == null || decorationsTilemap == null)
        {
            Debug.LogWarning("Asigna Ground Tilemap y Decorations Tilemap antes de generar.");
            return;
        }

        ClearDecorationsInternal();

        Random.InitState(seed);
        placedCells.Clear();

        HashSet<Vector3Int> validGroundCells = GetValidGroundCells();
        if (validGroundCells.Count == 0)
        {
            Debug.LogWarning("No se encontraron celdas válidas en Ground.");
            return;
        }

        foreach (DecorationRule rule in rules)
        {
            if (rule.tile == null || rule.count <= 0)
                continue;

            List<Vector3Int> candidates = GetCandidatesForRule(validGroundCells, rule);
            Shuffle(candidates);

            int placedCount = 0;
            List<Vector3Int> sameTypePlaced = new List<Vector3Int>();

            foreach (Vector3Int cell in candidates)
            {
                if (placedCount >= rule.count)
                    break;

                if (!CanPlaceAt(cell, rule, sameTypePlaced))
                    continue;

                decorationsTilemap.SetTile(cell, rule.tile);
                Debug.Log("Pintando tile en celda: " + cell + " con tile: " + rule.tile.name);
                placedCells.Add(cell);
                sameTypePlaced.Add(cell);
                placedCount++;
            }

            Debug.Log($"{rule.name}: colocados {placedCount} de {rule.count}");
        }

        MarkDirty();
    }

    public void ClearDecorations()
    {
        if (decorationsTilemap == null)
        {
            Debug.LogWarning("Asigna Decorations Tilemap.");
            return;
        }

        ClearDecorationsInternal();
        MarkDirty();
    }

    private void ClearDecorationsInternal()
    {
        decorationsTilemap.ClearAllTiles();
    }

    private HashSet<Vector3Int> GetValidGroundCells()
    {
        HashSet<Vector3Int> validCells = new HashSet<Vector3Int>();
        BoundsInt bounds = groundTilemap.cellBounds;

        Debug.Log("Ground bounds: " + groundTilemap.cellBounds);

        foreach (Vector3Int cell in bounds.allPositionsWithin)
        {
            if (!groundTilemap.HasTile(cell))
                continue;

            if (IsNearOuterGroundBorder(cell, mapBorderPadding))
                continue;

            if (IsBlocked(cell, blockedTilePadding))
                continue;

            validCells.Add(cell);
        }

        Debug.Log("Ground valid cells found: " + validCells.Count);

        return validCells;
    }

    private List<Vector3Int> GetCandidatesForRule(HashSet<Vector3Int> validCells, DecorationRule rule)
    {
        List<Vector3Int> candidates = new List<Vector3Int>();

        foreach (Vector3Int cell in validCells)
        {
            if (decorationsTilemap.HasTile(cell))
                continue;

            int edgeDistance = GetDistanceToGroundEdge(cell);

            bool accept = rule.zone switch
            {
                PlacementZone.Anywhere => true,
                PlacementZone.NearEdge => edgeDistance <= nearEdgeDistance,
                PlacementZone.Center => edgeDistance >= centerDistanceFromEdge,
                _ => true
            };

            if (accept)
                candidates.Add(cell);
        }

        return candidates;
    }

    private bool CanPlaceAt(Vector3Int cell, DecorationRule rule, List<Vector3Int> sameTypePlaced)
    {
        if (decorationsTilemap.HasTile(cell))
            return false;

        if (IsBlocked(cell, blockedTilePadding))
            return false;

        foreach (Vector3Int placed in placedCells)
        {
            if (CellDistance(cell, placed) < rule.minDistanceFromAnyDecoration)
                return false;
        }

        foreach (Vector3Int placed in sameTypePlaced)
        {
            if (CellDistance(cell, placed) < rule.minDistanceFromSameType)
                return false;
        }

        return true;
    }

    private bool IsBlocked(Vector3Int cell, int padding)
    {
        foreach (Tilemap blocked in blockedTilemaps)
        {
            if (blocked == null)
                continue;

            for (int x = -padding; x <= padding; x++)
            {
                for (int y = -padding; y <= padding; y++)
                {
                    Vector3Int checkCell = new Vector3Int(cell.x + x, cell.y + y, cell.z);
                    if (blocked.HasTile(checkCell))
                        return true;
                }
            }
        }

        return false;
    }

    private bool IsNearOuterGroundBorder(Vector3Int cell, int padding)
    {
        if (padding <= 0)
            return false;

        for (int x = -padding; x <= padding; x++)
        {
            for (int y = -padding; y <= padding; y++)
            {
                Vector3Int neighbor = new Vector3Int(cell.x + x, cell.y + y, cell.z);
                if (!groundTilemap.HasTile(neighbor))
                    return true;
            }
        }

        return false;
    }

    private int GetDistanceToGroundEdge(Vector3Int cell)
    {
        int distance = 0;

        while (true)
        {
            distance++;
            bool hasFullRing = true;

            for (int x = -distance; x <= distance; x++)
            {
                Vector3Int top = new Vector3Int(cell.x + x, cell.y + distance, cell.z);
                Vector3Int bottom = new Vector3Int(cell.x + x, cell.y - distance, cell.z);

                if (!groundTilemap.HasTile(top) || !groundTilemap.HasTile(bottom))
                {
                    hasFullRing = false;
                    break;
                }
            }

            if (hasFullRing)
            {
                for (int y = -distance + 1; y <= distance - 1; y++)
                {
                    Vector3Int left = new Vector3Int(cell.x - distance, cell.y + y, cell.z);
                    Vector3Int right = new Vector3Int(cell.x + distance, cell.y + y, cell.z);

                    if (!groundTilemap.HasTile(left) || !groundTilemap.HasTile(right))
                    {
                        hasFullRing = false;
                        break;
                    }
                }
            }

            if (!hasFullRing)
                return distance - 1;
        }
    }

    private int CellDistance(Vector3Int a, Vector3Int b)
    {
        return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));
    }

    private void Shuffle(List<Vector3Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    private void MarkDirty()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        if (decorationsTilemap != null)
            EditorUtility.SetDirty(decorationsTilemap);

        EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
    }
}