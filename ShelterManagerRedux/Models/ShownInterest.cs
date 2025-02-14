using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ShelterManagerRedux.Models

{
    public class ShownInterest
    {
        [Key]
        public int QueueID { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public int ShelterID { get; set; }

    }
}
