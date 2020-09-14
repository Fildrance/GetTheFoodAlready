## About project

This application server two purposes:
* First of all, i helps me to choose what to get for dinner at times i just don't know what i want. (And also it uses smarter filters then original site!).
* And second (but i guess most impartantly) this is sample of usual architectual approaches i use in most of projects. I usually work with web applications, but most of patterns can be used in literally every application from console to microservices, just on different scale. Of course most of patterns used is overengineering, but from this standpoint it is more inclined towards just showcase, minimalistic, yet fancy.

### Notable patterns
 * Strategies are used to choose how to roll vendor point and food items
 * Dispatcher from MediatR library is used to dispatch calls to loosly coupled handler classes, that incapsulate logic
 * Behaviours are natural implementation of interception (as it should be, is used to nest some CCC logic - logging, profiling, serving unique ids for logs)
 * ReactiveUI and RX approach enables interesting way to discribe user interaction, enabling throttle on user input etc. Also provides neat way to implement INotifyPropertyChanged on properties using 'ref'
 * MVVM is used to separate models from presentation cleanly
 * Clients from bridges comes with 
 * Castle Windsor is used as DI container

### Other stuff
 * Service layer is just proxy for Dispatcher calls, hiding both actual usage of dispatcher and service locator anti-pattern, which dispatcher really is behind the scene.
 * CefSharp is used due to original idea of showing parts of delivery service site in frames (which plenty of sites won't like with web-app and usual frame, due to security reasons). CefSharp on the other hand is Chromium, so no possible problems with such things. It also was later discovered that there is no wpf control for google maps : ) so CefSharp was also useful this way.
 * DaData is just service that knows a lot about addresses, buildings etc in Russian Federation. Can easily help with autocompleting address.
 * Nlog is used for basic profiling - can write to csv and later it will be easy to analyse.
 * Localization in wpf by default only supports choosing optimal locale by machine locale and changing locale by re-launching application. Some small hacks and xaml extension, coupled with very cute-styled combo-box enables changing locale on the run.


## How to launch
Project was made with .NET Framework 7.6.2 due to CefSharp not working on Core-wpf (at least at moment of project creation). To launch project locally:

 * clone project
  * at *'src/GetTheFoodAlready.Ui.Wpf/'* folder, find file *'App.config.example'*. Remove *'.example'* part from file name, and inside enter your **googleApiKey** and **dadataApiKey** (for google api visit their site - https://console.cloud.google.com/home/dashboard - need active **javascript map api**, **geocode api** and billing enabled - happily google give more then enough free credit every week for testing and fun, for dadata - just use '9ef80ca72272b01016db7d5c385bae8518b8deaf' key, or get key from https://dadata.ru/api/).
  * restore packages
  * build and launch application
  * enter 'декабристов 2 3' to search, select first option from drop-down a top of map that will open and follow wizard afterwards.
