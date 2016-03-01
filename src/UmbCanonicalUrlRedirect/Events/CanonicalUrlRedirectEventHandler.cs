using System;
using System.Web;
using umbraco;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Web.Routing;
using Umbraco.Core.Logging;
using Umbraco.Web;
using UmbCanonicalUrlRedirect.Support;

namespace UmbCanonicalUrlRedirect.Events
{
    public class CanonicalUrlRedirectEventHandler : ApplicationEventHandler
    {

        #region Fields

        /// <summary>
        /// Configuration settings.
        /// </summary>
        private readonly Settings settings;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructor to load settings.
        /// </summary>
        public CanonicalUrlRedirectEventHandler()
        {
            // Load settings from config file.
            this.settings = new Settings();
        }

        #endregion Constructors

        #region Methods

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
            var request = sender as PublishedContentRequest;
            var context = HttpContext.Current;

            // If the response is invalid, the page doesn't exist, or will be changed already, don't do anything more.
            if ((request == null) || (!request.HasPublishedContent) || (request.Is404) || (request.IsRedirect) || (request.ResponseStatusCode > 0))
            {
                // Log for debugging.
                LogHelper.Debug<CanonicalUrlRedirectEventHandler>("Stopping IntranetRestrict for requested URL {0} because request was null ({1}), there was no published content ({2}), was 404 ({3}), was a redirect ({4}), or status code ({5}) was already set.",
                    () => context.Request.Url.AbsolutePath,
                    () => (request == null),
                    () => (!request.HasPublishedContent),
                    () => (request.Is404),
                    () => (request.IsRedirect),
                    () => request.ResponseStatusCode);

                return;
            }

            // Check to see if we should not redirect this page. Use InitialPublishedContent to get properties for requested page, not a login redirect.
            if (request.InitialPublishedContent.GetPropertyValue<bool>("umbNoCanonicalRedirect", false))
            {
                // No redirect.
                return;
            }

            // Get URLs. Use InitialPublishedContent to get properties for requested page, not a login redirect.
            var requestedPath = context.Request.Url.GetLeftPart(UriPartial.Path);
            var properPath = request.InitialPublishedContent.UrlAbsolute();

            // If URLs don't match, perform redirect.
            if (requestedPath != properPath)
            {
                // Calculate proper path and query string.
                var properPathAndQuery = properPath + context.Request.Url.Query;

                // Substitute proper path for requested path.
                var redirectUrl = new Uri(request.Uri, properPathAndQuery);

                // Log for debugging.
                LogHelper.Debug<CanonicalUrlRedirectEventHandler>("Redirecting from {0} to {1} using {2} redirect.",
                    () => requestedPath,
                    () => properPath,
                    () => (settings.UseTemporaryRedirects ? "temporary" : "permanent"));

                // Set for Umbraco to perform redirection.
                if (settings.UseTemporaryRedirects)
                {
                    request.SetRedirect(redirectUrl.AbsoluteUri);
                }
                else
                {
                    request.SetRedirectPermanent(redirectUrl.AbsoluteUri);
                }

                return;
            }
        }

        #endregion Methods

    }
}