﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ShelterManagerRedux.Models

{
    public class Client
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClientID { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        public string F_Name { get; set; }
        public string M_Name { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string L_Name { get; set; }

        public int Shelter_Location_ID { get; set; }

        [Required(ErrorMessage = "Birth Date is required")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
        public DateTime Date_Birth { get; set; }


        public DateTime Active_Date { get; set; }

        public DateTime? Inactive_Date { get; set; }



        public List<ShelterLocation> Shelter_Locations { get; set; }
    }
}
