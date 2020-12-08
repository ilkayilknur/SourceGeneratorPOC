# C# Source Generators Sample Project
This project contains a source generator project and a test project. 

Source generator finds types implementing `IStartup` and calls the `Execute` method inside the generated method body.

You can simply call the generated function like below.
```csharp
StartupRunner.Run();
```
