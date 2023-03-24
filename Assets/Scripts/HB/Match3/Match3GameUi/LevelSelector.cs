using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HB.Audio;
using HB.Match3.Board;
using HB.Match3.Models;
using HB.Match3.Result;
using HB.Match3.View;
using HB.Match3.View.Quest;
using UnityEngine;
using UnityEngine.UI;

namespace HB.Match3.Match3GameUi
{
    public class LevelSelector : MonoBehaviour
    {
        private Match3MainBoard.Board _board;
        private BoardData _boardData;
        private BoardView _boardView;
        private QuestGiver _questGiver;
        private List<BoardData> _levels;
        private AudioPlayer _audioPlayer;
        private BoardViewData _boardViewData;

        private bool _show;
        private Color _inputColor;
        [SerializeField] private Button _selectButton;
        [SerializeField] private Button _rightButton;
        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _toggleButton;
        [SerializeField] private InputField _input;
        [SerializeField] private RectTransform _selectionPanel;
        [SerializeField] private Packages.GUI.Match3GameUi _ui;
        [SerializeField] private RectTransform _resultTransform;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Text _resultText;

        private void Start()
        {
            _boardViewData = Resources.Load<BoardViewData>("Match3/StaticAssets/board-view-data");
            _levels = Resources.LoadAll<BoardData>("Match3/LevelData").ToList();
            GameObject _audioPlayerGo = Resources.Load<GameObject>("AudioPlayer");
            _audioPlayer = Instantiate(_audioPlayerGo).GetComponent<AudioPlayer>();

            _inputColor = _input.textComponent.color;
            CreateBoardViewIfNotExist();
            _input.contentType = InputField.ContentType.IntegerNumber;
            _input.text = "1";
            _show = true;
            AddListeners();
            CloseResult();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void AddListeners()
        {
            _selectButton.onClick.AddListener(InitGame);
            _rightButton.onClick.AddListener(NextLevel);
            _leftButton.onClick.AddListener(PreviousLevel);
            _toggleButton.onClick.AddListener(TogglePanel);
            _restartButton.onClick.AddListener(InitGame);
            _input.onValidateInput += OnValidateInput;
        }

        private void TogglePanel()
        {
            float bp = ((RectTransform) _toggleButton.transform).sizeDelta.y * 2;
            float pos = (_selectionPanel.sizeDelta.y - bp) * 2;
            if (_show) _selectionPanel.DOAnchorPosY(-pos, 1f).onComplete = () => { _show = false; };
            else _selectionPanel.DOAnchorPosY(pos, 1f).onComplete = () => { _show = true; };
            CloseResult();
        }

        private void RemoveListeners()
        {
            _selectButton.onClick.RemoveListener(InitGame);
            _rightButton.onClick.RemoveListener(NextLevel);
            _leftButton.onClick.RemoveListener(PreviousLevel);
            _toggleButton.onClick.RemoveListener(TogglePanel);
            _restartButton.onClick.RemoveListener(InitGame);

            _input.onValidateInput -= OnValidateInput;
        }


        private char OnValidateInput(string text, int charindex, char addedchar)
        {
            int index = 0;

            if (int.TryParse(_input.text, out index))
            {
                _boardData = _levels.Find(data => data.index == index);
                if (_boardData == null)
                {
                    _input.textComponent.color = Color.red;
                }
                else
                {
                    _input.textComponent.color = _inputColor;
                }
            }
            else
            {
                _input.textComponent.color = _inputColor;
            }

            if (char.IsDigit(addedchar))
                return addedchar;

            return default;
        }

        private void PreviousLevel()
        {
            if (int.TryParse(_input.text, out int index))
            {
                index--;
                index = Mathf.Max(index, 0);
            }

            _input.text = index.ToString();
            CloseResult();
        }

        private void NextLevel()
        {
            int index = 0;
            if (int.TryParse(_input.text, out index))
            {
                index++;
                index = Mathf.Max(index, 0);
            }

            _input.text = index.ToString();
            CloseResult();
        }

        private void CreateBoardViewIfNotExist()
        {
            if (_boardView == null)
            {
                GameObject go = new GameObject("BoardView", typeof(BoardView));
                go.transform.SetParent(transform, false);
                _boardView = go.GetComponent<BoardView>();
                _boardView.SetViewData(_boardViewData, _audioPlayer);
            }
        }


        private void InitGame()
        {
            if (int.TryParse(_input.text, out int index))
            {
                _boardData = _levels.Find(data => data.index == index);
                if (_boardData == null)
                {
                    _input.textComponent.color = Color.red;
                    return;
                }
            }

            DisposeBoardView();
            InitNewBoardView();
            CloseResult();
        }

        private void DisposeBoardView()
        {
            QuestGiver.Dispose();
            _board?.Dispose();
        }

        private void InitNewBoardView()
        {
            _boardData.Deserialize();
            _boardView.SetBoardData(_boardData);
            //Initialize(_boardData, _boardViewData)
            _ui.Open();
            _ui.Init(_boardData,null, false,false,0);

            _board = new Match3MainBoard.Board(_boardData, _boardView);
            QuestGiver.SetData(_board, _boardView, _boardData.questData);
            QuestGiver.OnBlocksMatched += OnBlockMatched;
            QuestGiver.OnFinished = OnFinished;
        }

        private void OnFinished(Match3Result result)
        {
            _resultTransform.gameObject.SetActive(true);
            if (result.winStatus == WinStatus.Win)
            {
                _resultText.text = "[ WIN ]";
            }
            else
            {
                _resultText.text = "[ LOSE ]";
            }
        }

        private void OnBlockMatched(MoveData moveData)
        {
            _ui.UpdateOnMatches(moveData);
        }

        private void CloseResult()
        {
            _resultTransform.gameObject.SetActive(false);
        }

        private void Update()
        {
            _board?.Update(Time.deltaTime);
        }
    }
}