using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Garage.Match3;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using HB.Core.Modules.Audio;
using HB.Match3.Cells.Modules;
using UnityEngine;
using UnityEngine.Assertions;

namespace HB.Match3.View
{
    public class BoardView : MonoBehaviour, IBoardView
    {
        #region Private Fields
        private Grid _grid;
        private GridClickDetector _gridClickDetector;
        private bool _initialized;

        private LayerStack _layerStack;
        private BoardViewData _boardViewData;
        private TilemapSettings _tilemapSettings;
        private int _currentTutorialIndex;
        private List<BoardTutorial> _tutorialPrefabs;
        private static BoardTutorial _currentTutorial;
        private Action _onShowComplete;
        public const int Match3BoardLayerIndex = 14;
        public const string CollectCounterEffectName = "collect-counter";
        private static Dictionary<BlockColor, int> colorCount;
        private static Dictionary<BlockColor, Point> colorPosition;
        private Effect reshuffleEffect;
        private Effect fingerBoosterBoardEffect;
        public bool HasTutorial => _currentTutorial != null;

        #endregion

        #region IBoardView Interface

        public void CreateCellView(Cell cell)
        {
            CellView cellView = new CellView(_layerStack, _boardViewData);
            cell.SetView(cellView);
        }

        public void CreateView(BaseModule module, Point position)
        {
            IModuleView view = ModuleViewFactory.Create(module, _boardViewData);
            _layerStack.SetTile(view, position.x, position.y);
            view.Layer = _layerStack.GetLayer(module.layerName);
            module.SetView(view);
        }


        private void Awake()
        {
            gameObject.layer = Match3BoardLayerIndex;
        }

        #endregion

        #region Unity

        public void SetViewData(BoardViewData data, AudioPlayer audioPlayer)
        {
            _boardViewData = data;
            //if (AudioPlayer == null) AudioPlayer = audioPlayer;
        }

        public void SetBoardData(BoardData boardData)
        {
            if (!_initialized)
            {
                _initialized = true;
                CreateRequirements();
            }
            UpdateLayerStack(boardData);
            _currentTutorialIndex = 0;
            _tutorialPrefabs = new List<BoardTutorial>();
            _tutorialPrefabs.AddRange(boardData.boardTutorialPrefabs);
            colorCount = new Dictionary<BlockColor, int>();
            colorPosition = new Dictionary<BlockColor, Point>();
            BaseModule.Cleared -= AddCellToCollectCounter;
            BaseModule.Cleared += AddCellToCollectCounter;
        }

        public void ShowNextTutorial()
        {
            if (_currentTutorial != null)
            {
                Destroy(_currentTutorial.gameObject);
                _currentTutorial = null;
            }
            if (_tutorialPrefabs != null && _currentTutorialIndex < _tutorialPrefabs.Count)
            {
                _currentTutorial = Instantiate(_tutorialPrefabs[_currentTutorialIndex]);
                _currentTutorial.SetData(_tilemapSettings.boardOffset, _tilemapSettings.width, _tilemapSettings.height);
                _currentTutorial.transform.SetParent(_grid.transform, false);
                _currentTutorial.OnHide = ShowNextTutorial;
                _currentTutorialIndex++;
            }
        }

        public bool IsValidSwap(Point pos, Direction direction)
        {
            if (_currentTutorial == null) return true;
            return _currentTutorial.IsValidSwap(pos, direction);
        }

        private void AddCellToCollectCounter(BaseModule baseModule)
        {
            // Debug.Log($"Add {baseModule.id} to collection");
            if (baseModule is BlockModule blockModule)
            {
                var blockType = blockModule.blockType;
                if (QuestGiver.IsInQuest(blockType.ToString()) && blockModule.IgnoreQuestBySquareMatch == false)
                {
                    BlockColor blockColor = blockType.color;
                    if (colorCount.ContainsKey(blockColor))
                    {
                        colorCount[blockColor] += blockModule.Count;
                        colorPosition[blockColor] = blockModule.hitPosition;
                    }
                    else
                    {
                        colorCount[blockColor] = blockModule.Count;
                        colorPosition[blockColor] = blockModule.hitPosition;
                    }
                }
            }
        }

        public void PlayCollectCounter()
        {
            foreach (KeyValuePair<BlockColor, Point> colorPositionPair in colorPosition)
            {
                Point position = colorPositionPair.Value;
                BlockColor color = colorPositionPair.Key;
                var effect = _layerStack.PlayEffect(_layerStack.CellToWorld(position), CollectCounterEffectName);
                effect.SetCountAndColor(colorCount[color], color);
            }

            colorCount.Clear();
            colorPosition.Clear();
        }

        internal static void PlayAudio(string clipName, float delay = 0f)
        {
            //AudioPlayer?.PlayClip(clipName, delay);
        }

        private void CreateRequirements()
        {
            Assert.IsNotNull(_boardViewData);
            // Create Grid and move it to the correct position
            if (_grid == null)
            {
                GameObject gridGo = new GameObject("_grid");
                gridGo.transform.SetParent(transform, false);
                gridGo.layer = Match3BoardLayerIndex;
                _grid = gridGo.AddComponent<Grid>();
                _gridClickDetector = gridGo.AddComponent<GridClickDetector>();
            }

            _tilemapSettings = new TilemapSettings()
            {
                grid = _grid
            };
            _layerStack = new LayerStack(_tilemapSettings, _boardViewData);
        }

        private void UpdateLayerStack(BoardData boardData)
        {
            _tilemapSettings.boardOffset = boardData.boardOffset;
            _tilemapSettings.height = boardData.height;
            _tilemapSettings.width = boardData.width;
            _layerStack.UpdateTilemapSetting(_tilemapSettings);
            _layerStack.UpdateCheckerAndMask(boardData);
            _gridClickDetector.SetData(_tilemapSettings);
        }

        internal void ReleaseEffect(Effect effect)
        {
            _layerStack.ReleaseEffect(effect);
        }

        public Effect PlayEffect(string effectName, Vector3 position)
        {
            return _layerStack.PlayEffect(position, effectName);
        }

        public void PlayEffect(Point pos, string id, Action onClear)
        {
            var effect = _layerStack.PlayEffect(_layerStack.CellToWorld(pos), id);
            if (effect != null) effect.OnClear += onClear;
        }

        public void Dispose()
        {
            _layerStack?.Clear();
            _tutorialPrefabs?.Clear();
            if (_currentTutorial != null) Destroy(_currentTutorial.gameObject);
            BaseModule.Cleared -= AddCellToCollectCounter;
        }

        #endregion

        #region Public Methods
        public BoardViewData GetBlockViewData()
        {
            return _boardViewData;
        }

        public void Hide()
        {
            _grid.gameObject.SetActive(false);
        }

        public void Show(Action onComplete)
        {
            _onShowComplete = onComplete;
            _grid.gameObject.SetActive(true);

            // Hide all layers
            _layerStack.Hide();
            // unhide all layers
            _layerStack.Show(FadeLayersComplete);
        }
        private void FadeLayersComplete()
        {
            _onShowComplete?.Invoke();
            _onShowComplete = null;
        }

        private void BounceIn(Action onComplete)
        {
            _grid.transform.localScale = Vector3.zero;
            var showTween = _grid.transform.DOScale(Vector3.one, 1f);
            showTween.SetEase(Ease.OutElastic, 3f);
            showTween.onComplete += () => { onComplete?.Invoke(); };
        }

        public void Reshuffle()
        {
            PlayAudio("Reshuffle");
            reshuffleEffect = _layerStack.PlayEffect(Vector3.zero, "reshuffle");
            reshuffleEffect.OnClear += OnReshuffleClear;
        }

        private void OnReshuffleClear()
        {
            _layerStack.ReleaseEffect(reshuffleEffect);
        }

        public void PlayFingerBoosterEffect()
        {
            fingerBoosterBoardEffect = _layerStack.PlayEffect(_grid.transform.position, "finger-booster-board-effect");
        }

        public void ReleaseFingerBoosterEffect()
        {
            if (fingerBoosterBoardEffect != null)
            {
                _layerStack.ReleaseEffect(fingerBoosterBoardEffect);
                fingerBoosterBoardEffect = null;
            }
        }

        public void PlayBucketBlockEffect(Point startPos, Point targetPos, BlockType blockType, Action onComplete)
        {
            Vector3 startVec3 = _layerStack.CellToWorld(startPos);
            Vector3 endVec3 = _layerStack.CellToWorld(targetPos);
            var effect = _layerStack.PlayEffect(startVec3, endVec3, "bucket-spawner");
            effect.SetSprite(_boardViewData.blockViewData.blockViewDatas.Find(x => x.blockType == blockType).sprite);
            float impactDelay = effect.ClearDuration * 5 / 6;
            StartCoroutine(PlayBucketImpactEffect(impactDelay, endVec3, blockType.color));
            effect.OnComplete += () =>
            {
                // var impactEffect = (BucketBurstEffect)_layerStack.PlayEffect(endVec3, "bucket-impact");
                // impactEffect.SetColor(blockType.color);
                onComplete?.Invoke();
            };
        }

        public void PlayFlowerBlockEffect(Point startPos, Point targetPos, BlockType blockType, Action onComplete)
        {
            Vector3 startVec3 = _layerStack.CellToWorld(startPos);
            Vector3 endVec3 = _layerStack.CellToWorld(targetPos);
            var effect = _layerStack.PlayEffect(startVec3, endVec3, "collect");
            effect.SetSprite(_boardViewData.blockViewData.blockViewDatas.Find(x => x.blockType == blockType).sprite);
            float impactDelay = effect.ClearDuration * 5 / 6;
            StartCoroutine(PlayFlowerImpactEffect(impactDelay, endVec3, blockType.color));
            effect.OnComplete += () =>
            {
                // var impactEffect = (BucketBurstEffect)_layerStack.PlayEffect(endVec3, "bucket-impact");
                // impactEffect.SetColor(blockType.color);
                onComplete?.Invoke();
            };
        }
        private IEnumerator PlayBucketImpactEffect(float impactDelay, Vector3 endVec3, BlockColor color)
        {
            yield return new WaitForSeconds(impactDelay);
            var impactEffect = (BucketBurstEffect)_layerStack.PlayEffect(endVec3, "bucket-impact");
            impactEffect.SetColor(color);
        }

        private IEnumerator PlayFlowerImpactEffect(float impactDelay, Vector3 endVec3, BlockColor color)
        {
            yield return new WaitForSeconds(impactDelay);
            //var impactEffect = (BucketBurstEffect)_layerStack.PlayEffect(endVec3, "flower");
            //impactEffect.SetColor(color);
        }
        #endregion
    }
}