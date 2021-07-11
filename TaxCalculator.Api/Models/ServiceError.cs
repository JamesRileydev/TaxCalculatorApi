using System;
using System.Text.Json.Serialization;
using TaxCalculator.Api.Converters;

namespace TaxCalculator.Api.Models
{
    public interface IServiceError
    {
        public string Message { get; set; }

        [JsonConverter(typeof(ExceptionConverter))]
        public Exception Exception { get; set; }
    }

    public class ServiceError : IServiceError
    {
        public string Message { get; set; }

        [JsonConverter(typeof(ExceptionConverter))]
        public Exception Exception { get; set; }
    }
}
