
#Warning

**This is a quick hack** and the same functionality is implemented in Visual Studio 11 -- if you are stuck with Visual Studio 2010 and want some sandboxed solution deployment automation, this might work for you.

p.s.: if you have a better way to [de]activate a sandbox solution using the Client OM, please let me know!

#Steps

###1. Modify the _catalogs/solutions/Forms/Activate.aspx page in your 365 site.

Place following the script inside the PlaceHolderMain ContentPlaceHolder, before the `</asp:Content>` fragment:

    <script>

	    if(location.href.indexOf("deploy=true") > -1) {

		    ExecuteOrDelayUntilScriptLoaded(deploySolution, "SP.Ribbon.js");
	    }
	
	    function deploySolution() {

		    if(location.href.indexOf("Op=ACT") > -1)
		    {
			    SP.Ribbon.PageManager.get_instance().$k_1[0].handleCommand("Ribbon.ListForm.Display.Solution.ActivateSolution");
		    }
		
		    if(location.href.indexOf("Op=DEA") > -1)
		    {
			    SP.Ribbon.PageManager.get_instance().$k_1[0].handleCommand("Ribbon.ListForm.Display.Solution.DeactivateSolution");
		    }
	
	    }
	
    </script>

###2. Add the following on your Pre or Post Solution Deployment step in the SharePoint tab (Visual Studio project):**

"C:/path/to/SharePointOnlineDeploy.exe" "https://365site/" "$(TargetDir)$(TargetName).wsp"

#Thanks

- Claims Authentication based on the official Microsoft sample http://msdn.microsoft.com/en-us/library/hh147177.aspx, modified to support proxies.
- PeppeDotNet http://solutioninstaller365.codeplex.com for the Site.GetCatalog(121) trick and basic Client OM file upload code;

#Sample Output Window
    ...
    Active Deployment Configuration: Default
    Run Pre-Deployment Command:
    365: Deactivating solution...
    365: Uploading solution...
    365: Activating solution...
    365: Solution deployed.
    Recycle IIS Application Pool:
      Skipping application pool recycle because a sandboxed solution is being deployed.
    ...

#The MIT License (MIT)
Copyright (c) 2012 Francisco Aquino *faquino@live.com*

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
