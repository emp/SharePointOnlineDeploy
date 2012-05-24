using System;
using System.Threading;
using System.Windows.Forms;
using Microsoft.SharePoint.Client;
using MSDN.Samples.ClaimsAuth;

namespace SharePointOnlineDeploy
{
  class Program
  {
    private static System.Windows.Forms.Form _deactivation;
    private static System.Windows.Forms.Form _activation;
    private static string _path;
    private static string _site;
    private static ClientContext _context;
    private static bool _isDeactivated;

    [STAThread]
    static void Main(string[] args)
    {

      try
      {

        if (args.Length < 2)
          throw new Exception("Expected atleast 2 parameters: deployWSP.exe <url> <wsp_path> <debug>");

        _site = args[0];
        _path = args[1];

        using (_context = ClaimClientContext.GetAuthenticatedContext(_site))
        {

          if (_context != null)
          {
            _context.Load(_context.Site);
            _context.ExecuteQuery();

            var wspPath = _context.Site.ServerRelativeUrl + "_catalogs/solutions/" + System.IO.Path.GetFileName(_path);
            var wsp = _context.Web.GetFileByServerRelativeUrl(wspPath);
            _context.Load(wsp);
            _context.ExecuteQuery();

            _context.Load(wsp.ListItemAllFields);
            _context.ExecuteQuery();

            var itemId = wsp.ListItemAllFields["ID"];

            if (wsp.Exists)
            {

              Console.WriteLine("365: Deactivating solution...");

              _deactivation = new System.Windows.Forms.Form { WindowState = FormWindowState.Minimized };
              _deactivation.SuspendLayout();
              var webBrowser = new WebBrowser();
              webBrowser.Navigate(_site + "_catalogs/solutions/Forms/Activate.aspx?deploy=true&Op=DEA&ID=" + itemId);
              webBrowser.Navigated += webBrowser_Navigated;
              _deactivation.Controls.Add(webBrowser);
              _deactivation.ResumeLayout(false);

              Application.Run(_deactivation);

              while (!_isDeactivated) // await deactivation
                Thread.Sleep(200);
            }

            Console.WriteLine("365: Uploading solution...");

            var solutionList = _context.Site.GetCatalog(121);

            var info = new FileCreationInformation
            {
              Overwrite = true,
              Url = System.IO.Path.GetFileName(_path),
              Content = System.IO.File.ReadAllBytes(_path)
            };

            var solution = solutionList.RootFolder.Files.Add(info);
            _context.Load(solution);
            _context.ExecuteQuery();

            Console.WriteLine("365: Activating solution...");

            _activation = new System.Windows.Forms.Form { WindowState = FormWindowState.Minimized };
            _activation.SuspendLayout();
            var webBrowserAct = new WebBrowser();
            webBrowserAct.Navigate(_site + "_catalogs/solutions/Forms/Activate.aspx?deploy=true&Op=ACT&ID=" + itemId);
            webBrowserAct.Navigated += webBrowserAct_Navigated;
            _activation.Controls.Add(webBrowserAct);
            _activation.ResumeLayout(false);

            Application.Run(_activation);

          }
        }
      }
      catch (Exception ex)
      {
        Console.Write(ex.ToString());
        if (args.Length == 3 && args[2] == "debug")
          throw;
      }
    }

    static void webBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
    {
      if (!e.Url.AbsolutePath.Contains("AllItems.aspx"))
        return;

      _deactivation.Close();
      _isDeactivated = true;
    }

    static void webBrowserAct_Navigated(object sender, WebBrowserNavigatedEventArgs e)
    {
      if (!e.Url.AbsolutePath.Contains("AllItems.aspx"))
        return;

      _activation.Close();
      Console.WriteLine("365: Solution deployed.");
    }
  }
}
