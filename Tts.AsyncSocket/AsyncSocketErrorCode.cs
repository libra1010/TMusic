﻿namespace Tts.AsyncSocket
{
    /// <summary>
    /// 异步Socket错误码
    /// </summary>
    public enum AsyncSocketErrorCode
    {
        ServerStartFailure,
        ServerAcceptFailure,
        ClientSocketNoExist,
        ThrowSocketException
    };
}