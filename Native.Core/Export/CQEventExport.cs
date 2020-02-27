/*
 * 此文件由T4引擎自动生成, 请勿修改此文件中的代码!
 */
using System;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using Native.Core;
using Native.Core.Domain;
using Native.Sdk.Cqp;
using Native.Sdk.Cqp.Enum;
using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using Native.Sdk.Cqp.Expand;
using Native.Sdk.Cqp.Model;
using Unity;
using Unity.Injection;

namespace Native.App.Export
{
	/// <summary>	
	/// 表示酷Q事件导出的类	
	/// </summary>	
	public class CQEventExport	
	{	
		#region --构造函数--	
		/// <summary>	
		/// 由托管环境初始化的 <see cref="CQEventExport"/> 的新实例	
		/// </summary>	
		static CQEventExport ()	
		{	
			// 初始化 Costura.Fody	
			CosturaUtility.Initialize ();	
			
			Type appDataType = typeof (AppData);	
			appDataType.GetRuntimeProperty ("UnityContainer").GetSetMethod (true).Invoke (null, new object[] { new UnityContainer () });	
			// 调用方法进行注册	
			CQMain.Register (AppData.UnityContainer);	
			
			// 调用方法进行实例化	
			ResolveBackcall ();	
		}	
		#endregion	
		
		#region --核心方法--	
		/// <summary>	
		/// 返回酷Q用于识别本应用的 AppID 和 ApiVer	
		/// </summary>	
		/// <returns>酷Q用于识别本应用的 AppID 和 ApiVer</returns>	
		[DllExport (ExportName = "AppInfo", CallingConvention = CallingConvention.StdCall)]	
		private static string AppInfo ()	
		{	
			return "9,com.qwqaq.time-clock";	
		}	
		
		/// <summary>	
		/// 接收应用 Authcode, 用于注册接口	
		/// </summary>	
		/// <param name="authCode">酷Q应用验证码</param>	
		/// <returns>返回注册结果给酷Q</returns>	
		[DllExport (ExportName = "Initialize", CallingConvention = CallingConvention.StdCall)]	
		private static int Initialize (int authCode)	
		{	
			// 反射获取 AppData 实例	
			Type appDataType = typeof (AppData);	
			// 注册一个 CQApi 实例	
			AppInfo appInfo = new AppInfo ("com.qwqaq.time-clock", 1, 9, "打卡机", "1.0.0", 1, "qwqcode <1149527164@qq.com>", "", authCode);	
			appDataType.GetRuntimeProperty ("CQApi").GetSetMethod (true).Invoke (null, new object[] { new CQApi (appInfo) });	
			AppData.UnityContainer.RegisterInstance<CQApi> ("com.qwqaq.time-clock", AppData.CQApi);	
			// 向容器注册一个 CQLog 实例	
			appDataType.GetRuntimeProperty ("CQLog").GetSetMethod (true).Invoke (null, new object[] { new CQLog (authCode) });	
			AppData.UnityContainer.RegisterInstance<CQLog> ("com.qwqaq.time-clock", AppData.CQLog);	
			// 注册插件全局异常捕获回调, 用于捕获未处理的异常, 回弹给 酷Q 做处理	
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;	
			// 本函数【禁止】处理其他任何代码，以免发生异常情况。如需执行初始化代码请在Startup事件中执行（Type=1001）。	
			return 0;	
		}	
		#endregion	
		
		#region --私有方法--	
		/// <summary>	
		/// 全局异常捕获, 用于捕获开发者未处理的异常, 此异常将回弹至酷Q进行处理	
		/// </summary>	
		/// <param name="sender">事件来源对象</param>	
		/// <param name="e">附加的事件参数</param>	
		private static void CurrentDomain_UnhandledException (object sender, UnhandledExceptionEventArgs e)	
		{	
			Exception ex = e.ExceptionObject as Exception;	
			if (ex != null)	
			{	
				StringBuilder innerLog = new StringBuilder ();	
				innerLog.AppendLine ("发现未处理的异常!");	
				innerLog.AppendLine (ex.ToString ());	
				AppData.CQLog.SetFatalMessage (innerLog.ToString ());	
			}	
		}	
		
		/// <summary>	
		/// 读取容器中的注册项, 进行事件分发	
		/// </summary>	
		private static void ResolveBackcall ()	
		{	
			/*	
			 * Id: 1	
			 * Type: 1001	
			 * Name: 酷Q启动事件	
			 * Function: eventCQStartup	
			 * Priority: 40000	
			 */	
			if (AppData.UnityContainer.IsRegistered<ICQStartup> ("酷Q启动事件"))	
			{	
				EventeventCQStartupHandler += AppData.UnityContainer.Resolve<ICQStartup> ("酷Q启动事件").CQStartup;	
			}	
			
			/*	
			 * Id: 2	
			 * Type: 2	
			 * Name: 群消息处理	
			 * Function: eventGroupMsg	
			 * Priority: 30000	
			 */	
			if (AppData.UnityContainer.IsRegistered<IGroupMessage> ("群消息处理"))	
			{	
				EventeventGroupMsgHandler += AppData.UnityContainer.Resolve<IGroupMessage> ("群消息处理").GroupMessage;	
			}	
			
			/*	
			 * Id: 3	
			 * Type: 21	
			 * Name: 私聊消息处理	
			 * Function: eventPrivateMsg	
			 * Priority: 30000	
			 */	
			if (AppData.UnityContainer.IsRegistered<IPrivateMessage> ("私聊消息处理"))	
			{	
				EventeventPrivateMsgHandler += AppData.UnityContainer.Resolve<IPrivateMessage> ("私聊消息处理").PrivateMessage;	
			}	
			
		}	
		#endregion	
		
		#region --导出方法--	
		/// <summary>	
		/// 事件回调, 以下是对应 Json 文件的信息	
		/// <para>Id: 1</para>	
		/// <para>Type: 1001</para>	
		/// <para>Name: 酷Q启动事件</para>	
		/// <para>Function: eventCQStartup</para>	
		/// <para>Priority: 40000</para>	
		/// <para>IsRegex: False</para>	
		/// </summary>	
		public static event EventHandler<CQStartupEventArgs> EventeventCQStartupHandler;	
		[DllExport (ExportName = "eventCQStartup", CallingConvention = CallingConvention.StdCall)]	
		public static int EventeventCQStartup ()	
		{	
			if (EventeventCQStartupHandler != null)	
			{	
				CQStartupEventArgs args = new CQStartupEventArgs (AppData.CQApi, AppData.CQLog, 1, 1001, "酷Q启动事件", "eventCQStartup", 40000);	
				EventeventCQStartupHandler (typeof (CQEventExport), args);	
			}	
			return 0;	
		}	
		
		/// <summary>	
		/// 事件回调, 以下是对应 Json 文件的信息	
		/// <para>Id: 2</para>	
		/// <para>Type: 2</para>	
		/// <para>Name: 群消息处理</para>	
		/// <para>Function: eventGroupMsg</para>	
		/// <para>Priority: 30000</para>	
		/// <para>IsRegex: False</para>	
		/// </summary>	
		public static event EventHandler<CQGroupMessageEventArgs> EventeventGroupMsgHandler;	
		[DllExport (ExportName = "eventGroupMsg", CallingConvention = CallingConvention.StdCall)]	
		public static int EventeventGroupMsg (int subType, int msgId, long fromGroup, long fromQQ, string fromAnonymous, IntPtr msg, int font)	
		{	
			if (EventeventGroupMsgHandler != null)	
			{	
				CQGroupMessageEventArgs args = new CQGroupMessageEventArgs (AppData.CQApi, AppData.CQLog, 2, 2, "群消息处理", "eventGroupMsg", 30000, subType, msgId, fromGroup, fromQQ, fromAnonymous, msg.ToString(CQApi.DefaultEncoding), false);	
				EventeventGroupMsgHandler (typeof (CQEventExport), args);	
				return (int)(args.Handler ? CQMessageHandler.Intercept : CQMessageHandler.Ignore);	
			}	
			return 0;	
		}	
		
		/// <summary>	
		/// 事件回调, 以下是对应 Json 文件的信息	
		/// <para>Id: 3</para>	
		/// <para>Type: 21</para>	
		/// <para>Name: 私聊消息处理</para>	
		/// <para>Function: eventPrivateMsg</para>	
		/// <para>Priority: 30000</para>	
		/// <para>IsRegex: False</para>	
		/// </summary>	
		public static event EventHandler<CQPrivateMessageEventArgs> EventeventPrivateMsgHandler;	
		[DllExport (ExportName = "eventPrivateMsg", CallingConvention = CallingConvention.StdCall)]	
		public static int EventeventPrivateMsg (int subType, int msgId, long fromQQ, IntPtr msg, int font)	
		{	
			if (EventeventPrivateMsgHandler != null)	
			{	
				CQPrivateMessageEventArgs args = new CQPrivateMessageEventArgs (AppData.CQApi, AppData.CQLog, 3, 21, "私聊消息处理", "eventPrivateMsg", 30000, subType, msgId, fromQQ, msg.ToString(CQApi.DefaultEncoding), false);	
				EventeventPrivateMsgHandler (typeof (CQEventExport), args);	
				return (int)(args.Handler ? CQMessageHandler.Intercept : CQMessageHandler.Ignore);	
			}	
			return 0;	
		}	
		
		#endregion	
	}	
}