using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPopupUI : MonoBehaviour
{
    // 1. 아이템 버리기 확인 팝업 관련 변수들
    [Header("Confirmation Popup")]
    [SerializeField] private GameObject _confirmationPopupObject;
    [SerializeField] private Text _confirmationItemNameText; // 버릴 아이템 이름 텍스트
    [SerializeField] private Text _confirmationText; // "정말 버리시겠습니까?" 같은 문구
    [SerializeField] private Button _confirmationOkButton;     // 확인 버튼
    [SerializeField] private Button _confirmationCancelButton; // 취소 버튼

    // 2. 수량 입력 팝업 관련 변수들
    [Header("Amount Input Popup")]
    [SerializeField] private GameObject _amountInputPopupObject;
    [SerializeField] private Text _amountInputItemNameText;    // 아이템 이름
    [SerializeField] private InputField _amountInputField;     // 수량 입력 필드
    [SerializeField] private Button _amountPlusButton;         // 수량 증가 버튼
    [SerializeField] private Button _amountMinusButton;        // 수량 감소 버튼
    [SerializeField] private Button _amountInputOkButton;      // 확인 버튼
    [SerializeField] private Button _amountInputCancelButton;  // 취소 버튼

    private event Action OnConfirmationOK;         // 확인 팝업의 확인 버튼 눌렀을 때 동작할 델리게이트
    private event Action<int> OnAmountInputOK;     // 수량 입력 팝업의 확인 버튼 눌렀을 때 동작할 델리게이트

    private int _maxAmount; // 최대 수량 제한

    private void Awake()
    {
        InitUIEvents(); // 버튼 이벤트 바인딩
        HidePanel(); // 시작 시 전체 팝업 비활성화
        HideConfirmationPopup();
        HideAmountInputPopup();
    }

    private void Update()
    {
        // 팝업이 활성화된 상태에서 단축키 처리
        if (_confirmationPopupObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return))
                _confirmationOkButton.onClick?.Invoke();
            else if (Input.GetKeyDown(KeyCode.Escape))
                _confirmationCancelButton.onClick?.Invoke();
        }
        else if (_amountInputPopupObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return))
                _amountInputOkButton.onClick?.Invoke();
            else if (Input.GetKeyDown(KeyCode.Escape))
                _amountInputCancelButton.onClick?.Invoke();
        }
    }

    // 확인 팝업 열기 - 아이템 이름과 확인 콜백 지정
    public void OpenConfirmationPopup(Action okCallback, string itemName)
    {
        ShowPanel();
        ShowConfirmationPopup(itemName);
        SetConfirmationOKEvent(okCallback);
    }

    // 수량 입력 팝업 열기 - 아이템 이름, 최대 수량, 콜백 지정
    public void OpenAmountInputPopup(Action<int> okCallback, int currentAmount, string itemName)
    {
        _maxAmount = currentAmount - 1; // 현재 수량보다 1 적은 수까지만 입력 가능
        _amountInputField.text = "1";  // 기본 입력값 1로 초기화

        ShowPanel();
        ShowAmountInputPopup(itemName);
        SetAmountInputOKEvent(okCallback);
    }

    // 버튼 이벤트 바인딩
    private void InitUIEvents()
    {
        // [확인 팝업] 확인 버튼 클릭 시 - 팝업 닫고 콜백 실행
        _confirmationOkButton.onClick.AddListener(HidePanel);
        _confirmationOkButton.onClick.AddListener(HideConfirmationPopup);
        _confirmationOkButton.onClick.AddListener(() => OnConfirmationOK?.Invoke());

        // [확인 팝업] 취소 버튼 클릭 시 - 팝업 닫기만 수행
        _confirmationCancelButton.onClick.AddListener(HidePanel);
        _confirmationCancelButton.onClick.AddListener(HideConfirmationPopup);

        // [수량 팝업] 확인 버튼 클릭 시 - 입력된 수량을 int로 파싱하여 콜백 호출
        _amountInputOkButton.onClick.AddListener(HidePanel);
        _amountInputOkButton.onClick.AddListener(HideAmountInputPopup);
        _amountInputOkButton.onClick.AddListener(() => OnAmountInputOK?.Invoke(int.Parse(_amountInputField.text)));

        // [수량 팝업] 취소 버튼 클릭 시 - 팝업 닫기만 수행
        _amountInputCancelButton.onClick.AddListener(HidePanel);
        _amountInputCancelButton.onClick.AddListener(HideAmountInputPopup);

        // [-] 버튼 클릭 시 수량 감소 (1보다 작아지지 않음)
        _amountMinusButton.onClick.AddListener(() =>
        {
            int.TryParse(_amountInputField.text, out int amount);
            if (amount > 1)
            {
                int nextAmount = Input.GetKey(KeyCode.LeftShift) ? amount - 10 : amount - 1;
                if (nextAmount < 1)
                    nextAmount = 1;
                _amountInputField.text = nextAmount.ToString();
            }
        });

        // [+] 버튼 클릭 시 수량 증가 (최대 수량을 넘지 않음)
        _amountPlusButton.onClick.AddListener(() =>
        {
            int.TryParse(_amountInputField.text, out int amount);
            if (amount < _maxAmount)
            {
                int nextAmount = Input.GetKey(KeyCode.LeftShift) ? amount + 10 : amount + 1;
                if (nextAmount > _maxAmount)
                    nextAmount = _maxAmount;
                _amountInputField.text = nextAmount.ToString();
            }
        });

        // 직접 입력한 값이 유효 범위를 벗어나면 보정
        _amountInputField.onValueChanged.AddListener(str =>
        {
            int.TryParse(str, out int amount);
            bool flag = false;

            if (amount < 1)
            {
                flag = true;
                amount = 1;
            }
            else if (amount > _maxAmount)
            {
                flag = true;
                amount = _maxAmount;
            }

            if (flag)
                _amountInputField.text = amount.ToString();
        });
    }

    // 배경 패널 활성/비활성
    private void ShowPanel() => gameObject.SetActive(true);
    private void HidePanel() => gameObject.SetActive(false);

    // 확인 팝업 활성/비활성
    private void ShowConfirmationPopup(string itemName)
    {
        _confirmationItemNameText.text = itemName;
        _confirmationPopupObject.SetActive(true);
    }
    private void HideConfirmationPopup() => _confirmationPopupObject.SetActive(false);

    // 수량 입력 팝업 활성/비활성
    private void ShowAmountInputPopup(string itemName)
    {
        _amountInputItemNameText.text = itemName;
        _amountInputPopupObject.SetActive(true);
    }
    private void HideAmountInputPopup() => _amountInputPopupObject.SetActive(false);

    // 콜백 설정 함수들
    private void SetConfirmationOKEvent(Action handler) => OnConfirmationOK = handler;
    private void SetAmountInputOKEvent(Action<int> handler) => OnAmountInputOK = handler;
}
