using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeGenerator : MonoBehaviour
{
    private const int UP = 1, RIGHT = 2, DOWN = 4, LEFT = 8;
    private bool hasWallMerged = false;
    public int numRows = 10, numCols = 5;
    public Tilemap tilemap;
    public TileBase tileBase;
    private float[] probStopping = { 0f, 0f, 0.3f, 0.7f, 1f }, probBranchStopping = { 0f, 0.5f, 1f };
    private float probBranching = 0.7f, probWallMerging = 0.7f, probTunnel = 0.5f;
    private int[,] map;
    public void GenerateMaze()
    {
        int[,] cells = new int[numRows, numCols];
        initializeCells(cells);
        setGhostSpawn(cells);
        generateMaze(cells);
        map = cellToMap(cells);
    }

    private static void initializeCells(int[,] cells)
    {
        int numRows = cells.GetLength(0), numCols = cells.GetLength(1);
        for (int i = 0; i < numRows; i++)
            for (int j = 0; j < numCols; j++)
                cells[i, j] = -1;
    }

    private static void setGhostSpawn(int[,] cells)
    {
        int mid = cells.GetLength(0) / 2;
        cells[mid, 0] = RIGHT + UP + LEFT;
        cells[mid, 1] = UP + LEFT;
        cells[mid - 1, 0] = RIGHT + DOWN + LEFT;
        cells[mid - 1, 1] = DOWN + LEFT;
    }

    private void generateMaze(int[,] cells)
    {
        while (true)
        {
            List<Tuple<int, int>> candidateLeftCells = leftMostCells(cells);
            if (candidateLeftCells.Count == 0) break;
            Tuple<int, int> currentCell = candidateLeftCells[UnityEngine.Random.Range(0, candidateLeftCells.Count - 1)];
            List<Tuple<int, int>> possibleBranchPoints = new List<Tuple<int, int>>();
            bool branching = false, done = false;
            int size = 0, branchSize = 0;
            List<int> candidateOpenCells = adjacentOpenCells(cells, currentCell);
            int dir;
            if (candidateOpenCells.Count != 0)
            {
                if (candidateOpenCells.Count > 1) possibleBranchPoints.Add(currentCell);
                dir = candidateOpenCells[UnityEngine.Random.Range(0, candidateOpenCells.Count)];
            }
            else
            {
                cells[currentCell.Item1, currentCell.Item2] = 0;
                continue;
            }
            while (!done)
            {
                float probBreaking = branching ? probBranchStopping[branchSize] : probStopping[size];
                if (UnityEngine.Random.Range(0f, 1f) <= probBreaking) break;
                //if reached end of map or used cell, try branching
                switch (dir)
                {
                    case UP:
                        if (!branching && size > 0 && UnityEngine.Random.Range(0f, 1f) <= probBranching && possibleBranchPoints.Count > 0)
                        {
                            currentCell = possibleBranchPoints[UnityEngine.Random.Range(0, possibleBranchPoints.Count - 1)];
                            branching = true;
                            candidateOpenCells = adjacentOpenCells(cells, currentCell);
                            dir = candidateOpenCells[UnityEngine.Random.Range(0, candidateOpenCells.Count)];
                            continue;
                        }
                        else if (currentCell.Item1 == cells.GetLength(0) - 1 || cells[currentCell.Item1 + 1, currentCell.Item2] != -1)
                        {
                            done = true;
                            continue;
                        }
                        break;
                    case RIGHT:
                        if (!branching && size > 0 && UnityEngine.Random.Range(0f, 1f) <= probBranching && possibleBranchPoints.Count > 0)
                        {
                            currentCell = possibleBranchPoints[UnityEngine.Random.Range(0, possibleBranchPoints.Count - 1)];
                            branching = true;
                            candidateOpenCells = adjacentOpenCells(cells, currentCell);
                            dir = candidateOpenCells[UnityEngine.Random.Range(0, candidateOpenCells.Count)];
                            continue;
                        }
                        else if (currentCell.Item2 == cells.GetLength(1) - 1 || cells[currentCell.Item1, currentCell.Item2 + 1] != -1)
                        {
                            done = true;
                            continue;
                        }
                        break;
                    case DOWN:
                        if (!branching && size > 0 && UnityEngine.Random.Range(0f, 1f) <= probBranching && possibleBranchPoints.Count > 0)
                        {
                            currentCell = possibleBranchPoints[UnityEngine.Random.Range(0, possibleBranchPoints.Count - 1)];
                            branching = true;
                            candidateOpenCells = adjacentOpenCells(cells, currentCell);
                            dir = candidateOpenCells[UnityEngine.Random.Range(0, candidateOpenCells.Count)];
                            continue;
                        }
                        else if (currentCell.Item1 == 0 || cells[currentCell.Item1 - 1, currentCell.Item2] != -1)
                        {
                            done = true;
                            continue;
                        }
                        break;
                    case LEFT:
                        if (!branching && size > 0 && UnityEngine.Random.Range(0f, 1f) <= probBranching && possibleBranchPoints.Count > 0)
                        {
                            currentCell = possibleBranchPoints[UnityEngine.Random.Range(0, possibleBranchPoints.Count - 1)];
                            branching = true;
                            candidateOpenCells = adjacentOpenCells(cells, currentCell);
                            dir = candidateOpenCells[UnityEngine.Random.Range(0, candidateOpenCells.Count)];
                            continue;
                        }
                        else if (currentCell.Item1 == 0 || cells[currentCell.Item1, currentCell.Item2 - 1] != -1)
                        {
                            done = true;
                            continue;
                        }
                        break;
                }
                if (branching) branchSize++; else size++;
                if (cells[currentCell.Item1, currentCell.Item2] == -1) cells[currentCell.Item1, currentCell.Item2] = 0;
                switch (dir)
                {
                    case UP:
                        cells[currentCell.Item1, currentCell.Item2] += UP;
                        currentCell = new Tuple<int, int>(currentCell.Item1 + 1, currentCell.Item2);
                        if (cells[currentCell.Item1, currentCell.Item2] == -1) cells[currentCell.Item1, currentCell.Item2] = 0;
                        cells[currentCell.Item1, currentCell.Item2] += DOWN;
                        break;
                    case RIGHT:
                        cells[currentCell.Item1, currentCell.Item2] += RIGHT;
                        currentCell = new Tuple<int, int>(currentCell.Item1, currentCell.Item2 + 1);
                        if (cells[currentCell.Item1, currentCell.Item2] == -1) cells[currentCell.Item1, currentCell.Item2] = 0;
                        cells[currentCell.Item1, currentCell.Item2] += LEFT;
                        break;
                    case DOWN:
                        cells[currentCell.Item1, currentCell.Item2] += DOWN;
                        currentCell = new Tuple<int, int>(currentCell.Item1 - 1, currentCell.Item2);
                        if (cells[currentCell.Item1, currentCell.Item2] == -1) cells[currentCell.Item1, currentCell.Item2] = 0;
                        cells[currentCell.Item1, currentCell.Item2] += UP;
                        break;
                    case LEFT:
                        cells[currentCell.Item1, currentCell.Item2] += LEFT;
                        currentCell = new Tuple<int, int>(currentCell.Item1, currentCell.Item2 - 1);
                        if (cells[currentCell.Item1, currentCell.Item2] == -1) cells[currentCell.Item1, currentCell.Item2] = 0;
                        cells[currentCell.Item1, currentCell.Item2] += RIGHT;
                        break;
                }
                if (adjacentOpenCells(cells, currentCell).Count > 1)
                    possibleBranchPoints.Add(currentCell);
            }
        }
    }

    private int[,] cellToMap(int[,] cells)
    {
        int numRows = cells.GetLength(0), numCols = cells.GetLength(1);
        int mapRows = (numRows * 3) + 3, mapCols = numCols * 3 + 1;
        int[,] map = new int[mapRows, mapCols];
        for (int i = 0; i < numRows; i++)
            for (int j = 0; j < numCols; j++)
            {
                int bottomLefty = (i * 3) + 1, bottomLeftx = j * 3;
                //Bottom and right edge are paths by default
                map[bottomLefty, bottomLeftx] = map[bottomLefty, bottomLeftx + 1] = -1;
                map[bottomLefty, bottomLeftx + 2] = map[bottomLefty + 1, bottomLeftx + 2] = map[bottomLefty + 2, bottomLeftx + 2] = -1;
                //Top left 2x2 are always walls
                map[bottomLefty + 1, bottomLeftx] = map[bottomLefty + 1, bottomLeftx + 1] = 0;
                map[bottomLefty + 2, bottomLeftx] = map[bottomLefty + 2, bottomLeftx + 1] = 0;
                switch (cells[i, j])
                {
                    case 0:
                        if (!hasWallMerged || UnityEngine.Random.Range(0f, 1f) <= probWallMerging)
                        {//If hasn't merged before or prob proced, merge
                            if (j == numCols - 1)
                            {//Check if right most column
                                map[bottomLefty + 1, bottomLeftx + 2] = map[bottomLefty + 2, bottomLeftx + 2] = 0;
                                if (i == 0) //Check if bottom corner
                                    map[bottomLefty, bottomLeftx] = map[bottomLefty, bottomLeftx + 1] = map[bottomLefty, bottomLeftx + 2] = 0;
                                else if (i == numRows - 1) //If top corner, set top row to 1 so it's detected when doing top path
                                    map[bottomLefty + 2, bottomLeftx] = map[bottomLefty + 2, bottomLeftx + 1] = map[bottomLefty + 2, bottomLeftx + 2] = 1;
                            }
                            else if (i == 0)//Check if bottom row, not corner
                                map[bottomLefty, bottomLeftx] = map[bottomLefty, bottomLeftx] = 0;
                            else if (i == numRows - 1)//Check if top row, not corner
                                map[bottomLefty + 2, bottomLeftx] = map[bottomLefty + 2, bottomLeftx + 1] = 1;
                        }
                        break;
                    case 2:
                        map[bottomLefty + 1, bottomLeftx + 2] = map[bottomLefty + 2, bottomLeftx + 2] = 0;
                        break;
                    case 3:
                        map[bottomLefty + 1, bottomLeftx + 2] = map[bottomLefty + 2, bottomLeftx + 2] = 0;
                        break;
                    case 4:
                        map[bottomLefty, bottomLeftx] = map[bottomLefty, bottomLeftx + 1] = 0;
                        break;
                    case 5:
                        map[bottomLefty, bottomLeftx] = map[bottomLefty, bottomLeftx + 1] = 0;
                        break;
                    case 6:
                        map[bottomLefty, bottomLeftx] = map[bottomLefty, bottomLeftx + 1] = 0;
                        map[bottomLefty + 1, bottomLeftx + 2] = map[bottomLefty + 2, bottomLeftx + 2] = 0;
                        break;
                    case 7:
                        map[bottomLefty, bottomLeftx] = map[bottomLefty, bottomLeftx + 1] = 0;
                        map[bottomLefty + 1, bottomLeftx + 2] = map[bottomLefty + 2, bottomLeftx + 2] = 0;
                        break;
                    case 10:
                        map[bottomLefty + 1, bottomLeftx + 2] = map[bottomLefty + 2, bottomLeftx + 2] = 0;
                        break;
                    case 11:
                        map[bottomLefty + 1, bottomLeftx + 2] = map[bottomLefty + 2, bottomLeftx + 2] = 0;
                        break;
                    case 12:
                        map[bottomLefty, bottomLeftx] = map[bottomLefty, bottomLeftx + 1] = 0;
                        break;
                    case 13:
                        map[bottomLefty, bottomLeftx] = map[bottomLefty, bottomLeftx + 1] = 0;
                        break;
                    case 14:
                        map[bottomLefty, bottomLeftx] = map[bottomLefty, bottomLeftx + 1] = 0;
                        map[bottomLefty + 1, bottomLeftx + 2] = map[bottomLefty + 2, bottomLeftx + 2] = 0;
                        break;
                    case 15:
                        map[bottomLefty, bottomLeftx] = map[bottomLefty, bottomLeftx + 1] = 0;
                        map[bottomLefty + 1, bottomLeftx + 2] = map[bottomLefty + 2, bottomLeftx + 2] = 0;
                        break;
                }
            }

        //Top path, and top and bottom outer walls of map
        for (int i = 0; i < mapCols - 1; i++)
        {
            map[mapRows - 2, i] = (map[mapRows - 3, i] == 1) ? 0 : -1;
            map[mapRows - 1, i] = map[0, i] = 0;
        }

        //Outer wall corners of map
        map[0, mapCols - 1] = map[mapRows - 1, mapCols - 1] = 0;

        //Right and left outer wall of map
        for (int i = 1; i < mapRows - 1; i++)
            map[i, mapCols - 1] = 0;

        int ghostSpawny = mapRows / 2 - 2;
        //Below ghost spawn
        for (int i = 0; i < 6; i++)
            map[ghostSpawny - 1, i] = -1;

        //Right and left of ghost spawn
        for (int i = ghostSpawny; i < ghostSpawny + 5; i++)
            map[i, 5] = -1;

        //Inside host spawn
        for (int i = ghostSpawny + 1; i < ghostSpawny + 4; i++)
            for (int j = 0; j < 4; j++)
                map[i, j] = -1;

        //Walls of ghost spawn
        for (int i = ghostSpawny + 1; i < ghostSpawny + 4; i++)
            map[i, 4] = 0;
        for (int i = 0; i < 5; i++)
            map[ghostSpawny, i] = map[ghostSpawny + 4, i] = 0;

        //Make first tunnel
        for (int index = 0; index < mapRows; index++)
        {
            int possibleTunnelRow = UnityEngine.Random.Range(2, mapRows - 5);
            if (map[possibleTunnelRow, mapCols - 2] != 0)
            {
                map[possibleTunnelRow, mapCols - 1] = -1;
                break;
            }
        }

        //Check probability for second tunnel
        if (UnityEngine.Random.Range(0f, 1f) <= probTunnel)
            for (int index = 0; index < mapRows; index++)
            {
                int possibleTunnelRow = UnityEngine.Random.Range(6, mapRows - 2);
                if (map[possibleTunnelRow, mapCols - 2] != 0 && map[possibleTunnelRow, mapCols - 1] != -1)
                {
                    map[possibleTunnelRow, mapCols - 1] = -1;
                    break;
                }
            }

        return map;
    }

    public int[,] getMap()
    {
        return map;
    }

    public void renderMap(int[,] map)
    {
        int numRows = map.GetLength(0);
        int numCols = map.GetLength(1);
        int yOffset = (numRows - 1) / 2;
        for (int i = 0; i < numRows; i++)
            for (int j = 0; j < numCols; j++)
            {
                Vector3Int v = new Vector3Int(j, i - yOffset, 0);
                if (map[i, j] != -1) renderTile(v, tileBase);
                
                if (j != 0)
                {
                    Vector3Int mirrorV = new Vector3Int(-j, i - yOffset, 0);
                    if (map[i, j] != -1) renderTile(mirrorV, tileBase);
                }
            }
    }

    public void ClearTileMap()
    {
        tilemap.ClearAllTiles();
    }

    private void renderTile(Vector3Int pos, TileBase tile)
    {
        if (pos.x == 1 && pos.y == 2) return;
        if (pos.x == 0 && pos.y == 2) return;
        if (pos.x == -1 && pos.y == 2) return;
        tilemap.SetTile(pos, tile);
    }

    private List<Tuple<int, int>> leftMostCells(int[,] cells)
    {
        List<Tuple<int, int>> leftMostCells = new List<Tuple<int, int>>();
        for (int i = 0; i < numCols; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
                if (cells[j, i] == -1) leftMostCells.Add(new Tuple<int, int>(j, i));
            }
            if (leftMostCells.Count > 0) break;
        }
        return leftMostCells;
    }

    private static List<int> adjacentOpenCells(int[,] cells, Tuple<int, int> center)
    {
        List<int> adjacentOpenCells = new List<int>();
        int x = center.Item2, y = center.Item1;
        if (y > 0 && cells[y - 1, x] == -1) adjacentOpenCells.Add(DOWN);
        if (y < cells.GetLength(0) - 1 && cells[y + 1, x] == -1) adjacentOpenCells.Add(UP);
        if (x < cells.GetLength(1) - 1 && cells[y, x + 1] == -1) adjacentOpenCells.Add(RIGHT);
        return adjacentOpenCells;
    }
}