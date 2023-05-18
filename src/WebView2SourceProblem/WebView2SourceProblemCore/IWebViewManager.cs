using Microsoft.Web.WebView2.Core;

namespace WebView2SourceProblemCore
{
    public interface IWebViewManager
    {
        void Init(CoreWebView2 coreWebView);
        void Run();
    }
}
