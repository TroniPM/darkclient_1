using UnityEngine;
using System.Collections;
using Mogo.Util;

public class RuneUILogicManager
{

    private static RuneUILogicManager m_instance;

    public static RuneUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new RuneUILogicManager();
            }

            return RuneUILogicManager.m_instance;

        }
    }

    void OnGotoDragonUp()
    {
        LoggerHelper.Debug("GotoDragonUp");
    }

    void OnRuneUIComposeUp()
    {
        LoggerHelper.Debug("RuneUIComposeUp");
        EventDispatcher.TriggerEvent<bool>(Events.RuneEvent.AutoCombine, false);
    }

    void OnRuneUICloseUp()
    {
        LoggerHelper.Debug("RuneUICloseUp");
    }

    void OnRuneUIPackageGirdDrag(int newGrid, int oldGrid)
    {
        LoggerHelper.Debug(newGrid + " "+oldGrid);
        EventDispatcher.TriggerEvent<int, int, bool>(Events.RuneEvent.ChangeIndex, oldGrid, newGrid, false);
    }

    void OnRuneUIInsetGridDrag(int newGrid, int oldGrid)
    {
        LoggerHelper.Debug(newGrid + " " + oldGrid);
        EventDispatcher.TriggerEvent<int, int>(Events.RuneEvent.ChangePosi, oldGrid, newGrid);
    }

    void OnRuneUIPackageGridDragToInsetGrid(int newGrid, int oldGrid)
    {
        LoggerHelper.Debug(newGrid + " " + oldGrid);
        EventDispatcher.TriggerEvent<int, int>(Events.RuneEvent.PutOn, oldGrid, newGrid);
    }

    void OnRuneUIInsetGridDragToPackageGrid(int newGrid, int oldGrid)
    {
        LoggerHelper.Debug(newGrid + " " + oldGrid);
        EventDispatcher.TriggerEvent<int, int>(Events.RuneEvent.PutDown, oldGrid, newGrid);
    }

    void OnRuneUIInsetGridUp(int id)
    {
        
    }

    void OnRuneUIInsetGridUpDouble(int id)
    {
        EventDispatcher.TriggerEvent<int, int>(Events.RuneEvent.PutDown, id, -1);
    }

    void OnRuneUIPackageGridUp(int id)
    {
        
    }

    void OnRuneUIPackageGridUpDouble(int id)
    {
        EventDispatcher.TriggerEvent<int, int>(Events.RuneEvent.PutOn, id, -1);
    }

    public void Initialize()
    {
        RuneUIViewManager.Instance.GOTODRAGONUP += OnGotoDragonUp;
        RuneUIViewManager.Instance.RUNEUICOMPOSEUP += OnRuneUIComposeUp;
        RuneUIViewManager.Instance.RUNEUICLOSEUP += OnRuneUICloseUp;
        RuneUIViewManager.Instance.RUNEUIINSETGRIDUP += OnRuneUIInsetGridUp;
        RuneUIViewManager.Instance.RUNEUIINSETGRIDUPDOUBLE += OnRuneUIInsetGridUpDouble;
        RuneUIViewManager.Instance.RUNEUIPACKAGEGRIDUP += OnRuneUIPackageGridUp;
        RuneUIViewManager.Instance.RUNEUIPACKAGEGRIDUPDOUBLE += OnRuneUIPackageGridUpDouble;

        EventDispatcher.AddEventListener<int, int>("RuneUIPackageGridDrag", OnRuneUIPackageGirdDrag);
        EventDispatcher.AddEventListener<int, int>("RuneUIInsetGridDrag", OnRuneUIInsetGridDrag);
        EventDispatcher.AddEventListener<int, int>("RuneUIPackageGridDragToInsetGrid", OnRuneUIPackageGridDragToInsetGrid);
        EventDispatcher.AddEventListener<int, int>("RuneUIPInsetGridDragToPackageGrid", OnRuneUIInsetGridDragToPackageGrid);
    }

    public void Release()
    {
        RuneUIViewManager.Instance.GOTODRAGONUP -= OnGotoDragonUp;
        RuneUIViewManager.Instance.RUNEUICOMPOSEUP -= OnRuneUIComposeUp;
        RuneUIViewManager.Instance.RUNEUICLOSEUP -= OnRuneUICloseUp;
        RuneUIViewManager.Instance.RUNEUIINSETGRIDUP -= OnRuneUIInsetGridUp;
        RuneUIViewManager.Instance.RUNEUIINSETGRIDUPDOUBLE -= OnRuneUIInsetGridUpDouble;
        RuneUIViewManager.Instance.RUNEUIPACKAGEGRIDUP -= OnRuneUIPackageGridUp;
        RuneUIViewManager.Instance.RUNEUIPACKAGEGRIDUPDOUBLE -= OnRuneUIPackageGridUpDouble;

        EventDispatcher.RemoveEventListener<int, int>("RuneUIPackageGridDrag", OnRuneUIPackageGirdDrag);
        EventDispatcher.RemoveEventListener<int, int>("RuneUIInsetGridDrag", OnRuneUIInsetGridDrag);
        EventDispatcher.RemoveEventListener<int, int>("RuneUIPackageGridDragToInsetGrid", OnRuneUIPackageGridDragToInsetGrid);
        EventDispatcher.RemoveEventListener<int, int>("RuneUIPInsetGridDragToPackageGrid", OnRuneUIInsetGridDragToPackageGrid);
    }

    public void SetLifePower(int power)
    {
        RuneUIViewManager.Instance.SetRuneUILifePower(power);
    }

    public void UnLockInsetGrid(int gridId)
    {
        RuneUIViewManager.Instance.UnLockInsetGrid(gridId);
    }

    public void SetInsetGridInfo(int level,int type)
    {
        //´ýÐø
       // RuneUIViewManager.Instance.SetInsetGridInfo(level,type);
    }
}
