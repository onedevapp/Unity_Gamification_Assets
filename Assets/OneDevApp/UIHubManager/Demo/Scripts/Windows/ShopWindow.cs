using OneDevApp;

public class ShopWindow : GenericWindow
{

    public void OnMainMenuBtnClicked()
    {
        GameService.OnMainMenuSignal.Dispatch();
    }
}
