using UnityEngine;

using UnityEngine.UI;

using UnityEngine.Audio;

using UnityEngine.EventSystems;

public class Audio : MonoBehaviour

{

    [SerializeField] private AudioMixer audioMixer;

    [Header("Sliders")]

    [SerializeField] private Slider BGMSlider;

    [SerializeField] private Slider SESlider;

    [Header("Test Sound for SE")]

    [SerializeField] private AudioSource testSE; // ← テスト再生用SE AudioSource

    private void Start()

    {

        // ミキサーの音量をスライダーに反映

        audioMixer.GetFloat("BGM", out float bgmVolume);

        BGMSlider.value = bgmVolume;

        audioMixer.GetFloat("SE", out float seVolume);

        SESlider.value = seVolume;

        // SEスライダーを離したときのイベントを追加

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

        // BGMの音量だけ変更、再生制御はしない

        audioMixer.SetFloat("BGM", volume);

    }

    public void SetSE(float volume)

    {

        // SEの音量だけ変更

        audioMixer.SetFloat("SE", volume);

    }

    private void OnSESliderReleased()

    {

        // スライダーを離したときにテストSEだけを鳴らす

        if (testSE != null && testSE.clip != null)

        {

            testSE.PlayOneShot(testSE.clip);

        }

        else

        {

            Debug.LogWarning("TestSE AudioSource または clip が設定されていません。");

        }

    }

}
