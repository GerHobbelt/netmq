using System;
using System.Collections.Generic;
using System.Text;

namespace NetMQ
{
    /// <summary>
    /// NetMQMessage的类型，上层应用需要根据该字段判断获取到的是数据还是其他信息。
    /// </summary>
    public enum NetMQMessageType
    {
        /// <summary>
        /// 错误
        /// </summary>
        Error = -1,
        /// <summary>
        /// 数据
        /// </summary>
        Data = 0,
        /// <summary>
        /// 清理命令，需要清理上层会话数据
        /// </summary>
        DisConnected = 10,
        /// <summary>
        /// socket错误
        /// </summary>
        SocketError = 100,
    }
}
