
namespace InteractiveLSUMap;
public class HybridWebView : WebView
{
    public static readonly BindableProperty JavaScriptFunctionProperty = BindableProperty.Create(
        nameof(JavaScriptFunction), typeof(string), typeof(HybridWebView), default(string));

    public string JavaScriptFunction
    {
        get => (string)GetValue(JavaScriptFunctionProperty);
        set => SetValue(JavaScriptFunctionProperty, value);
    }

    public void InvokeJavaScriptFunction(string function) =>
        MainThread.BeginInvokeOnMainThread(async () =>
            await EvaluateJavaScriptAsync(function));
}