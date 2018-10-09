using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum dirction
{
    start,
    up,
    down,
    left,
    right,
}
public class Panel_Game : MonoBehaviour
{
    public PanelStart PanelStart;
    public GridLayoutGroup GridLayout;
    public Cell cellPrefab;
    public Text logText;
    public Toggle StartToggle;
    public Toggle OpenToggle;
    public LineRenderer LineRenderer;
    //n *n 的所有格子列表
     List<Cell> cellList= new List<Cell>();
      Cell startCell;
      private bool result = false;

    public void OnReturnClick()
    {
        logText.text = String.Empty;
        answerCells.Clear();
        result = false;
        cellList.Clear();
        startCell = null;
        while(GridLayout.transform.childCount>0)
        {
            DestroyImmediate(GridLayout.transform.GetChild(0).gameObject);
        }
        PanelStart.gameObject.SetActive(true);
        LineRenderer.SetVertexCount(0);
        this.gameObject.SetActive(false);

    }
    //          j
    //  i       *****************
    //          *
    //          *
    //          *
    //          *
    //          *
    public void CreatGrid(int length,int width)
    {
        result = false;
        cellList.Clear();
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Cell cell = Instantiate(cellPrefab);
                cell.posX = i;
                cell.posY = j;
                cell.transform.SetParent(GridLayout.transform);
                cell.transform.localPosition = Vector3.zero;
                cell.transform.localScale = Vector3.one;
                cell.gameObject.SetActive(true);
                cellList.Add(cell);
            }
        }
        GridLayout.constraintCount = width;
        foreach(var cell in cellList)
        {
            cell.OnCellClickAction = OnCellClick;
        }
    }

    public void CreatLine()
    {
        if(result)
            return;
        //校验
        if(startCell == null)
        {
            logText.text = "未设置起始点";
            return;
        }
        logText.text = String.Empty;
        InitCells(cellList);
        if(DPS(startCell))
        {
            Vector3[] pos = answerCells.Select((a) => { return a.transform.position + Vector3.back; }).ToArray();
            List<Vector3> LinePos = new List<Vector3>();
            for(int i = 0;i < pos.Length - 1;i++)
            {
                Vector3[] temp = SplitVector3(pos[i],pos[i + 1],5);
                LinePos.AddRange(temp.ToList());
            }
            LineRenderer.SetVertexCount(LinePos.Count);
            LineRenderer.SetPositions(LinePos.ToArray());
            logText.text = "成功!";
            result = true;
        }
        else
        {
            logText.text = "失败";
            result = false;
        }
    }

    /// <summary>
    /// 初始化邻居关系
    /// </summary>
    /// <param name="cells"></param>
    private void InitCells(List<Cell> cells)
    {
        for (int i = 0; i < cells.Count; i++)
        {

            if (cells[i].Status==CellStatus.Close)
            {
                continue;
            }
            if (cells[i].upCell ==null )
            {
                cells[i].upCell = GetCell(cells, cells[i].posX - 1, cells[i].posY);
                if(cells[i].upCell != null)
                    cells[i].upCell.downCell = cells[i];
            }
            if(cells[i].downCell == null)
            {
                cells[i].downCell = GetCell(cells,cells[i].posX + 1,cells[i].posY);
                if(cells[i].downCell != null)
                    cells[i].downCell.upCell = cells[i];
            }
            if(cells[i].leftCell == null)
            {
                cells[i].leftCell = GetCell(cells,cells[i].posX,cells[i].posY-1);
                if(cells[i].leftCell != null)
                    cells[i].leftCell.rightCell = cells[i];
            }
            if(cells[i].rightCell == null)
            {
                cells[i].rightCell = GetCell(cells,cells[i].posX,cells[i].posY+1);
                if(cells[i].rightCell != null)
                    cells[i].rightCell.leftCell = cells[i];
            }
        }
    }

    private Cell GetCell(List<Cell> cells,int x,int y)
    {
        foreach (var cell in cells)
        {
            if(cell.Status != CellStatus.Close && cell.posX == x && cell.posY == y)
                return cell;
        }
        return null;
    }

    /// <summary>
    /// 微分向量
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="count">算起始结束一共分多少</param>
    /// <returns></returns>
    private Vector3[] SplitVector3(Vector3 start,Vector3 end,int count)
    {
        Vector3[] ret = new Vector3[count];
        float x = (end.x - start.x) / (count-1);
        float y = (end.y - start.y) / (count - 1);
        float z = (end.z - start.z) / (count - 1);
        for (int i = 0; i < count; i++)
        {
            ret[i] = start + new Vector3(x, y, z)*i;
        }
        return ret;
    }

    List<Cell> answerCells = new List<Cell>();
    private bool DPS(Cell cell)
    {
        cell.Status = CellStatus.Use;
        answerCells.Add(cell);
        if(cell.upCell != null && cell.upCell.Status == CellStatus.Open)
        {
            if(DPS(cell.upCell))
                return true;
        }
         if(cell.downCell != null && cell.downCell.Status == CellStatus.Open)
        {
            if(DPS(cell.downCell))
            return true;
        }
         if(cell.leftCell != null && cell.leftCell.Status == CellStatus.Open)
         {
             if (DPS(cell.leftCell))
                 return true;
         }
         if(cell.rightCell != null && cell.rightCell.Status == CellStatus.Open)
         {
             if (DPS(cell.rightCell))
                 return true;
         }

         List<Cell> openCells = cellList.Where(a => a.Status == CellStatus.Open).ToList();
         if(openCells.Count <= 0)
         {
             logText.text = "Success!";
             return true;
         }
        answerCells.Remove(cell);
        cell.Status = CellStatus.Open;
        return false;
    }
    void OnCellClick(Cell cell)
    {
        if(StartToggle.isOn)
        {
            foreach(var cell1 in cellList)
            {
                if(cell1.Status == CellStatus.Start)
                {
                    cell1.Status = CellStatus.Open;
                    break;
                }
            }
            cell.Status = CellStatus.Start;
            startCell = cell;
            return;
        }
        else if(cell.Status == CellStatus.Start && OpenToggle.isOn)
        {
            return;
        }
        else
        {
            cell.Status = cell.Status == CellStatus.Open ? CellStatus.Close : CellStatus.Open;
        }
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}
