using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Electro.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public int Price { get; set; }

        public double PreviousPrice
    {
        get { return (Price * 0.1)+ Price; }
    }

        public int InStock { get; set; }

        public string Description { get; set; }
        public string Images { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string AddedBy { get; set; }
        public int Rating { get; set; } = RandomNumber(3, 6);
        public DateTime AddedOn { get; set; } = DateTime.Now;
        [NotMapped]
        public string DisplayId { get; set; }


        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
    }
}