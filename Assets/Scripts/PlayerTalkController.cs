using Unity.VisualScripting;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using TMPro;
public class PlayerTalkController : MonoBehaviour
{
    [SerializeField]
    GameObject TalkWindow;
    [SerializeField]
    private TextMeshProMode TextmeshProMode;
    private TextMeshPro _textMeshPro;
    private TextMeshProUGUI textMeshProUGUI;
    private TMP_Text TMP_Text;

    bool isTalk = false;
    
    public enum TextMeshProMode{TextMeshPro,TextMeshProUGUI,TMP_Text}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TalkWindow.SetActive(false);

    }
    public void OnTalk(InputValue value)
    {
        if (isTalk)
        {
            TalkWindow.SetActive(true);
            textMeshProUGUI=GetComponent<TextMeshProUGUI>();
            textMeshProUGUI.SetText("aaaaaaaaaa");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag == "TalkNPC")
        {
            isTalk = true;
             Debug.Log("会話可能です");
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.tag =="TalkNPC")
        {
            Debug.Log("会話可能です");
        }
        
    }
    // Update is called once per frame
    void Update()
    {
      
    }
}
