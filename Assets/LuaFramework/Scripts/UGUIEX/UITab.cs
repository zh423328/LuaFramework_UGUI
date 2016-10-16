using UnityEngine;
using UnityEngine.UI;
///
/// 按钮父节点命名为list
/// 焦点图片命名为focus
/// 可自动查找
///
public class UITab : MonoBehaviour
{
    public Transform m_btnParent;
    public RectTransform m_focus;

    public Color m_focusTextColor = Color.yellow;

    public delegate void OnClickEvent(GameObject go);
    private OnClickEvent m_onClickBtn;

    private bool m_init = false;
    void Start()
    {
        if(m_init) return;

        m_init = true;

        if(m_btnParent == null)
            m_btnParent = transform.FindChild("list");

        if(m_focus == null)
            m_focus = transform.FindChild("focus").GetComponent<RectTransform>();

        for(int i = 0; i < m_btnParent.childCount; i++)
        {
            GameObject go = m_btnParent.FindChild(i.ToString()).gameObject;
            Button btn = go.GetComponent<Button>();

            if(btn != null)
            {
                btn.onClick.AddListener(
                    delegate()
                {
                    OnClickBtn(go);
                });
                UIButton.Get(go);
            }
        }

        SetIndex(1);
    }

    // 逻辑注册
    public void RegisterClickEvent(OnClickEvent click)
    {
        m_onClickBtn = click;
    }

    public void SetIndex(int index)
    {
        if(index >= m_btnParent.childCount)
        {
            return;
        }

        GameObject obj = m_btnParent.FindChild(index.ToString()).gameObject;
        OnClickBtn(obj);
    }

    private void OnClickBtn(GameObject go)
    {
        if(m_onClickBtn != null)
            m_onClickBtn(go);

        if(m_focus != null)
        {
            m_focus.SetParent(go.transform);
            m_focus.localPosition = Vector3.zero;
            m_focus.SetAsFirstSibling();
            UIButton.Get(go).SetTextColor(m_focusTextColor);
        }

        for(int i = 0; i < m_btnParent.childCount; i++)
        {
            GameObject obj = m_btnParent.FindChild(i.ToString()).gameObject;

            if(obj != go)
                UIButton.Get(obj).SetTextRawColor();
        }
    }

    public static UITab Get(GameObject go)
    {
        UITab load = go.GetComponent<UITab>();

        if(load == null) load = go.AddComponent<UITab>();

        if(!load.m_init) load.Start();

        return load;
    }
}