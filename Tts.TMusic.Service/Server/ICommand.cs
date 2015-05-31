﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tts.TMusic.Common;

namespace Tts.TMusic.Service.Server
{
    public  interface ICommand
    {
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="content">内容</param>
        void Exec(CommandFlag cmd, byte[] content);
    }
}
