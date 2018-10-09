using System;
using UnityEngine;
using UnityEngine.UI;

public class PanelStart : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

public InputField InputFieldLength;
    public InputField InputFieldwidth;
    public Panel_Game GamePanel;
    public Text logText;
    private int length;
    private int width;
    public void OnClick()
    {
        int length = -1;
        int width = -1;
        int.TryParse(InputFieldLength.text, out length);
        int.TryParse(InputFieldwidth.text,out width);
        if (length>0 && width >0)
        {
            GamePanel.CreatGrid(length,width);
            GamePanel.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
            logText.text = String.Empty;
        }
        else
        {
            logText.text = "输入有效数字！";
        }
    }
}
