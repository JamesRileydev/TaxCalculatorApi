using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaxCalculator.Api.Models
{
    public class Receipt
    {


        public decimal Tax;

        public decimal Total { get; set; }
    }
}
