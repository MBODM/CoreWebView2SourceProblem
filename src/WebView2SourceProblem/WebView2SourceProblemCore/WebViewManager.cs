using System.Diagnostics;
using Microsoft.Web.WebView2.Core;

namespace WebView2SourceProblemCore
{
    public sealed class WebViewManager : IWebViewManager
    {
        private const string StartUrl = "https://www.curseforge.com/wow/addons/deadly-boss-mods";
        private const string FetchUrl = "https://www.curseforge.com/api/v1/mods/3358/files/4485146/download";

        private CoreWebView2? coreWebView = null;

        public void Init(CoreWebView2 coreWebView)
        {
            this.coreWebView = coreWebView ?? throw new ArgumentNullException(nameof(coreWebView));
        }

        public void Run()
        {
            if (coreWebView == null)
            {
                throw new InvalidOperationException("Not initialized, please make sure you called the Init() method.");
            }

            coreWebView.NavigationStarting += CoreWebView_NavigationStarting;
            coreWebView.NavigationCompleted += CoreWebView_NavigationCompleted;
            coreWebView.SourceChanged += CoreWebView_SourceChanged;

            Navigate(StartUrl);
        }

        private void CoreWebView_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            if (e.Uri == StartUrl)
            {
                Debug.WriteLine("[NavigationStarting SUCCESS] Reached handler for StartUrl.");
            }
            else if (e.Uri == FetchUrl)
            {
                Debug.WriteLine("[NavigationStarting SUCCESS] Reached handler for FetchUrl.");
            }
            else if (e.Uri.Contains("api-key=") && e.IsRedirected)
            {
                Debug.WriteLine("[NavigationStarting SUCCESS] Reached handler for api-key download URL.");

                // Redirect example URL -> https://edge.forgecdn.net/files/4485/146/DBM-10.0.35.zip?api-key=267C6CA3
                // In reality the download will start (DownloadStarting event and so on) after this...

                Debug.WriteLine("Example finished, will cancel the naviation now to stop the example.");
                e.Cancel = true;
            }
            else
            {
                Debug.WriteLine("[NavigationStarting ERROR] Given URL not allowed, will cancel the navigation.");
                e.Cancel = true;
            }
        }

        private void CoreWebView_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (coreWebView == null) return; // Enforced by NRT

            if (coreWebView.Source == StartUrl)
            {
                Debug.WriteLine("[NavigationCompleted SUCCESS] Reached handler for StartUrl.");

                // Simulating the fetched URL (to navigate to) by using the FetchUrl constant here.
                // In reality that URL is fetched from the JSON (embedded in StartUrl website)...

                Navigate(FetchUrl); // <-- Here is the problem/issue: CoreWebView2 will not navigate to FetchUrl and not changes Source property.
            }
            else if (coreWebView.Source == FetchUrl)
            {
                Debug.WriteLine("[NavigationCompleted SUCCESS] Reached handler for FetchUrl, some redirects will occur now.");
            }
            else
            {
                Debug.WriteLine("[NavigationCompleted ERROR] Given URL not allowed, will not continue with navigation.");
            }
        }

        private void CoreWebView_SourceChanged(object? sender, CoreWebView2SourceChangedEventArgs e)
        {
            if (coreWebView == null) return; // Enforced by NRT

            Debug.WriteLine($"[SourceChanged SUCCESS] CoreWebView.Source changed to {coreWebView.Source}");
        }

        private void Navigate(string url)
        {
            if (coreWebView == null) return; // Enforced by NRT

            coreWebView.Stop(); // Just to make sure
            coreWebView.Navigate(url);
        }
    }
}
