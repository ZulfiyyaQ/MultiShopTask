﻿using System.ComponentModel.DataAnnotations;

namespace MULTİSHOPMVC.Areas.Admin.ViewModels
{
    public class CreateCategoriesVM
    {
        [Required(ErrorMessage = "Ad daxil etmeyiniz mutleqdir")]
        [MaxLength(30, ErrorMessage = "Uzunlugu 30 dan uzun olmamalidir")]
        [MinLength(3, ErrorMessage = "Uzunlugu en azi 3 olmalidir")]

        public string Name { get; set; }

        public IFormFile Photo { get; set; }
    }
}
