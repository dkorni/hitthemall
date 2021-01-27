using System;
using System.Runtime.InteropServices;
using DG.Tweening;
using Enemy;
using GameAnalyticsSDK;
using Leveling;
using Player;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game
{
    public class Game : MonoBehaviour, IInitializable, IDisposable
    {
        public static Game Instance;

        private LevelContainer m_levelContainer;
        private Level m_level;
        private PlayerController m_player;
        private EnemyContainer m_enemyContainer;
        private LockerController m_lockerController;

        [HideInInspector] public ReactiveProperty<GameState> State = new ReactiveProperty<GameState>(GameState.Lobby);
        [HideInInspector] public readonly CompositeDisposable Disposable = new CompositeDisposable();
        [HideInInspector] public readonly ReactiveProperty<int> CoinsCount = new ReactiveProperty<int>();

        [Inject] private TreasureBoxController _boxController;

        [Inject]
        private void Construct(LevelContainer levelContainer, PlayerController playerController,
            LockerController lockerController)
        {
            m_levelContainer = levelContainer;
            m_player = playerController;
            m_lockerController = lockerController;

            Instance = this;
        }

        public void Initialize()
        {
            GameAnalytics.Initialize();

            DOTween.SetTweensCapacity(600, 200);
            State.Subscribe(OnStateChanged).AddTo(Disposable);
            m_levelContainer.EnemyContainer.IsAllEnemiesDestroyed.Subscribe(OnAllEnemiesDestroyedChange)
                .AddTo(Disposable);
            m_lockerController.IsLockerSafe.Subscribe(OnLockerSafetyChanged);
            _boxController.OnFinished += () => State.Value = GameState.FailMenu;
            State.Value = GameState.Lobby;

            //load coins
            CoinsCount.Value = PlayerPrefs.GetInt("Coins", 995);
        }

        public void AddMoney(int amount)
        {
            CoinsCount.Value += amount;
            PlayerPrefs.SetInt("Coins", CoinsCount.Value);
            PlayerPrefs.Save();
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "1", 1, "Coins", "1");
        }

        private void OnStateChanged(GameState state)
        {
            Debug.Log(state);

            switch (state)
            {
                case GameState.Lobby:
                    DOVirtual.DelayedCall(0.3f,
                        () => State.Value = GameState.RoundPrepare);

                    // disable physics btw coins, slingshot and stickmen
                    Physics.IgnoreLayerCollision(9,10);
                    Physics.IgnoreLayerCollision(10,11);

                    break;
                case GameState.RoundPrepare:
                    m_level = m_levelContainer.CurrentLevel;
                    m_enemyContainer = m_level.EnemyContainer;

                    // m_player.ToggleInput(false);
                    m_enemyContainer.DeactivateAll();
                    m_lockerController.IsLockerSafe.Value = true;

                    break;
                case GameState.Round:
                    GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "Game",
                        m_levelContainer.CurrentLevel.name);
                    m_enemyContainer.ActivateAll();
                    m_player.ToggleInput(true);
                    break;
                case GameState.Win:
                    GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Game",
                        m_levelContainer.CurrentLevel.name);
                    m_enemyContainer.DeactivateAll();
                    m_player.ToggleInput(false);
                    break;
                case GameState.Fail:
                    GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "Game",
                        m_levelContainer.CurrentLevel.name);
                    m_enemyContainer.DeactivateAll();
                    break;
            }
        }

        private void OnAllEnemiesDestroyedChange(bool isDestroyed)
        {
            if (isDestroyed)
                State.Value = GameState.Win;
        }

        private void OnLockerSafetyChanged(bool safe)
        {
            if (!safe && State.Value == GameState.Round)
                State.Value = GameState.Fail;
        }

        public void Reload(bool next)
        {
            if (next)
                m_levelContainer.NextLevel();
            SceneManager.LoadScene("SampleScene");
        }

        public void Dispose()
        {
            Disposable.Dispose();
            this.DOKill();
        }
    }

    public enum GameState
    {
        Lobby,
        RoundPrepare,
        Round,
        Win,
        Fail,
        FailMenu
    }
}