namespace Labirint.Web.Services;

public class MotionService(IJSRuntime jsRuntime)
{
    public bool IsReduced
    {
        get
        {
            if (jsRuntime is not IJSInProcessRuntime runtime)
            {
                return false;
            }

            try
            {
                return runtime.Invoke<bool>("labirintPage.isMotionReduced");
            }
            catch (JSException)
            {
                return false;
            }
        }
    }
}
