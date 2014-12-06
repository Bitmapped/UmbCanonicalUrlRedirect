using System;
using System.Web;
using Umbraco.Core;
using Umbraco.Web.Routing;
using Umbraco.Core.Configuration;
using umbraco;

namespace UmbCanonicalUrlRedirect.Events
{
    public class CanonicalUrlRedirectEventHandler : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            PublishedContentRequest.Prepared += PublishedContentRequest_Prepared;
        }

        /// <summary>
        /// Event handler to redirect traffic to the canonical URL.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PublishedContentRequest_Prepared(object sender, EventArgs e)
        {
            // Get request.
            PublishedContentRequest request = sender as PublishedContentRequest;
            HttpContext context = HttpContext.Current;

            // Ensure there is a valid and it has published content. Otherwise, return without doing anything.
            if ((request == null) || (!request.HasPublishedContent))
            {
                return;
            }

            // Get URLs.
            string requestedPath = context.Request.Url.AbsolutePath;
            string properPath = request.InitialPublishedContent.Url;

            // If URLs don't match, perform redirect.
            if (requestedPath != properPath)
            {
                bool noCanonicalRedirect = false;

                // Check to see if we should redirect this page.  If page property is null, assume we should allow redirect.                
                if (request.InitialPublishedContent.GetProperty("umbNoCanonicalRedirect") != null)
                {
                    noCanonicalRedirect = (bool)request.InitialPublishedContent.GetProperty("umbNoCanonicalRedirect").Value;
                }

                // Perform canonical URL redirection if OK.
                if (!noCanonicalRedirect)
                {
                    // Substitute proper path for requested path.
                    string redirectUrl = request.Uri.AbsoluteUri.Replace(requestedPath, properPath);

                    // Set for Umbraco to perform redirection.
                    request.SetRedirectPermanent(redirectUrl);

                    return;
                }

                // Check if we need to handle trailing slash.
                if ((GlobalSettings.UseDirectoryUrls) && (UmbracoConfig.For.UmbracoSettings().RequestHandler.AddTrailingSlash))
                {
                    // Get URL with trailing slash.
                    var pathWithSlash = context.Request.Url.EndPathWithSlash().AbsolutePath;

                    // Check to see if current URL has trailing slash.
                    if (requestedPath != pathWithSlash)
                    {
                        // Set for Umbraco to perform redirection.
                        request.SetRedirectPermanent(pathWithSlash);
                    }
                }


            }
        }
    }
}