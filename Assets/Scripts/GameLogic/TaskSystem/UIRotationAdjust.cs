/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：UIRotationAdjust
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：
// 模块描述：用于UI正对镜头
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;

public class UIRotationAdjust : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.eulerAngles = Camera.main.transform.eulerAngles;
        //transform.LookAt(Camera.main.transform);
        //transform.Rotate(new Vector3(0,180,0));
	}
}
