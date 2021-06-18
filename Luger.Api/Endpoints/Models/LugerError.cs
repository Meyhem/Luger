using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Luger.Api.Endpoints.Models
{
    public class LugerError
    {
        public string? ErrorMessage { get; init; }
        public string? GenericDescription { get; init; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LugerErrorCode ErrorCode { get; init; }
        public string? Stack { get; init; }

        //public static LugerError FromMemzException(MemzException ex)
        //{
        //    return new()
        //    {
        //        ErrorCode = ex.ErrorCode,
        //        ErrorMessage = ex.Message,
        //        Stack = ex.StackTrace,
        //        GenericDescription = ex.ErrorCode.ToString()
        //    };
        //}

        public static LugerError FromModelState(ModelStateDictionary ms)
        {
            return new()
            {
                ErrorCode = LugerErrorCode.ValidationError,
                ErrorMessage = string.Join(", ", ms.Values
                    .SelectMany(state => state.Errors)
                    .Select(error => error.ErrorMessage)),
                GenericDescription = "Invalid fields",
                Stack = null
            };
        }

        public static LugerError FromException(Exception ex)
        {
            return new()
            {
                ErrorCode = LugerErrorCode.Unknown,
                ErrorMessage = ex.Message,
                Stack = ex.StackTrace,
                GenericDescription = LugerErrorCode.Unknown.ToString()
            };
        }

        public static LugerError From(LugerErrorCode code, string message, string? description = null)
        {
            return new()
            {
                ErrorCode = code,
                ErrorMessage = message,
                GenericDescription = description
            };
        }
    }
}
