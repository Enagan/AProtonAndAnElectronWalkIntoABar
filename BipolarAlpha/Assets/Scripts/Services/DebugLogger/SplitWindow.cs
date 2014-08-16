using UnityEngine;
using UnityEditor;

public class SplitWindow : EditorWindow
{
  public float currentScrollViewHeight;
  bool resize = false;
  Rect cursorChangeRect;
  private static Texture2D splitTex;

  public void OnEnable()
  {
    currentScrollViewHeight = this.position.height / 2;

  }

  private void createSplitTex()
  {
    splitTex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
    splitTex.SetPixel(0, 0, new Color(0.335f, 0.335f, 0.335f));
    splitTex.Apply();
  }
  public void drawWindow(float width)
  {
    if (splitTex == null)
      this.createSplitTex();

    cursorChangeRect = new Rect(0, currentScrollViewHeight, width, 2.0f);

    

    ResizeScrollView();

    GUILayout.FlexibleSpace();



  }

  private void ResizeScrollView()
  {

    GUI.DrawTexture(cursorChangeRect, splitTex);
    EditorGUIUtility.AddCursorRect(cursorChangeRect, MouseCursor.ResizeVertical);

    if (Event.current.type == EventType.mouseDown && cursorChangeRect.Contains(Event.current.mousePosition))
    {
      resize = true;
    }
    if (resize)
    {
      currentScrollViewHeight = Event.current.mousePosition.y;
      cursorChangeRect.Set(cursorChangeRect.x, currentScrollViewHeight, cursorChangeRect.width, cursorChangeRect.height);
    }
    if (Event.current.type == EventType.MouseUp)
      resize = false;
  }
}