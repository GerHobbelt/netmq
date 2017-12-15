using System;
using System.Collections.Generic;
using System.Text;

namespace NetMQ.Core
{
    /// <summary>
    /// 关闭连接的接口
    /// </summary>
    public interface IDisconnect
    {
        /// <summary>
        /// 关闭socket
        /// </summary>
        void Disconnect();
    }
}
