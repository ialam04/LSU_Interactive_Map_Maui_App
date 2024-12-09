
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

    public async Task InvokeJavaScriptFunction(string function)
    {
        await MainThread.InvokeOnMainThreadAsync(() => EvaluateJavaScriptAsync(function));
    }
}