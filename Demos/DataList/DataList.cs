using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

[Serializable]
public class DataList
{
    public string name;
    public DataItem key;
    public List<DataItem> values;

    public float[] GetDataByIndex(int index)
    {
        var datas = new float[values.Count];
        for (int i = 0; i < values.Count; ++i)
        {
            datas[i] = values[i].values[index];
        }
        return datas;
    }

    public float[] GetDataByKey(float keyValue)
    {
        var index = key.values.IndexOf(keyValue);
        return GetDataByIndex(index);
    }
}

[Serializable]
public class DataItem
{
    public string name;
    public List<float> values;
    public string unitName;

    #region graph
    public bool graph = true;
    public string graphName;
    public float axisMin;
    public float axisMax;
    public float axisSize;
    #endregion
}