﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GetTheFoodAlready.Ui.Wpf.Resources {
    using System;
    
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class BrowserHelper {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal BrowserHelper() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("GetTheFoodAlready.Ui.Wpf.Resources.BrowserHelper", typeof(BrowserHelper).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на &lt;!DOCTYPE html&gt;
        ///&lt;html&gt;
        ///&lt;head&gt;
        ///	&lt;script src=&quot;https://code.jquery.com/jquery-3.5.1.min.js&quot;
        ///	        integrity=&quot;sha256-9/aliU8dGd2tb6OSsuzixeV4y/faTqgFtohetphbbj0=&quot;
        ///	        crossorigin=&quot;anonymous&quot;&gt;&lt;/script&gt;
        ///	&lt;script src=&quot;https://polyfill.io/v3/polyfill.min.js?features=default&quot;&gt;&lt;/script&gt;
        ///	&lt;script src=&quot;https://maps.googleapis.com/maps/api/js?key=AIzaSyDq3LH7PoQlHWUil0vLdUt3Bc7J5Hyjksc&amp;callback=initMap&amp;libraries=geometry,places&amp;v=weekly&amp;language=ru&quot;
        ///	        defer&gt;&lt;/script&gt;
        ///	&lt;style&gt;
        ///		#map { height: 10 [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string SetupLocationGoogleMapsDummy {
            get {
                return ResourceManager.GetString("SetupLocationGoogleMapsDummy", resourceCulture);
            }
        }
    }
}
