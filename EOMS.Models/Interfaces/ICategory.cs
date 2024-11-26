using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EOMS.Models
{
    public interface ICategory
    {
        int Id { get; set; }
        string Name { get; set; }
        int DisplayOrder { get; set; }
    }
}