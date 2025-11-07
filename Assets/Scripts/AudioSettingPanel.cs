using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class AudioSettingsPanel : MonoBehaviour
{
    [Header("Mixer & UI")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private GameObject settingsPanel; // ← パネル（最初は非表示）
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SESlider;
    [SerializeField] private AudioSource testSE;        // テスト再生用 SE
    [SerializeField] private Button openButton;         // パネルを開くボタン
    [SerializeField] private Button closeButton;        // パネルを閉じるボタン

    private void Start()
    {
        // パネルを最初は非表示に
        settingsPanel.SetActive(false);

        // ミキサーの音量をスライダーに反映
        audioMixer.GetFloat("BGM", out float bgmVolume);
        BGMSlider.value = bgmVolume;

        audioMixer.GetFloat("SE", out float seVolume);
        SESlider.value = seVolume;

        // ボタンイベント登録
        openButton.onClick.AddListener(OpenPanel);
        closeButton.onClick.AddListener(ClosePanel);

        // スライダーイベント登録（離したときにテスト音再生）
        EventTrigger trigger = SESlider.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerUp
        };
        entry.callback.AddListener((data) => OnSESliderReleased());
        trigger.triggers.Add(entry);
    }

    public void SetBGM(float volume)
    {
        audioMixer.SetFloat("BGM", volume);
    }

    public void SetSE(float volume)
    {
        audioMixer.SetFloat("SE", volume);
    }

    private void OnSESliderReleased()
    {
        if (testSE != null && testSE.clip != null)
        {
            testSE.PlayOneShot(testSE.clip);
        }
    }

    private void OpenPanel()
    {
        settingsPanel.SetActive(true);
    }

    private void ClosePanel()
    {
        settingsPanel.SetActive(false);
    }
}