//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright ?2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Sample script showing how easy it is to implement a standard button that swaps sprites.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Image Button")]
public class UIImageButton : MonoBehaviour
{
	public UISprite target;
	public string normalSprite;
	public string hoverSprite;
	public string pressedSprite;
    public bool makePixelPerfect = true;
    private bool selected = false;    

	void OnEnable ()
	{
		if (target != null)
		{
			target.spriteName = UICamera.IsHighlighted(gameObject) ? hoverSprite : normalSprite;
		}
	}

	void Start ()
	{
		if (target == null) target = GetComponentInChildren<UISprite>();
	}

	void OnHover (bool isOver)
	{
		if (enabled && target != null && !selected)
		{
			target.spriteName = isOver ? hoverSprite : normalSprite;
            if (makePixelPerfect)
			    target.MakePixelPerfect();
		}
	}

	void OnPress (bool pressed)
	{
		if (enabled && target != null)
		{
			target.spriteName = pressed ? pressedSprite : normalSprite;
            if (makePixelPerfect)
			    target.MakePixelPerfect();
		}
	}

    public void SelectedStatus(bool isSelected)
    {
        selected = isSelected;
        if (isSelected)
        {
            target.spriteName = pressedSprite;
        }
        else
        {
            target.spriteName = normalSprite;
        }
    }
}