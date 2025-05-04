using System;

namespace IChat.Protocol.Contracts
{
    /// <summary>
    /// 表示服务器到客户端的基础响应
    /// </summary>
    public class BaseResponse
    {
        /// <summary>
        /// 关联的请求ID
        /// </summary>
        public Guid RequestId { get; set; }

        /// <summary>
        /// 响应时间戳
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 响应是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 错误代码（如果有）
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// 错误消息（如果有）
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 服务器版本信息
        /// </summary>
        public string ServerVersion { get; set; }

        /// <summary>
        /// 创建一个成功的响应
        /// </summary>
        /// <param name="requestId">关联的请求ID</param>
        /// <returns>成功的响应对象</returns>
        public static BaseResponse CreateSuccess(Guid requestId)
        {
            return new BaseResponse
            {
                RequestId = requestId,
                Success = true
            };
        }

        /// <summary>
        /// 创建一个失败的响应
        /// </summary>
        /// <param name="requestId">关联的请求ID</param>
        /// <param name="errorCode">错误代码</param>
        /// <param name="errorMessage">错误消息</param>
        /// <returns>失败的响应对象</returns>
        public static BaseResponse CreateError(Guid requestId, string errorCode, string errorMessage)
        {
            return new BaseResponse
            {
                RequestId = requestId,
                Success = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage
            };
        }
    }

    /// <summary>
    /// 带有数据的基础响应，用于返回特定类型的数据
    /// </summary>
    /// <typeparam name="T">响应数据的类型</typeparam>
    public class BaseResponse<T> : BaseResponse
    {
        /// <summary>
        /// 响应数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 创建一个带有数据的成功响应
        /// </summary>
        /// <param name="requestId">关联的请求ID</param>
        /// <param name="data">响应数据</param>
        /// <returns>成功的响应对象</returns>
        public static BaseResponse<T> CreateSuccess(Guid requestId, T data)
        {
            return new BaseResponse<T>
            {
                RequestId = requestId,
                Success = true,
                Data = data
            };
        }

        /// <summary>
        /// 创建一个失败的响应
        /// </summary>
        /// <param name="requestId">关联的请求ID</param>
        /// <param name="errorCode">错误代码</param>
        /// <param name="errorMessage">错误消息</param>
        /// <returns>失败的响应对象</returns>
        public new static BaseResponse<T> CreateError(Guid requestId, string errorCode, string errorMessage)
        {
            return new BaseResponse<T>
            {
                RequestId = requestId,
                Success = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                Data = default
            };
        }
    }
}