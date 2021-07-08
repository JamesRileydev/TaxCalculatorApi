using System;
using System.Text.Json.Serialization;
using TaxCalculator.Api.Converters;

namespace TaxCalculator.Api.Models
{
    public class ServiceError
    {
        public string Message { get; set; }

        [JsonConverter(typeof(ExceptionConverter))]
        public Exception Exception { get; set; }
    }
}
