using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OneDevApp
{
    public enum SceneLoaderType
    {
        Instant = 0,
        Button = 1,
        AnyKey = 2,
    }

    public class SceneLoader : MonoInstance<SceneLoader>
    {
        [Header("Settings")]
        public SceneLoaderType sceneLoaderType = SceneLoaderType.Instant;
        [Range(0.5f, 7)] public float SceneSmoothLoad = 3;
        [Range(0.5f, 7)] public float FadeInSpeed = 2;
        [Range(0.5f, 7)] public float FadeOutSpeed = 2;
        public bool useTimeScale = false;

        [Header("Background")]
        public bool useBackgrounds = true;
        [Range(1, 60)] public float TimePerBackground = 5;
        [Range(0.5f, 7)] public float FadeBackgroundSpeed = 2;
        [Range(0.5f, 5)] public float TimeBetweenBackground = 0.5f;
        public List<Sprite> BackgroundsList = new List<Sprite>();

        [Header("Tips")]
        public bool RandomTips = false;
        [Range(1, 60)] public float TimePerTip = 5;
        [Range(0.5f, 5)] public float FadeTipsSpeed = 2;
        [Range(0.5f, 5)] public float TimeBetweenTips = 0.5f;
        public List<string> TipsList = new List<string>();

        [Header("Loading")]
        public bool FadeLoadingBarOnFinish = false;
        [Range(50, 1000)] public float LoadingCircleSpeed = 300;
        private string LoadingTextFormat = "{0}";

        [Header("References")]
        [SerializeField] private GameObject RootUI;
        [SerializeField] private GameObject FlashImage = null;
        [SerializeField] private Image BackgroundImage = null;
        [SerializeField] private Image TipsTextImage = null;
        [SerializeField] private Text TipText = null;
        [SerializeField] private GameObject ContinueAnyKeyUI = null;
        [SerializeField] private GameObject ContinueButtonUI = null;
        [SerializeField] private GameObject ProgressUI = null;
        [SerializeField] private Text ProgressText = null;
        [SerializeField] private Slider LoadBarSlider = null;
        [SerializeField] private RectTransform LoadingCircle = null;
        [SerializeField] private CanvasGroup LoadingCircleCanvas = null;

        /// <summary>
        /// On scene loaded
        /// </summary>
        [SerializeField]
        public UnityEvent onSceneLoaded = default;

        private AsyncOperation async = null;
        private bool isOperationStarted = false;
        private bool FinishLoad = false;
        private CanvasGroup RootAlpha = null;
        private CanvasGroup BackgroundAlpha = null;
        private CanvasGroup TipsTextAlpha = null;
        private CanvasGroup LoadingBarAlpha = null;
        private bool isTipFadeOut = false;
        private int CurrentTip = 0;
        private int CurrentBackground = 0;
        private float lerpValue = 0;
        private bool canSkipWithKey = false;
        private SceneLoaderType cacheLoaderType = SceneLoaderType.Instant;

        /// <summary>
        /// 
        /// </summary>
        public void LoadScene(int SceneIndex)
        {
            LoadScene(SceneIndex, sceneLoaderType);
        }

        public void LoadScene(int SceneIndex, SceneLoaderType _sceneLoaderType)
        {
            cacheLoaderType = _sceneLoaderType;
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadScene(string SceneName)
        {
            LoadScene(SceneName, sceneLoaderType);
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadScene(string SceneName, SceneLoaderType _sceneLoaderType)
        {
            cacheLoaderType = _sceneLoaderType;
            SetupUI();
            RootUI.SetActive(true);
            StartCoroutine(StartAsyncOperation(SceneName));
        }
        
        /// <summary>
        /// 
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            //Setup UI
            RootUI.SetActive(false);
            RootAlpha = RootUI.GetComponent<CanvasGroup>();

            if (BackgroundImage != null) { BackgroundAlpha = BackgroundImage.GetComponent<CanvasGroup>(); }

            if (TipsTextAlpha != null) { TipsTextAlpha = TipsTextImage.GetComponent<CanvasGroup>(); }

            if (LoadBarSlider != null) { LoadingBarAlpha = LoadBarSlider.GetComponent<CanvasGroup>(); }

            //SetupUI();
        }

        /// <summary>
        /// 
        /// </summary>
        void Update()
        {
            if (!isOperationStarted)
                return;
            if (async == null)
                return;

            UpdateUI();
            if(LoadingCircle.gameObject.activeInHierarchy)
                LoadingRotator();
            SkipWithKey();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        void SetupUI()
        {
            //Show all UI
            async = null;
            isOperationStarted = false;
            FinishLoad = false;
            RootAlpha.alpha = 0;
            CurrentTip = 0;
            CurrentBackground = 0;
            lerpValue = 0;
            canSkipWithKey = false;

            if (BackgroundAlpha != null) BackgroundAlpha.alpha = 0;
            if (LoadingBarAlpha != null) LoadingBarAlpha.alpha = 1;
            if (TipsTextAlpha != null) TipsTextAlpha.alpha = 0;
            if (LoadingCircleCanvas != null) LoadingCircleCanvas.alpha = 1;

            ProgressUI.gameObject.SetActive(true);

            if (FlashImage != null) { FlashImage.SetActive(false); }

            if (ContinueAnyKeyUI != null)
            {
                ContinueAnyKeyUI.SetActive(false);
            }

            if (ContinueButtonUI != null)
            {
                ContinueButtonUI.SetActive(false);
                ContinueButtonUI.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                ContinueButtonUI.GetComponentInChildren<Button>().onClick.AddListener(() => LoadNextScene());
            }

            if (BackgroundImage != null && useBackgrounds)
            {
                if (BackgroundsList.Count > 1)
                {
                    StartCoroutine(BackgroundTransition());
                    BackgroundImage.color = Color.white;
                }
                else if (BackgroundsList != null && BackgroundsList.Count > 0)
                {
                    BackgroundImage.sprite = BackgroundsList[0];
                    BackgroundImage.color = Color.white;
                }
            }

            if (LoadBarSlider != null) { LoadBarSlider.value = 0; }

            if (ProgressText != null)
            {
                ProgressText.text = string.Format(LoadingTextFormat, 0);
            }

            if (TipText != null && TipsList.Count > 1)
            {
                if (RandomTips)
                {
                    CurrentTip = Random.Range(0, TipsList.Count);
                    TipText.text = TipsList[CurrentTip];
                }
                else
                {
                    TipText.text = TipsList[0];
                }
                StartCoroutine(TipsLoop());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void SkipWithKey()
        {
            if (!canSkipWithKey)
                return;

            if (Input.anyKeyDown)
            {
                LoadNextScene();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        void UpdateUI()
        {
            //Get progress of load level
            float Extra = 0.1f;
            float p = (async.progress + Extra); //Fix problem of 90%
            lerpValue = Mathf.Lerp(lerpValue, p, DeltaTime * SceneSmoothLoad);
            if (async.isDone || lerpValue > 0.99f)
            {
                //Called one time what is inside in this function.
                if (!FinishLoad)
                {
                    OnFinish();
                }
            }

            if (LoadBarSlider != null) { LoadBarSlider.value = lerpValue; }

            if (ProgressText != null)
            {
                float lerpValuePer = Mathf.Clamp((lerpValue * 100), 0, 100);
                string percent = lerpValuePer.ToString("F0");
                ProgressText.text = string.Format(LoadingTextFormat, percent);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void OnFinish()
        {
            FinishLoad = true;
            if (FlashImage != null) { FlashImage.SetActive(true); }

            //Can skip when next level is loaded.
            switch (sceneLoaderType)
            {
                case SceneLoaderType.Button:
                    if (ContinueButtonUI != null)
                    {
                        ContinueButtonUI.SetActive(true);
                        ProgressUI.SetActive(false);
                    }
                    break;
                case SceneLoaderType.AnyKey:
                    canSkipWithKey = true;
                    if (ContinueAnyKeyUI != null)
                    {
                        ContinueAnyKeyUI.SetActive(true);
                        ProgressUI.SetActive(false);
                    }
                    break;
                case SceneLoaderType.Instant:
                default:
                    LoadNextScene();
                    break;
            }

            if (FadeLoadingBarOnFinish)
            {
                if (LoadingCircleCanvas != null) { StartCoroutine(FadeOutCanvas(LoadingCircleCanvas, 0.5f)); }
                if (LoadingBarAlpha != null) { StartCoroutine(FadeOutCanvas(LoadingBarAlpha, 1)); }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator StartAsyncOperation(string level)
        {
            yield return FadeInCanvas(RootAlpha);

            async = SceneManager.LoadSceneAsync(level);
            
            async.allowSceneActivation = false;

            isOperationStarted = true;
            yield return async;
        }


        /// <summary>
        /// 
        /// </summary>
        private void LoadNextScene()
        {
            StartCoroutine(LoadNextSceneIE());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadNextSceneIE()
        {
            if (onSceneLoaded != null)
            {
                onSceneLoaded.Invoke();
            }
            
            async.allowSceneActivation = true;

            yield return FadeOutCanvas(RootAlpha);

            RootUI.SetActive(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator BackgroundTransition()
        {
            while (true)
            {
                BackgroundImage.sprite = BackgroundsList[CurrentBackground];
                while (BackgroundAlpha.alpha < 1)
                {
                    BackgroundAlpha.alpha += DeltaTime * FadeBackgroundSpeed * 0.8f;
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForSeconds(TimePerBackground);
                while (BackgroundAlpha.alpha > 0)
                {
                    BackgroundAlpha.alpha -= DeltaTime * FadeBackgroundSpeed;
                    yield return new WaitForEndOfFrame();
                }
                CurrentBackground = (CurrentBackground + 1) % BackgroundsList.Count;
                yield return new WaitForSeconds(TimeBetweenBackground);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator TipsLoop()
        {
            while (true)
            {
                TipText.text = TipsList[CurrentTip];

                while (TipsTextAlpha.alpha < 1)
                {
                    TipsTextAlpha.alpha += DeltaTime * FadeTipsSpeed * 0.8f;
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForSeconds(TimePerTip);
                while (TipsTextAlpha.alpha > 0)
                {
                    TipsTextAlpha.alpha -= DeltaTime * FadeTipsSpeed;
                    yield return new WaitForEndOfFrame();
                }


                if (RandomTips)
                {
                    int lastTip = CurrentTip;
                    CurrentTip = Random.Range(0, TipsList.Count);
                    while (CurrentTip == lastTip)
                    {
                        CurrentTip = Random.Range(0, TipsList.Count);
                        yield return null;
                    }
                }
                else
                {
                    CurrentTip = (CurrentTip + 1) % TipsList.Count;
                }

                yield return new WaitForSeconds(TimeBetweenTips);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void LoadingRotator()
        {
            if (LoadingCircle == null)
                return;

            LoadingCircle.Rotate(-Vector3.forward * DeltaTime * LoadingCircleSpeed);
        }

        private float DeltaTime
        {
            get { return (useTimeScale) ? Time.deltaTime : Time.unscaledDeltaTime; }
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator FadeOutCanvas(CanvasGroup alpha, float delay = 0)
        {
            yield return new WaitForSeconds(delay);
            while (alpha.alpha > 0)
            {
                alpha.alpha -= DeltaTime * FadeOutSpeed;
                yield return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator FadeInCanvas(CanvasGroup alpha, float delay = 0)
        {
            yield return new WaitForSeconds(delay);
            while (alpha.alpha < 1)
            {
                alpha.alpha += DeltaTime * FadeInSpeed;
                yield return null;
            }
        }
    }

}