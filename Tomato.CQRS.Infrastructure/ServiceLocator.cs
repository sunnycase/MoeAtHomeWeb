using StructureMap;
using StructureMap.Configuration.DSL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.CQRS
{
    /// <summary>
    /// 服务定位器
    /// </summary>
    public abstract class ServiceLocator
    {
        /// <summary>
        /// 默认服务定位器
        /// </summary>
        public static ServiceLocator Default { get; set; }

        /// <summary>
        /// 全局 IoC 容器
        /// </summary>
        public abstract IContainer IoC { get; }

        /// <summary>
        /// 基于会话的 IoC 容器
        /// </summary>
        public abstract IContainer PerSessionIoC { get; }

        /// <summary>
        /// 命令总线
        /// </summary>
        public ICommandBus CommandBus
        {
            get { return PerSessionIoC.GetInstance<ICommandBus>(); }
        }

        /// <summary>
        /// 从全局 IoC 容器创建实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>类型的实例</returns>
        public T Get<T>()
        {
            return IoC.GetInstance<T>();
        }

        /// <summary>
        /// 从全局 IoC 容器创建实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>类型的实例</returns>
        public object Get(Type type)
        {
            return IoC.GetInstance(type);
        }

        /// <summary>
        /// 从基于会话的 IoC 容器创建实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>类型的实例</returns>
        public T GetPerSession<T>()
        {
            return PerSessionIoC.GetInstance<T>();
        }

        /// <summary>
        /// 从基于会话的 IoC 容器创建实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>类型的实例</returns>
        public object GetPerSession(Type type)
        {
            return PerSessionIoC.GetInstance(type);
        }

        /// <summary>
        /// 注册配置程序
        /// </summary>
        /// <typeparam name="TRegistry">配置类型</typeparam>
        public void AddRegistry<TRegistry>() where TRegistry : Registry, new()
        {
            IoC.Configure(o => o.AddRegistry<TRegistry>());
        }

        /// <summary>
        /// 注册配置程序
        /// </summary>
        /// <param name="registry">配置</param>
        public void AddRegistry(Registry registry)
        {
            IoC.Configure(o => o.AddRegistry(registry));
        }
    }
}
