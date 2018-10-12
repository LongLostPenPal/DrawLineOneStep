using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
    private Cell[][] cells;
    Cell startCell;
    Cell endCell;
    /// <summary>
    /// 只有一个打开邻居的格子
    /// </summary>
    Cell OneOpenNeighboeCell;
    //开局打开格子数量
    private int openCellCnt = 0;
    private bool result = false;
    private string TipString1 =  "未设置起始点";
    private string TipString2 = "成功! 用时:";
    private string TipString3 = "失败";
    public void OnReturnClick()
    {
        logText.text = String.Empty;
        answerCells.Clear();
        openCellCnt = 0;
        result = false;
        cells = null;
        startCell = null;
        endCell = null;
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
        cells = new Cell[length][]; 
        for (int i = 0; i < length; i++)
        {
            cells[i] = new Cell[width];
            for (int j = 0; j < width; j++)
            {
                Cell cell = Instantiate(cellPrefab);
                cell.posX = i;
                cell.posY = j;
                cell.transform.SetParent(GridLayout.transform);
                cell.transform.localPosition = Vector3.zero;
                cell.transform.localScale = Vector3.one;
                cell.gameObject.SetActive(true);
                cell.OnCellClickAction = OnCellClick;
                cells[i][j] = cell;
            }
        }
        GridLayout.constraintCount = width;
    }

    public void CreatLine()
    {
        if(result)
            return;
        //校验
        if(startCell == null)
        {
            logText.text =TipString1;
            return;
        }
        openCellCnt=InitCells(cells);
        var  oneNeighborCells= GetOneNeighborOpenCells();
        if(oneNeighborCells.Count > 1)
        {
            logText.text = TipString3;
            return;
        }
        else if(oneNeighborCells.Count==1)
        {
            endCell = oneNeighborCells[0];
        }
        logText.text = String.Empty;
        DateTime dataDateTime1 = DateTime.Now;
        if(DFS(startCell))
        {
            Vector3[] pos = answerCells.Select((a) => a.transform.position + Vector3.back).ToArray();
            List<Vector3> linePos = new List<Vector3>();
            for(int i = 0;i < pos.Length - 1;i++)
            {
                Vector3[] temp = SplitVector3(pos[i],pos[i + 1],5);
                linePos.AddRange(temp.ToList());
            }
            LineRenderer.SetVertexCount(linePos.Count);
            LineRenderer.SetPositions(linePos.ToArray());
            logText.text = TipString2;
            result = true;
            DateTime dataDateTime2 = DateTime.Now;
            int s = (dataDateTime2 - dataDateTime1).Seconds;
            int ms = (dataDateTime2 - dataDateTime1).Milliseconds;
            logText.text +="  "+ s + "s " + ms + "ms ";
        }
        else
        {
            logText.text = TipString3;
            result = false;
        }
    }

    /// <summary>
    /// 初始化邻居关系
    /// </summary>
    /// <param name="cells"></param>
    private int InitCells(Cell[][] cells)
    {
        int openCnt = 0;
        for (int i = 0; i < cells.Length; i++)
        {
            for (int j = 0; j < cells[i].Length; j++)
            {
                if (cells[i][j].Status == CellStatus.Close)
                {
                    continue;
                }
                openCnt++;
                if(i - 1 >= 0)
                {
                    cells[i][j].NeighborCellsList.Add(cells[i - 1][j]);
                }
                if(i + 1 < cells.Length)
                {
                    cells[i][j].NeighborCellsList.Add(cells[i + 1][j]);
                }
                if(j - 1 >= 0)
                {
                    cells[i][j].NeighborCellsList.Add(cells[i][j - 1]);
                }
                if(j + 1 < cells[i].Length)
                {
                    cells[i][j].NeighborCellsList.Add(cells[i][j + 1]);
                }
            }
        }
        return openCnt;
    }
    /// <summary>
    /// 开始时检测所有格子
    /// </summary>
    /// <returns></returns>
    private List<Cell> GetOneNeighborOpenCells()
    {
        List<Cell> retCells = new List<Cell>();
        for(int i = 0;i < cells.Length;i++)
        {
            for(int j = 0;j < cells[i].Length;j++)
            {
                if (cells[i][j].Status==CellStatus.Open && cells[i][j].GetNeighborOpenCount()<=1)
                {
                    retCells.Add(cells[i][j]);
                }
            }
        }
        return retCells;
    }
    /// <summary>
    /// 遍历时 检测当前格子邻居只有一个出口的方法（当前格子只会影响自己邻居状态，不用遍历全部格子）
    /// 这个可以写成扩展方法
    /// </summary>
    /// <returns></returns>
    private static List<Cell> GetOneNeighborOpenCells(Cell cell)
    {
        return cell.NeighborCellsList.Where(cell1 => cell1.Status == CellStatus.Open && cell1.GetNeighborOpenCount() <= 1).ToList();
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

    private bool DFS(Cell cell)
    {
        //TODO 当起始就有一个终点时
        cell.Status = CellStatus.Use;
//        Debug.LogError(cell.posX+"  "+cell.posY);
        answerCells.Add(cell);

        Action undoAction = () =>
        {
            answerCells.Remove(cell);
            cell.Status = CellStatus.Open;
        };

        List<Cell> neighborOneOpen = GetOneNeighborOpenCells(cell);
        if(neighborOneOpen.Count > 1 &&endCell!=null)//走一步 造成两个邻居死亡
        {
            undoAction();
            return false;
        }
        if(neighborOneOpen.Count == 1)
        {
            if(DFS(neighborOneOpen[0]))
            {
                return true;
            }
            else
            {
                undoAction();
                return false;
            }
        }

        if (cell.NeighborCellsList.Where(cell1 => cell1.Status == CellStatus.Open).Any(DFS))
        {
            return true;
        }
         if(openCellCnt == answerCells.Count)
         {
             return true;
         }
        undoAction();
        return false;
    }
    void OnCellClick(Cell cell)
    {
        if(StartToggle.isOn)
        {
            if (null !=startCell)
            {
                startCell.Status = CellStatus.Open;
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
