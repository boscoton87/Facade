# Facade

Facade is a lightweight IoC library.  It provides the ability to register objects, constructors, and methods.  Both global and container specific service mapping is available, allowing for flexible service management.  

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. 

### Prerequisites

```
.Net Standard 2.0 (For library)
.Net Framework 4.6.1 (For test project)
Visual Studio 2017
NUnit 3 Test Adapter
```

### Installing

```
Pull Master branch.
```

```
Open Solution in Visual Studio.
```

```
Build Solution (If necessary perform NuGet Restore).
```

## Running the tests

```
Using Visual Studio Test Explorer, select Run All.
```

## Example Usage

#### Registering a Constructor Globally
```C#
Container.RegisterGlobalType<IService, Service>( "Some Parameter" );
```

#### Creating an Instance of Globally Registered Type
```C#
IService service = Container.ResolveGlobalType<IService>();
```

#### Removing Global Constructor Mapping
```C#
Container.RemoveGlobalTypeMapping<IService>();
```

#### Registering an Instance Locally then Resolve it.
```C#
Container container = new Container();
container.RegisterInstance<IService>( new Service( "Some Parameter" ) );
IService service = container.ResolveInstance<IService>();
```

#### Registering a Method Globally and then Invoke it.
```C#
string methodName = "greet";
var greeter = new Action<string>( ( name ) => Console.WriteLine( $"Greetings from {name}!" ) );
Container.RegisterGlobalMethod( methodName, greeter.Method, greeter.Target );
Container.InvokeGlobalMethod( methodName, "Alex" );
```

## Authors

* **Alex Harper** - [boscoton87](https://github.com/boscoton87)

## License

This project is licensed under the Apache-2.0 License.
