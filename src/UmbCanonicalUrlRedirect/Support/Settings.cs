using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace UmbCanonicalUrlRedirect.Support
{
    public class Settings
    {
        #region Fields

        // Define constants.
        private const string AppKey_UseTemporaryRedirects = "CanonicalUrlRedirect:UseTemporaryRedirects";

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Load settings from configuration file.
        /// </summary>
        public Settings()
        {
            // Load values from config files.
            this.UseTemporaryRedirects = this.ConfigLoadBool(AppKey_UseTemporaryRedirects, false);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Use temporary 302 redirects rather than permanent 301 redirects.
        /// </summary>
        public bool UseTemporaryRedirects { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Load configuration value for boolean.
        /// </summary>
        /// <param name="key">Key to load.</param>
        /// <param name="defaultValue">Default value to return if key is not specified.</param>
        /// <returns>True/false.</returns>
        private bool ConfigLoadBool(string key, bool defaultValue)
        {
            // If key was not set, default to false.
            if (WebConfigurationManager.AppSettings[key] == null)
            {
                return defaultValue;
            }

            // Value was set. Test it.
            try
            {
                return Boolean.Parse(WebConfigurationManager.AppSettings[key]);
            }
            catch
            {
                throw new ConfigurationErrorsException(String.Format("Value for {0} not correctly specified.", key));
            }
        }

        #endregion Methods
    }


}
