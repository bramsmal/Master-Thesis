using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Zappar;

public class VideoPlayerControl : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Button startButton;
    public Button audioButton;
    public Button repeatButton;
    public Button skipButton;
    public Button backButton;
    public float skipAmount = 5f;
    public Sprite playSprite;
    public Sprite pauseSprite;
    public Sprite muteSprite;
    public Sprite unmuteSprite;
    private bool isPlaying = false;
    private bool isMuted = false;
    private ZapparImageTrackingTarget imageTrackingTarget;
    private MeshRenderer meshRenderer;
    private Canvas canvas;
    public MeshRenderer quad;
    public MeshRenderer previewObj;
    private bool firstPlayed = false;

    void Start()
    {
        imageTrackingTarget = GetComponent<ZapparImageTrackingTarget>();
        videoPlayer = GetComponentInChildren<VideoPlayer>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        canvas = GetComponentInChildren<Canvas>();

        if (imageTrackingTarget)
        {
            imageTrackingTarget.OnSeenEvent.AddListener(OnImageTargetFound);
            imageTrackingTarget.OnNotSeenEvent.AddListener(OnImageTargetLost);
        }

        if (videoPlayer)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.Pause();
        }

        startButton.onClick.AddListener(StartVideo);
        audioButton.onClick.AddListener(AudioVideo);
        repeatButton.onClick.AddListener(RepeatVideo);
        skipButton.onClick.AddListener(SkipForward);
        backButton.onClick.AddListener(SkipBackward);

        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        vp.Stop();
        vp.Play();
        vp.Pause();
        isPlaying = false;
        firstPlayed = false;
        previewObj.enabled = true;
        quad.enabled = false;
        startButton.GetComponent<Image>().sprite = playSprite;
    }

    private void OnImageTargetFound()
    {
        if (meshRenderer != null)
            meshRenderer.enabled = true;
        if (canvas)
        {
            canvas.enabled = true;
        }
        if (!firstPlayed)
        {
            quad.enabled = false;
            previewObj.enabled = true;
        }
    }

    private void OnImageTargetLost()
    {
        videoPlayer.Pause();
        if (meshRenderer != null)
            meshRenderer.enabled = false;
        if (canvas)
        {
            canvas.enabled = false;
        }
        isPlaying = false;
        startButton.GetComponent<Image>().sprite = playSprite;

        if (!firstPlayed)
        {
            previewObj.enabled = false;
        }
    }

    public void StartVideo()
    {
        if (!isPlaying)
        {
            if (!videoPlayer.isPlaying)
            {
                startButton.GetComponent<Image>().sprite = pauseSprite;
                videoPlayer.Play();
                isPlaying = true;
            }
        }
        else
        {
            if (videoPlayer.isPlaying)
            {
                startButton.GetComponent<Image>().sprite = playSprite;
                videoPlayer.Pause();
                isPlaying = false;
            }
        }

        if (!firstPlayed)
        {
            if (previewObj != null)
            {
                previewObj.enabled = false;
                quad.enabled = true;
                firstPlayed = true;
            }
        }
    }

    void AudioVideo()
    {
        isMuted = !isMuted;
        videoPlayer.SetDirectAudioMute(0, isMuted);
        audioButton.GetComponent<Image>().sprite = isMuted ? muteSprite : unmuteSprite;
    }

    void RepeatVideo()
    {
        if (!videoPlayer.isPlaying)
        {
            startButton.GetComponent<Image>().sprite = pauseSprite;
            isPlaying = true;
        }
        videoPlayer.Stop();
        videoPlayer.Play();
    }

    void SkipForward()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.time = Mathf.Min((float)(videoPlayer.time + skipAmount), (float)videoPlayer.length);
        }
    }

    void SkipBackward()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.time = Mathf.Max(0, (float)(videoPlayer.time - skipAmount));
        }
    }

    void OnDestroy()
    {
        if (imageTrackingTarget != null)
        {
            imageTrackingTarget.OnSeenEvent.RemoveListener(OnImageTargetFound);
            imageTrackingTarget.OnNotSeenEvent.RemoveListener(OnImageTargetLost);
        }
    }
}
