//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment
//----------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using Mogo.Util;
using Mogo.GameData;

/// <summary>
/// Editable text input field.
/// </summary>

public class MogoInput : MonoBehaviour
{
	public delegate char Validator (string currentText, char nextChar);

	public enum KeyboardType
	{
		Default = 0,
		ASCIICapable = 1,
		NumbersAndPunctuation = 2,
		URL = 3,
		NumberPad = 4,
		PhonePad = 5,
		NamePhonePad = 6,
		EmailAddress = 7,
	}

	public delegate void OnSubmit (string inputString);

	/// <summary>
	/// Current input, available inside OnSubmit callbacks.
	/// </summary>

    static public MogoInput current;

	/// <summary>
	/// Text label modified by this input.
	/// </summary>

	public UILabel label;
    public UISprite spriteBG;
    public UIPanel panel;

    public string LogicText;

	/// <summary>
	/// Maximum number of characters allowed before input no longer works.
	/// </summary>

	public int maxChars = 0;

	/// <summary>
	/// Visual carat character appended to the end of the text when typing.
	/// </summary>

	public string caratChar = "|";

	/// <summary>
	/// Delegate used for validation.
	/// </summary>

	public Validator validator;

	/// <summary>
	/// Type of the touch screen keyboard used on iOS and Android devices.
	/// </summary>

	public KeyboardType type = KeyboardType.Default;

	/// <summary>
	/// Whether this input field should hide its text.
	/// </summary>

	public bool isPassword = false;

	/// <summary>
	/// Whether to use auto-correction on mobile devices.
	/// </summary>

	public bool autoCorrect = false;

	/// <summary>
	/// Whether the label's text value will be used as the input's text value on start.
	/// By default the label is just a tooltip of sorts, letting you choose helpful
	/// half-transparent text such as "Press Enter to start typing", while the actual
	/// value of the input field will remain empty.
	/// </summary>

	public bool useLabelTextAtStart = false;

	/// <summary>
	/// Color of the label when the input field has focus.
	/// </summary>

	public Color activeColor = Color.white;

	/// <summary>
	/// Event receiver that will be notified when the input field submits its data (enter gets pressed).
	/// </summary>

	public GameObject eventReceiver;

	/// <summary>
	/// Function that will be called on the event receiver when the input field submits its data.
	/// </summary>

	public string functionName = "OnSubmit";

	/// <summary>
	/// Delegate that will be notified when the input field submits its data (by default that's when Enter gets pressed).
	/// </summary>

	public OnSubmit onSubmit;

	string mText = "";
	string mDefaultText = "";
	Color mDefaultColor = Color.white;
	UIWidget.Pivot mPivot = UIWidget.Pivot.Left;
	float mPosition = 0f;
    List<GameObject> mFaceList = new List<GameObject>();

#if UNITY_IPHONE || UNITY_ANDROID
#if UNITY_3_4
	iPhoneKeyboard mKeyboard;
#else
	TouchScreenKeyboard mKeyboard;
#endif
#else
	string mLastIME = "";
#endif

	/// <summary>
	/// Input field's current text value.
	/// </summary>

	public virtual string text
	{
		get
		{
			if (mDoInit) Init();
			return mText;
		}
		set
		{
			if (mDoInit) Init();

			mText = value;

			if (label != null)
			{
				if (string.IsNullOrEmpty(value)) value = mDefaultText;

				label.supportEncoding = false;
				label.text = selected ? value + caratChar : value;
				label.showLastPasswordChar = selected;
				label.color = (selected || value != mDefaultText) ? activeColor : mDefaultColor;
			}

            UpdateLabel();
		}
	}


	/// <summary>
	/// Whether the input is currently selected.
	/// </summary>

	public bool selected
	{
		get
		{
			return UICamera.selectedObject == gameObject;
		}
		set
		{
			if (!value && UICamera.selectedObject == gameObject) UICamera.selectedObject = null;
			else if (value) UICamera.selectedObject = gameObject;

		}
	}

	/// <summary>
	/// Set the default text of an input.
	/// </summary>

	public string defaultText
	{
		get
		{
			return mDefaultText;
		}
		set
		{
			if (label.text == mDefaultText) label.text = value;
			mDefaultText = value;
		}
	}

	/// <summary>
	/// Labels used for input shouldn't support color encoding.
	/// </summary>

	protected void Init ()
	{
		if (mDoInit)
		{
			mDoInit = false;
			if (label == null) label = GetComponentInChildren<UILabel>();

			if (label != null)
			{
				if (useLabelTextAtStart) mText = label.text;
				mDefaultText = label.text;
				mDefaultColor = label.color;
				label.supportEncoding = false;
				mPivot = label.pivot;
				mPosition = label.cachedTransform.localPosition.x;
			}
			else enabled = false;
		}
	}

	bool mDoInit = true;

	/// <summary>
	/// If the object is currently highlighted, it should also be selected.
	/// </summary>

	void OnEnable () { if (UICamera.IsHighlighted(gameObject)) OnSelect(true); }

	/// <summary>
	/// Remove the selection.
	/// </summary>

	void OnDisable () { if (UICamera.IsHighlighted(gameObject)) OnSelect(false); }

	/// <summary>
	/// Selection event, sent by UICamera.
	/// </summary>

	void OnSelect (bool isSelected)
	{
		if (mDoInit) Init();

		if (label != null && enabled && NGUITools.GetActive(gameObject))
		{
			if (isSelected)
			{
               // mText = (label.text == mDefaultText) ? "" :  label.text;
				label.color = activeColor;
				if (isPassword) label.password = true;

#if UNITY_IPHONE || UNITY_ANDROID
				if (Application.platform == RuntimePlatform.IPhonePlayer ||
					Application.platform == RuntimePlatform.Android)
				{
#if UNITY_3_4
					mKeyboard = iPhoneKeyboard.Open(mText, (iPhoneKeyboardType)((int)type), autoCorrect);
#else
					if (isPassword)
					{
						mKeyboard = TouchScreenKeyboard.Open(mText, TouchScreenKeyboardType.Default, false, false, true);
					}
					else
					{
						mKeyboard = TouchScreenKeyboard.Open(mText, (TouchScreenKeyboardType)((int)type), autoCorrect);
					}
#endif
				}
				else
#endif
				{
					Input.imeCompositionMode = IMECompositionMode.On;
					Transform t = label.cachedTransform;
					Vector3 offset = label.pivotOffset;
					offset.y += label.relativeSize.y;
					offset = t.TransformPoint(offset);
					Input.compositionCursorPos = UICamera.currentCamera.WorldToScreenPoint(offset);
				}
				//UpdateLabel();
			}
			else
			{
#if UNITY_IPHONE || UNITY_ANDROID
				if (mKeyboard != null)
				{
					mKeyboard.active = false;
				}
#endif
				if (string.IsNullOrEmpty(mText))
				{
					label.text = mDefaultText;
					label.color = mDefaultColor;
					if (isPassword) label.password = false;
				}
				else label.text = mText;

				label.showLastPasswordChar = false;
				Input.imeCompositionMode = IMECompositionMode.Off;
				RestoreLabel();
			}
		}
	}

#if UNITY_IPHONE || UNITY_ANDROID
	/// <summary>
	/// Update the text and the label by grabbing it from the iOS/Android keyboard.
	/// </summary>

	void Update()
	{
        UpdateLabel();

		if (mKeyboard != null)
		{

			string text = mKeyboard.text;
         
			if (mText != text)
			{
				mText = "";

				for (int i = 0; i < text.Length; ++i)
				{
					char ch = text[i];
					if (validator != null) ch = validator(mText, ch);
                    if (ch != 0)
                    {
                        mText += ch;
                    }
				}

				if (mText != text) mKeyboard.text = mText;
				//UpdateLabel();
			}

			if (mKeyboard.done)
			{
				mKeyboard = null;
				current = this;
				if (onSubmit != null) onSubmit(mText);
				if (eventReceiver == null) eventReceiver = gameObject;
				eventReceiver.SendMessage(functionName, mText, SendMessageOptions.DontRequireReceiver);
				current = null;
				selected = false;
			}
		}
	}
#else
	void Update ()
	{
		if (selected && mLastIME != Input.compositionString)
		{
			mLastIME = Input.compositionString;
			UpdateLabel();
		}
	}
#endif

	/// <summary>
	/// Input event, sent by UICamera.
	/// </summary>

	void OnInput (string input)
	{
		if (mDoInit) Init();

		if (selected && enabled && NGUITools.GetActive(gameObject))
		{
			// Mobile devices handle input in Update()
			if (Application.platform == RuntimePlatform.Android) return;
			if (Application.platform == RuntimePlatform.IPhonePlayer) return;


			for (int i = 0, imax = input.Length; i < imax; ++i)
			{
				char c = input[i];

				if (c == '\b')
				{
					// Backspace
					if (mText.Length > 0) 
					{
                        if (mText.Length > 2 && mText.Substring(mText.Length - 3, 3) == "[-]")  //Maifeo
                        {
                            
                            int tmp = 4;

                            while (tmp < mText.Length)
                            {
                                if (mText[mText.Length - tmp] == ']' && tmp + 7 <= mText.Length)
                                {
                                    if (mText[mText.Length - (tmp + 7)] == '[')
                                    {
                                        mText = mText.Substring(0,mText.Length - (tmp + 7) - 14);
                                        break;
                                    }
                                }

                                ++tmp;
                            }
                        }                                                                       //MaiFeo
                        else if (mText.Length > 0 && mText[mText.Length - 1] == '}')
                        {
                            int tmp = 2;

                            while (tmp < mText.Length)
                            {
                                if (mText[mText.Length - tmp] == ':' && tmp + 1 <= mText.Length)
                                {
                                    if (mText[mText.Length - (tmp + 1)] == '{')
                                    {
                                        mText = mText.Substring(0, mText.Length - (tmp + 1) - 10);

                                        AssetCacheMgr.ReleaseInstance(mFaceList[mFaceList.Count - 1]);
                                        mFaceList.Remove(mFaceList[mFaceList.Count - 1]);
                                        break;
                                    }
                                }

                                ++tmp;
                            }
                            //mText = mText.Substring(0, mText.Length - 14);
                        }
                        else
                        {
                            mText = mText.Substring(0, mText.Length - 1);
                        }
						SendMessage("OnInputChanged", this, SendMessageOptions.DontRequireReceiver);
					}
				}
				else if (c == '\r' || c == '\n' )
				{
					if (UICamera.current.submitKey0 == KeyCode.Return || UICamera.current.submitKey1 == KeyCode.Return)
					{
						// Not multi-line input, or control isn't held
						if (!label.multiLine || (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl)))
						{
							// Enter
							current = this;
							if (onSubmit != null) onSubmit(mText);
							if (eventReceiver == null) eventReceiver = gameObject;
							eventReceiver.SendMessage(functionName, mText, SendMessageOptions.DontRequireReceiver);
							current = null;
							selected = false;
							return;
						}
					}

					// If we have an input validator, validate the input first
					if (validator != null) c = validator(mText, c);

					// If the input is invalid, skip it
					if (c == 0) continue;

					// Append the character
					if (c == '\n' || c == '\r')
					{
						if (label.multiLine) mText += "\n";
					}
					else mText += c;

					// Notify the listeners
					SendMessage("OnInputChanged", this, SendMessageOptions.DontRequireReceiver);
				}
				else if (c >= ' ')
				{
					// If we have an input validator, validate the input first
                    if (validator != null)
                        c = validator(mText, c);

					// If the input is invalid, skip it
					if (c == 0) continue;

					// Append the character and notify the "input changed" listeners.
					mText += c;
					SendMessage("OnInputChanged", this, SendMessageOptions.DontRequireReceiver);
				}
			}

			// Ensure that we don't exceed the maximum length
			UpdateLabel();
		}
	}

    string SplitLogicText(string text)
    {
     //   text = text.Replace("[FACE]", "");
        string tmp = text;


        int length = text.Length;

        for (int i = 0; i < length; ++i)
        {
            if (text[i] == '[' && i + 7 < length && text[i + 7] == ']')
            {
                int index = i + 8;

                while (index < length)
                {
                    if (text[index] == '[' && index + 2 < length && text.Substring(index, 3) == "[-]")
                    {
                        tmp = tmp.Replace(text.Substring(i, index + 3 - i), "");
                        break;
                    }

                    ++index;
                }
            }
            else if (text[i] == '{' && i + 1 < length && text[i + 1] == ':')
            {
                int index = i + 2;

                while(index < length)
                {
                    if(text[index] == '}')
                    {
                        tmp = tmp.Replace(text.Substring(i,index - i + 1),"");
                        break;
                    }

                    ++index;
                }
            }
        }

        return tmp;
    }

    string AddFaceToChatUIInput(string text)
    {
        string tmp = text;
		string lastText = text;
		string lastTmp = text;
        int length = text.Length;

        int faceIconId = 1;
      

        for (int i = 0; i < length; ++i)
        {
            if (text[i] == '{' && i + 1 < length && text[i + 1] == ':')
            {
                int index = i + 2;

                while (index < length)
                {
                    if (text[index] == '}')// && index + 2 < length && text.Substring(index, 3) == "[-]")
                    {
						lastText = text.Substring(index+1);
						lastTmp = lastText;
						int lastLenth = lastText.Length;
						
						for (int l = 0; l < lastLenth; ++l)
        				{
            				if (lastText[l] == '{' && l + 1 < lastLenth && lastText[l + 1] == ':')
            				{
                				int lastIndex = l + 2;

                				while (lastIndex < lastLenth)
                				{
                    				if (lastText[lastIndex] == '}')// && index + 2 < length && text.Substring(index, 3) == "[-]")
                    				{
                                        //Debug.LogError(lastText.Substring(l, lastIndex - l + 1));
                        				lastTmp = lastTmp.ReplaceFirst(lastText.Substring(l, lastIndex - l + 1),"　");     
                   
                        				break;
                    				}

	                    			++lastIndex;
            				    }
							}
           	 			}

                        //Debug.LogError(text.Substring(i+2, 1));
                        tmp = tmp.ReplaceFirst(text.Substring(i, index - i + 1), "　");
                        bool isAlready = false;
                        int faceId = -1;

                        if (text.Substring(i, index - i + 1).Length == 4)
                        {
                             faceIconId = int.Parse(text.Substring(i + 2, 1))+1;
                        }
                        else if (text.Substring(i, index - i + 1).Length == 5)
                        {
                             faceIconId = int.Parse(text.Substring(i + 2, 2))+1;
                        }
                        for (int j = 0; j < mFaceList.Count; ++j)
                        {
                            if (mFaceList[j].name == "Face" + i.ToString())
                            {
                                isAlready = true;
                                faceId = j;
                                break;
                            }
                        }

                        if (!isAlready)
                        {
                            AssetCacheMgr.GetUIInstance("FaceIconDialog.prefab", (prefab, id, go) =>
                            {


                                GameObject obj = (GameObject)go;
                                obj.name = "Face" + i.ToString();
                                obj.SetActive(true);
                                obj.transform.parent = transform.Find("CommunityUIPrivateInputList");
                                obj.transform.localScale = new Vector3(0.7f, 0.7f, 1);
                                obj.transform.localPosition = new Vector3(label.transform.localPosition.x -
                                    label.font.CalculatePrintedSize(text.Substring(index+1), true, UIFont.SymbolStyle.None).x * 22,
                                    label.transform.localPosition.y, label.transform.localPosition.z);

                                UISprite sp = obj.transform.GetComponentsInChildren<UISprite>(true)[0];
                                sp.pivot = UIWidget.Pivot.Right;
                                sp.transform.localPosition = Vector3.zero;

                                //Debug.LogError(faceIconId + " " + UIFaceIconData.dataMap[faceIconId].facefirst);
                                sp.spriteName = UIFaceIconData.dataMap[faceIconId].facefirst;
                                obj.GetComponentsInChildren<UISpriteAnimation>(true)[0].namePrefix =
                                    UIFaceIconData.dataMap[faceIconId].facehead;

                                mFaceList.Add(obj);
                            });
                        }
                        else
                        {
                            if (faceId != -1)
                            {
                                mFaceList[faceId].transform.localPosition = new Vector3(label.transform.localPosition.x -
                                    label.font.CalculatePrintedSize(lastTmp,true, UIFont.SymbolStyle.None).x * 22,
									label.transform.localPosition.y, label.transform.localPosition.z);
                            }
                        }
                        break;
                    }

                    ++index;
                }
            }
        }

        float lineWidth = CommunityUILogicManager.Instance.GetLabelOffect();
        Vector2 vec2CamPos = CommunityUILogicManager.Instance.GetCameraSize();

        //float fOffect = -lineWidth + 22 * label.font.CalculatePrintedSize(tmp, true, UIFont.SymbolStyle.None).x;
        //if (fOffect >= 0)
        //{
        //    fOffect = 0;
        //}
        //label.transform.localPosition = new Vector3(fOffect+270, label.transform.localPosition.y, label.transform.localPosition.z);

        float textWidth = 22 * label.font.CalculatePrintedSize(tmp, true, UIFont.SymbolStyle.None).x;
        float fOffect = vec2CamPos.x - textWidth;

        if (fOffect <= vec2CamPos.y)
        {
            fOffect = vec2CamPos.y;
        }

        CommunityUILogicManager.Instance.SetTextCameraPosX(fOffect);

        spriteBG.transform.localScale = new Vector3(lineWidth, spriteBG.transform.localScale.y, spriteBG.transform.localScale.z);
        spriteBG.transform.localPosition = new Vector3(270, spriteBG.transform.localPosition.y, spriteBG.transform.localPosition.z);
        //panel.clipRange = new Vector4(panel.clipRange.x, panel.clipRange.y, CommunityUILogicManager.Instance.GetPanelClipSize(), panel.clipRange.w);
        return tmp;
        //bool isAlready = false;
        //int faceId = -1;
            

        //for (int j = 0; j < mFaceList.Count; ++j)
        //{
        //    if (mFaceList[j].name == "Face" + i.ToString())
        //    {
        //        isAlready = true;
        //        faceId = j;
        //        break;
        //    }
        //}

        //if (!isAlready)
        //{
        //    AssetCacheMgr.GetUIInstance("ChatUIFace.prefab", (prefab, id, go) =>
        //    {
        //        GameObject obj = (GameObject)go;
        //        obj.name = "Face" + i.ToString();
        //        obj.SetActive(true);
        //        obj.transform.parent = transform;
        //        obj.transform.localPosition = new Vector3(label.transform.localPosition.x -
        //            label.font.CalculatePrintedSize(text.Substring(index + 1), true, UIFont.SymbolStyle.None).x  * 22,
        //            obj.transform.localPosition.y, obj.transform.localPosition.z);
        //        LoggerHelper.Debug(label.font.CalculatePrintedSize(text.Substring(index + 1), true, UIFont.SymbolStyle.None));
        //        LoggerHelper.Debug(text.Substring(index + 1));

        //        mFaceList.Add(obj);
        //    });
        //}
        //else
        //{
        //    if (faceId != -1)
        //    {
        //        mFaceList[faceId].transform.localPosition = label.transform.localPosition - new Vector3(
        //            label.font.CalculatePrintedSize(text.Substring(index + 1), true, UIFont.SymbolStyle.None).x * 22, 0, 0);
        //    }
        //}

    }

    string SplitViewText(string text)
    {
       // text = text.Replace("[FACE]", "    ");
        string tmp = text;

        int length = text.Length;

        for (int i = 0; i < length; ++i)
        {
            if (text[i] == '<' && i + 5 < length && text.Substring(i, 6) == "<info=")
            {
                int index = i + 6;

                while (index < length)
                {
                    if (text[index] == '>')// && index + 2 < length && text.Substring(index, 3) == "[-]")
                    {
                        tmp = tmp.Replace(text.Substring(i, index - i + 1), "");
                        break;
                    }

                    ++index;
                }
            }
            else if (text[i] == '<' && i + 5 < length && text.Substring(i, 6) == "<face=")
            {
                int index = i + 6;

                while (index < length)
                {
                    if (text[index] == '>')// && index + 2 < length && text.Substring(index, 3) == "[-]")
                    {
                        tmp = tmp.Replace(text.Substring(i, index - i + 1), "");                       
                        break;
                    }
                    ++index;
                }
            }
        }

        return tmp;
    }


	/// <summary>
	/// Update the visual text label, capping it at maxChars correctly.
	/// </summary>

	void UpdateLabel ()
	{
		if (mDoInit) Init();
		if (maxChars > 0 && mText.Length > maxChars) mText = mText.Substring(0, maxChars);

		if (label.font != null)
		{
			// Start with the text and append the IME composition and carat chars
			string processed;

			if (isPassword && selected)
			{
				processed = "";
				for (int i = 0, imax = mText.Length; i < imax; ++i) processed += "*";
				processed += Input.compositionString + caratChar;
			}
			else processed = selected ? (mText + Input.compositionString + caratChar) : mText;

			// Now wrap this text using the specified line width
			//label.supportEncoding = false;

			if (label.multiLine)
			{
				processed = label.font.WrapText(processed, label.lineWidth / label.cachedTransform.localScale.x, 0, false, UIFont.SymbolStyle.None);
			}
			else
			{
				string fit = label.font.GetEndOfLineThatFits(processed, label.lineWidth / label.cachedTransform.localScale.x, false, UIFont.SymbolStyle.None);

				if (fit != processed)
				{
					processed = fit;
					Vector3 pos = label.cachedTransform.localPosition;
					pos.x = mPosition + label.lineWidth;
					label.cachedTransform.localPosition = pos;

					if (mPivot == UIWidget.Pivot.Left) label.pivot = UIWidget.Pivot.Right;
					else if (mPivot == UIWidget.Pivot.TopLeft) label.pivot = UIWidget.Pivot.TopRight;
					else if (mPivot == UIWidget.Pivot.BottomLeft) label.pivot = UIWidget.Pivot.BottomRight;
				}
				else RestoreLabel();
			}

			// Update the label's visible text

            LogicText = SplitLogicText(processed);
            label.text = AddFaceToChatUIInput(SplitViewText(processed));// processed;
            //label.text = processed;


			label.showLastPasswordChar = selected;
		}
	}

    public int GetInputLabelContentCount()
    {
        int count = label.text.Length;

        for (int i = 0; i < label.text.Length; ++i)
        {
            if (label.text[i] == '[')
            {
                if (i + 7 < label.text.Length )
                {
                    if (label.text[i + 7] == ']')
                    {
                        count -= 11;
                    }
                }
            }
        }

        return count;
    }
    /// <summary>
    /// Restore the input label's pivot point and position.
    /// </summary>

    void RestoreLabel()
	{
		if (label != null)
		{
			label.pivot = mPivot;
			Vector3 pos = label.cachedTransform.localPosition;
			pos.x = mPosition;
			label.cachedTransform.localPosition = pos;
		}
	}

    public void EmptyInput()
    {
        text = "";

        for (int i = 0; i < mFaceList.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(mFaceList[i]);
        }


        mFaceList.Clear();
    }

    void Awake()
    {
        spriteBG = transform.GetComponentsInChildren<UISprite>(true)[0];
        panel = transform.parent.GetComponentsInChildren<UIPanel>(true)[0];
    }

}
