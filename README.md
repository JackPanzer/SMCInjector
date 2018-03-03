# SMCInjector

Simple injector that extends the .net standard dependency injector to add the possibility of using a familiar xml format.

## Getting Started

To start using the SMCInjector, first you have to compile it using the target framework .net standard 1.x at least.

Once the dll has been added to the project, invoke the injector like
```
public void Startup(string configFile, IServiceCollection services)
{
    services = new ServiceCollection();
    services.AddSmcInjection(configFile);
}
```

## Considerations

* So far, raw primitives and other services can't be injected at the same time (something like MyService(IService2, int num))
* Can't define twice the same mapping
* Can't specify the version of the assemblies or signatures (yet...)

### Installing

This package is still under development and has not been published yet to Nuget.

So far, the only option for installation is cloning the master and compile the project.

### Config file example

The config file is organized in three blocks:

* Services
* Mappings
* Constructors

The services is where we define the interfaces/classes and where are we aiming at.

```
<Services>
    <!-- Common service -->
    <Registry Alias="IProductService" Class="SMCInjector.Tests.Services.IProductService" Assembly="SMCInjector.Tests" />
    <Registry Alias="ProductService" Class="SMCInjector.Tests.Services.ProductServiceMock" Assembly="SMCInjector.Tests" />

    <!-- Referencing generics -->
    <Registry Alias="IRepository" Class="SMCInjector.Tests.Services.IRepository`1" Assembly="SMCInjector.Tests" />
    <Registry Alias="RepositoryMock" Class="SMCInjector.Tests.Services.RepositoryMock`1" Assembly="SMCInjector.Tests" />
    
    <!-- This are used for the Constructor settings -->
    <Registry Alias="SimpleWcfService" Class="SMCInjector.Tests.Services.SimpleWcfService" Assembly="SMCInjector.Tests" />
    <Registry Alias="SimpleWcfService2" Class="SMCInjector.Tests.Services.SimpleWcfService2" Assembly="SMCInjector.Tests" />
</Services>
```

The mappings is where we connect the different services.

```
<Mappings>
    <!-- Setting up common dependency injection -->
    <Mapping Name="ProductServiceMapping" From="IProductService" To="ProductService" InjectionType="Scoped" />
    <Mapping Name="RepositoryServiceMapping" From="IRepository" To="RepositoryMock" InjectionType="Scoped" />

    <!-- Setting up dependency injection for services that needs to receive parameters -->
    <Mapping Name="SimpleWcfServiceMapping" From="SimpleWcfService" To="SimpleWcfService" InjectionType="Singleton" />
    <Mapping Name="SimpleWcfServiceMapping2" From="SimpleWcfService2" To="SimpleWcfService2" InjectionType="Singleton" />
</Mappings>
```

And the constructors is where define which mappings need specific parameters.

```
<Constructors>
    <Constructor MapName="SimpleWcfServiceMapping">
        <Param Name="binding" Value="ITestService_Binding" />
    </Constructor>

    <!-- Attributes for specific constructors -->
    <Constructor MapName="SimpleWcfServiceMapping2">
        <Param Name="binding" Value="ITestService_Binding" />
        <Param Name="numRequests" Value="10" />
        <Param Name="ttl" Value="10.4" />
    </Constructor>
</Constructors>
```
## Running the tests

There is a test project included with the solution. The test were build with MSTest.

If the test project target framework is .net standard instead of netcoreapp, VS2017 (at least community edition) will fail to detect any test project

## Authors

* Juan Manuel Pedraza <jackpanzer2012@gmail.com>

## License

This project is licensed under the GPL v3 - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Billie Thompson - Great README.md example! - [PurpleBooth](https://github.com/PurpleBooth)