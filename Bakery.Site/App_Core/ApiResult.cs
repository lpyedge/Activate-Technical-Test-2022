using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace Bakery.Site
{
    public class ApiResult
    {
        public enum ECode
        {
            [Description("执行失败")] Fail = 0,
            [Description("执行成功")] Success = 1,
            [Description("未知错误")] UnKonwError = 2,


            [Description("未登录")] OffLine = 10,
            [Description("验证失败")] AuthorizationFailed = 11,
            [Description("对象已存在")] TargetExist = 12,
            [Description("对象不存在")] TargetNotExist = 13,
            [Description("对象已变动")] TargetChanged = 14,
            [Description("数据格式错误")] DataFormatError = 15


            //[System.ComponentModel.Description("密码格式错误")]
            //PasswordFormatError = 50,
            //[System.ComponentModel.Description("邮件格式错误")]
            //EmailFormatError = 51,
            //[System.ComponentModel.Description("金额格式错误")]
            //AmountFormatError = 52,

            //[System.ComponentModel.Description("密码错误")]
            //PasswordError = 101,
            //[System.ComponentModel.Description("邮件码错误")]
            //EmailCodeError = 102,
            //[System.ComponentModel.Description("金额过小")]
            //AmountToLower = 105,
            //[System.ComponentModel.Description("金额过大")]
            //AmountToBig = 106,
            //[System.ComponentModel.Description("金额不相符")]
            //AmountNotEquals = 107,
            //[System.ComponentModel.Description("金额不足")]
            //AmountNotEnough = 108,
        }

        public ApiResult()
        {
            code = ECode.Success;
        }

        public ApiResult(ECode p_code)
        {
            code = p_code;
        }

        public ApiResult(string p_message, ECode p_code = ECode.Fail)
        {
            code = p_code;
            this.message = p_message;
        }

        public ECode code { get; set; }

        public bool result => code == ECode.Success;

        public string message { get; private set; }
        
        public static JsonResult RCode()
        {
            return new JsonResult(new ApiResult());
        }
        public static JsonResult RCode(ECode? code = null)
        {
            var result = new ApiResult(code ?? ECode.Success);
            return new JsonResult(result);
        }

        public static JsonResult RCode(string message = null, ECode code = ECode.Fail)
        {
            var result = message != null ? new ApiResult(message, code) : new ApiResult();
            return new JsonResult(result);
        }

        
        public static JsonResult RData<T>(T data,ECode? code = null)
        {
            var result = new ApiResult<T>(data)
            {
                code = code ?? ECode.Success
            };
            return new JsonResult(result);
        }
    }

    public class ApiResult<T> : ApiResult
    {
        public ApiResult()
        {
            code = ECode.Success;
        }

        public ApiResult(T p_data)
        {
            code = ECode.Success;
            data = p_data;
        }

        public T data { get; set; }
    }
}