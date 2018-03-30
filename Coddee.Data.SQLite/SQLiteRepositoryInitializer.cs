using System;
using System.Collections.Generic;
using System.Text;

namespace Coddee.Data.SQLite
{
    public class SQLiteRepositoryInitializer : IRepositoryInitializer
    {
        private readonly ISQLiteDBManager _dbManager;
        private readonly IObjectMapper _mapper;
        public int RepositoryType { get; } = (int) RepositoryTypes.SQLite;

        public SQLiteRepositoryInitializer(ISQLiteDBManager dbManager,IObjectMapper mapper)
        {
            _dbManager = dbManager;
            _mapper = mapper;
        }
        public void InitializeRepository(IRepositoryManager repositoryManager, IRepository repository, Type implementedInterface)
        {
            ((ISQLiteRepository)repository).Initialize(_dbManager, repositoryManager, _mapper, implementedInterface);
        }
    }
}
