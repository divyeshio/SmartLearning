using SmartLearning.SharedKernel;

namespace SmartLearning.Infrastructure.Data;

public interface IDomainEventDispatcher
{
  Task DispatchAndClearEvents(IEnumerable<EntityBase> entitiesWithEvents);
}
