using MongoAppWPF.Interfaces.Users.Service;
using MongoAppWPF.Service.Users;
using Ninject.Modules;

namespace MongoAppWPF.Service.DI
{
    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IUserService>().To<UserService>().InTransientScope();
        }
    }
}
