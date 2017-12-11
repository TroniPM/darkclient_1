#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：FXData
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.3.26
// 模块描述：特效配置实体。
//----------------------------------------------------------------*/
#endregion
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Mogo.GameData
{
    public class FXData : GameData<FXData>
    {
        public string player { get; set; }
        /// <summary>
        /// 是否一直保留。0不保留，1保留
        /// </summary>
        public FXStatic isStatic { get; set; }
        /// <summary>
        /// 是否与其他特效冲突。0不冲突，1冲突
        /// </summary>
        public FXConflict isConflict { get; set; }
        /// <summary>
        /// 0为普通，1为飞行物
        /// </summary>
        public EffectType effectType { get; set; }
        public FXLocationType locationType { get; set; }
        public int level { get; set; }
        public int group { get; set; }
        public Vector3 location { get; set; }
        public String slot { get; set; }
        public String resourcePath { get; set; }
        public float duration { get; set; }
        public String weaponTailSlot { get; set; }
        public String weaponTailMaterial { get; set; }
        public int weaponTailEmitTime { get; set; }
        public float weaponTailLeftTime { get; set; }
        public int weaponTailDurationTime { get; set; }
        public int weaponTailSubdivisions { get; set; }
        public String anim { get; set; }
        public int soundDelay { get; set; }
        public int fadeDelay { get; set; }
        public int fadeDulation { get; set; }
        public float fadeStart { get; set; }
        public float fadeEnd { get; set; }
        public String shader { get; set; }
        public List<float> rimWidth { get; set; }
        public List<float> rimPower { get; set; }
        public List<float> finalPower { get; set; }
        public List<float> r { get; set; }
        public List<float> g { get; set; }
        public List<float> b { get; set; }
        public List<float> a { get; set; }
        public List<int> shaderDuration { get; set; }

        public String dissonShader { get; set; }
        public String nosieTexture { get; set; }
        public float nosieOffetFrom { get; set; }
        public float nosieOffetTo { get; set; }
        public Color dissonColor { get; set; }
        public int dissonDuration { get; set; }
        public int dissonDelay { get; set; }

        public static readonly string fileName = "xml/FXData";
        //public static Dictionary<int, FXData> dataMap { get; set; }
    }

    public enum EffectType : byte
    {
        Normal = 0,
        Flying = 1,
    }

    public enum FXConflict : byte
    {
        NotConflict = 0,
        Conflict = 1,
    }

    public enum FXStatic : byte
    {
        NotStatic = 0,
        Static = 1
    }

    public enum FXLocationType : byte
    {
        World = 0,
        SelfLocal = 1,
        SelfSlot = 2,
        SelfWorld = 3,
        SlotWorld = 4
    }
}