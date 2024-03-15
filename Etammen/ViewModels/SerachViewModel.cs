using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Etammen.ViewModels
{
    public class SerachViewModel
    {
       [Display(Name ="Specialty")]
       public SelectList Specialties { get; set; }
       public Dictionary<string, List<string>> city_areaDict { get; set; }
       
    }
}
