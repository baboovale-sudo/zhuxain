using System;
using System.Threading;
using OLAPlug; // 引用插件命名空间

namespace OLA
{
    public class GameTask
    {
        private OLAPlugServer _ola;
        private long _hwnd;

        // 回调函数，用于与 TaskWorker 交互
        private Action<string> _log;
        private Action<string, string> _updateStatus;
        private Func<bool> _checkIsStopped; // 检查是否需要停止 (返回 true 表示需要停止)

        public GameTask(OLAPlugServer ola, long hwnd, Action<string> log, Action<string, string> updateStatus, Func<bool> checkIsStopped)
        {
            _ola = ola;
            _hwnd = hwnd;
            _log = log;
            _updateStatus = updateStatus;
            _checkIsStopped = checkIsStopped;
        }

        /// <summary>
        /// 任务分发入口
        /// </summary>
        public void Execute(string taskName)
        {
            switch (taskName)
            {
                case "主线任务":
                    MainQuest();
                    break;
                case "每日活跃":
                    DailyActive();
                    break;
                case "自动签到":
                    AutoSign();
                    break;
                case "支线任务":
                    SideQuest();
                    break;
                case "挂机任务":
                    AfkTask();
                    break;
                default:
                    _log?.Invoke($"未定义的任务: {taskName}");
                    SmartSleep(1000);
                    break;
            }
        }

        // ==========================================
        // ⬇️ 具体任务逻辑写在这里
        // ==========================================

        private void MainQuest()
        {
            // 示例：这里写具体的找图、点击逻辑
            // _ola.FindMultiColor(...)

            for (int i = 0; i < 120; i++)
            {
                if (!SmartSleep(1000)) return; // 如果返回 false，说明收到了停止信号，直接退出方法

                // 模拟业务日志
                // _log?.Invoke($"主线进度...{i}%"); 
            }
        }

        private void DailyActive()
        {
            for (int i = 0; i < 10; i++)
            {
                if (!SmartSleep(1000)) return;
            }
        }

        private void AutoSign()
        {
            if (!SmartSleep(2000)) return;
            _log?.Invoke("点击签到按钮");
            // _ola.LeftClick(); 
        }

        private void SideQuest()
        {
            for (int i = 0; i < 10; i++)
            {
                if (!SmartSleep(1000)) return;
            }
        }

        private void AfkTask()
        {
            for (int i = 0; i < 100; i++)
            {
                if (!SmartSleep(5000)) return;
            }
        }

        // ==========================================
        // 🛠️ 辅助方法
        // ==========================================

        /// <summary>
        /// 智能延时：支持在延时期间响应 暂停/停止 信号
        /// </summary>
        /// <param name="ms">毫秒</param>
        /// <returns>如果是正常延时结束返回 true；如果是被停止/取消返回 false</returns>
        private bool SmartSleep(int ms)
        {
            // 将长延时拆分为 100ms 的小片段来检测状态
            int slice = 100;
            int count = ms / slice;
            int remain = ms % slice;

            for (int i = 0; i < count; i++)
            {
                // _checkIsStopped() 内部已经包含了 "暂停时死循环等待" 的逻辑
                // 如果它返回 true，说明用户点击了停止，或者 Token 取消了
                if (_checkIsStopped()) return false;
                Thread.Sleep(slice);
            }

            if (remain > 0)
            {
                if (_checkIsStopped()) return false;
                Thread.Sleep(remain);
            }

            return true;
        }
    }
}