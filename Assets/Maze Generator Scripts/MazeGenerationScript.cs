using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using System.Linq;
using UnityEngine;
using Unity.VisualScripting;

public class MazeGenerationScript : MonoBehaviour
{
    public MazeCell mazeCellPrefab;

    public GameObject TheMaze;

    public int MazeX;

    public int MazeZ;

    private MazeCell[,] mazeGrid;

    private bool FirstUse = true;

    public GameObject Camera;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || FirstUse == true)
        {   
            FirstUse = false;

            Camera.transform.position = new Vector3(MazeX/2,(MazeX+MazeZ)/2*2,MazeZ/2); // Camera

            foreach(Transform child in TheMaze.transform)
            {
                Destroy(child.gameObject);
            }

            mazeGrid = new MazeCell[MazeX,MazeZ];

            for (int x = 0; x < MazeX; x++)
            {
                for (int z = 0; z < MazeZ; z++)
                {
                    mazeGrid[x,z] = Instantiate(mazeCellPrefab, new Vector3(x,0,z), Quaternion.identity);
                    mazeGrid[x,z].transform.parent = TheMaze.transform;
                }
            }

            GenerateMaze(null, mazeGrid[0,0]);
        }
    }

    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell,currentCell);

        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                GenerateMaze(currentCell, nextCell);
            }
        } while (nextCell != null);
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);

        return unvisitedCells.OrderBy(_ => Random.Range(1,10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.position.x;
        int z = (int)currentCell.transform.position.z;

        if (x + 1 < MazeX)
        {
            var cellToRight = mazeGrid[x + 1, z];

            if (cellToRight.IsVisited == false)
            {
                yield return cellToRight;
            }
        }

        if (x - 1 >= 0)
        {
            var cellToLeft = mazeGrid[x - 1, z];

            if (cellToLeft.IsVisited == false)
            {
                yield return cellToLeft;
            }
        }

        if (z + 1 < MazeZ)
        {
            var cellToFront = mazeGrid[x, z + 1];

            if (cellToFront.IsVisited == false)
            {
                yield return cellToFront;
            }
        }
        if (z - 1 >= 0)
        {
            var cellToBack = mazeGrid[x, z - 1];

            if (cellToBack.IsVisited == false)
            {
                yield return cellToBack;
            }
        }
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
       if (previousCell == null)
       {
            return;
       }

       if (previousCell.transform.position.x < currentCell.transform.position.x)
       {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
       }

       if (previousCell.transform.position.x > currentCell.transform.position.x)
       {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
       }

       if (previousCell.transform.position.z < currentCell.transform.position.z)
       {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
       }

       if (previousCell.transform.position.z > currentCell.transform.position.z)
       {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
       }
    }
}
