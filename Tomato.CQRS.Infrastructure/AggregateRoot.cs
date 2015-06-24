using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.CQRS
{
    /// <summary>
    /// 聚合根
    /// </summary>
    public abstract class AggregateRoot
    {
        //class AggregateRootProxyBuilder
        //{
        //    private const string ProxyAssemblyName = "CRQSProxyAssembly";
        //    private const string ProxyModuleName = "CRQSProxyModule.dll";
        //    private const string ProxyTypeNameFormat = "{0}_Proxy";
        //    private const string ProxyFieldNameFormat = "__{0}Field";

        //    private static AssemblyBuilder asmBuilder;
        //    private static ModuleBuilder moduleBuilder;

        //    private TypeBuilder typeBuilder;
        //    private Type realType;

        //    static AggregateRootProxyBuilder()
        //    {
        //        var appDomain = AppDomain.CurrentDomain;
        //        var asmName = new AssemblyName(ProxyAssemblyName);
        //        asmBuilder = appDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);
        //        moduleBuilder = asmBuilder.DefineDynamicModule(ProxyModuleName);
        //    }

        //    public AggregateRootProxyBuilder(Type realType)
        //    {
        //        this.realType = realType;

        //        DefineProxyType();
        //        DecorateEvents();
        //    }

        //    /// <summary>
        //    /// 定义代理类型
        //    /// </summary>
        //    private void DefineProxyType()
        //    {
        //        typeBuilder = moduleBuilder.DefineType(string.Format(ProxyTypeNameFormat, realType.Name), TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed, realType);
        //    }

        //    /// <summary>
        //    /// 装饰事件
        //    /// </summary>
        //    private void DecorateEvents()
        //    {
        //        foreach (var ent in realType.GetEvents(BindingFlags.Public | BindingFlags.Instance))
        //        {
        //            // 处理程序类型必须可转换为 RoutedEventHandler<>
        //            if (typeof(RoutedEventHandler<>).IsAssignableFrom(ent.EventHandlerType))
        //            {
        //                ImplementEvent(ent);
        //            }
        //        }
        //    }

        //    /// <summary>
        //    /// 实现事件
        //    /// </summary>
        //    private void ImplementEvent(EventInfo ent)
        //    {
        //        // 定义 RoutedEvent<> 类型的实例字段
        //        var eventArgsType = ent.EventHandlerType.GenericTypeArguments[0];
        //        var eventHandlerType = (typeof(RoutedEventHandler<>)).MakeGenericType(eventArgsType);
        //        var routedEventType = (typeof(RoutedEvent<>)).MakeGenericType(eventArgsType);
        //        var routedEventName = MakeProxyFieldName(ent.Name);
        //        var routedEvent = typeBuilder.DefineField(routedEventName, routedEventType, FieldAttributes.Private);
        //        //routedEvent.SetValue()

        //        // 重写事件访问器
        //        var newEvent = typeBuilder.DefineEvent(ent.Name, EventAttributes.None, ent.GetType());
        //        // add
        //        var addMethod = typeBuilder.DefineMethod("add_" + ent.Name, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Virtual);
        //        var ilGen = addMethod.GetILGenerator();
        //        // this.__eventField
        //        ilGen.Emit(OpCodes.Ldarg_0);
        //        ilGen.Emit(OpCodes.Ldfld, routedEvent);
        //        // value : RoutedEventHandler<TEventArgs>
        //        ilGen.Emit(OpCodes.Ldarg_1);
        //        // this.__eventField.Add(value)
        //        ilGen.EmitCall(OpCodes.Call, routedEventType.GetMethod("Add", new[] { eventHandlerType }), null);
        //        ilGen.Emit(OpCodes.Ret);
        //        newEvent.SetAddOnMethod(addMethod);
        //        // remove
        //        var removeMethod = typeBuilder.DefineMethod("remove_" + ent.Name, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Virtual);
        //        ilGen = removeMethod.GetILGenerator();
        //        // this.__eventField
        //        ilGen.Emit(OpCodes.Ldarg_0);
        //        ilGen.Emit(OpCodes.Ldfld, routedEvent);
        //        // value : RoutedEventHandler<TEventArgs>
        //        ilGen.Emit(OpCodes.Ldarg_1);
        //        // this.__eventField.Remove(value)
        //        ilGen.EmitCall(OpCodes.Call, routedEventType.GetMethod("Remove", new[] { eventHandlerType }), null);
        //        ilGen.Emit(OpCodes.Ret);
        //        newEvent.SetRemoveOnMethod(removeMethod);
        //    }

        //    public static string MakeProxyFieldName(string name)
        //    {
        //        return string.Format(ProxyFieldNameFormat, name);
        //    }

        //    public Type BuildType()
        //    {
        //        return typeBuilder.CreateType();
        //    }
        //}

        public AggregateRoot()
        {

        }

        ///// <summary>
        ///// 包装聚合对象
        ///// </summary>
        ///// <typeparam name="T">对象类型</typeparam>
        ///// <param name="obj">对象</param>
        ///// <returns>包装后的对象</returns>
        //protected static T Wrap<T>(T obj)
        //{
        //    return obj;
        //}

        ///// <summary>
        ///// 引发事件
        ///// </summary>
        ///// <typeparam name="TEventArgs">参数类型</typeparam>
        ///// <param name="eventName">事件名称</param>
        ///// <param name="e">参数</param>
        ///// <returns>一个异步操作</returns>
        //protected Task RaiseEvent<TEventArgs>(string eventName, RoutedEventHandler<TEventArgs> handler, TEventArgs e) where TEventArgs : RoutedEventArgs
        //{
        //    var routedEventName = AggregateRootProxyBuilder.MakeProxyFieldName(eventName);
        //    var routedEventField = GetType().GetField(routedEventName, BindingFlags.NonPublic | BindingFlags.Instance);
        //    var routedEvent = (RoutedEvent<TEventArgs>)routedEventField.GetValue(this);
        //    return routedEvent.ExecuteChainAsync(e);
        //}
    }
}
