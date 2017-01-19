using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
[RequireComponent(typeof(BoxCollider))]
public class GizmosTest : MonoBehaviour {
    public BoxCollider m_BoxCollider;
    // OnDrawGizmos()会在编辑器的Scene视图刷新的时候被调用
    // 我们可以在这里绘制一些用于Debug的数据 
    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube( transform.position + m_BoxCollider.center, m_BoxCollider.size );
        Gizmos.color = new Color( 1f, 0f, 0f, 0.3f );
        Gizmos.DrawCube( transform.position + m_BoxCollider.center, m_BoxCollider.size );
    }
    // OnDrawGizmosSelect()类似于OnDrawGizmos()，它会在当该组件所属的物体被选中时被调用 
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube( transform.position + m_BoxCollider.center, m_BoxCollider.size );
        Gizmos.color = new Color( 1f, 1f, 0f, 0.3f );
        Gizmos.DrawCube( transform.position + m_BoxCollider.center, m_BoxCollider.size );
    }
}
