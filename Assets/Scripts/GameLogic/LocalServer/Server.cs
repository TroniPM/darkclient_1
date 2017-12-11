using System;
using System.Collections.Generic;
using Mogo.Util;

namespace Mogo.GameLogic.LocalServer
{
	public class Server
	{
        private LocalServerSceneManager sceneManager;
        private List<KeyValuePair<string, Action<object[]>>> handles;

        public Server()
        {
            handles = new List<KeyValuePair<string, Action<object[]>>>();
            sceneManager = LocalServerSceneManager.Instance;
            sceneManager.Initialize();
            AddListeners();
        }

        private void AddListeners()
        {
            AddListener("MissionReq");
            AddListener("CliEntityActionReq");
            AddListener("CliEntitySkillReq");
            AddListener("UseHpBottleReq");
        }

        private void AddListener(string methodName)
        {
            var method = sceneManager.GetType().GetMethod(methodName);
            var e = new KeyValuePair<string, Action<object[]>>(String.Concat(Mogo.Util.Utils.SVR_RPC_HEAD, methodName), (args) =>
            {//RPC回调事件处理
                try
                {
                    //LoggerHelper.Debug("RPC_resp: " + methodName);
                    method.Invoke(sceneManager, args);
                }
                catch (Exception ex)
                {
                    var sb = new System.Text.StringBuilder();
                    sb.Append("method paras are: ");
                    foreach (var methodPara in method.GetParameters())
                    {
                        sb.Append(methodPara.ParameterType + " ");
                    }
                    sb.Append(", rpc resp paras are: ");
                    foreach (var realPara in args)
                    {
                        sb.Append(realPara.GetType() + " ");
                    }

                    Exception inner = ex;
                    while (inner.InnerException != null)
                    {
                        inner = inner.InnerException;
                    }
                    LoggerHelper.Error(String.Format("SVR_RPC req error: method name: {0}, message: {1} {2} {3}", methodName, sb.ToString(), inner.Message, inner.StackTrace));
                }
            });
            EventDispatcher.AddEventListener<object[]>(e.Key, e.Value);
            handles.Add(e);
        }

        private void RemoveListeners()
        {
            foreach (var h in handles)
            {
                EventDispatcher.RemoveEventListener<object[]>(h.Key, h.Value);
            }
        }

        public void Stop()
        {
            RemoveListeners();
        }
	}
}
