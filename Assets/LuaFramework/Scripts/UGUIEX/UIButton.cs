//按钮扩展增加shader等
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerEnterHandler,
    IPointerExitHandler

{
    public Image m_image;
    public Text m_text;

    private GameObject m_obj;
    private Color m_rawColor;
    private bool m_gray = false;// 默认可用，变灰不可以再变色
    private bool m_init = false;

    private enum eBtnEffectTpye
    {
        None,
        Light,
        Dark,
        Gray,
    }

    public void Start()
    {
        m_init = true;
        m_obj = gameObject;

        if(null == m_image)
            m_image = GetComponent<Image>();

        if(null == m_text)
            m_text = GetComponentInChildren<Text>();

        if(null != m_text)
            m_rawColor = m_text.color;
    }

    public void SetGray(bool bGray)
    {
        m_gray = bGray;

        if(bGray)
            SetShader(eBtnEffectTpye.Gray);
        else
            SetShader(eBtnEffectTpye.None);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetShader(eBtnEffectTpye.Dark);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        SetShader(eBtnEffectTpye.None);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetShader(eBtnEffectTpye.Light);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        SetShader(eBtnEffectTpye.None);
    }

    public void SetText(string text)
    {
        m_text.text = text;
    }

    public void SetTextColor(Color col)
    {
        m_text.color = col;
    }

    public void SetTextRawColor()
    {
        m_text.color = m_rawColor;
    }

    private void SetShader(eBtnEffectTpye type)
    {
        if(m_image.material == null)
            return;

        if(m_gray && type != eBtnEffectTpye.Gray)
            return;

        if(m_image.material.shader == Shader.Find("UI/Default"))
            m_image.material = new Material(Shader.Find("UI/Button"));

        m_image.material.SetInt("_type", (int)type);
    }

    public static UIButton Get(GameObject go)
    {
        UIButton load = go.GetComponent<UIButton>();

        if(load == null) load = go.AddComponent<UIButton>();

        if(!load.m_init) load.Start();

        return load;
    }
}