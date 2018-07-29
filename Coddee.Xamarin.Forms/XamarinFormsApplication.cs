using System;
using System.Collections.Generic;
using System.Text;
using Coddee.AppBuilder;

namespace Coddee.Xamarin.Forms
{
    public class XamarinFormsApplication : IApplication
    {
        /// <inheritdoc />
        public XamarinFormsApplication(Guid applicationID, string applicationName)
        {
            ApplicationID = applicationID;
            ApplicationName = applicationName;
            ApplicationType = ApplicationTypes.Console;
        }

        /// <inheritdoc />
        public Guid ApplicationID { get; }
        /// <inheritdoc />
        public string ApplicationName { get; }
        /// <inheritdoc />
        public ApplicationTypes ApplicationType { get; }
    }
}
