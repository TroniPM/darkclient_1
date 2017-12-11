using UnityEngine;
using System.Collections;

public class EquipModelImge : MonoBehaviour 
{
    Transform m_camEquip;

    void Awake()
    {
        m_camEquip = transform.parent.Find("EquipCamera");
    }
    void OnEnable()
    {
        if (MogoWorld.thePlayer.vocation == Mogo.Game.Vocation.Assassin)
        {
            //MenuUIViewManager.Instance.LoadPlayerModel(2.6f,0.12f,0.12f,0,0,0.12f);
            MenuUIViewManager.Instance.LoadPlayerModel(2.43f, 0.49f, 0.42f, 0, 0.08f, 0f);
        }
        else if (MogoWorld.thePlayer.vocation == Mogo.Game.Vocation.Archer)
        {
            MenuUIViewManager.Instance.LoadPlayerModel(2.75f,0,0.1f,0);
        }
        else if (MogoWorld.thePlayer.vocation == Mogo.Game.Vocation.Mage)
        {
            MenuUIViewManager.Instance.LoadPlayerModel(2.6f,0, 0.15f, 0);
        }
        else
        {
            MenuUIViewManager.Instance.LoadPlayerModel(2.82f, 0.53f, 0.59f, 0, 0.02f, 0.02f);
        }
    }

    void OnDisable()
    {
        MenuUIViewManager.Instance.DisablePlayerModel();
    }

    void OnDrag(Vector2 vec)
    {
        //m_camEquip.RotateAround(MogoWorld.thePlayer.Transform.FindChild("slot_camera").position,Vector3.up,0);

        MogoWorld.thePlayer.GameObject.transform.Rotate(new Vector3(0,-vec.x,0));
    }
}
