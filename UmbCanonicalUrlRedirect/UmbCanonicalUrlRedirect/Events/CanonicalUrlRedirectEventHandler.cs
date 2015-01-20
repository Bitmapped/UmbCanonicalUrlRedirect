using System;
using System.Web;
using umbraco;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Web.Routing;
using Umbraco.Core.Logging;

namespace UmbCanonicalUrlRedirect.Events
{
    public class CanonicalUrlRedirectEventHandler : ApplicationEventHandler
    {
        /// <summary>
        /// Register event handler on start.
        /// </summary>
        /// <param name="httpApplicationBase">Umbraco application.</param>
        /// <param name="applicationContext">Application context.</param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            PublishedContentRequest.Prepared += PublishedContentRequest_Prepared;
        }

        /// <summary>
        /// Event handler to redirect traffic to the canonical URL.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event args</param>
        private void PublishedContentRequest_Prepared(object sender, EventArgs e)
        {
            // Get request.
            PublishedContentRequest request = sender as PublishedContentRequest;
            HttpContext context = HttpContext.Current;

            // If the response is invalid, the page doesn't exist, or will be changed already, don't do anything more.
            if ((request == null) || (request.Is404) || (request.IsRedirect) || (request.ResponseStatusCode > 0))
            {
                // Log for debugging.
                LogHelper.Debug<CanonicalUrlRedirectEventHandler>("Stopping CanonicalUrlRedirect for requested URL {0} because request was null ({1}), was 404 ({2}), was a redirect ({3}), or status code ({4}) was already set.",
                    () => context.Request.Url.AbsolutePath,
                    () => (request == null),
                    () => (request.Is404),
                    () => (request.IsRedirect),
                    () => request.ResponseStatusCode);

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

                    // Log for debugging.
                    LogHelper.Debug<CanonicalUrlRedirectEventHandler>("Permanently redirecting {0} to {1}.",
                        () => requestedPath,
                        () => properPath);

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

                        // Log for debugging.
                        LogHelper.Debug<CanonicalUrlRedirectEventHandler>("Adding slash and permanently redirecting {0}.",
                            () => pathWithSlash);
                    }
                }
            }
        }
    }
}