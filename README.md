# Unity Auto Registration

### Install from Nuget
Add it to your project with nuget: Install-Package UnityAutoRegistration

http://nuget.org/List/Packages/UnityAutoRegistration

### Project Description

Rules for determining whether to include/exclude type/assembly are predicates (Predicate<T>) so you can use lambda syntax to specify them or direct method name. There are a few methods in If helper class (like Implements, DecoratedWith) to cover some common scenarios of type registering.

For example, auto registration configuration using Unity Auto Registration may look like:
```
var container = new UnityContainer();

container
    .ConfigureAutoRegistration()
    .ExcludeAssemblies(a => a.GetName().FullName.Contains("Test"))
    .Include(If.Implements<ILogger>, Then.Register().UsingPerCallMode())
    .Include(If.ImplementsITypeName, Then.Register().WithTypeName())
    .Include(If.Implements<ICustomerRepository>, Then.Register().WithName("Sample"))
    .Include(If.Implements<IOrderRepository>,
             Then.Register().AsSingleInterfaceOfType().UsingPerCallMode())
    .Include(If.DecoratedWith<LoggerAttribute>,
             Then.Register()
                      .As<IDisposable>()
                      .WithTypeName()
                      .UsingLifetime<MyLifetimeManager>())
    .Exclude(t => t.Name.Contains("Trace"))
    .ApplyAutoRegistration();
```
