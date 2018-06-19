# Myxas.ConfigStringLocalizer
[![Build status](https://img.shields.io/appveyor/ci/lethek/configstringlocalizer.svg)](https://ci.appveyor.com/project/lethek/configstringlocalizer/branch/master) 
[![Test status](https://img.shields.io/appveyor/tests/lethek/configstringlocalizer.svg)](https://ci.appveyor.com/project/lethek/configstringlocalizer/branch/master) 

[![NuGet Version](https://img.shields.io/nuget/v/Myxas.ConfigStringLocalizer.svg?style=flat)](https://www.nuget.org/packages/Myxas.ConfigStringLocalizer/) 
[![NuGet Downloads](https://img.shields.io/nuget/dt/Myxas.ConfigStringLocalizer.svg)](https://www.nuget.org/packages/Myxas.ConfigStringLocalizer/) 

# Introduction 
ConfigStringLocalizer is a .NET library that provides an implementation of [IStringLocalizer](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.localization.istringlocalizer) for loading resource-strings from an [IConfiguration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration) instance, as opposed to .NET's default of compiled RESX files. This allows you to keep your translations within a JSON file, an XML file, SQL Server, or almost any other form of storage you need (provided that an appropriate custom ConfigurationProvider is configured).

There is also out-of-the-box support for hot-reloading of changes to your resource strings if the ConfigurationProvider supports a ReloadOnChange option (as Microsoft's file-based ones do).

Although these libraries and interfaces were designed for .NET Core, they are targeting .NET Standard 1.3 and up so can also be used on .NET Framework 4.6 and newer.

# Getting Started
TODO: Guide users through getting your code up and running on their own system. In this section you can talk about:
1.	Installation process
2.	Software dependencies
3.	Latest releases
4.	API references

# Build and Test
TODO: Describe and show how to build your code and run the tests. 

# Contribute
TODO: Explain how other users and developers can contribute to make your code better. 

If you want to learn more about creating good readme files then refer the following [guidelines](https://www.visualstudio.com/en-us/docs/git/create-a-readme). You can also seek inspiration from the below readme files:
- [ASP.NET Core](https://github.com/aspnet/Home)
- [Visual Studio Code](https://github.com/Microsoft/vscode)
- [Chakra Core](https://github.com/Microsoft/ChakraCore)