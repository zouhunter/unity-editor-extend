using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LoadForceDataObject.asset", menuName = "生成/加载数据")]
public class LoadForceDataObject : ScriptableObject {
    public List<DataList> dataList;
    public List<string> descList;
    [Multiline(5)]
    public string progrem;
}
