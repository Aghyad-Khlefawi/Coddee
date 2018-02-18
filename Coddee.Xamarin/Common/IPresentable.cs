using Xamarin.Forms;

namespace Coddee.Xamarin.Common
{
    /// <summary>
    /// Implemented by object that have an associated page
    /// </summary>
    public interface IPresentable
    {
        /// <summary>
        /// Returns the page associated with this object
        /// </summary>
        /// <returns></returns>
        Page GetPage(int pageIndex);

        /// <summary>
        /// Returns the page associated with this object
        /// </summary>
        /// <returns></returns>
        Page GetPage();
    }

    /// <summary>
    /// Implemented by object that have an associated page
    /// </summary>
    public interface IPresentable<TPage> : IPresentable where TPage : Page
    {
        TPage Page { get; }
    }
}