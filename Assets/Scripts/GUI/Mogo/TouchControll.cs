using UnityEngine;
using System.Collections;
using Mogo.Game;

public class TouchControll : MonoBehaviour 
{
    EntityParent m_epObj;
    Vector3 vec;

    void Start()
    {
        m_epObj = MogoWorld.thePlayer;
    }

    void Move()
    {
        Vector3 cursorPos = Input.mousePosition;
        Ray cursorRay = Camera.main.ScreenPointToRay(cursorPos);
        RaycastHit hit;
        if(Physics.Raycast(cursorRay, out hit,10000,1<<9))
        {
            m_epObj.MoveTo(hit.point.x,hit.point.z);
            vec = hit.point;

            //LoggerHelper.Debug(vec);
           
            if (hit.collider.gameObject.tag == "Terrain")
            {
                //MoveTo(hit.point);
                
            }
        }

    }

    void OnPress(bool isOver)
    {
        if (!isOver)
        {
            Move();
        }
    }
}
