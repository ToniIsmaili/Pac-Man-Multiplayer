using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeGenerator : MonoBehaviour
{
    private const int UP = 1, RIGHT = 2, DOWN = 4, LEFT = 8;
    public int numRows = 10, numCols = 5;
    // Removed the floor sprite and wall sprite variables and added refrences to the tilemap and the tile rule (tilebase)
    public Tilemap tilemap;
    public TileBase tilebase;
    private float[] probStopping = { 0f, 0f, 0.3f, 0.7f, 1f };
    private float[] probBranchStopping = { 0f, 0.5f, 1f };
    private float probBranching = 0.7f, probWallMerging = 0.7f;
    public void GenerateMaze()
    {
        int[,] cells = new int[numRows, numCols];
        initializeCells(cells);
        setGhostSpawn(cells);
        generateMaze(cells);
        int[,] map = cellToMap(cells);
        renderMap(map, tilemap, tilebase);
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
                map[bottomLefty, bottomLeftx] = -1;
                map[bottomLefty, bottomLeftx + 1] = -1;
                map[bottomLefty, bottomLeftx + 2] = -1;
                map[bottomLefty + 1, bottomLeftx + 2] = -1;
                map[bottomLefty + 2, bottomLeftx + 2] = -1;
                switch (cells[i, j])
                {
                    case 1:
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + LEFT;
                        map[bottomLefty + 2, bottomLeftx] = UP + DOWN + RIGHT;
                        map[bottomLefty + 2, bottomLeftx + 1] = UP + DOWN + LEFT;
                        break;
                    case 2:
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + LEFT + RIGHT;
                        map[bottomLefty + 1, bottomLeftx + 2] = UP + LEFT + RIGHT;
                        map[bottomLefty + 2, bottomLeftx] = DOWN + RIGHT;
                        map[bottomLefty + 2, bottomLeftx + 1] = DOWN + RIGHT + LEFT;
                        map[bottomLefty + 2, bottomLeftx + 2] = DOWN + RIGHT + LEFT;
                        break;
                    case 3:
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + LEFT + RIGHT;
                        map[bottomLefty + 1, bottomLeftx + 2] = UP + LEFT + RIGHT;
                        map[bottomLefty + 2, bottomLeftx] = UP + DOWN + RIGHT;
                        map[bottomLefty + 2, bottomLeftx + 1] = UP + DOWN + RIGHT + LEFT;
                        map[bottomLefty + 2, bottomLeftx + 2] = DOWN + RIGHT + LEFT;
                        break;
                    case 4:
                        map[bottomLefty, bottomLeftx] = UP + RIGHT + DOWN;
                        map[bottomLefty, bottomLeftx + 1] = UP + DOWN + LEFT;
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT + DOWN;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + LEFT + DOWN;
                        map[bottomLefty + 2, bottomLeftx] = DOWN + RIGHT;
                        map[bottomLefty + 2, bottomLeftx + 1] = DOWN + LEFT;
                        break;
                    case 5:
                        map[bottomLefty, bottomLeftx] = UP + RIGHT + DOWN;
                        map[bottomLefty, bottomLeftx + 1] = UP + DOWN + LEFT;
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT + DOWN;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + LEFT + DOWN;
                        map[bottomLefty + 2, bottomLeftx] = UP + DOWN + RIGHT;
                        map[bottomLefty + 2, bottomLeftx + 1] = UP + DOWN + LEFT;
                        break;
                    case 6:
                        map[bottomLefty, bottomLeftx] = UP + RIGHT + DOWN;
                        map[bottomLefty, bottomLeftx + 1] = UP + DOWN + LEFT;
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT + DOWN;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + LEFT + DOWN + RIGHT;
                        map[bottomLefty + 1, bottomLeftx + 2] = UP + LEFT + RIGHT;
                        map[bottomLefty + 2, bottomLeftx] = DOWN + RIGHT;
                        map[bottomLefty + 2, bottomLeftx + 1] = DOWN + RIGHT + LEFT;
                        map[bottomLefty + 2, bottomLeftx + 2] = DOWN + RIGHT + LEFT;
                        break;
                    case 7:
                        map[bottomLefty, bottomLeftx] = UP + RIGHT + DOWN;
                        map[bottomLefty, bottomLeftx + 1] = UP + DOWN + LEFT;
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT + DOWN;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + LEFT + DOWN + RIGHT;
                        map[bottomLefty + 1, bottomLeftx + 2] = UP + LEFT + RIGHT;
                        map[bottomLefty + 2, bottomLeftx] = UP + DOWN + RIGHT;
                        map[bottomLefty + 2, bottomLeftx + 1] = UP + DOWN + RIGHT + LEFT;
                        map[bottomLefty + 2, bottomLeftx + 2] = DOWN + RIGHT + LEFT;
                        break;
                    case 8:
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT + LEFT;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + LEFT;
                        map[bottomLefty + 2, bottomLeftx] = DOWN + RIGHT + LEFT;
                        map[bottomLefty + 2, bottomLeftx + 1] = DOWN + LEFT;
                        break;
                    case 9:
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT + LEFT;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + LEFT;
                        map[bottomLefty + 2, bottomLeftx] = DOWN + RIGHT + LEFT + UP;
                        map[bottomLefty + 2, bottomLeftx + 1] = DOWN + LEFT + UP;
                        break;
                    case 10:
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT + LEFT;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + RIGHT + LEFT;
                        map[bottomLefty + 1, bottomLeftx + 2] = UP + RIGHT + LEFT;
                        map[bottomLefty + 2, bottomLeftx] = DOWN + RIGHT + LEFT;
                        map[bottomLefty + 2, bottomLeftx + 1] = DOWN + RIGHT + LEFT;
                        map[bottomLefty + 2, bottomLeftx + 2] = DOWN + RIGHT + LEFT;
                        break;
                    case 11:
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT + LEFT;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + RIGHT + LEFT;
                        map[bottomLefty + 1, bottomLeftx + 2] = UP + RIGHT + LEFT;
                        map[bottomLefty + 2, bottomLeftx] = UP + DOWN + RIGHT + LEFT;
                        map[bottomLefty + 2, bottomLeftx + 1] = UP + DOWN + RIGHT + LEFT;
                        map[bottomLefty + 2, bottomLeftx + 2] = DOWN + RIGHT + LEFT;
                        break;
                    case 12:
                        map[bottomLefty, bottomLeftx] = UP + RIGHT + DOWN;
                        map[bottomLefty, bottomLeftx + 1] = UP + DOWN + LEFT;
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT + DOWN + LEFT;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + LEFT + DOWN;
                        map[bottomLefty + 2, bottomLeftx] = DOWN + RIGHT + LEFT;
                        map[bottomLefty + 2, bottomLeftx + 1] = DOWN + LEFT;
                        break;
                    case 13:
                        map[bottomLefty, bottomLeftx] = UP + RIGHT + DOWN + LEFT;
                        map[bottomLefty, bottomLeftx + 1] = UP + DOWN + LEFT;
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT + DOWN + LEFT;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + DOWN + LEFT;
                        map[bottomLefty + 2, bottomLeftx] = UP + RIGHT + DOWN + LEFT;
                        map[bottomLefty + 2, bottomLeftx + 1] = UP + DOWN + LEFT;
                        break;
                    case 14:
                        map[bottomLefty, bottomLeftx] = UP + RIGHT + DOWN;
                        map[bottomLefty, bottomLeftx + 1] = UP + DOWN + LEFT;
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT + DOWN + LEFT;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + RIGHT + DOWN + LEFT;
                        map[bottomLefty + 1, bottomLeftx + 2] = UP + RIGHT + LEFT;
                        map[bottomLefty + 2, bottomLeftx] = RIGHT + DOWN + LEFT;
                        map[bottomLefty + 2, bottomLeftx + 1] = RIGHT + DOWN + LEFT;
                        map[bottomLefty + 2, bottomLeftx + 2] = RIGHT + DOWN + LEFT;
                        break;
                    case 15:
                        map[bottomLefty, bottomLeftx] = UP + RIGHT + DOWN + LEFT;
                        map[bottomLefty, bottomLeftx + 1] = UP + DOWN + LEFT;
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT + DOWN + LEFT;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + RIGHT + DOWN + LEFT;
                        map[bottomLefty + 1, bottomLeftx + 2] = UP + RIGHT + LEFT;
                        map[bottomLefty + 2, bottomLeftx] = UP + RIGHT + DOWN + LEFT;
                        map[bottomLefty + 2, bottomLeftx + 1] = UP + RIGHT + DOWN + LEFT;
                        map[bottomLefty + 2, bottomLeftx + 2] = RIGHT + DOWN + LEFT;
                        break;
                    default: // case 0
                        if ((i == 0 || i == (numRows - 1)) && UnityEngine.Random.Range(0f, 1f) <= probWallMerging)
                        {
                            //If top or bottom row, try merging walls
                            if (j == numCols - 1)
                            {
                                //Check if corner
                                if (i == 0)
                                {
                                    //Check if top or bottom corner
                                    map[bottomLefty, bottomLeftx] = map[bottomLefty + 1, bottomLeftx] = UP + RIGHT + DOWN;
                                    map[bottomLefty, bottomLeftx + 1] = map[bottomLefty, bottomLeftx + 2] = UP + RIGHT + DOWN + LEFT;
                                    map[bottomLefty + 1, bottomLeftx + 1] = map[bottomLefty + 1, bottomLeftx + 2] = UP + RIGHT + DOWN + LEFT;
                                    map[bottomLefty + 2, bottomLeftx] = RIGHT + DOWN;
                                    map[bottomLefty + 2, bottomLeftx + 1] = map[bottomLefty + 2, bottomLeftx + 2] = RIGHT + DOWN + LEFT;
                                }
                                else
                                {
                                    map[bottomLefty + 1, bottomLeftx] = UP + RIGHT;
                                    map[bottomLefty + 1, bottomLeftx + 1] = map[bottomLefty + 1, bottomLeftx + 2] = UP + RIGHT + LEFT;
                                    map[bottomLefty + 2, bottomLeftx] = UP + RIGHT + DOWN;
                                    map[bottomLefty + 2, bottomLeftx + 1] = map[bottomLefty + 2, bottomLeftx + 2] = UP + RIGHT + DOWN + LEFT;
                                }
                            }
                            else
                            {
                                map[bottomLefty + 1, bottomLeftx] = UP + RIGHT;
                                map[bottomLefty + 1, bottomLeftx + 1] = UP + LEFT;
                                map[bottomLefty + 2, bottomLeftx] = DOWN + RIGHT;
                                map[bottomLefty + 2, bottomLeftx + 1] = DOWN + LEFT;
                                if (i == 0)
                                {
                                    //Check if top or bottom piece
                                    map[bottomLefty, bottomLeftx] = UP + RIGHT + DOWN;
                                    map[bottomLefty, bottomLeftx + 1] = UP + LEFT + DOWN;
                                    map[bottomLefty + 1, bottomLeftx] += DOWN;
                                    map[bottomLefty + 1, bottomLeftx + 1] += DOWN;
                                }
                                else
                                {
                                    map[bottomLefty + 2, bottomLeftx] += UP;
                                    map[bottomLefty + 2, bottomLeftx + 1] += UP;
                                }
                            }
                        }
                        else if (j == numCols - 1 && UnityEngine.Random.Range(0f, 1f) <= probWallMerging && i != 0 && i != (numRows - 1))
                        {
                            //If rightmost column but not corners, try merging walls
                            map[bottomLefty + 1, bottomLeftx] = UP + RIGHT;
                            map[bottomLefty + 1, bottomLeftx + 1] = map[bottomLefty + 1, bottomLeftx + 2] = UP + RIGHT + LEFT;
                            map[bottomLefty + 2, bottomLeftx] = DOWN + RIGHT;
                            map[bottomLefty + 2, bottomLeftx + 1] = map[bottomLefty + 2, bottomLeftx + 2] = RIGHT + DOWN + LEFT;
                        }
                        else
                        {
                            map[bottomLefty + 1, bottomLeftx] = UP + RIGHT;
                            map[bottomLefty + 1, bottomLeftx + 1] = UP + LEFT;
                            map[bottomLefty + 2, bottomLeftx] = DOWN + RIGHT;
                            map[bottomLefty + 2, bottomLeftx + 1] = DOWN + LEFT;
                        }
                        break;
                }
            }

        //Top path, and top and bottom outer walls of map
        for (int i = 0; i < mapCols - 1; i++)
        {
            if (map[mapRows - 3, i] % 2 == 1)
            {
                map[mapRows - 2, i] = UP + DOWN;
                map[mapRows - 2, i] += (map[mapRows - 2, i - 1] == -1) ? RIGHT : LEFT;
                if (i == mapCols - 2 || i == mapCols - 3) map[mapRows - 2, i] += RIGHT;
            }
            else
                map[mapRows - 2, i] = -1;
            if (i == 0)
            {
                map[mapRows - 1, i] = (map[mapRows - 2, i] % 2 == 1) ? DOWN + RIGHT : RIGHT;
                map[0, i] = (map[1, i] / 4 % 2 == 1) ? UP + RIGHT : RIGHT;
            }
            else
            {
                map[mapRows - 1, i] = (map[mapRows - 2, i] / 4 % 2 == 1) ? DOWN + RIGHT + LEFT : RIGHT + LEFT;
                map[0, i] = (map[1, i] / 4 % 2 == 1) ? UP + RIGHT + LEFT : RIGHT + LEFT;
            }
        }

        //Outer wall corners of map
        map[0, mapCols - 1] = LEFT + UP;
        map[mapRows - 1, mapCols - 1] = LEFT + DOWN;

        //Right and left outer wall of map
        for (int i = 1; i < mapRows - 1; i++)
            map[i, mapCols - 1] = (map[i, mapCols - 2] / 2 % 2 == 1) ? UP + DOWN + LEFT : UP + DOWN;

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
            map[i, 4] = UP + DOWN;
        for (int i = 1; i < 4; i++)
        {
            map[ghostSpawny, i] = LEFT + RIGHT;
            map[ghostSpawny + 4, i] = LEFT + RIGHT;
        }
        //Corners of ghost spawn
        map[ghostSpawny, 0] = RIGHT;
        map[ghostSpawny + 4, 0] = RIGHT;
        map[ghostSpawny, 4] = LEFT + UP;
        map[ghostSpawny + 4, 4] = LEFT + DOWN;
        return map;
    }

    // Changed the parameters in the rendermap function to accept the tilemap and rule tile
    private static void renderMap(int[,] map, Tilemap tilemap, TileBase tilebase)
    {
        int numRows = map.GetLength(0);
        int numCols = map.GetLength(1);
        int yOffset = (numRows - 1) / 2;
        for (int i = 0; i < numRows; i++)
            for (int j = 0; j < numCols; j++)
            {
                // Made the renderTile function only run when there is a wall
                if (j == 0)
                {
                    if (map[i, j] != -1) renderTile(j, i - yOffset, tilemap, tilebase);
                }
                else
                {
                    if (map[i, j] != -1) renderTile(j, i - yOffset, tilemap, tilebase);
                    if (map[i, j] != -1) renderTile(-j, i - yOffset, tilemap, tilebase);
                }
            }
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

    // Changed the renderTile function to work with the tiles and not sprites
    private static void renderTile(int x, int y, Tilemap tilemap, TileBase tilebase)
    {
        // SetTile takes the coordinates of the grid and what tile you want to place
        tilemap.SetTile(new Vector3Int(x, y, 0), tilebase);
    }

    private static int oppositeIndex(int index)
    {
        switch (index)
        {
            case RIGHT:
                return LEFT;
            case RIGHT + UP:
                return LEFT + UP;
            case RIGHT + DOWN:
                return LEFT + DOWN;
            case RIGHT + UP + DOWN:
                return LEFT + UP + DOWN;
            case LEFT:
                return RIGHT;
            case LEFT + UP:
                return RIGHT + UP;
            case LEFT + DOWN:
                return RIGHT + DOWN;
            case LEFT + UP + DOWN:
                return RIGHT + UP + DOWN;
            default:
                return index;
        }
    }
}