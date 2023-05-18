using WebView2SourceProblemCore;

namespace WebView2SourceProblem
{
    public partial class MainForm : Form
    {
        private bool initialized;

        private readonly IWebViewManager webViewManager;

        public MainForm(IWebViewManager webViewManager)
        {
            this.webViewManager = webViewManager ?? throw new ArgumentNullException(nameof(webViewManager));

            InitializeComponent();
        }

        private async void Button_Click(object sender, EventArgs e)
        {
            if (!initialized)
            {
                initialized = true;

                await webView2.EnsureCoreWebView2Async();
                webViewManager.Init(webView2.CoreWebView2);
            }

            webViewManager.Run();
        }
    }
}
