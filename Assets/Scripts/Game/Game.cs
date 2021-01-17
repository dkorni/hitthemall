using System;
using System.Runtime.InteropServices;
using DG.Tweening;
using Enemy;
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
            DOTween.SetTweensCapacity(600, 200);
            State.Subscribe(OnStateChanged).AddTo(Disposable);
            m_levelContainer.EnemyContainer.IsAllEnemiesDestroyed.Subscribe(OnAllEnemiesDestroyedChange)
                .AddTo(Disposable);
            m_lockerController.IsLockerSafe.Subscribe(OnLockerSafetyChanged);

            State.Value = GameState.Lobby;
        }

        private void OnStateChanged(GameState state)
        {
            Debug.Log(state);

            switch (state)
            {
                case GameState.Lobby:
                    DOVirtual.DelayedCall(0.3f,
                        () => State.Value = GameState.RoundPrepare);
                    break;
                case GameState.RoundPrepare:
                    m_level = m_levelContainer.CurrentLevel;
                    m_enemyContainer = m_level.EnemyContainer;

                    m_player.ToggleInput(false);
                    m_enemyContainer.DeactivateAll();
                    m_lockerController.IsLockerSafe.Value = true;

                    DOVirtual.DelayedCall(2f,
                        () => State.Value = GameState.Round);
                    break;
                case GameState.Round:
                    m_enemyContainer.ActivateAll();
                    m_player.ToggleInput(true);
                    break;
                case GameState.Win:
                    m_enemyContainer.DeactivateAll();
                    m_player.ToggleInput(false);

                    DOVirtual.DelayedCall(2f,
                        () => Reload(true));
                    break;
                case GameState.Fail:
                    m_enemyContainer.DeactivateAll();
                    m_player.ToggleInput(false);

                    DOVirtual.DelayedCall(2f,
                        () => Reload(false));

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
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

        private void Reload(bool next)
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
        Fail
    }
}