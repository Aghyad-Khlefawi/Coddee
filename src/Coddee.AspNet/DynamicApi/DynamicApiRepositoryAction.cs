using System.Threading.Tasks;
using Coddee.Data;

namespace Coddee.AspNet
{
    /// <summary>
    /// An <see cref="IDynamicApiAction"/> implementation for repository actions.
    /// </summary>
    public class DynamicApiRepositoryAction : DynamicApiActionBase
    {
       
        /// <summary>
        /// The name of the repository.
        /// </summary>
        public string RepositoryName { get; set; }

        /// <summary>
        /// The used repository manager instance.
        /// </summary>
        public IRepositoryManager RepositoryManager { get; set; }

        /// <param name="context"></param>
        /// <inheritdoc />
        protected override object GetInstnaceOwner(object context)
        {
            var instance = RepositoryManager.GetRepository(RepositoryName);
            instance.SetContext(context);
            return  instance;
        }
    }
}