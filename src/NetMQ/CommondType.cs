using System;
using System.Collections.Generic;
using System.Text;

namespace NetMQ
{
    public enum NetMQMessageCommondType
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
        Clear = 10,
    }
}
