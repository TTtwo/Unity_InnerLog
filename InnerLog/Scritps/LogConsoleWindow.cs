using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LogConsoleWindow : MonoBehaviour {
    private static LogConsoleWindow MInstance;

    private float MWindowPosX;
    private float MWindowPosY;
    private float MWindowWidth;
    private float MWindowHeight;
    private Rect mWindowRect;
    private Rect mMoveRect;
    private Rect mLabelRect;
    private Rect mStackDragRect;
    private float mLogStackHeight;
    private float mLabelHeight;

    private Texture mInfoIcon;
    private Texture mErrorIcon;
    private Texture mWarnIcon;
    private Texture mLine;

    private bool mIsShowError = true;
    private bool mIsShowInfo = true;
    private bool mIsShowWarn = true;
    private int mInfoCount = 0;
    private int mErrorCount = 0;
    private int mWarnCount = 0;
    private GUIStyle mStyle = new GUIStyle();
            

    private struct LogMessage
    {
        public string message;
        public LogType type;
        public bool canShow;
        public Texture icon;

        public void SetCanShow()
        { 
            LogConsoleWindow _mIn = LogConsoleWindow.MInstance;
            switch (type)
            { 
                case LogType.Log:
                    canShow = _mIn.mIsShowInfo;
                        break;
                case LogType.Warning:
                     canShow = _mIn.mIsShowWarn;
                        break;
                case LogType.Error:
                     canShow = _mIn.mIsShowError;
                        break;
                case LogType.Exception:
                     canShow = _mIn.mIsShowError;
                        break;
            }
        }

        public void SetIcon()
        {
            LogConsoleWindow _mIn = LogConsoleWindow.MInstance;
            switch (type)
            {
                case LogType.Log:
                    icon = _mIn.mInfoIcon;
                    break;
                case LogType.Warning:
                    icon = _mIn.mWarnIcon;
                    break;
                case LogType.Error:
                    icon = _mIn.mErrorIcon;
                    break;
                case LogType.Exception:
                    icon = _mIn.mErrorIcon;
                    break;
            }
        }
    }

    private List<LogMessage> mMessageStack;
    //private List<LogMessage> mShowList = new List<LogMessage>();

    void Awake()
    {
        MWindowPosX = 20;
        MWindowPosY = 20;
        MWindowWidth = 330;
        MWindowHeight = 300;
        mLabelHeight = 40;
        mLogStackHeight = 0;
        mStyle.fontSize = 16;
        mInfoIcon = Resources.Load("Icons/LogInfo") as Texture;
        mErrorIcon = Resources.Load("Icons/LogError") as Texture;
        mWarnIcon = Resources.Load("Icons/LogWarn") as Texture;
        mLine = Resources.Load("Icons/Line") as Texture;
        mMessageStack = new List<LogMessage>();
        MInstance = this;
        Application.RegisterLogCallback(LogListener);
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        if (!Debuger.EnableLogOnScreen) Destroy(gameObject);
    }

    void OnDestroy()
    {
        Application.RegisterLogCallback(null);
    }

    Event mCurrent;
    Vector2 mMousePos;


    bool mIsShowLog = false;
    //string logstring;
    void OnGUI()
    {
        if (!mIsShowLog)
        {
            if (GUI.Button(new Rect(0, 0, 50, 50), "show"))
            {
                mIsShowLog = true;
            }
            return;
        }
        mCurrent = Event.current;
        mMousePos = mCurrent.mousePosition;
        //GUI.Button(new Rect(100, 100, 500, 100), logstring);

        mWindowRect = new Rect(MWindowPosX, MWindowPosY, MWindowWidth, MWindowHeight);
        mWindowRect = GUI.Window(0, mWindowRect, DoMyWindow, "控制台");
        mMoveRect = new Rect(MWindowPosX, MWindowPosY, MWindowWidth, 20);

        MouseEvent();
        
    }

    private Vector2 mScrollPos = Vector2.zero;
    int mCanShowLogTotalCount = 0;
    void DoMyWindow(int windowID)
    {
        Rect _clearRect = new Rect(5,20,40, 30);
        if (GUI.Button(_clearRect, "清空"))
        {
            mMessageStack.Clear();
            mLogStackHeight = 0;
            mErrorCount = mInfoCount = mWarnCount = 0;
            mCanShowLogTotalCount = 0;
        }

        Rect _closeRect = new Rect(MWindowWidth - 45,20,40,30);
        if (GUI.Button(_closeRect, "关闭"))
        {
            mIsShowLog = false;
        }
        GUI.DrawTexture(new Rect(0, 55, MWindowWidth, 2), mLine);
        float _stackHeight = MWindowHeight - 100;
        mStackDragRect = new Rect(0, 55, MWindowWidth, _stackHeight);
        mScrollPos = GUI.BeginScrollView(mStackDragRect, mScrollPos, new Rect(0, 20, MWindowWidth - 20, mLogStackHeight));
        int _logTotalCount = mMessageStack.Count;
        int _showLogAreaCount = (int)(_stackHeight / 40);
        int _showLogHalfCount = (int)((_showLogAreaCount + 10) / 2);
        int _index = (int)(mScrollPos.y / 40 + _showLogAreaCount / 2);
        if (_index < 0) _index = 0;
        int _showIndex = _index - _showLogHalfCount;
        if (_showIndex < 0) _showIndex = 0;
        int _hideIndex = _index + _showLogHalfCount;
        if (_hideIndex >= mCanShowLogTotalCount) _hideIndex = mCanShowLogTotalCount;
        mLogStackHeight = 0;
        //logstring =  ""+mScrollPos.y.ToString() + "  _index" + _index.ToString() + "  _show"+ _showIndex.ToString()+"  _hide"+_hideIndex.ToString()+"  half"+_showLogHalfCount.ToString();
        int _showLogCount = 0;
        for (int i = 0; i != _logTotalCount; i++)
        {
            LogMessage _log = mMessageStack[i];
            if (!_log.canShow) continue;
            mLogStackHeight += 40;
            _showLogCount++;
            if (_showLogCount < _showIndex || _showLogCount > _hideIndex) continue;
            GUI.DrawTexture(new Rect(10, mLogStackHeight, 20, 20), _log.icon);
            GUI.Label(new Rect(32, mLogStackHeight, MWindowWidth, mLabelHeight), new GUIContent(_log.message), mStyle);
        }
        //logstring += " "+_showCount.ToString();
        GUI.EndScrollView();
        GUI.DrawTexture(new Rect(0, MWindowHeight - 40, MWindowWidth, 2), mLine);
        int _x = 10;
        float _y = MWindowHeight - 30;
        string _state = "";
        Callback _setCanShow = delegate()
        {
            mCanShowLogTotalCount = 0;
            for (int i = 0; i != _logTotalCount; i++)
            {
                LogMessage _p = new LogMessage();
                _p = mMessageStack[i];
                _p.SetCanShow();
                mMessageStack[i] = _p;
                if (_p.canShow)
                {
                    mCanShowLogTotalCount++;
                }
            }
            RollLogToButtom();
        };
        _state = mIsShowInfo ? "显示" : "隐藏";
        if (GUI.Button(new Rect(_x, _y - 5, 40, 30), _state))
        {
            mIsShowInfo = !mIsShowInfo;
            _setCanShow();
        }
        _x += 40;
        GUI.DrawTexture(new Rect(_x, _y, 15, 15), mInfoIcon);
        _x += 20;
        GUI.Label(new Rect(_x, _y, 60, 20), mInfoCount.ToString());
        _x += 60;

        _state = mIsShowWarn ? "显示" : "隐藏";
        if (GUI.Button(new Rect(_x, _y - 5, 40, 30), _state))
        {
            mIsShowWarn = !mIsShowWarn;
            _setCanShow();
        }
        _x += 40;
        GUI.DrawTexture(new Rect(_x, _y, 15, 15), mWarnIcon);
        _x += 20;
        GUI.Label(new Rect(_x, _y, 60, 20), mWarnCount.ToString());
        _x += 60;

        _state = mIsShowError ? "显示" : "隐藏";
        if (GUI.Button(new Rect(_x, _y - 5, 40, 30), _state))
        {
            mIsShowError = !mIsShowError;
            _setCanShow();
        }
        _x += 40;
        GUI.DrawTexture(new Rect(_x, _y, 15, 15), mErrorIcon);
        _x += 20;
        GUI.Label(new Rect(_x, _y, 60, 20), mErrorCount.ToString());
        _x += 60;
        
    }

    private bool IsContains(Rect _rect)
    {
        return _rect.Contains(mMousePos);
    }

    bool mIsSelect = false;

    Callback _doDrag = null;
    private void MouseEvent()
    {
        if (!mCurrent.isMouse) return;
        if (Input.GetMouseButton(0))
        {
#if !UNITY_EDITOR
            if (Input.touchCount != 1) return;
#endif
            if (!mIsSelect)
            {
                if (IsContains(mMoveRect))
                {
                    mIsSelect = true;
                    _doDrag = MoveWindow;
                    return;
                }

                Rect _dragArea = new Rect
                (
                    MWindowPosX,
                    MWindowPosY + mStackDragRect.y,
                    mStackDragRect.width,
                    mStackDragRect.height
                );
                if (IsContains(_dragArea))
                {
                    mIsSelect = true;
                    _doDrag = DragStack;
                    return;
                }

                Rect _extendY = new Rect
                (
                   MWindowPosX,
                   MWindowPosY + MWindowHeight,
                   MWindowWidth,
                   20
                );
                if (IsContains(_extendY))
                {
                    mIsSelect = true;
                    _doDrag = ExtendY;
                    return;
                }

                Rect _extendX = new Rect
                (
                   MWindowPosX  + MWindowWidth,
                   MWindowPosY,
                   20,
                   MWindowHeight
                );
                if (IsContains(_extendX))
                {
                    mIsSelect = true;
                    _doDrag = ExtendX;
                    return;
                }
            }
            else
            {
                if(_doDrag != null)
                   _doDrag();
            }
            
        }
        else
        {
            mIsSelect = false;
            _doDrag = null;
        }
    }

    void MoveWindow()
    {
        MWindowPosX += mCurrent.delta.x;
#if UNITY_EDITOR
        MWindowPosY += mCurrent.delta.y;
#else
        MWindowPosY -= mCurrent.delta.y;
#endif
    }

    void DragStack()
    {
        float _pos = mScrollPos.y;
#if UNITY_EDITOR
        _pos -= mCurrent.delta.y;
#else
        _pos += mCurrent.delta.y;
#endif
        mScrollPos.y = _pos;
    }

    void ExtendY()
    { 
#if UNITY_EDITOR
        MWindowHeight += mCurrent.delta.y;
#else
        MWindowHeight -= mCurrent.delta.y;
#endif
        if (MWindowHeight < 200) MWindowHeight = 200;
    }

    void ExtendX()
    {
        MWindowWidth += mCurrent.delta.x;
        if (MWindowWidth < 330) MWindowWidth = 330;
    }


    void LogListener(string condition, string stackTrace, LogType type)
    {
        LogMessage _log = new LogMessage();
        _log.type = type;
        if (type == LogType.Warning)
        {
            condition = "<color=yellow>" + condition + "</color>";
            mWarnCount++;
            if (mWarnCount > 999) mWarnCount = 999;
        }
        else if (type == LogType.Error || type == LogType.Exception)
        {
            condition = "<color=red>" + condition + "</color>";
            mErrorCount++;
            if (mErrorCount > 999) mErrorCount = 999;
        }
        else
        {
            condition = "<color=white>" + condition + "</color>";
            mInfoCount++;
            if (mInfoCount > 999) mInfoCount = 999;
        }
        _log.message = condition;
        _log.SetCanShow();
        _log.SetIcon();
        if (_log.canShow)
        {
            mCanShowLogTotalCount++;
        }
        mMessageStack.Add(_log);
        RollLogToButtom();
    }

    void RollLogToButtom()
    {
        mScrollPos.y = mMessageStack.Count * 40;
    }
}