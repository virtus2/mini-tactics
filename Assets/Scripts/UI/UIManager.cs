using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // 싱글톤
    [Header("유닛 명령 창")] 
    [SerializeField] 
    private CommandWindow commandWindow;
    [Header("타일 가리키는 화살표")]
    [SerializeField]
    private GameObject tileArrow;
    [Header("유닛 선택 원")]
    [SerializeField]
    private GameObject selectCircle;
    [Header("턴 표시 텍스트")]
    [SerializeField]
    private Text turnText;
    [Header("턴 종료 버튼")]
    [SerializeField]
    private Button endTurnButton;
    [Header("메세지 텍스트")]
    [SerializeField]
    private Text messageText;
    [Header("포착 범위 표시기")]
    [SerializeField]
    private Button sightButton;
    private bool sightButtonClicked = false;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        commandWindow.Hide();
        tileArrow.SetActive(false);
        selectCircle.SetActive(false);

        endTurnButton.onClick.AddListener(OnEndTurnButtonClick);
    }
    public void OnSightButtonClicked()
    {
        if (sightButtonClicked == false)
            BattleManager.Instance.ShowFindTiles(true);
        else
            BattleManager.Instance.ShowFindTiles(false);
        sightButtonClicked = !sightButtonClicked;
    }
    #region 메세지 텍스트
    public void ShowMessageText(string msg, float speed=1f)
    {
        messageText.text = msg;
        StartCoroutine(MessageFadeOut(speed));
    }
    private IEnumerator MessageFadeOut(float speed)
    {
        messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, 1);
        while (messageText.color.a > 0.0f)
        {
            messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, messageText.color.a - (Time.deltaTime * speed));
            yield return null;
        }
    }
    #endregion
    #region 턴 종료 버튼
    public void OnEndTurnButtonClick()
    {
        BattleManager.Instance.PlayerEndTurn();
    }
    #endregion
    #region 턴 표시 텍스트
    public void ShowTurnText(bool playerTurn)
    {
        turnText.gameObject.SetActive(true);
        if (playerTurn)
        {
            turnText.color = Color.white;
            turnText.text = "나의 턴";
        }
        else
        {
            turnText.color = Color.red;
            turnText.text = "적의 턴";
        }
    }
    public void HideTurnText()
    {
        turnText.gameObject.SetActive(false);
    }
    #endregion 
    #region 유닛 명령 창
    public void ShowCommandWindow(Unit unit)
    {
        if(unit.isPlayerUnit)
        {
            commandWindow.Show(unit);

        }
        else
        {

        }
    }

    public void HideCommandWindow()
    {
        commandWindow.Hide();
    }
    #endregion
    #region 타일 화살표
    public void ShowTileArrow()
    {
        tileArrow.SetActive(true);
    }
    public void HideTileArrow()
    {
        tileArrow.SetActive(false);
    }
    public void MoveTileArrow(FieldTile tile)
    {
        tileArrow.transform.localPosition = new Vector3(tile.x, tile.y, -0.25f);
    }
    #endregion
    #region 유닛 선택 원
    public void ShowSelectCircle()
    {
        selectCircle.SetActive(true);
    }
    public void HideSelectCircle()
    {
        selectCircle.SetActive(false);
    }
    public void MoveSelectCircle(Unit unit)
    {
        selectCircle.transform.localPosition = new Vector3(unit.x, unit.y, 0.05f);
    }
    #endregion
}
