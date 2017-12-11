using System;
using System.Collections.Generic;
using UnityEngine;
using Mogo.Util;

namespace Mogo.View
{
	class MonsterHead : Head
	{
        private GameObject m_billboard;

        public MonsterHead(string text, BILLBOARDTYPE type)
        {
            AssetCacheMgr.GetUIInstance("BattleBillboard.prefab", (prefab, guid, go) =>
            {
                m_billboard = (GameObject)go;
                m_billboard.AddComponent<BattleBillboardAnim>();
            });

            if (m_billboard == null)
            {
                LoggerHelper.Error("No Prefab Can Find!");
                return;
            }

            UILabel _lbl = m_billboard.transform.GetChild(0).GetComponentsInChildren<UILabel>(true)[0];

            switch (type)
            {
                case BILLBOARDTYPE.BiggerRed:
                    _lbl.color = new Color(1, 0, 0, 1);
                    m_billboard.transform.localScale *= 1.2f;
                    break;

                case BILLBOARDTYPE.NormalGreen:
                    _lbl.color = new Color(0, 1, 0, 1);
                    break;
            }

            _lbl.text = text;
        }

        public void AddToParent(Transform parent, Quaternion rotation, Vector3 pos)
        {
            m_billboard.transform.rotation = rotation;
            m_billboard.transform.parent = parent;
            m_billboard.transform.position = pos;
        }

        public override void UpdatePosi(UnityEngine.Vector3 posi)
        {
        }

        public override void Remove()
        {
        }
	}
}
