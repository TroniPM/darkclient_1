using System;
using System.Collections.Generic;
using UnityEngine;
using Mogo.Util;
using Mogo.Game;

namespace Mogo.View
{
    public enum HeadBloodColor
    {
        Red,
        Blue,
    }
	class PlayerHead : Head
	{
        private GameObject m_billboard;
        private GameObject m_goBillboardBlood;
        private GameObject m_goBillboardAnger;
        private UILabel name; // 名字
        private UILabel tong; // 公会
        private UIFilledSprite bar;
        private UISprite bg;
        private UIFilledSprite blood; // 血条
        private MogoBloodAnim m_bloodAnim; // 血条动画
        private UILabel testInfo;
        private UIFilledSprite m_fsInfoBillboardAnger; // 怒气条

        public PlayerHead(uint id,            
            Transform trans ,Action<PlayerHead, uint,Transform,EntityParent> OnFinished,EntityParent self,
            bool showBlood, HeadBloodColor bloodColor = HeadBloodColor.Red,
            bool showAnger = false)
        {           
            AssetCacheMgr.GetUIInstance("InfoBillboard.prefab", (prefab, guid, go) =>
            {
                m_billboard = (GameObject)go;
                m_billboard.name = id.ToString();              

                m_goBillboardBlood = m_billboard.transform.Find("InfoBillboardBlood").gameObject;
                m_goBillboardAnger = m_billboard.transform.Find("InfoBillboardAnger").gameObject;
                ShowBillboardBlood(showBlood);
                SetBillboardBloodColor(bloodColor);
                ShowBillboardAnger(showAnger);

                blood = m_billboard.transform.Find("InfoBillboardBlood/InfoBillboardBloodFG").GetComponentInChildren<UIFilledSprite>();
                name = m_billboard.transform.Find("InfoBillboardName").GetComponentInChildren<UILabel>();
                tong = m_billboard.transform.Find("InfoBillboardTong").GetComponentInChildren<UILabel>();
                testInfo = m_billboard.transform.Find("InfoBillboardTestInfo").GetComponentsInChildren<UILabel>(true)[0];
                bar = m_billboard.transform.Find("InfoBillboardBlood/InfoBillboardBloodFG").GetComponentInChildren<UIFilledSprite>();
                bg = m_billboard.transform.Find("InfoBillboardBlood/InfoBillboardBloodBG0").GetComponentInChildren<UISprite>();
                m_bloodAnim = m_billboard.transform.Find("InfoBillboardBlood").GetComponentsInChildren<MogoBloodAnim>(true)[0];
                m_fsInfoBillboardAnger = m_billboard.transform.Find("InfoBillboardAnger/InfoBillboardAngerFG").GetComponentInChildren<UIFilledSprite>();

                OnFinished(this,id,trans,self);
            });
        }

        #region 逻辑处理

        /// <summary>
        /// 是否显示血条
        /// </summary>
        /// <param name="isShow"></param>
        public void ShowBillboardBlood(bool isShow)
        {
            if (m_goBillboardBlood)
                m_goBillboardBlood.SetActive(isShow);
        }

        /// <summary>
        /// 设置血条
        /// </summary>
        /// <param name="_blood"></param>
        public void SetBillboardBlood(float _blood)
        {
            //Debug.LogError("SetBillboardBlood " + _blood);
            if (blood)
            {
                m_bloodAnim.PlayBloodAnim(_blood);
                blood.fillAmount = _blood;
            }
        }

        /// <summary>
        /// 设置血条颜色
        /// </summary>
        /// <param name="bloodColor"></param>
        public void SetBillboardBloodColor(HeadBloodColor bloodColor = HeadBloodColor.Red)
        {
            switch (bloodColor)
            {
                case HeadBloodColor.Red:
                    if(blood != null)
                        blood.spriteName = "zdjm_shengmingtiao";
                    break;
                case HeadBloodColor.Blue:
                    if(blood != null)
                        blood.spriteName = "zdjm_shengmingtiaolanse";
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 是否显示怒气条
        /// </summary>
        /// <param name="isShow"></param>
        public void ShowBillboardAnger(bool isShow)
        {
            if (m_goBillboardAnger != null)
                m_goBillboardAnger.SetActive(isShow);
        }

        /// <summary>
        /// 设置怒气条
        /// </summary>
        /// <param name="_anger"></param>
        public void SetBillboardAnger(float _anger)
        {
            if (m_fsInfoBillboardAnger)
            {
                m_fsInfoBillboardAnger.fillAmount = _anger;
            }
        }

        #endregion

        public void SetTestInfo(string text)
        {
            if (testInfo)
            {
                testInfo.text = text;
            }
        }

        public void ShowTestInfo(bool isShow)
        {
            if (testInfo)
            {
                testInfo.gameObject.SetActive(isShow);
            }
        }

        /// <summary>
        /// 是否显示头顶面板
        /// </summary>
        /// <param name="isShow"></param>
        public void ShowBillboard(bool isShow)
        {
            if (m_billboard != null)
            {
                m_billboard.SetActive(isShow);
            }
        }

        /// <summary>
        /// 设置名字
        /// </summary>
        /// <param name="_name"></param>
        public void SetName(string _name)
        {
            if (name)
            {
                name.MakePositionPerfect();
                name.text = _name;
            }
        }

        public void SetTong(string _tong)
        {
            if(tong)
            {
                tong.MakePositionPerfect();
                tong.text = _tong;
            }
        }

        public void SetBloodFG(string name)
        {
            if (bar)
            {
                bar.spriteName = name;
            }
        }

        public void SetBloodBG(string name)
        {
            if (bg)
            {
                bg.spriteName = name;
            }
        }

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
