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
ConfigStringLocalizer is installed from NuGet:

## Installation
```powershell
Install-Package Myxas.ConfigStringLocalizer
```

You'll also need a configuration provider, e.g. `Microsoft.Extensions.Configuration.Json`:

```powershell
Install-Package Microsoft.Extensions.Configuration.Json
```

## Resource-strings formats
ConfigStringLocalizer doesn't care about the exact format (or even if your configuration source holds other data too - as long as the resources are in their own sub-hierarchy of that config). The format is abstracted away by relying on the provided IConfiguration instance. What it does care about is that the resource-strings data are structured the way that it expects. See the following JSON for example:

```json
{
  "Close": {
    "de": "Schließen",
    "de-AT-ar": "Schliessen",
    "de-DE-bb": "Schliessen",
    "en": "Close",
    "es": "Cerrar",
    "id": "Tutup",
    "it": "Chiudi",
    "ms": "Tutup",
    "pt-BR": "Fechar",
    "zh": "关"
  },
  "Colour": {
    "de": "Farbe",
    "en-AU": "Colour",
    "en-US": "Color",
    "en-GB": "Colour",
    "es": "Color",
    "id": "Warna",
    "it": "Colore",
    "ms": "Warna",
    "pt": "Cor",
    "zh-Hans": "颜色",
    "zh-Hant": "顏色"
  }
}
```

We have the resource-string key ("Close" and "Colour" in the above example) acting as the parent/section-group for a list of CultureInfo names and respective translations.

You can split things up into as many different config files/sources as you wish, even repeating the same resource-string key in each file but with different or overlapping translations in each - you just need to make sure you pass all of those configs into the Localizer's setup. The IConfiguration implementation takes care of merging them all together, and the ConfigStringLocalizer takes care of fallback in event of missing translations.

If multiple config sources are provided and there are translations across those "files" which conflict - a translation from a latter-loaded file overrides/takes precedence.

Have a look at this project's unit-tests and data for more examples.

**Important:** You cannot use a colon (`:`) within a resource-string key because that character has special meaning to IConfiguration implementations as a key delimiter. The workaround is to encode the character as `&colon;` within your key text inside the config. However, within your code, you can (and should) simply use `:` instead - ConfigStringLocalizer will take care of transparently encoding & decoding it for you. Note, you can easily override the encoder/decoder used if you're not happy with the existing workaround, or if the format you're using requires other characters to also be escaped/encoded.

## How to register the LocalizerFactory for Dependency-Injection on ASP.NET Core
If you're using ASP.NET Core, register and configure the localizer factory with your services:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    var config = new ConfigurationBuilder()
        .AddJsonFile("Resources/File1.json", optional:false, reloadOnChange:true)
        .AddJsonFile("Resources/File2.json", optional:false, reloadOnChange:true)
        .Build();

    services.AddConfigLocalization(options => {
        options.Configuration = config;
    });
}
```

## How to manually create a Localizer instance

```csharp
var config = new ConfigurationBuilder()
    .AddIniFile("Resources/File1.ini")
    .Build();

var localizer = new ConfigStringLocalizer(config);
```

## Options to further customize

You'll note that when configuring DI for ASP.NET Core in the example above, an "options" variable was configured. This object (class `ConfigLocalizationOptions`) has 4 properties that you can set:

<dl>
    <dt>IConfiguration Configuration</dt>
    <dd>The only mandatory thing to provide - this gives the Localizer all your resources-strings.</dd>
    <dt>StringComparer KeyComparer</dt>
    <dd>The KeyComparer is used for looking up against resource-string keys. By default, if none is provided here, these lookups are case-sensitive and use .NET's default generic equality comparer. Note: this does not affect culture name lookups, they are always case-insensitive.</dd>
    <dt>Func&lt;string, string&gt; KeyEncoder</dt>
    <dd>When you attempt to retrieve a resource-string, this is executed on the input key you're trying to lookup. The assumption is that the keys within your configuration source are already encoded. The default KeyEncoder replaces all instances of ":" with "&amp;colon;".</dd>
    <dt>Func&lt;string, string&gt; KeyDecoder</dt>
    <dd>When you call the localizer's GetAllStrings() method, the KeyDecoder is executed on every resource-string key from your configuration source. By default, it replaces all instances of "&amp;colon;" with ":".</dd>
</dl>

You can also directly pass each of these customizations to the localizer's constructor if you're calling it manually. There's also an additional `withCulture` parameter that allows you to explicitly specify a culture to use when looking up resource-strings instead of defaulting to the current thread's UI culture. The full constructor signature is:

```csharp
ConfigStringLocalizer(IConfiguration config,
    StringComparer keyComparer = null,
    Func<string, string> keyEncoder = null,
    Func<string, string> keyDecoder = null,
    CultureInfo withCulture = null)
```

# Build and Test
Just build the solution; there are no special requirements apart from having the minimum .NET versions installed. The Tests project currently targets .NET 4.6, .NET Core 2.1, .NET Core 2.0 and .NET Core 1.1.

```
dotnet build Myxas.ConfigStringLocalizer.sln
dotnet test tests/Myxas.ConfigStringLocalizer.Tests
```

# Contribute
Pull requests, issues and feature requests are all totally welcome and would be greatly appreciated!
