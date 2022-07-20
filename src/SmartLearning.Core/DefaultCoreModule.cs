using Autofac;
using SmartLearning.Core.Interfaces;
using SmartLearning.Core.Services;

namespace SmartLearning.Core;

public class DefaultCoreModule : Module
{
  protected override void Load(ContainerBuilder builder)
  {
    /*builder.RegisterType<ToDoItemSearchService>()
        .As<IToDoItemSearchService>().InstancePerLifetimeScope();*/

    builder.RegisterType<ClassroomService>()
        .As<IClassroomService>().InstancePerLifetimeScope();
  }
}
