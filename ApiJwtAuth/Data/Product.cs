using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiJwtAuth.Data
{
    public class Product
    {
        public int Id { get; set;}
        public string? Name {get;set;} = string.Empty;
        public double? Price {get; set;}
    }
}