using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeartHealthy.Models
{
    public class Users
    {
        [PrimaryKey]
        [Identity]
        public int Id { get; set; }
        [NotNull]
        public string Nome { get; set; }
        [NotNull]
        public string Cognome { get; set; }
        [NotNull]
        public string Email { get; set; }
        [NotNull]
        public string Password { get; set; }
        public bool Doctor { get; set; }
        public bool FP { get; set; }
    }
}
