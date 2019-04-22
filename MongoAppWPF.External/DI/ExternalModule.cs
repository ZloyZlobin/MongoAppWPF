using Microsoft.Extensions.Configuration;
using MongoAppWPF.External.MongoDB;
using MongoAppWPF.External.Users;
using MongoAppWPF.Interfaces.Users;
using Ninject;
using Ninject.Modules;

namespace MongoAppWPF.External.DI
{
    public class ExternalModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IMongoDBClient>().To<MongoDBClient>().InSingletonScope();
            Bind<IUserRepository>().To<UserRepository>().InTransientScope();
            Bind<MongoDBConfig>().ToMethod(ctx =>
                ctx.Kernel.Get<IConfiguration>().GetSection(nameof(MongoDBConfig)).Get<MongoDBConfig>());
        }
    }
}
