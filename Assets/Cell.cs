using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum CellStatus
{
    Start,
    Open,
    Close,
    Use,
}
public class Cell : MonoBehaviour
{
    CellStatus _cellStatus = CellStatus.Open;

    public CellStatus Status
    {
        get { return _cellStatus; }
        set
        {
            _cellStatus = value;
            switch (_cellStatus)
            {
                    case CellStatus.Start:
                    GetComponent<Image>().color = Color.green;
                    break;
                    case CellStatus.Open:
                    GetComponent<Image>().color = Color.gray;
                    break;
                    case CellStatus.Close:
                    GetComponent<Image>().color = Color.black;
                    break;
            }
        }
    }
    public int posX;
    public int posY;
    public List<Cell> NeighborCellsList;

    public Action<Cell> OnCellClickAction;
    public void OnClick()
    {
        if(OnCellClickAction!=null)
        {
            OnCellClickAction(this);
        }
    }
    public int GetNeighborOpenCount()
    {
        return NeighborCellsList.Count(cell => cell != null && cell.Status <= CellStatus.Open);
    }
}
