using UnityEngine;

namespace Editor
{
    public static class GUIStyles
    {
        public static GUIStyle Header
        {
            get
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.fontStyle = FontStyle.Bold;
                return style;
            }
        }
        
        public static GUIStyle Rich
        {
            get
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.richText = true;
                return style;
            }
        }
    }
}