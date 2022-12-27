using Tank.HexMap;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{
    //HexCell 颜色
    public Color[] Colors;
    //
    public HexGrid HexGrid;
    //
    private Color activeColor;

    private void Awake()
    {
        SelectColor(0);
    }
    
    private void Update()
    {
        if(Input.GetMouseButton(0)&&
           !EventSystem.current.IsPointerOverGameObject())
            HandleInput();
    }
    //点击屏幕
    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            //设置HexCell颜色
            HexGrid.ColorCell(hit.point,activeColor);
        }
    }
    //选择颜色
    public void SelectColor(int index)
    {
        activeColor = Colors[index];
    }
    
}
