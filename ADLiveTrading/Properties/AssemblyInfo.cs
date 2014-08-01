using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using WealthLab.Extensions.Attribute;

// Управление общими сведениями о сборке осуществляется с помощью 
// набора атрибутов. Измените значения этих атрибутов, чтобы изменить сведения,
// связанные со сборкой.
[assembly: AssemblyTitle("ADLiveTrading")]
[assembly: AssemblyDescription("Alfa-Direct Static/Streaming Provider and Broker Adapter")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Real-Time Trading Ltd.")]
[assembly: AssemblyProduct("ADLiveTrading")]
[assembly: AssemblyCopyright("Copyright © 2013 Real-Time Trading Ltd.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Параметр ComVisible со значением FALSE делает типы в сборке невидимыми 
// для COM-компонентов.  Если требуется обратиться к типу в этой сборке через 
// COM, задайте атрибуту ComVisible значение TRUE для этого типа.
[assembly: ComVisible(false)]

// Следующий GUID служит для идентификации библиотеки типов, если этот проект будет видимым для COM
[assembly: Guid("0664fbf2-2884-44aa-91ee-b95058ffcbab")]

// Сведения о версии сборки состоят из следующих четырех значений:
//
//      Основной номер версии
//      Дополнительный номер версии 
//      Номер построения:
//            0 - Alpha, X - Ver, XX - Hot Fixes
//            1 - Beta, X - Ver, XX - Hot Fixes
//            2 - RC, X - Ver, XX - Hot Fixes
//            3 - Release, X - SP, XX - Hot Fixes
//      Редакция
//
// Можно задать все значения или принять номер построения и номер редакции по умолчанию, 
// используя "*", как показано ниже:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("3.0.0.0")]
[assembly: AssemblyFileVersion("3.0.1115.150")]
[assembly: AssemblyInformationalVersion("3.0 Beta 2")]

// Сведения о сборке передаваемые в WLD 
[assembly: ExtensionInfo(
    ExtensionType.Provider,
    "ADLiveTrading",
    "Alfa-Direct Static/Streaming Provider and Broker Adapter",
    "Импорт данных и отправка ордеров через терминал Alfa-Direct",
    "3.0 Beta 2",
    "Real-Time Trading Ltd.",
    "RealTimeTrading.ADLiveTrading.Resources.ADLogo[16x16].png",
    ExtensionLicence.Commercial,
    new string[] { "ADLiveTrading.dll", "Interop.ADLite.dll" },
    PublisherUrl = @"http://realtimetrading.ru")]

#if DEBUG
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ADLiveTradingUnitTests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
#endif