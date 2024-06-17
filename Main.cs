using glitcher.core;
using Servers = glitcher.core.Servers;

namespace LightHTTPServerTester
{
    /// <summary>
    /// **Ligth HTTP Server Tester**
    /// This application is to test a simple http server class for bigger projects.
    /// </summary>
    /// <remarks>
    /// Author: Marco Fernandez (marcofdz.com / glitcher.dev)<br/>
    /// Last modified: 2024.06.16 - June 16, 2024
    /// </remarks>
    public partial class Main : Form
    {
        private Servers.LightHTTPServer? _httpServer = null;

        public Main()
        {
            InitializeComponent();
            _httpServer = new Servers.LightHTTPServer();
        }

        private void btn_ShowLogger_Click(object sender, EventArgs e)
        {
            LogViewer.GetInstance().Show();
        }

        private void btn_Update_Click(object sender, EventArgs e)
        {
            if (_httpServer != null)
            {
                int.TryParse(txt_Port.Text.Trim(), out int port);
                int.TryParse(txt_MaxConnections.Text.Trim(), out int maxConnections);
                string basePathLocal = txt_BasePathLocal.Text.Trim();
                bool allowCORS = chk_AllowCORS.Checked;
                bool tryServeFirstLocal = chk_ServeFirstLocal.Checked;
                bool restartOnUpdate = chk_RestartOnUpdate.Checked;
                _httpServer.tryServeFirstLocal = tryServeFirstLocal;
                _httpServer.Update(port, maxConnections, basePathLocal, allowCORS, restartOnUpdate);
            }
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            if (_httpServer != null)
            {
                _httpServer.Start();
            }

        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            if (_httpServer != null)
            {
                _httpServer.Stop();
            }
        }

        private void btn_OpenBrowser_Click(object sender, EventArgs e)
        {
            if (_httpServer != null)
            {
                if (_httpServer.endpoints != null)
                    if (_httpServer.endpoints.Count > 0)
                        Utils.OpenWebBrowser(_httpServer.endpoints[0]);
            }
        }

        private void btn_OpenAppDir_Click(object sender, EventArgs e)
        {
            Utils.OpenAppFolder();
        }
    }
}