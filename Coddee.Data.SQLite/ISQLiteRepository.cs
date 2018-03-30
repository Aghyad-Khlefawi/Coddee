using System;

namespace Coddee.Data.SQLite
{
    public interface ISQLiteRepository : IRepository
    {
        /// <summary>
        /// Do any required initialization
        /// </summary>
        void Initialize(ISQLiteDBManager dbManager,
            IRepositoryManager repositoryManager,
            IObjectMapper mapper,
            Type implementedInterface,
            RepositoryConfigurations config = null);
    }
}