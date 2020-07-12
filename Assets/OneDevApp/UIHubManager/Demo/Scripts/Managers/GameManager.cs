using OneDevApp;

public class GameManager : MonoInstance<GameManager>
{
    public GameController gameController { get; private set; }
    public UIController uiController { get; private set; }


    protected override void Awake()
    {
        base.Awake();

        gameController = GetComponent<GameController>();
        uiController = GetComponent<UIController>();
    }
}
