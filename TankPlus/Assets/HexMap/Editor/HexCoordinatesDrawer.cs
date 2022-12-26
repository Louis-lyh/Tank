using UnityEditor;
using UnityEngine;

namespace Tank.HexMap
{
    /// <summary>
    ///六边形坐标系的属性抽屉 
    /// </summary>
    [CustomPropertyDrawer(typeof(HexCoordinates))]
    public class HexCoordinatesDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            HexCoordinates coordinates = new HexCoordinates(
                property.FindPropertyRelative("X").intValue,
                property.FindPropertyRelative("Z").intValue
                );
            //显示类名
            position = EditorGUI.PrefixLabel(position,GUIUtility.GetControlID(FocusType.Passive),label);
            //显示坐标
            GUI.Label(position,coordinates.ToString());
        }
    }
}

