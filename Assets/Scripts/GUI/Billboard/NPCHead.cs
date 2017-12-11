using System;
using System.Collections.Generic;
using UnityEngine;
using Mogo.Util;

namespace Mogo.View
{
	class NPCHead : Head
	{
        private GameObject m_billboard;

        public NPCHead(uint id)
        {
            AssetCacheMgr.GetUIInstance("TaskBillboard.prefab", (prefab, guid, go) =>
            {
                m_billboard = (GameObject)go;
                m_billboard.name = id.ToString();
            });

            if (m_billboard == null)
            {
                LoggerHelper.Error("No Prefab Can Find!");
                return;
            }

            //for (int i = 0; i < 3; ++i)
            //{
            //    m_ssIcon[i] = _goBillboard.transform.GetChild(i).GetComponentInChildren<UISlicedSprite>();
            //}

            
        }

        public override void ShowTaskIcon(uint idx)
        {
            //根据索引显示
        }

        //public void ShowTaskIcon0(uint playerId)
        //{
        //    m_dictTaskBillboard[playerId][0].gameObject.SetActive(true);
        //    m_dictTaskBillboard[playerId][1].gameObject.SetActive(false);
        //    m_dictTaskBillboard[playerId][2].gameObject.SetActive(false);
        //}

        //public void ShowTaskIcon1(uint playerId)
        //{
        //    m_dictTaskBillboard[playerId][0].gameObject.SetActive(false);
        //    m_dictTaskBillboard[playerId][1].gameObject.SetActive(true);
        //    m_dictTaskBillboard[playerId][2].gameObject.SetActive(false);

        //}

        //public void ShowTaskIcon2(uint playerId)
        //{
        //    m_dictTaskBillboard[playerId][0].gameObject.SetActive(false);
        //    m_dictTaskBillboard[playerId][1].gameObject.SetActive(false);
        //    m_dictTaskBillboard[playerId][2].gameObject.SetActive(true);
        //}

        public override void UpdatePosi(Vector3 posi)
        {
            if (m_billboard)
            {
                m_billboard.transform.position = posi;
            }
        }

        public void AddToParent(Transform parent, Quaternion rotation)
        {
            if (m_billboard)
            {
                m_billboard.transform.rotation = rotation;
                m_billboard.transform.parent = parent;
            }
        }

        public override void Remove()
        {
            if (m_billboard == null)
            {
                return;
            }
            m_billboard.transform.parent = null;
            GameObject.Destroy(m_billboard);
        }
	}
}
