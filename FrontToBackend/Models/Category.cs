using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FrontToBackend.Models
{
    public class Category
    {
        public int id { get; set; }
        [Required(ErrorMessage ="bosh olmaz"),MinLength(5,ErrorMessage ="5 den az olmaz")]
        public string Name{ get; set; }
        [Required, MaxLength(200)]
        public string Desc{ get; set; }
        public List <Product> products { get; set; }
    }
}
